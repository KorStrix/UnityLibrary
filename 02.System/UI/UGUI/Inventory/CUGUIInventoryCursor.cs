#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-02 오후 7:43:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CUGUIInventoryCursor : CSingletonDynamicMonoBase<CUGUIInventoryCursor>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EUIElementName_ForInit
    {
        Image_SlotItem,
    }

    /* public - Field declaration            */

    [Header("필요한 UI Element")] [Space(10)]
    [GetComponentInChildren(nameof(EUIElementName_ForInit.Image_SlotItem), bIsPrint_OnNotFound = false)]
    [Rename_Inspector("Image `" + nameof(EUIElementName_ForInit.Image_SlotItem) + "`", false)]
    public Image p_pImage_CurrentItem;

    /* protected & private - Field declaration         */

    private Camera _pCamera;
    private Canvas _pRootCanvas;
    private RectTransform _pRecTransRootCanvas;

    [Header("디버깅용")]
    [SerializeField]
    [Rename_Inspector("현재 데이터", false)]
    IInventorySlotData _pCurrentSlotData;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoUpdatePosition(Vector3 v3CursorPos)
    {
        Vector2 v2ConvertPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_pRecTransRootCanvas, v3CursorPos, _pCamera, out v2ConvertPos);

        transform.position = _pRecTransRootCanvas.TransformPoint(v2ConvertPos);
    }

    public void DoSet_InventorySlotItem(IInventorySlotData pInventorySlotData)
    {
        _pCurrentSlotData = pInventorySlotData;

        if(pInventorySlotData != null)
        {
            if(p_pImage_CurrentItem) p_pImage_CurrentItem.sprite = pInventorySlotData.IInventorySlotData_GetItemIcon();
        }
        else
        {
            p_pImage_CurrentItem.sprite = null;
        }

    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        this.GetComponentInChildren(out p_pImage_CurrentItem);
        p_pImage_CurrentItem.raycastTarget = false;

        _pRootCanvas = GetComponentInParent<Canvas>();
        _pRecTransRootCanvas = (RectTransform)_pRootCanvas.transform;
        _pCamera = _pRootCanvas.worldCamera;

        gameObject.SetActive(false);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}