#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-10 오전 11:53:54
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if PHOTON_UNITY_NETWORKING
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// 
/// </summary>
public class ManagerPhotonChat : CSingletonDynamicMonoBase<ManagerPhotonChat>, IChatClientListener
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public delegate void del_OnGetMessage(Photon.Chat.ChatChannel pChannel, string[] arrSender, object[] arrMessage);

    public class OnGetMessage : CObserverSubject<ChatChannel, string[], object[]>
    {
        new public event del_OnGetMessage Subscribe
        {
            add { base.DoRegist_Listener(new System.Action<ChatChannel, string[], object[]>(value), false); }
            remove { base.DoRemove_Listener(new System.Action<ChatChannel, string[], object[]>(value)); }
        }
    }

    /* public - Field declaration            */

    public CObserverSubject p_Event_OnConnected { get; private set; } = new CObserverSubject();
    public OnGetMessage p_Event_OnGetMessages { get; private set; } = new OnGetMessage();

    public string p_strUserName { get; private set; }
    public ChatClient p_pChatClient { get; private set; }

    [Rename_Inspector("앱 버젼")]
    public string p_strAppVersion = "1.0";

    /* protected & private - Field declaration         */

    [Rename_Inspector("포톤 세팅", false), SerializeField]
    protected AppSettings _pChatAppSettings;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoConnect(string strUserName)
    {
        p_strUserName = strUserName;

        p_pChatClient = new ChatClient(this);
#if !UNITY_WEBGL
        p_pChatClient.UseBackgroundWorkerForSending = true;
#endif

        p_pChatClient.Connect(_pChatAppSettings.AppIdChat, p_strAppVersion, new Photon.Chat.AuthenticationValues(p_strUserName));
        Debug.Log("Connecting as: " + p_strUserName);
    }

    public void DoSubscribeChannel(string strChannel)
    {
        p_pChatClient.Subscribe(strChannel);
    }

    public void DoSendMessage(string strChannelName, string strMessage)
    {
        p_pChatClient.PublishMessage(strChannelName, strMessage);
    }

    public void DoClear_Message(string strChannelName)
    {
        ChatChannel pChannel;
        if (p_pChatClient.TryGetChannel(strChannelName, false, out pChannel))
            pChannel.ClearMessages();
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pChatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        p_pChatClient?.Service();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        PhotonNetwork.RemoveCallbackTarget(this);
        p_pChatClient?.Disconnect();
    }

#region Photon // ========================================================================== //

    public void DebugReturn(DebugLevel eLevel, string strMessage)
    {
        switch (eLevel)
        {
            case DebugLevel.ERROR:
                Debug.LogError(strMessage);
                break;
            case DebugLevel.WARNING:
                Debug.LogWarning(strMessage);
                break;

            default:
                Debug.Log(strMessage);
                break;
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange " + state);
    }

    public void OnConnected()
    {
        //if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
        //{
        //    this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
        //}

        //if (this.FriendsList != null && this.FriendsList.Length > 0)
        //{
        //    this.chatClient.AddFriends(this.FriendsList); // Add some users to the server-list to get their status updates

        //    // add to the UI as well
        //    foreach (string _friend in this.FriendsList)
        //    {
        //        if (this.FriendListUiItemtoInstantiate != null && _friend != this.UserName)
        //        {
        //            this.InstantiateFriendButton(_friend);
        //        }

        //    }
        //}

        p_Event_OnConnected.DoNotify();
        p_pChatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }

    public void OnDisconnected()
    {
        Debug.Log("OnDisconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel pChannel = null;
        if (p_pChatClient.TryGetChannel(channelName, out pChannel) == false)
        {
            Debug.LogError("ShowChannel failed to find channel: " + channelName);
            return;
        }

        p_Event_OnGetMessages.DoNotify(pChannel, senders, messages);
        // Debug.Log("OnGetMessages: " + string.Join(", ", channelName, senders, messages));
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage: " + string.Join(", ", sender, message, channelName));
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("OnStatusUpdate: " + string.Join(", ", user, status, gotMessage, message));
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed: " + string.Join(", ", channels));
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from channel '" + channels.ToStringFull() + "'.");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

#endregion Photon

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}
#endif