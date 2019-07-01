#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-06 오후 8:52:59
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace StrixLibrary_Example
{
    /// <summary>
    /// 
    /// </summary>
    public class RelatedByElementsList_Example_Calculator : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum ECalculateType
        {
            Nothing,
            [RegistSubString("+")]
            Plus,
            [RegistSubString("-")]
            Minus,
            [RegistSubString("*")]
            Multi,
            [RegistSubString("/")]
            Division
        }

        [System.Serializable]
        public class Calculator : CRelateByOther<Calculator, ECalculateType, int>
        {
            public int iTest;

            public override ValueDropdownList<ECalculateType> IRelateByOther_GetEditorDisplayNameList()
            {
                return ValueDropdownListHelper.Create_Enum_ValueDropdownList<ECalculateType>();
            }

            public override bool IRelateByOther_IsRequireOtherItem()
            {
                return pRelateType != ECalculateType.Nothing;
            }

            public override string IRelateByOther_GetDisplayName()
            {
                return iTest.ToString();
            }

            public override int GetResult()
            {
                return iTest;
            }

            public override string IRelateByOther_GetDisplayRelateName(ECalculateType pRelateType)
            {
                return pRelateType.ToStringSub();
            }
        }

        [System.Serializable]
        public class CustomList_CustomClass : RelatedByElementsListBase<Calculator, ECalculateType, int>
        {
            protected override int Calculate_Relation(int pResult_A, ECalculateType pRelateType, int pResult_B)
            {
                Debug.Log($"Calculate_Relation : {pResult_A} {pRelateType} {pResult_B}");

                switch (pRelateType)
                {
                    case ECalculateType.Nothing: return pResult_A;
                    case ECalculateType.Plus: return pResult_A + pResult_B;
                    case ECalculateType.Minus: return pResult_A - pResult_B;
                    case ECalculateType.Multi: return pResult_A * pResult_B;
                    case ECalculateType.Division: return pResult_A / pResult_B;
                }

                return 0;
            }

            protected override bool Check_IsPriority_Relation(ECalculateType pRelateType)
            {
                return pRelateType == ECalculateType.Multi || pRelateType == ECalculateType.Division;
            }
        }

        /* public - Field declaration            */

        public CustomList_CustomClass listCustom_Class = new CustomList_CustomClass();

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


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RelatedByElementsList_Example_Calculator.CustomList_CustomClass))]
    public class CustomList_Example_Drawer : RelatedByElementsList_Drawer<RelatedByElementsList_Example_Calculator.Calculator, RelatedByElementsList_Example_Calculator.ECalculateType, int>
    {
        int _iResult;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            position.y += position.height - (EditorCodeHelper.singleLineHeight * 2);
            position.height = EditorCodeHelper.singleLineHeight;
            if (GUI.Button(position, "Calculate Result"))
            {
                var listCalculator = GetDrawTargetList(property);
                _iResult = listCalculator.DoCalculate_Relate();
            }
            position.y += EditorCodeHelper.singleLineHeight;

            EditorCodeHelper.LabelField(position, "Result: " + _iResult);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorCodeHelper.singleLineHeight * 2;
        }
    }

#endif
}