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
    public class RelatedByElementsList_Example_Bool : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        // Bool 과 Bool 사이의 관계를 정의합니다.
        // SubString을 통해 간단하게 Display합니다.
        public enum EBoolCalculateType
        {
            Nothing,
            [RegistSubString("&&")]
            And,
            [RegistSubString("||")]
            Or,
            [RegistSubString("^")]
            Xor,
        }

        // 계산기를 정의합니다.
        // 계산기는 CRelateByOther를 상속받아야 합니다.
        [System.Serializable]
        public class BoolCalculator : CRelateByOther<BoolCalculator, EBoolCalculateType, bool>
        {
            public bool bValue;

            public override ValueDropdownList<EBoolCalculateType> IRelateByOther_GetEditorDisplayNameList()
            {
                return OdinExtension.GetValueDropDownList_EnumSubString<EBoolCalculateType>();
            }

            public override bool IRelateByOther_IsRequireOtherItem()
            {
                return pRelateType != EBoolCalculateType.Nothing;
            }

            public override string IRelateByOther_GetDisplayName()
            {
                return bValue.ToString();
            }

            public override bool GetResult()
            {
                return bValue;
            }

            public override string IRelateByOther_GetDisplayRelateName(EBoolCalculateType pRelateType)
            {
                return pRelateType.ToStringSub();
            }
        }

        [System.Serializable]
        public class BoolCalculator_List : RelatedByElementsListBase<BoolCalculator, EBoolCalculateType, bool>
        {
            protected override bool Calculate_Relation(bool pResult_A, EBoolCalculateType pRelateType, bool pResult_B)
            {
                Debug.Log($"Calculate_Relation : {pResult_A} {pRelateType} {pResult_B}");

                switch (pRelateType)
                {
                    case EBoolCalculateType.And: return pResult_A && pResult_B;
                    case EBoolCalculateType.Or: return pResult_A || pResult_B;
                    case EBoolCalculateType.Xor: return pResult_A ^ pResult_B;
                }

                return pResult_A;
            }
        }

        /* public - Field declaration            */

        public BoolCalculator_List listCalculator = new BoolCalculator_List();

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

    [CustomPropertyDrawer(typeof(RelatedByElementsList_Example_Bool.BoolCalculator_List))]
    public class BoolCalculator_List_Drawer : RelatedByElementsList_Drawer<RelatedByElementsList_Example_Bool.BoolCalculator, RelatedByElementsList_Example_Bool.EBoolCalculateType, bool>
    {
        bool _bResult;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            position.y += position.height - (EditorCodeHelper.singleLineHeight * 2);
            position.height = EditorCodeHelper.singleLineHeight;
            if (GUI.Button(position, "Calculate Result"))
            {
                var listCalculator = GetDrawTargetList(property);
                _bResult = listCalculator.DoCalculate_Relate();
            }
            position.y += EditorCodeHelper.singleLineHeight;

            EditorCodeHelper.LabelField(position, "Result: " + _bResult);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorCodeHelper.singleLineHeight * 2;
        }
    }

#endif
}