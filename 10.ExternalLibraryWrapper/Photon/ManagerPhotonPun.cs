#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-10 오후 9:35:21
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// 
/// </summary>
public class ManagerPhotonPun : CSingletonDynamicMonoBase<ManagerPhotonPun>, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EConnectedState
    {
        Connected,
        DisConnected,
        ConnectToMaster,
    }

    /* public - Field declaration            */

    public CObserverSubject<EConnectedState> p_Event_OnChangeConnectState { get; private set; } = new CObserverSubject<EConnectedState>();
    public CObserverSubject<List<RoomInfo>> p_Event_OnUpdateRoom { get; private set; } = new CObserverSubject<List<RoomInfo>>();

    public List<RoomInfo> p_listRoom { get; private set; } = new List<RoomInfo>();

    /* protected & private - Field declaration         */

    System.Action _OnJoinedRoom;
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoConnect(string strNickName)
    {
        PhotonNetwork.NickName = strNickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void DoJoinLobby()
    {
        if(PhotonNetwork.InLobby == false)
            PhotonNetwork.JoinLobby();
    }

    public void DoCreateRoom(string strRoomName, byte iMaxPlayers, System.Action OnJoinedRoom)
    {
        _OnJoinedRoom = OnJoinedRoom;
        strRoomName = (strRoomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : strRoomName;

        RoomOptions pRoomOptions = new RoomOptions { MaxPlayers = iMaxPlayers };
        //pRoomOptions.CustomRoomProperties
        //pRoomOptions.CustomRoomPropertiesForLobby
        PhotonNetwork.CreateRoom(strRoomName, pRoomOptions, null);
    }

    public void DoJoinRoom(string strRoomName, System.Action OnJoinedRoom)
    {
        _OnJoinedRoom = OnJoinedRoom;
        Photon.Pun.PhotonNetwork.JoinRoom(strRoomName);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        DontDestroyOnLoad(gameObject);
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        PhotonNetwork.AddCallbackTarget(this);
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnConnected()
    {
        Debug.Log("Photon-Pun - " + nameof(OnConnected), this);

        p_Event_OnChangeConnectState.DoNotify(EConnectedState.Connected);
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Photon-Pun - " + nameof(OnConnectedToMaster), this);

        p_Event_OnChangeConnectState.DoNotify(EConnectedState.ConnectToMaster);
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Photon-Pun - " + nameof(OnDisconnected), this);

        p_Event_OnChangeConnectState.DoNotify(EConnectedState.DisConnected);
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("Photon-Pun - " + nameof(OnRegionListReceived), this);
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log("Photon-Pun - " + nameof(OnCustomAuthenticationResponse), this);
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("Photon-Pun - " + nameof(OnCustomAuthenticationFailed), this);
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        Debug.Log("Photon-Pun - " + nameof(OnFriendListUpdate), this);
    }

    public void OnCreatedRoom()
    {
        Debug.Log("Photon-Pun - " + nameof(OnCreatedRoom), this);
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Photon-Pun - " + nameof(OnCreateRoomFailed), this);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Photon-Pun - " + nameof(OnJoinedRoom), this);

        _OnJoinedRoom?.Invoke();
        _OnJoinedRoom = null;        
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Photon-Pun - " + nameof(OnJoinRoomFailed), this);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Photon-Pun - " + nameof(OnJoinRandomFailed), this);
    }

    public void OnLeftRoom()
    {
        Debug.Log("Photon-Pun - " + nameof(OnLeftRoom), this);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Photon-Pun - " + nameof(OnPlayerEnteredRoom), this);

    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Photon-Pun - " + nameof(OnPlayerLeftRoom), this);

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Photon-Pun - " + nameof(OnRoomPropertiesUpdate), this);

    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Photon-Pun - " + nameof(OnPlayerPropertiesUpdate), this);

    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Photon-Pun - " + nameof(OnMasterClientSwitched), this);

    }

    public void OnJoinedLobby()
    {
        Debug.Log("Photon-Pun - " + nameof(OnJoinedLobby), this);
    }

    public void OnLeftLobby()
    {
        Debug.Log("Photon-Pun - " + nameof(OnJoinRoomFailed), this);
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        p_listRoom = roomList;
        p_Event_OnUpdateRoom.DoNotify(roomList);

        Debug.Log("Photon-Pun - " + nameof(OnRoomListUpdate), this);
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("Photon-Pun - " + nameof(OnLobbyStatisticsUpdate), this);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}
#endif