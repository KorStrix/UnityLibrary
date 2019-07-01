#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-02 오후 6:45:39
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CUGUIInventory : CUIObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("슬롯 오리지널")]
    public CUGUIInventorySlot p_pSlotOrigin = null;

    public List<CUGUIInventorySlot> p_listInventorySlot { get; private set; } = new List<CUGUIInventorySlot>();

    /* protected & private - Field declaration         */

    private Transform _pTransform_Grid_Slot;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoUpdate_Slot()
    {
        p_listInventorySlot.Clear();
        GetComponentsInChildren(p_listInventorySlot);
    }

    public void DoSetInvetoryItem(List<IInventorySlotData> listSlotData, bool bIsAutoGenerateSlot = false)
    {
        DoSetInvetoryItem<IInventorySlotData>(listSlotData, bIsAutoGenerateSlot);
    }

    public void DoSetInvetoryItem<T>(List<T> listSlotData, bool bIsAutoGenerateSlot = false)
        where T : IInventorySlotData
    {
        for(int i = 0; i < p_listInventorySlot.Count; i++)
            p_listInventorySlot[i].DoSet_InventorySlot(null);

        if (listSlotData == null)
            return;

        for(int i = 0; i < listSlotData.Count; i++)
        {
            CUGUIInventorySlot pSlot = null;
            if (i >= p_listInventorySlot.Count)
            {
                if (bIsAutoGenerateSlot == false)
                    break;

                pSlot = DoCreate_NewSlot();
            }
            else
                pSlot = p_listInventorySlot[i];

            pSlot.DoSet_InventorySlot(listSlotData[i]);
        }
    }

    public CUGUIInventorySlot DoCreate_NewSlot()
    {
        if(p_pSlotOrigin == null)
        {
            Debug.LogError(name + "DoCreate_NewSlot - p_pSlotOrigin == null", this);
            return null;
        }

        GameObject pObjectSlot = Instantiate(p_pSlotOrigin.gameObject);
        pObjectSlot.transform.SetParent(_pTransform_Grid_Slot);
        pObjectSlot.transform.DoResetTransform();
        pObjectSlot.SetActive(true);
        pObjectSlot.gameObject.name = "Slot_" + p_listInventorySlot.Count;

        CUGUIInventorySlot pNewSlot = pObjectSlot.GetComponent<CUGUIInventorySlot>();
        p_listInventorySlot.Add(pNewSlot);

        return pNewSlot;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        Init_SlotOrigin();
        DoUpdate_Slot();
        for (int i = 0; i < p_listInventorySlot.Count; i++)
            p_listInventorySlot[i].EventOnAwake();

        DoSetInvetoryItem(null);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Init_SlotOrigin()
    {
        if (p_pSlotOrigin)
        {
            _pTransform_Grid_Slot = p_pSlotOrigin.transform.parent;
            p_pSlotOrigin.SetActive(false);
        }
    }

    #endregion Private
}