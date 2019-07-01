using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class Network_UDP_Test : CManagerNetworkUDPBase<Network_UDP_Test, Network_UDP_Test.STestSession, Network_UDP_Test.SPacketHeader>
    {
        const string const_strPacketHeaderCheck = "패킷헤더체크";

        public enum ETestPacketType : byte
        {
            None,
            Test_Struct,
            Test_Class,
            Test_Reliable_Send,
            Test_Reliable_Respond
        }

        public class STestSession : CNetworkSession
        {
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class SPacketHeader : INetworkPacketHeader
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string strValue;
            public int iPayload;
            public ETestPacketType ePacketType;

            public int INetworkPacketHeader_Get_Payload()
            {
                return iPayload;
            }

            public void INetworkPacketHeader_Set_Header(int iPayload, int iPacketType)
            {
                this.iPayload = iPayload;
                this.ePacketType = (ETestPacketType)iPacketType;
                strValue = ePacketType.ToString();
            }

            public int INetworkPacketHeader_Get_PacketType()
            {
                return (int)ePacketType;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct SPacketTest_Struct : INetworkPacket
        {
            public int iValue;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string strValue;

            static public SPacketTest_Struct Dummy()
            {
                return new SPacketTest_Struct(0, "");
            }

            public int INetworkPacket_Get_PacketID()
            {
                return (int)ETestPacketType.Test_Struct;
            }

            public SPacketTest_Struct(int iValue, string strValue)
            {
                this.iValue = iValue;
                this.strValue = strValue;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class SPacketTest_Class : INetworkPacket
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string strValue;
            public float fValue;

            static public SPacketTest_Struct Dummy()
            {
                return new SPacketTest_Struct(0, "");
            }

            public int INetworkPacket_Get_PacketID()
            {
                return (int)ETestPacketType.Test_Class;
            }

            public SPacketTest_Class(float fValue, string strValue)
            {
                this.strValue = strValue;
                this.fValue = fValue;
            }
        }

        [UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator Convert_Struct_To_Byte_To_Struct_Test()
        {
            SPacketTest_Struct pPacket1 = new SPacketTest_Struct(1, "테스트Test123!@#");
            SPacketTest_Struct pPacket2 = new SPacketTest_Struct();

            Assert.AreNotEqual(pPacket1, pPacket2);

            byte[] arrData = ByteExtension.ConvertByteArray(pPacket1);
            ByteExtension.ConvertPacket(arrData, out pPacket2);

            Assert.AreEqual(pPacket1, pPacket2);

            yield return null;
        }


        const string strTestTargetIP = "127.0.0.1";
        const int iTestPort = 9999;

        static SPacketTest_Struct pPacketCheckRecieve;
        static bool bIsRecievePacket_OnFail;

        [UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator LocalUDP_Test()
        {
            pPacketCheckRecieve = SPacketTest_Struct.Dummy();
            Assert.AreEqual(pPacketCheckRecieve, SPacketTest_Struct.Dummy());

            Network_UDP_Test.EventMakeSingleton();
            Network_UDP_Test.instance.DoStartListen_UDP(iTestPort);

            bIsRecievePacket_OnFail = false;
            SPacketTest_Struct pSendPacket = new SPacketTest_Struct(1, "보냈다");

            // 보내기 전에는 체크용 패킷과 보낼 패킷과 일치하지 않는다.
            Assert.AreNotEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
            Assert.AreNotEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);

            Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, pSendPacket);
            while (bIsRecievePacket_OnFail == false)
            {
                yield return null;
            }

            // 패킷을 받은 뒤에는 체크용 패킷과 보낼 패킷이 일치한다.
            Assert.AreEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
            Assert.AreEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);
        }

        [UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator Local_UDP_Connect_Session_Test()
        {
            Network_UDP_Test.EventMakeSingleton(true);
            Network_UDP_Test.instance.DoStartListen_UDP(iTestPort);

            // 세션이 하나도 없어야 한다.
            while (Network_UDP_Test.instance.p_mapNetworkSession.Count != 0)
            {
                yield return YieldManager.GetWaitForSecond(.1f);
            }
            Assert.IsTrue(Network_UDP_Test.instance.p_mapNetworkSession.Count == 0, Network_UDP_Test.instance.p_mapNetworkSession.ToString());

            SPacketTest_Class pSendPacket = new SPacketTest_Class(2, "보냈다");
            Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, pSendPacket);

            yield return YieldManager.GetWaitForSecond(.05f);
            STestSession pSession = Network_UDP_Test.instance.p_mapNetworkSession.Values.First();

            DateTime pDateTime = DateTime.Now;
            float fSecond = pSession.GetLastConnectionTimeGap_Second(ref pDateTime);
            //Debug.Log("Last Connection Second :" + fSecond);

            Assert.IsTrue(pSession.p_eConnectionState == ENetworkConnectionState.새로접속 || pSession.p_eConnectionState == ENetworkConnectionState.연결중, pSession.p_eConnectionState.ToString());

            // 타임아웃보다 조금 더 기다린다.
            yield return YieldManager.GetWaitForSecond(p_fTimeOutSecond + 0.5f);

            pDateTime = DateTime.Now;
            fSecond = pSession.GetLastConnectionTimeGap_Second(ref pDateTime);
            Assert.IsTrue(pSession.p_eConnectionState == ENetworkConnectionState.연결잠시끊김, pSession.p_eConnectionState.ToString() + " Last Connection Second : " + fSecond);

            // 타임아웃에서 연결종료시간보다 조금 더 기다린다.
            yield return YieldManager.GetWaitForSecond(p_fTimeDeleteSession - p_fTimeOutSecond + 0.5f);
            Assert.IsTrue(pSession.p_eConnectionState == ENetworkConnectionState.연결끊김, pSession.p_eConnectionState.ToString() + " Last Connection Second : " + fSecond);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class SPacketTest_ReliableSend : INetworkPacket_Reliable
        {
            public int iValue;

            public int INetworkPacket_Get_PacketID()
            {
                return (int)ETestPacketType.Test_Reliable_Send;
            }

            public void INetworkPacket_Reliable_CheckIsRespond(int iPacketType_RecvRespond, byte[] arrPacket_RecvRespond, out bool bRespond_IsSuccess)
            {
                SPacketTest_ReliableRespond pPacketRespond;
                if (arrPacket_RecvRespond.Convert_ToStruct(out pPacketRespond))
                    bRespond_IsSuccess = pPacketRespond.strValue.Equals("정상응답");
                else
                    bRespond_IsSuccess = false;
            }

            public int[] INetworkPacket_Reliable_Get_RespondPacketType()
            {
                return new int[] { (int)ETestPacketType.Test_Reliable_Respond };
            }

            public float INetworkPacket_Reliable_WaitTime_Second()
            {
                return 0.1f;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class SPacketTest_ReliableRespond : INetworkPacket
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string strValue;

            public SPacketTest_ReliableRespond(string strValue)
            {
                this.strValue = strValue;
            }

            public int INetworkPacket_Get_PacketID()
            {
                return (int)ETestPacketType.Test_Reliable_Respond;
            }
        }

        static int iReliableRecvCount;

        [UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator Local_UDP_ReliablePacket_Test()
        {
            Network_UDP_Test.EventMakeSingleton();
            Network_UDP_Test.instance.DoStartListen_UDP(iTestPort);

            iReliableRecvCount = 0;
            bIsRecievePacket_OnFail = false;
            SPacketTest_ReliableSend pSendPacket = new SPacketTest_ReliableSend();
            Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, pSendPacket);

            yield return YieldManager.GetWaitForSecond(0.1f);

            STestSession pSesion = Network_UDP_Test.instance.p_mapNetworkSession.Values.First();
            Assert.IsTrue(pSesion.CheckIsRequire_ReliablePacket());

            // 릴라이어블 패킷은 정해진 시간동안 기다렸다가 설정한 응답 패킷이 오지 않으면 재전송을 한다.
            // 테스트 케이스의 경우 0.1초마다 재전송을 한다.
            while (pSesion.CheckIsRequire_ReliablePacket())
            {
                yield return YieldManager.GetWaitForSecond(0.1f);
            }
            Assert.IsTrue(iReliableRecvCount >= 3); // 릴라이어블 패킷 Send를 받으면 ++이 된다.
            Debug.Log(" Finsih - iReliableRecvCount  : " + iReliableRecvCount);

            yield break;
        }

        [Test]
        [Category("StrixLibrary")]
        public void RingBuffer_Enqueue_Dequeue_Test()
        {
            SPacketTest_Struct pPacketTest = new SPacketTest_Struct(1, "인큐_디큐_테스트");
            SPacketTest_Struct pPacketTest2 = new SPacketTest_Struct(2, "더미데이터");

            byte[] arrPacketData = ByteExtension.ConvertByteArray(pPacketTest);
            int iDataSize = arrPacketData.Length;

            var pBuffer = new CCircularBuffer<byte>(10240);
            pBuffer.Enqueue(arrPacketData);

            Assert.AreNotEqual(pPacketTest.iValue, pPacketTest2.iValue);
            Assert.AreNotEqual(pPacketTest.strValue, pPacketTest2.strValue);

            byte[] arrPacketData2 = pBuffer.Dequeue_OrNull(iDataSize);
            arrPacketData2.Convert_ToStruct(out pPacketTest2);

            Assert.AreEqual(pPacketTest.iValue, pPacketTest2.iValue);
            Assert.AreEqual(pPacketTest.strValue, pPacketTest2.strValue);
        }


        // 기본 UDP 송수신 테스트 코드를 위해 전개했으나,
        // 본래 여기에서는 보통 해당 패킷을 보낸 IP를 블랙 리스트 후보에 올리고, 해당 패킷은 삭제해야 한다.
        protected override void OnFirstPacket_IsNot_PacketHeader(STestSession pSessionSender_OrNull, byte[] arrRecieveData, string strIP, out bool bIsDeletePacket)
        {
            Debug.Log("Test OnFirstPacket_IsNot_PacketHeader");

            bIsDeletePacket = true;
        }

        // 프로젝트 레벨에선, 페킷 헤더 내용을 토대로 페이로드에 있는 패킷을 파싱해야 한다.
        // 테스트 코드이므로 생략
        protected override void OnRecievePacket(STestSession pSessionSender, SPacketHeader pPacketHeader, byte[] arrRecieveData, string strIP)
        {
            /// <see cref="Network_UDP_Test.로컬_UDP송수신_패킷헤더및_패킷디큐_테스트"/>
            switch (pPacketHeader.ePacketType)
            {
                case ETestPacketType.Test_Struct:
                    {
                        SPacketTest_Struct pPacketTest1;
                        if (arrRecieveData.Convert_ToStruct(out pPacketTest1))
                        {
                            pPacketCheckRecieve = pPacketTest1;
                            Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, new SPacketTest_Struct(2, "받았다"));
                            bIsRecievePacket_OnFail = true;
                        }
                    }

                    break;

                case ETestPacketType.Test_Class:
                    break;

                case ETestPacketType.Test_Reliable_Send:
                    if (iReliableRecvCount++ >= 3)
                        Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, new SPacketTest_ReliableRespond("정상응답"));
                    else
                    {
                        Debug.Log("릴라이어블 리시브 카운트 : " + iReliableRecvCount + " " + DateTime.Now.ToString("mm:ss.") + DateTime.Now.Millisecond);
                        Network_UDP_Test.instance.DoSendPacket(strTestTargetIP, iTestPort, new SPacketTest_ReliableRespond("거짓응답"));
                    }

                    break;

                case ETestPacketType.Test_Reliable_Respond:
                    break;

                // 정해진 패킷 타입이 아닌 경우 -
                default:
                    Debug.Log(" OnRecievePacket - " + pPacketHeader.ePacketType);
                    break;
            }
        }

        protected override void OnInitManager(out float fTimeOutSecond, out float fTimeDeleteSessionSecond)
        {
            fTimeOutSecond = 0.3f;
            fTimeDeleteSessionSecond = 0.6f;
        }

        protected override void OnInitSession(STestSession pSessionSender, SPacketHeader pPacketHeader)
        {
        }

        protected override void OnGeneratePacketHeader(INetworkPacket pPacket, out bool bIsGenerate, out SPacketHeader pPacketHeader_Generated)
        {
            bIsGenerate = true;
            pPacketHeader_Generated = new SPacketHeader();
        }
    }

    /// <summary>
    /// CircularBuffer에 문제가 있는것 같아 직접 큐를 래핑해서 썼는데,
    /// 오히려 이걸 쓰면 패킷이 더 손실된다.
    /// </summary>
    static public class QueueHelper
    {
        static public void Enqueue<T>(this Queue<T> pQueueTarget, T[] arrData)
        {
            for (int i = 0; i < arrData.Length; i++)
                pQueueTarget.Enqueue(arrData[i]);
        }

        static public T[] Peek_OrNull<T>(this Queue<T> pQueueTarget, int iPeekLength)
        {
            T[] arrPeek = new T[iPeekLength];
            for (int i = 0; i < iPeekLength; i++)
                arrPeek[i] = pQueueTarget.ElementAt(i);

            return arrPeek;
        }

        static public T[] Dequeue_OrNull<T>(this Queue<T> pQueueTarget, int iDequeueLength)
        {
            T[] arrPeek = new T[iDequeueLength];
            for (int i = 0; i < iDequeueLength; i++)
                arrPeek[i] = pQueueTarget.Dequeue();

            return arrPeek;
        }
    }
}
