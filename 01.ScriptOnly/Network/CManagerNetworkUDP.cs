#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-26 오후 3:12:39
 *	기능 : 
 *	
 *	패킷 생성은 하단 링크의 구조체 마샬링 부분과 Test 코드 참조
 *	https://docs.microsoft.com/ko-kr/dotnet/framework/interop/marshaling-classes-structures-and-unions
   ============================================ */
/// <see cref="Network_UDP_Test.SPacketTest_Struct">

#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using System.Linq;

public interface INetworkPacketHeader
{
    int INetworkPacketHeader_Get_Payload();
    int INetworkPacketHeader_Get_PacketType();
    void INetworkPacketHeader_Set_Header(int iPayload, int iPacketType);
}

public interface INetworkPacket
{
    int INetworkPacket_Get_PacketID();
}

public interface INetworkPacket_Reliable : INetworkPacket
{
    int[] INetworkPacket_Reliable_Get_RespondPacketType();
    void INetworkPacket_Reliable_CheckIsRespond(int iPacketType_RecvRespond, byte[] arrPacket_RecvRespond, out bool bRespond_IsSuccess);
    float INetworkPacket_Reliable_WaitTime_Second();
}


public class CNetworkSession : IDictionaryItem<string>
{
    public struct ReliablePacketWrapper
    {
        public INetworkPacket_Reliable p_pPacket { get; private set; }
        public int p_iSendPort { get; private set; }
        public DateTime p_pDateTimeSend { get; private set; }
        public byte[] p_arrByte_IncludeHeader { get; private set; }
        public int[] p_arrRespondPacketType { get; private set; }

        public ReliablePacketWrapper(INetworkPacket_Reliable pPacket, int[] arrRespondPacketType, byte[] arrByte_IncludeHeader, int iSendPort)
        {
            p_pPacket = pPacket;
            p_arrByte_IncludeHeader = arrByte_IncludeHeader;
            p_iSendPort = iSendPort;
            p_pDateTimeSend = DateTime.Now;
            p_arrRespondPacketType = arrRespondPacketType;
        }

        public bool CheckIsRequire_Resend(ref DateTime pDateTimeNow)
        {
            TimeSpan pTimeGap = pDateTimeNow - p_pDateTimeSend;
            float fWaitTime_Second = pTimeGap.Seconds + pTimeGap.Milliseconds / 1000f;
            return fWaitTime_Second >= p_pPacket.INetworkPacket_Reliable_WaitTime_Second();
        }

        public void DoResetSendTime(ref DateTime pDateTimeNow)
        {
            p_pDateTimeSend = pDateTimeNow;
        }
    }

    public bool p_bIsInit_PacketHeader { get; private set; }
    public string p_strIP { get; private set; }
    public Dictionary<int[], ReliablePacketWrapper> p_mapReliablePacket { get; private set; }
    public ENetworkConnectionState p_eConnectionState { get; private set; }

    private System.Action<CNetworkSession, ENetworkConnectionState> _OnConnectionStateChange;
    private DateTime _pLastSucessConnectionTime;

    public bool CheckIsRequire_ReliablePacket()
    {
        return p_mapReliablePacket.Count != 0;
    }

    public void DoAddReliablePacket(INetworkPacket_Reliable pPacket, byte[] arrByte_IncludeHeader, int iSendPort)
    {
        int[] arrRespondPacketType = pPacket.INetworkPacket_Reliable_Get_RespondPacketType();
        p_mapReliablePacket.Add(arrRespondPacketType, new ReliablePacketWrapper(pPacket, arrRespondPacketType, arrByte_IncludeHeader, iSendPort));
    }

    public void DoRemoveReliablePacket(ReliablePacketWrapper pPacket)
    {
        p_mapReliablePacket.Remove(pPacket.p_arrRespondPacketType);
    }

    public void DoSetIP(string strIP)
    {
        this.p_strIP = strIP;
        p_mapReliablePacket = new Dictionary<int[], ReliablePacketWrapper>();
        _pLastSucessConnectionTime = DateTime.Now;
    }

    public void DoInitSession(System.Action<CNetworkSession, ENetworkConnectionState> OnConnectionStateChange)
    {
        p_bIsInit_PacketHeader = true;
        _OnConnectionStateChange = OnConnectionStateChange;

        // 이벤트 호출을 위해 강제로 세팅
        p_eConnectionState = ENetworkConnectionState.연결끊김;
        DoChangeConnectionState(ENetworkConnectionState.새로접속);
    }

