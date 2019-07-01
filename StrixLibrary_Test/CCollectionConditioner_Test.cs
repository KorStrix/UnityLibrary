using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CCollectionConditioner_Test : CCollectionConditionerBase<CCollectionConditioner_Test.Element, CCollectionConditioner_Test.Test_Recipe>
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

        // ========================================================================== //

        [Test]
        public void CalculateTest()
        {
            CCollectionConditioner_Test pTester = new GameObject(nameof(CalculateTest)).AddComponent<CCollectionConditioner_Test>();

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

        [Test]
        public void CalculateTest2()
        {
            CCollectionConditioner_Test pTester = new GameObject(nameof(CalculateTest)).AddComponent<CCollectionConditioner_Test>();

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

}
