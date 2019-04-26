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
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
#endif

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
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

    [ShowInInspector]
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

#region Test
#if UNITY_EDITOR

public class CCollectionConditionerTester : CCollectionConditionerBase<CCollectionConditionerTester.Element, CCollectionConditionerTester.Test_Recipe>
{
    public class Element
    {
        public string strElementName;

        public Element(string strElementName)
        {
            this.strElementName = strElementName;
        }
    }

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
            foreach(var pElement in arrElement)
            {
                int iValue;
                if(int.TryParse(pElement.strElementName, out iValue))
                {
                    if(iValue % 2 == 0)
                        listElementMatched.Add(pElement);
                }
            }
        }
    }

    // ========================================================================== //

    [Test]
    public void CalculateTest()
    {
        CCollectionConditionerTester pTester = new GameObject(nameof(CalculateTest)).AddComponent<CCollectionConditionerTester>();

        List<Test_Recipe> listRecipe = new List<Test_Recipe>();
        listRecipe.Add(new Recipe_IsEventNumber_AndOnlyOne());

        List<Test_Recipe> listRecipe_Return = new List<Test_Recipe>();
        IEnumerable<Element> arrElements = new Element[] { new Element("1"), new Element("2"), new Element("3") };
        pTester.DoInit(listRecipe);
        pTester.DoCalculate_Is(arrElements, ref listRecipe_Return);

        Assert.AreEqual(listRecipe_Return.Count, 1);

        List<Element> listCombineResult = new List<Element>();
        listRecipe_Return[0].IRecipe_CombineResult(arrElements, ref listCombineResult);

        Assert.AreEqual(listCombineResult.Count, 1);
        Assert.AreEqual(listCombineResult[0].strElementName, "2");
    }

    // ========================================================================== //

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
                for(int j = i + 1; j < arrElement.Length; j++)
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

    [Test]
    public void CalculateTest2()
    {
        CCollectionConditionerTester pTester = new GameObject(nameof(CalculateTest)).AddComponent<CCollectionConditionerTester>();

        List<Test_Recipe> listRecipe = new List<Test_Recipe>();
        listRecipe.Add(new Recipe_IsOddNumber_AndCombine());
        List<Test_Recipe> listRecipe_Return = new List<Test_Recipe>();
        IEnumerable<Element> arrElements = new Element[] { new Element("1"), new Element("2"), new Element("3"), new Element("4"), new Element("5") };
        pTester.DoInit(listRecipe);
        pTester.DoCalculate_Is(arrElements, ref listRecipe_Return);

        Assert.AreEqual(listRecipe_Return.Count, 1);

        List<Element> listCombineResult = new List<Element>();
        listRecipe_Return[0].IRecipe_CombineResult(arrElements, ref listCombineResult);

        Assert.AreEqual(listCombineResult.Count, 3);
        Assert.AreEqual(listCombineResult[0].strElementName, "13");
        Assert.AreEqual(listCombineResult[1].strElementName, "15");
        Assert.AreEqual(listCombineResult[2].strElementName, "35");
    }
}

#endif
#endregion Test

#if UNITY_EDITOR

public class CCollectionConditioner_Drawer<CLASS_DRIVEN, CLASS_ELEMENT, CLASS_RECIPE> : OdinValueDrawer<CLASS_DRIVEN>
    where CLASS_DRIVEN : CCollectionConditionerBase<CLASS_ELEMENT, CLASS_RECIPE>
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

