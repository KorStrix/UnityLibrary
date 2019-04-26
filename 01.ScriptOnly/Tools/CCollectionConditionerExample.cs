#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-23 오후 6:48:14
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CCollectionConditionerExample : CCollectionConditionerBase<CCollectionConditionerExample.Element, CCollectionConditionerExample.Test_Recipe>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public abstract class Test_Recipe : IRecipe<Element>
    {
        public string IHasName_GetName()
        {
            return GetType().GetFriendlyName();
        }

        public abstract void IRecipe_CombineResult(IEnumerable<Element> arrElement_Matched, ref List<Element> listElementResult_Default_IsCountZero);
        public abstract void IRecipe_IsMatch(IEnumerable<Element> arrElement, ref List<Element> listElementMatched);
    }

    /// <summary>
    /// 숫자 중에 홀수 넘버만 2개씩 합칩니다.
    /// </summary>
    public class Recipe_IsOddNumber_AndCombine : Test_Recipe
    {
        public override void IRecipe_CombineResult(IEnumerable<Element> arrElement_Matched, ref List<Element> listElementResult_Default_IsCountZero)
        {
            IRecipe_IsMatch(arrElement_Matched, ref listElementResult_Default_IsCountZero);
            Element[] arrElement = listElementResult_Default_IsCountZero.ToArray();
            listElementResult_Default_IsCountZero.Clear();

            for (int i = 0; i < arrElement.Length; i++)
            {
                for (int j = i + 1; j < arrElement.Length; j++)
                {
                    listElementResult_Default_IsCountZero.Add(new Element(arrElement[i].strElementName + arrElement[j].strElementName));
                }
            }
        }

        public override void IRecipe_IsMatch(IEnumerable<Element> arrElement, ref List<Element> listElementMatched)
        {
            foreach (var pElement in arrElement)
            {
                int iValue;
                if (int.TryParse(pElement.strElementName, out iValue))
                {
                    if (iValue % 2 == 1)
                        listElementMatched.Add(pElement);
                }
            }
        }
    }

    /// <summary>
    /// 짝수만 하나씩 분리합니다.
    /// </summary>
    public class Recipe_IsEventNumber_AndOnlyOne : Test_Recipe
    {
        public override void IRecipe_CombineResult(IEnumerable<Element> arrElement_Matched, ref List<Element> listElementResult_Default_IsCountZero)
        {
            IRecipe_IsMatch(arrElement_Matched, ref listElementResult_Default_IsCountZero);
        }

        public override void IRecipe_IsMatch(IEnumerable<Element> arrElement, ref List<Element> listElementMatched)
        {
            foreach (var pElement in arrElement)
            {
                int iValue;
                if (int.TryParse(pElement.strElementName, out iValue))
                {
                    if (iValue % 2 == 0)
                        listElementMatched.Add(pElement);
                }
            }
        }
    }

    public class Element
    {
        public string strElementName;

        public Element(string strElementName)
        {
            this.strElementName = strElementName;
        }
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}