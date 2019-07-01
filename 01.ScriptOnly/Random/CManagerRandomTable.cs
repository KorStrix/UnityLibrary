using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-03 오후 11:48:05
   Description : 
   ============================================ */

public interface IRandomItem
{
    int IRandomItem_GetPercent();
}

public static class CExtension_Enumerator_Random
{
    static public Class_Random GetRandomItem<Class_Random>(this List<Class_Random> pIter)
        where Class_Random : class, IRandomItem
    {
        return CManagerRandomTable<Class_Random>.instance.GetRandomItem(pIter);
    }
}

public class CManagerRandomTable<CLASS_Resource>
    where CLASS_Resource : class, IRandomItem
{
    static private CManagerRandomTable<CLASS_Resource> _instance;

    static public CManagerRandomTable<CLASS_Resource> instance
    {
        get
        {
            if (_instance == null)
                _instance = new CManagerRandomTable<CLASS_Resource>();

            return _instance;
        }
    }

    /* enum & struct declaration                */

    public enum ERandomGetMode
    {
        Peek,
        Delete,
    }

    /* private - Field declaration           */

    private List<CLASS_Resource> _listRandomTable = new List<CLASS_Resource>();
    private List<CLASS_Resource> _listRandomTable_Delete = new List<CLASS_Resource>();
    private HashSet<CLASS_Resource> _setWinTable_OnDelete = new HashSet<CLASS_Resource>();

    private ERandomGetMode _eRandomGetMode = ERandomGetMode.Peek;

    private int _iTotalValue = 0;
    private int _iTotalValue_Decrease_OnDelete = 0;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoClearRandomItemTable()
    {
        _listRandomTable.Clear();
        _iTotalValue = 0;
    }

    public void DoAddRandomItem(CLASS_Resource pRandomItem, bool bIsSort = true)
    {
        _listRandomTable.Add(pRandomItem);
        int iPercent = pRandomItem.IRandomItem_GetPercent();
        if (iPercent <= 0)
        {
            Debug.LogWarning("Percent is less or equal to zero" + pRandomItem.ToString());
            return;
        }
        _iTotalValue += iPercent;

        if (_eRandomGetMode == ERandomGetMode.Delete)
            _listRandomTable_Delete.Add(pRandomItem);

        if (bIsSort)
            ProcSortList();
    }

    public void DoAddRandomItem_Range(IEnumerable<CLASS_Resource> listRandomItem)
    {
        IEnumerator<CLASS_Resource> pIter = listRandomItem.GetEnumerator();
        while (pIter.MoveNext())
        {
            DoAddRandomItem(pIter.Current, false);
        }

        ProcSortList();
    }

    public CLASS_Resource GetItem_AsMono(string strItemName)
    {
        for (int i = 0; i < _listRandomTable.Count; i++)
        {
            MonoBehaviour pMono = _listRandomTable[i] as MonoBehaviour;
            if (pMono != null)
            {
                if (pMono.name.Equals(strItemName))
                    return _listRandomTable[i];
            }
        }

        return null;
    }

    public CLASS_Resource GetRandomItem(List<CLASS_Resource> listRandomItem)
    {
        int iMaxValue = 0;
        for (int i = 0; i < listRandomItem.Count; i++)
            iMaxValue += listRandomItem[i].IRandomItem_GetPercent();

        return ProcGetRandomItem(iMaxValue, listRandomItem);
    }

    public CLASS_Resource GetRandomItem(List<CLASS_Resource> listRandomItem, int iLimitValue)
    {
        int iMaxValue = 0;
        for (int i = 0; i < listRandomItem.Count; i++)
            iMaxValue += listRandomItem[i].IRandomItem_GetPercent();

        return ProcGetRandomItem(iLimitValue, GetRandomList_CutGreaterMaxValue(iLimitValue, listRandomItem));
    }

    public CLASS_Resource GetRandomItem()
    {
        if (_eRandomGetMode == ERandomGetMode.Delete)
            return ProcGetRandomItem_Delete(_iTotalValue);
        else
            return ProcGetRandomItem_Peek(_iTotalValue);
    }

    public CLASS_Resource GetRandomItem(int iMaxValue)
    {
        CLASS_Resource pRandomItem = null;
        List<CLASS_Resource> listRandom;
        if (_eRandomGetMode == ERandomGetMode.Delete)
            listRandom = GetRandomList_CutGreaterMaxValue(iMaxValue, _listRandomTable_Delete);
        else
            listRandom = GetRandomList_CutGreaterMaxValue(iMaxValue, _listRandomTable);

        if (listRandom.Count != 0)
        {
            int iRandomIndex = Random.Range(0, listRandom.Count);
            if (_eRandomGetMode == ERandomGetMode.Delete)
            {
                do
                {
                    if (_setWinTable_OnDelete.Contains(listRandom[iRandomIndex]))
                    {
                        listRandom.RemoveAt(iRandomIndex);
                        iRandomIndex = Random.Range(0, listRandom.Count);
                        continue;
                    }
                    else
                        break;

                } while (_setWinTable_OnDelete.Contains(listRandom[iRandomIndex]) && listRandom.Count != 0);

                pRandomItem = listRandom[iRandomIndex];
                ProcAddDeleteItem(pRandomItem);
            }
            else
                pRandomItem = ProcGetRandomItem(iMaxValue, listRandom);
        }

        return pRandomItem;
    }

    public void DoSetRandomMode(ERandomGetMode eRandomMode)
    {
        _eRandomGetMode = eRandomMode;
    }

    public void DoReset_OnDeleteMode()
    {
        _setWinTable_OnDelete.Clear();
        _iTotalValue_Decrease_OnDelete = 0;
        _listRandomTable_Delete.Clear();
        _listRandomTable_Delete.AddRange(_listRandomTable);
    }

    public float DoCalculatePercent(CLASS_Resource pItem, int iMaxValue = 0)
    {
        if (iMaxValue == 0)
            iMaxValue = _iTotalValue;

        int iItemPercentValue = pItem.IRandomItem_GetPercent();
        float fPercent = (float)iItemPercentValue / (float)_iTotalValue;

        return fPercent * 100;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcSortList()
    {
        _listRandomTable.Sort(ComparerRandomItem);

        if (_eRandomGetMode == ERandomGetMode.Delete)
            _listRandomTable_Delete.Sort(ComparerRandomItem);
    }

    private CLASS_Resource ProcGetRandomItem_Peek(int iMaxValue)
    {
        if (iMaxValue > _iTotalValue)
            iMaxValue = _iTotalValue;

        return ProcGetRandomItem(iMaxValue, _listRandomTable);
    }

    private CLASS_Resource ProcGetRandomItem_Delete(int iMaxValue)
    {
        int iTotalValue = iMaxValue - _iTotalValue_Decrease_OnDelete;
        if (iMaxValue > iTotalValue)
            iMaxValue = iTotalValue;

        CLASS_Resource pRandomItem = ProcGetRandomItem(iMaxValue, _listRandomTable_Delete);
        ProcAddDeleteItem(pRandomItem);

        return pRandomItem;
    }

    private CLASS_Resource ProcGetRandomItem(int iMaxValue, List<CLASS_Resource> listTable)
    {
        if (listTable.Count == 1)
            return listTable[0];

        CLASS_Resource pRandomItem = null;
        int iRandomValue = Random.Range(0, iMaxValue);
        int iCheckValue = 0;
        for (int i = 0; i < listTable.Count; i++)
        {
            CLASS_Resource pRandomItemCurrent = listTable[i];
            iCheckValue += pRandomItemCurrent.IRandomItem_GetPercent();
            if (iRandomValue < iCheckValue)
            {
                pRandomItem = pRandomItemCurrent;
                break;
            }
        }

        return pRandomItem;
    }

    private List<CLASS_Resource> _listRandomTable_Temp = new List<CLASS_Resource>();
    private List<CLASS_Resource> GetRandomList_CutGreaterMaxValue(int iMaxValue, List<CLASS_Resource> listTable)
    {
        _listRandomTable_Temp.Clear();
        for (int i = 0; i < listTable.Count; i++)
        {
            CLASS_Resource pRandomItemCurrent = listTable[i];
            if (pRandomItemCurrent.IRandomItem_GetPercent() <= iMaxValue)
                _listRandomTable_Temp.Add(pRandomItemCurrent);
            else
                break;
        }

        return _listRandomTable_Temp;
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

    private int ComparerRandomItem(CLASS_Resource pResourceX, CLASS_Resource pResourceY)
    {
        int iValueX = pResourceX.IRandomItem_GetPercent();
        int iValueY = pResourceY.IRandomItem_GetPercent();

        return iValueX.CompareTo(iValueY);
    }

    private void ProcAddDeleteItem(CLASS_Resource pItem)
    {
        if (pItem == null) return;

        _listRandomTable_Delete.Remove(pItem);
        _iTotalValue_Decrease_OnDelete += pItem.IRandomItem_GetPercent();
        _setWinTable_OnDelete.Add(pItem);
    }
}