#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-23 오후 6:36:41
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
#endif
#endif

public interface IRecipe<CLASS_ELEMENT> : IHasName
{
    void IRecipe_IsMatch(IEnumerable<CLASS_ELEMENT> arrElement, ref List<CLASS_ELEMENT> listElementMatched_Default_IsCountZero);
    void IRecipe_CombineResult(IEnumerable<CLASS_ELEMENT> arrElement_Matched, ref List<CLASS_ELEMENT> listElementResult_Default_IsCountZero);
}

/// <summary>
/// 
/// </summary>
public class CCollectionConditionerBase<CLASS_ELEMENT, CLASS_RECIPE> : CObjectBase
    where CLASS_RECIPE : IRecipe<CLASS_ELEMENT>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class CRecipeContainer
    {
        public CLASS_RECIPE p_pRecipeInstance { get; private set; }

        public CRecipeContainer(CLASS_RECIPE pRecipeInstance)
        {
            p_pRecipeInstance = pRecipeInstance;
        }
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    [SerializeField]
    List<CLASS_RECIPE> _listRecipe_Instance = new List<CLASS_RECIPE>();

    List<CLASS_ELEMENT> _listElement_Temp = new List<CLASS_ELEMENT>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInit(List<CLASS_RECIPE> listRecipe)
    {
        _listRecipe_Instance.AddRange(listRecipe);
    }

    public bool DoCalculate_Is(IEnumerable<CLASS_ELEMENT> arrElement, ref List<CLASS_RECIPE> listRecipe_Possible)
    {
        listRecipe_Possible.Clear();
        for (int i = 0; i < _listRecipe_Instance.Count; i++)
        {
            _listElement_Temp.Clear();
            CLASS_RECIPE pCurrentRecipe = _listRecipe_Instance[i];
            pCurrentRecipe.IRecipe_IsMatch(arrElement, ref _listElement_Temp);
            if (_listElement_Temp.Count != 0)
                listRecipe_Possible.Add(pCurrentRecipe);
        }

        return false;
    }

    public void DoCombine(IEnumerable<CLASS_ELEMENT> arrElement, CLASS_RECIPE pRecipe, ref List<CLASS_ELEMENT> listCombineResult)
    {
        pRecipe.IRecipe_CombineResult(arrElement, ref listCombineResult);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}

#if ODIN_INSPECTOR
#if UNITY_EDITOR

public class CCollectionConditioner_Drawer<CLASS_DERIVED, CLASS_ELEMENT, CLASS_RECIPE> : OdinValueDrawer<CLASS_DERIVED>
    where CLASS_DERIVED : CCollectionConditionerBase<CLASS_ELEMENT, CLASS_RECIPE>
    where CLASS_RECIPE : IRecipe<CLASS_ELEMENT>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if(GUILayout.Button("Test"))
        {

        }

        base.CallNextDrawer(label);
    }
}

#endif
#endif

