#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-14 오후 9:47:53
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;

#if PHOTON_UNITY_NETWORKING
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum ECustomPropertiesName
{
    RoomPlayer_IsReady,
}

static public class PhotonHelper
{
    static public bool IsReady_Room(this Player pPlayer)
    {
        object bIsReady;
        if (pPlayer.CustomProperties.TryGetValue(nameof(ECustomPropertiesName.RoomPlayer_IsReady), out bIsReady))
            return (bool)bIsReady;
        else
            return false;
    }

    static public void DoSetReady_Room(this Player pPlayer, bool bIsReady)
    {
        pPlayer.DoSetProperties(nameof(ECustomPropertiesName.RoomPlayer_IsReady), bIsReady );
    }

    static public void DoSetProperties(this Player pPlayer, string strKey, object pValue)
    {
        Hashtable pTable = new Hashtable
        {
            { strKey, pValue}
        };

        pPlayer.SetCustomProperties(pTable);
    }

}
#endif