    public void DoChangeConnectionState(ENetworkConnectionState eConnectionState)
    {
        if (p_eConnectionState != eConnectionState)
        {
            p_eConnectionState = eConnectionState;
            if(_OnConnectionStateChange != null)
                _OnConnectionStateChange(this, eConnectionState);
        }
    }

    public float GetLastConnectionTimeGap_Second(ref DateTime pDateTime)
    {
        TimeSpan pTimeGap = pDateTime - _pLastSucessConnectionTime;
        return pTimeGap.Seconds + (pTimeGap.Milliseconds / 1000f);
    }

    public void UpdateConnectionTime()
    {
        DoChangeConnectionState(ENetworkConnectionState.연결중);
        _pLastSucessConnectionTime = DateTime.Now;
    }

    public string IDictionaryItem_GetKey()
    {
        return p_strIP;
    }
}

public enum ENetworkConnectionState
{
    새로접속,
    연결중,
    연결잠시끊김,
    연결끊김,
}

[RequireComponent(typeof(CCompoDontDestroyObj))]
abstract public class CManagerNetworkUDPBase<Class_Derived, Class_SessionDerived, Packet_Header> : CSingletonDynamicMonoBase<Class_Derived>
    where Class_Derived : CManagerNetworkUDPBase<Class_Derived, Class_SessionDerived, Packet_Header>, new()
    where Class_SessionDerived : CNetworkSession, new()
    where Packet_Header : INetworkPacketHeader
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /// <summary>
    /// 유니티 함수는 유니티 쓰레드에서만 동작하기 때문에,
    /// 리시브 쓰레드에서 받은 리시브 데이터를 유니티 쓰레드로 넘겨야 한다.
    /// </summary>
    public struct InnerThread_To_ThreadMessage
    {
        public IPEndPoint pRecieveIP { get; private set; }
        public byte[] arrRecieveData { get; private set; }

        public InnerThread_To_ThreadMessage(IPEndPoint pRecieveIP, byte[] arrRecieveData)
        {
            this.pRecieveIP = pRecieveIP;
            this.arrRecieveData = arrRecieveData;
        }
    }

    /* public - Field declaration            */

    public delegate void OnChange_SesionConnection(Class_SessionDerived pSession, ENetworkConnectionState eState);
    public event OnChange_SesionConnection p_Event_OnChange_SessionConnection;

    public Dictionary<string, Class_SessionDerived> p_mapNetworkSession { get; protected set; }

    public float p_fTimeOutSecond { get { return _fTimeOutSecond; } }
    public float p_fTimeDeleteSession { get { return _fTimeDeleteSession; } }


    /* protected & private - Field declaration         */

    protected Dictionary<string, CCircularBuffer<byte>> _mapRecvBuffer = new Dictionary<string, CCircularBuffer<byte>>();
    //protected Dictionary<string, Queue<byte>> _mapRecvBuffer = new Dictionary<string, Queue<byte>>();

    List<Class_SessionDerived> _listSessionTemp = new List<Class_SessionDerived>();
    public List<CNetworkSession.ReliablePacketWrapper> _listReliablePacketTemp = new List<CNetworkSession.ReliablePacketWrapper>();
    Queue<InnerThread_To_ThreadMessage> _pQueueRecieveThreadMessage = new Queue<InnerThread_To_ThreadMessage>();

    CCircularBuffer<byte> _pBufferSend = new CCircularBuffer<byte>(1024);
    UdpClient _pClientUDP_Recv;
    UdpClient _pClientUDP_Send = new UdpClient();

    Thread _ThreadReceive;

    float _fTimeOutSecond;
    float _fTimeDeleteSession;
    int _iPacketHeaderSize = ByteExtension.SizeOf<Packet_Header>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/
     
    public void DoSendPacket_BroadCast<Packet>(int iPort, Packet pPacket)
        where Packet : INetworkPacket
    {
        IPEndPoint pSendIP = new IPEndPoint(IPAddress.Broadcast, iPort);
        byte[] arrSendByte = CombineBuffer_Header_And_Packet(pPacket);
        _pClientUDP_Send.Send(arrSendByte, arrSendByte.Length, pSendIP);
    }

    public void DoSendPacket<Packet>(string strIP, int iPort, Packet pPacket)
        where Packet : INetworkPacket
    {
        if (p_mapNetworkSession.ContainsKey(strIP) == false)
            MakeSession(strIP);

        byte[] arrSendByte = CombineBuffer_Header_And_Packet(pPacket);
        SendPacket(p_mapNetworkSession[strIP], arrSendByte, pPacket, iPort);
    }

    public void DoSendPacket<Packet>(string strIP, int iPort, Packet pPacket, Packet_Header pPacketHeader)
    where Packet : INetworkPacket
    {
        if (p_mapNetworkSession.ContainsKey(strIP) == false)
            MakeSession(strIP);

        byte[] arrSendByte = CombineBuffer_Header_And_Packet(pPacket, pPacketHeader);
        SendPacket(p_mapNetworkSession[strIP], arrSendByte, pPacket, iPort);
    }


    public void DoSendPacket<Packet>(int iPort, Packet pPacket, params Class_SessionDerived[] arrSession)
        where Packet : INetworkPacket
    {
        if (arrSession == null || arrSession.Length == 0)
            return;

        byte[] arrSendByte = CombineBuffer_Header_And_Packet(pPacket);
        for (int i = 0; i < arrSession.Length; i++)
            SendPacket(arrSession[i], arrSendByte, pPacket, iPort);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Packet"></typeparam>
    /// <param name="iPort"></param>
    /// <param name="pPacket"></param>
    /// <param name="pPacketHeader"></param>
    /// <param name="eConnectionStateRequest"></param>
    /// <param name="arrSession"></param>
    /// <returns>패킷 송신에 성공한 세션들</returns>
    public Class_SessionDerived[] DoSendPacket_OrNull<Packet>(int iPort, Packet pPacket, ENetworkConnectionState eConnectionStateRequest, params Class_SessionDerived[] arrSession)
        where Packet : INetworkPacket
    {
        if (arrSession == null || arrSession.Length == 0)
            return null;

        byte[] arrSendByte = CombineBuffer_Header_And_Packet(pPacket);
        _listSessionTemp.Clear();
        for (int i = 0; i < arrSession.Length; i++)
        {
            if (arrSession[i].p_eConnectionState != eConnectionStateRequest)
                continue;

            SendPacket(arrSession[i], arrSendByte, pPacket, iPort);
            _listSessionTemp.Add(arrSession[i]);
        }

        return _listSessionTemp.ToArray();
    }

    public void DoStartListen_UDP(int iPort)
    {
        if (_pClientUDP_Recv != null)
            _pClientUDP_Recv.Close();

        _pClientUDP_Recv = new UdpClient();
        _pClientUDP_Recv.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _pClientUDP_Recv.Client.Bind(new IPEndPoint(IPAddress.Any, iPort));

        if (_ThreadReceive != null)
            _ThreadReceive.Abort();

        _ThreadReceive = new Thread(new ThreadStart(ListenUDP));
        _ThreadReceive.IsBackground = true;
        _ThreadReceive.Start();
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        p_mapNetworkSession = new Dictionary<string, Class_SessionDerived>();
        OnInitManager(out _fTimeOutSecond, out _fTimeDeleteSession);
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        StartCoroutine(CoCheck_ReliablePacket());

        while(true)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            Check_SessionConnection();
        }
    }

    private IEnumerator CoCheck_ReliablePacket()
    {
        while(true)
        {
            DateTime pDateTime = DateTime.Now;
            var arrSession_RequireReliable = p_mapNetworkSession.Values.Where(pSession => pSession.CheckIsRequire_ReliablePacket());
            foreach (var pSession in arrSession_RequireReliable)
            {
                foreach (var pReliablePacket in pSession.p_mapReliablePacket.Values)
                {
                    if (pReliablePacket.CheckIsRequire_Resend(ref pDateTime) == false)
                        continue;

                    SendPacket(pSession, pReliablePacket.p_arrByte_IncludeHeader, pReliablePacket.p_pPacket, pReliablePacket.p_iSendPort, true);
                    pReliablePacket.DoResetSendTime(ref pDateTime);
                }
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        Update_RecieveMessageQueue();
    }

    private void OnDestroy()
    {
        _mapRecvBuffer.Clear();
        if (_pClientUDP_Recv != null)
            _pClientUDP_Recv.Close();
    }

    /* protected - [abstract & virtual]         */

    abstract protected void OnInitManager(out float fTimeOutSecond, out float fTimeDeleteSessionSecond);
    abstract protected void OnFirstPacket_IsNot_PacketHeader(Class_SessionDerived pSessionSender_OrNull, byte[] arrRecieveData, string strIP, out bool bIsDeletePacket);
    abstract protected void OnRecievePacket(Class_SessionDerived pSessionSender, Packet_Header pPacketHeader, byte[] arrRecieveData, string strIP);
    abstract protected void OnInitSession(Class_SessionDerived pSessionSender, Packet_Header pPacketHeader);

    virtual protected void OnGeneratePacketHeader(INetworkPacket pPacket, out bool bIsGenerate, out Packet_Header pPacketHeader_Generated) { bIsGenerate = false; pPacketHeader_Generated = default(Packet_Header); }

    // ========================================================================== //

    #region Private

    private void SendPacket(Class_SessionDerived pSessionRecv, byte[] arrByte, INetworkPacket pPacket, int iPort, bool bIsReliableResend = false)
    {
        IPEndPoint pSendIP = new IPEndPoint(IPAddress.Parse(pSessionRecv.p_strIP), iPort);
        _pClientUDP_Send.Send(arrByte, arrByte.Length, pSendIP);

        INetworkPacket_Reliable pPacketIsReliable = pPacket as INetworkPacket_Reliable;
        if (bIsReliableResend == false && pPacketIsReliable != null)
            pSessionRecv.DoAddReliablePacket(pPacketIsReliable, arrByte, iPort);
    }

    private byte[] CombineBuffer_Header_And_Packet<Packet>(Packet pPacket) where Packet : INetworkPacket
    {
        return CombineBuffer_Header_And_Packet(pPacket, ByteExtension.ConvertByteArray(pPacket));
    }

    private byte[] CombineBuffer_Header_And_Packet<Packet>(Packet pPacket, Packet_Header pPacketHeader) where Packet : INetworkPacket
    {
        bool bIsGenerate = pPacketHeader != null;
        if (bIsGenerate == false)
            OnGeneratePacketHeader(pPacket, out bIsGenerate, out pPacketHeader);

        _pBufferSend.Clear();
        if (bIsGenerate)
        {
            pPacketHeader.INetworkPacketHeader_Set_Header(ByteExtension.SizeOf<Packet>(), pPacket.INetworkPacket_Get_PacketID());
            _pBufferSend.Enqueue(ByteExtension.ConvertByteArray(pPacketHeader));
        }
        _pBufferSend.Enqueue(ByteExtension.ConvertByteArray(pPacket));

        return _pBufferSend.Dequeue_OrNull(_pBufferSend.Count);
    }

    private byte[] CombineBuffer_Header_And_Packet(INetworkPacket pPacket, byte[] arrByte)
    {
        bool bIsGenerate;
        Packet_Header pPacketHeader;
        OnGeneratePacketHeader(pPacket, out bIsGenerate, out pPacketHeader);

        _pBufferSend.Clear();
        if (bIsGenerate)
        {
            pPacketHeader.INetworkPacketHeader_Set_Header(arrByte.Length, pPacket.INetworkPacket_Get_PacketID());
            _pBufferSend.Enqueue(ByteExtension.ConvertByteArray(pPacketHeader));
        }
        _pBufferSend.Enqueue(ByteExtension.ConvertByteArray(pPacket));

        return _pBufferSend.Dequeue_OrNull(_pBufferSend.Count);
    }

    private void ListenUDP()
    {
        _mapRecvBuffer.Clear();
        try
        {
            while (true)
            {
                IPEndPoint pRecieveIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] arrData = _pClientUDP_Recv.Receive(ref pRecieveIP);
                if (arrData != null && arrData.Length != 0)
                {
                    // 일단 받은 데이터를 버퍼에 넣는다.
                    _pQueueRecieveThreadMessage.Enqueue(new InnerThread_To_ThreadMessage(pRecieveIP, arrData));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
    
    private void Check_SessionConnection()
    {
        DateTime pDateTimeNow = DateTime.Now;
        _listSessionTemp.Clear();
        foreach (var pSession in p_mapNetworkSession.Values)
        {
            float fSecondGap = pSession.GetLastConnectionTimeGap_Second(ref pDateTimeNow);
            if (fSecondGap > _fTimeDeleteSession)
            {
                _listSessionTemp.Add(pSession);
                pSession.DoChangeConnectionState(ENetworkConnectionState.연결끊김);
            }
            else if (fSecondGap > _fTimeOutSecond)
            {
                pSession.DoChangeConnectionState(ENetworkConnectionState.연결잠시끊김);
            }
        }

        for (int i = 0; i < _listSessionTemp.Count; i++)
        {
            p_mapNetworkSession.Remove(_listSessionTemp[i].p_strIP);
            _mapRecvBuffer.Remove(_listSessionTemp[i].p_strIP);
        }
    }

    private void Update_RecieveMessageQueue()
    {
        if (_pQueueRecieveThreadMessage.Count == 0)
            return;

        InnerThread_To_ThreadMessage pRecieveMessage = _pQueueRecieveThreadMessage.Dequeue();
        var pIP = pRecieveMessage.pRecieveIP; // 이구간에서 NullException이 떴기에 어디서 떴는지 체크하기 위해 라인 분해
        string strIP = pIP.Address.ToString();

        if (_mapRecvBuffer.ContainsKey(strIP) == false)
            _mapRecvBuffer.Add(strIP, new CCircularBuffer<byte>(10240));

        //if (_mapRecvBuffer.ContainsKey(strIP) == false)
        //    _mapRecvBuffer.Add(strIP, new Queue<byte>(10240));

        var pBuffer = _mapRecvBuffer[strIP];
        pBuffer.Enqueue(pRecieveMessage.arrRecieveData);

        // 먼저 패킷 헤더만큼 왔는지 버퍼에서 체크한다.
        if (pRecieveMessage.arrRecieveData.Length < _iPacketHeaderSize)
            return;

        byte[] arrPacketHeader = pBuffer.Peek_OrNull(_iPacketHeaderSize);
        // 패킷 헤더 사이즈 만큼 왔는데, 패킷 헤더가 아닌 경우 자식에게 판단을 맡긴다.

        Packet_Header pPacketHeader;
        if (arrPacketHeader.Convert_ToStruct(out pPacketHeader) == false)
        {
            bool bIsDeletePacket;
            if(p_mapNetworkSession.ContainsKey(strIP))
                OnFirstPacket_IsNot_PacketHeader(p_mapNetworkSession[strIP], arrPacketHeader, strIP, out bIsDeletePacket);
            else
                OnFirstPacket_IsNot_PacketHeader(null, arrPacketHeader, strIP, out bIsDeletePacket);

            if (bIsDeletePacket)
            {
                pBuffer.Dequeue_OrNull(pRecieveMessage.arrRecieveData.Length);
                if (pBuffer.Count == 0)
                    pBuffer.Reset();
            }

            return;
        }

        if (p_mapNetworkSession.ContainsKey(strIP) == false)
            MakeSession(strIP, pPacketHeader);

        Class_SessionDerived pSession = p_mapNetworkSession[strIP];
        if(pSession.p_bIsInit_PacketHeader == false)
            InitSession(pSession, pPacketHeader);

        pSession.UpdateConnectionTime();

        int iPayload = pPacketHeader.INetworkPacketHeader_Get_Payload();
        if (_iPacketHeaderSize + iPayload > pBuffer.Count)
            return;
        
        // 패킷 헤더가 맞는 경우, 패킷 헤더의 페이로드만큼 빼내고 자식에게 넘긴다.
        pBuffer.Dequeue_OrNull(_iPacketHeaderSize);
        byte[] arrPacket = pBuffer.Dequeue_OrNull(iPayload);

        _listReliablePacketTemp.Clear();
        if (pSession.CheckIsRequire_ReliablePacket())
        {
            int iPacketType = pPacketHeader.INetworkPacketHeader_Get_PacketType();
            foreach (var pPacketWrapper in pSession.p_mapReliablePacket.Values)
            {
                if (pPacketWrapper.p_arrRespondPacketType.Contains(iPacketType) == false)
                    continue;

                bool bIsSuccess;
                pPacketWrapper.p_pPacket.INetworkPacket_Reliable_CheckIsRespond(iPacketType, arrPacket, out bIsSuccess);
                if (bIsSuccess)
                    _listReliablePacketTemp.Add(pPacketWrapper);
            }

            foreach (var pRemovePacket in _listReliablePacketTemp)
            {
                pSession.DoRemoveReliablePacket(pRemovePacket);
            }
        }
        OnRecievePacket(pSession, pPacketHeader, arrPacket, strIP);
    }

    private void MakeSession(string strIP)
    {
        Class_SessionDerived pSessionNew = new Class_SessionDerived();
        pSessionNew.DoSetIP(strIP);
        p_mapNetworkSession.Add(strIP, pSessionNew);
    }

    private void MakeSession(string strIP, Packet_Header pPacketHeader)
    {
        MakeSession(strIP);
        InitSession(p_mapNetworkSession[strIP], pPacketHeader);
    }

    private void InitSession(Class_SessionDerived pSession, Packet_Header pPacketHeader)
    {
        OnInitSession(pSession, pPacketHeader);
        pSession.DoInitSession(ExcuteOnChangeSessionConnect);
    }

    private void ExcuteOnChangeSessionConnect(CNetworkSession pSession, ENetworkConnectionState eState)
    {
        if (p_Event_OnChange_SessionConnection != null)
            p_Event_OnChange_SessionConnection(p_mapNetworkSession[pSession.p_strIP], eState);
    }

    #endregion Private

    // ========================================================================== //
}