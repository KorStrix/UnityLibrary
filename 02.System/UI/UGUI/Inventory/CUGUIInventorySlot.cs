#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-02 오후 6:45:59
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

public interface IInventorySlotData
{
    Sprite IInventorySlotData_GetItemIcon();
    string IInventorySlotData_GetItemName();
    bool IInventorySlotData_IsPossibleOverlap();
}

/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
public class CUGUIInventorySlot : CUIObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EUIElementName_ForInit
    {
        Image_SlotItemIcon,
        Text_SlotItemName,
    }

    /* public - Field declaration            */

    [Rename_Inspector("슬롯 인덱스", false)]
    public int p_iSlotIndex;

#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    [Rename_Inspector("현재 데이터")]
    public IInventorySlotData p_pInventoryData { get; private set; }

    [Header("필요한 UI Element")] [Space(10)]

    [ChildRequireComponent(nameof(EUIElementName_ForInit.Text_SlotItemName), bIsPrint_OnNotFound = false)]
    public Text p_pText_ItemName;
    [ChildRequireComponent(nameof(EUIElementName_ForInit.Image_SlotItemIcon), bIsPrint_OnNotFound = false)]
    public Image p_pImage_ItemIcon;

    /* protected & private - Field declaration         */


    [Header("디버깅용")]
    [SerializeField]
    [Rename_Inspector("선택 되었는지", false)]
    bool _bSelected;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_CurrentSelected(bool bSelected)
    {
        _bSelected = bSelected;
    }

    public void DoSet_InventorySlot(IInventorySlotData pInventoryData)
    {
        p_pInventoryData = pInventoryData;

        OnUpdate_InventorySlot(p_pInventoryData);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

#if UNITY_EDITOR
    private void Update()
    {
        p_iSlotIndex = transform.GetSiblingIndex();
    }
#endif

    /* protected - [abstract & virtual]         */

    virtual protected void OnUpdate_InventorySlot(IInventorySlotData pInventoryData)
    {
        if(pInventoryData != null)
        {
            if(p_pText_ItemName) p_pText_ItemName.text = pInventoryData.IInventorySlotData_GetItemName();
            if (p_pImage_ItemIcon)
            {
                p_pImage_ItemIcon.enabled = true;
                p_pImage_ItemIcon.sprite = pInventoryData.IInventorySlotData_GetItemIcon();
            }
        }
        else
        {
            if (p_pText_ItemName) p_pText_ItemName.text = "Empty Slot";
            if (p_pImage_ItemIcon) p_pImage_ItemIcon.enabled = false;
        }
    }

    // ========================================================================== //

#region Private

#endregion Private
}