#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-02 오후 7:24:22
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class InventorySystemExample : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("처음 인벤토리에 넣을 아이템들")]
    public List<InventoryItem_Example> p_listInventoryExample = new List<InventoryItem_Example>();

    /* protected & private - Field declaration         */

    List<IInventorySlotData> _listSlotData = new List<IInventorySlotData>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        CUGUIInventory pInventoryPanel = FindObjectOfType<CUGUIInventory>();
        pInventoryPanel.EventOnAwake();
        pInventoryPanel.DoSetInvetoryItem(p_listInventoryExample);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}