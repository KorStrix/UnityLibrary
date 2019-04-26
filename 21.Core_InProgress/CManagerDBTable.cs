#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-08 오후 10:10:22
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public class CManagerDBTable<CLASS_DRIVEN, CLASS_DB> : CSingletonMonoBase<CLASS_DRIVEN>
    where CLASS_DRIVEN : CManagerDBTable<CLASS_DRIVEN, CLASS_DB>
    where CLASS_DB : ScriptableObject, IDictionaryItem<string>, new()
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public List<CLASS_DB> p_list_DBContainer { get; private set; } = new List<CLASS_DB>();

    /* protected & private - Field declaration         */

#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    protected Dictionary<string, CLASS_DB> _mapDBContainer = new Dictionary<string, CLASS_DB>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInit(string strDBDataArray)
    {
        p_list_DBContainer.Clear();
        _mapDBContainer.Clear();

        CLASS_DB[] arrDBData = JsonUtilityExtension.DoReadJsonArray_ScriptableObject<CLASS_DB>(strDBDataArray);
        for (int i = 0; i < arrDBData.Length; i++)
        {
            CLASS_DB pData = arrDBData[i];
            string strDBDataKey = pData.IDictionaryItem_GetKey();
            if (string.IsNullOrEmpty(strDBDataKey))
                continue;

            pData.name = strDBDataKey;
            OnInitDBContainer(strDBDataKey, pData);

            _mapDBContainer.Add(strDBDataKey, pData);
            p_list_DBContainer.Add(pData);
        }
    }

    public CLASS_DB GetData(string strDBKey)
    {
        if (_mapDBContainer.ContainsKey_PrintOnError(strDBKey))
            return _mapDBContainer[strDBKey];
        else
            return null;
    }
    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */

    virtual protected void OnInit(CLASS_DB[] arrDBData)
    {

    }

    virtual protected void OnInitDBContainer(string strDBKey, CLASS_DB pData)
    {

    }

    // ========================================================================== //

    #region Private

    #endregion Private
}