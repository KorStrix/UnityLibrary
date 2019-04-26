using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StrixSO/Example/" + nameof(InventoryItem_Example))]
public class InventoryItem_Example : ScriptableObject, IInventorySlotData
{
    [Rename_Inspector("아이템 이미지")]
    public Sprite pSprite_ItemImage;
    [Rename_Inspector("아이템 이름")]
    public string strItemText;
    [Rename_Inspector("아이템이 겹쳐지는지")]
    public bool bIsPossibleOverlap;
    [Rename_Inspector("아이템 설명")]
    public string strItemExplane;

    public Sprite IInventorySlotData_GetItemIcon()
    {
        return pSprite_ItemImage;
    }

    public string IInventorySlotData_GetItemName()
    {
        return strItemText;
    }

    public bool IInventorySlotData_IsPossibleOverlap()
    {
        return bIsPossibleOverlap;
    }
}
