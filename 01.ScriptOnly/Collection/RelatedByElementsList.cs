#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-06 오후 8:31:39
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

// Interface로 구현하고 싶었으나, Interface로 Getter/Setter를 통한 pRelateOther, eRelateType이 값이 저장이 안되서 클래스로 구현
public abstract class CRelateByOther<CLASS_DERIVED, TYPE_ITEM_RELATE, TYPE_RESULT>
    where CLASS_DERIVED : CRelateByOther<CLASS_DERIVED, TYPE_ITEM_RELATE, TYPE_RESULT>
{
    [HideInInspector]
    public int iBracket_Start_Count;
    [HideInInspector]
    public int iBracket_Finish_Count;

    [HideInInspector]
    public bool bEnable = true;

    [HideInInspector]
    public TYPE_ITEM_RELATE pRelateType;

    /// <summary>
    /// 다른 아이템이 필요한지 체크합니다.
    /// </summary>
    /// <returns></returns>
    abstract public bool IRelateByOther_IsRequireOtherItem();

    /// <summary>
    /// 요소의 Display할 이름을 얻습니다.
    /// </summary>
    /// <returns></returns>
    abstract public string IRelateByOther_GetDisplayName();

    /// <summary>
    /// 관계의 Display할 이름을 얻습니다.
    /// </summary>
    /// <param name="pRelateType"></param>
    /// <returns></returns>
    abstract public string IRelateByOther_GetDisplayRelateName(TYPE_ITEM_RELATE pRelateType);

    /// <summary>
    /// 해당 List 전체의 Display할 이름을 얻습니다.
    /// </summary>
    /// <returns></returns>
    abstract public ValueDropdownList<TYPE_ITEM_RELATE> IRelateByOther_GetEditorDisplayNameList();

    /// <summary>
    /// List의 식의 결과를 얻습니다.
    /// </summary>
    /// <returns></returns>
    abstract public TYPE_RESULT GetResult();

    public bool IRelateByOther_IsBracket_Start() { return iBracket_Start_Count > 0; }
    public bool IRelateByOther_IsBracket_Finish() { return iBracket_Finish_Count > 0; }
}

/// <summary>
/// Item끼리 관계를 만들 수 있는 List입니다.
/// Unity Serilaize때문에 반드시 상속을 받아 구현해야 합니다.
/// </summary>
[System.Serializable]
public abstract class RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> : IEnumerable<CLASS_ELEMENT>
    where CLASS_ELEMENT : CRelateByOther<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT>
{
    public class RelatedExpressionTree : ExpressionTreeBase<ExpressionElement, TYPE_RESULT, RelateTypeWrapper>
    {
        System.Func<TYPE_RESULT, TYPE_ITEM_RELATE, TYPE_RESULT, TYPE_RESULT> _OnCalculate;
        System.Func<TYPE_ITEM_RELATE, bool> _OnCheck_IsPriority_Relation;
        public void SetCalculateFunc(System.Func<TYPE_RESULT, TYPE_ITEM_RELATE, TYPE_RESULT, TYPE_RESULT> OnCalculate, System.Func<TYPE_ITEM_RELATE, bool> OnCheck_IsPriority_Relation)
        {
            _OnCalculate = OnCalculate;
            _OnCheck_IsPriority_Relation = OnCheck_IsPriority_Relation;
        }

        protected override bool Check_IsOperator(ExpressionElement pElement)
        {
            return pElement.Check_IsOperator();
        }

        protected override bool Check_IsPriority_Operator(RelateTypeWrapper pOperator)
        {
            if (pOperator.eRelateType == RelateTypeWrapper.RelateType.RelateType)
                return _OnCheck_IsPriority_Relation(pOperator.pRelateType);
            else
                return false;
        }

        protected override bool Check_IsValue(ExpressionElement pElement)
        {
            return pElement.Check_IsValue();
        }

        protected override RelateTypeWrapper Convert_Operator(ExpressionElement pElement)
        {
            return pElement.pRelateType;
        }

        protected override TYPE_RESULT Convert_Value(ExpressionElement pElement)
        {
            return pElement.pElement.GetResult();
        }

        protected override RelateTypeWrapper IExpressionElement_GetParenthesis_Close()
        {
            return RelateTypeWrapper.Close_Parenthesis;
        }

        protected override RelateTypeWrapper IExpressionElement_GetParenthesis_Open()
        {
            return RelateTypeWrapper.Open_Parenthesis;
        }

        protected override TYPE_RESULT OnCalculateOperation(TYPE_RESULT pValue_Left, TYPE_RESULT pValue_Right, RelateTypeWrapper pOperator)
        {
            return _OnCalculate(pValue_Left, pOperator.pRelateType, pValue_Right);
        }
    }

    public struct RelateTypeWrapper
    {
        public enum RelateType
        {
            Open_Parenthesis,
            Close_Parenthesis,
            RelateType,
        }

        public RelateType eRelateType;
        public TYPE_ITEM_RELATE pRelateType;

        public RelateTypeWrapper(RelateType eRelateType)
        {
            this.eRelateType = eRelateType;
            pRelateType = default(TYPE_ITEM_RELATE);
        }

        public RelateTypeWrapper(TYPE_ITEM_RELATE pRelateType)
        {
            eRelateType = RelateType.RelateType;
            this.pRelateType = pRelateType;
        }


        static RelateTypeWrapper _Open_Parenthesis = new RelateTypeWrapper(RelateType.Open_Parenthesis);
        static RelateTypeWrapper _Close_Parenthesis = new RelateTypeWrapper(RelateType.Close_Parenthesis);

        static public RelateTypeWrapper Open_Parenthesis => _Open_Parenthesis;
        static public RelateTypeWrapper Close_Parenthesis => _Close_Parenthesis;
    }

    public struct ExpressionElement
    {
        public enum EElementType
        {
            ValueType,
            RelateOperatorType,
        }

        public EElementType eElementType { get; private set; }
        public CLASS_ELEMENT pElement { get; private set; }
        public RelateTypeWrapper pRelateType { get; private set; }

        public ExpressionElement(CLASS_ELEMENT pElement)
        {
            eElementType = EElementType.ValueType;
            this.pElement = pElement;
            this.pRelateType = RelateTypeWrapper.Close_Parenthesis;
        }

        public ExpressionElement(RelateTypeWrapper pRelateType)
        {
            eElementType = EElementType.RelateOperatorType;
            this.pElement = null;
            this.pRelateType = pRelateType;
        }

        public bool Check_IsOperator()
        {
            return eElementType == EElementType.RelateOperatorType;
        }

        public bool Check_IsValue()
        {
            return eElementType == EElementType.ValueType;
        }

        public override string ToString()
        {
            switch (eElementType)
            {
                case EElementType.ValueType: return pElement.GetResult().ToString();
                case EElementType.RelateOperatorType:
                    {
                        switch (pRelateType.eRelateType)
                        {
                            case RelateTypeWrapper.RelateType.Open_Parenthesis: return "(";
                            case RelateTypeWrapper.RelateType.Close_Parenthesis: return ")";
                            case RelateTypeWrapper.RelateType.RelateType: return pRelateType.pRelateType.ToString();
                        }
                    }
                    break;
            }

            return "";
        }
    }

    // ========================================================================================

    public List<CLASS_ELEMENT> p_listElement = new List<CLASS_ELEMENT>();

    List<ExpressionElement> _listExpressionElement = new List<ExpressionElement>();
    RelatedExpressionTree _pExpressionTree = new RelatedExpressionTree();

    // ========================================================================================

    public RelatedByElementsListBase()
    {
        _pExpressionTree.SetCalculateFunc(Calculate_Relation, Check_IsPriority_Relation);
    }

    public void Add(CLASS_ELEMENT pElementA)
    {
        p_listElement.Add(pElementA);
    }

    public virtual string GetElementDisplayName(CLASS_ELEMENT pElement)
    {
        string strDisplayName = pElement.IRelateByOther_GetDisplayName();
        for(int i = 0; i < pElement.iBracket_Start_Count; i++)
            strDisplayName = "(" + strDisplayName;

        for (int i = 0; i < pElement.iBracket_Finish_Count; i++)
            strDisplayName = strDisplayName + ")";

        if (pElement.IRelateByOther_IsRequireOtherItem())
            strDisplayName = $"{strDisplayName} {pElement.IRelateByOther_GetDisplayRelateName(pElement.pRelateType)}";

        return strDisplayName;
    }

    public override string ToString()
    {
        StringBuilder pStrBuilder = new StringBuilder();
        for (int i = 0; i < p_listElement.Count; i++)
        {
            CLASS_ELEMENT pElement = p_listElement[i];
            if (pElement.bEnable == false)
                continue;

            pStrBuilder.Append(GetElementDisplayName(pElement));
            if (i != p_listElement.Count - 1)
                pStrBuilder.Append(" ");
        }

        return pStrBuilder.ToString();
    }

    public TYPE_RESULT DoCalculate_Relate()
    {
        _listExpressionElement.Clear();

        int iIndex = 0;
        CLASS_ELEMENT pEnableElement_First = GetEnableElement_Next(iIndex, ref iIndex);
        if (pEnableElement_First == null)
            return default(TYPE_RESULT);

        CLASS_ELEMENT pEnableElement_Prev = GetEnableElement_Next(iIndex + 1, ref iIndex);
        Add_To_ExpressionList(_listExpressionElement, pEnableElement_First, pEnableElement_Prev, true);
        if(pEnableElement_Prev != null)
        {
            for (int i = iIndex + 1; i < p_listElement.Count; i++)
            {
                CLASS_ELEMENT pEnableElement = GetEnableElement_Next(i, ref i);
                if (pEnableElement != null)
                {
                    Add_To_ExpressionList(_listExpressionElement, pEnableElement_Prev, pEnableElement, false);
                    pEnableElement_Prev = pEnableElement;
                }
            }
        }

        // Debug.Log(_listExpressionElement.ToStringList());
        return _pExpressionTree.EvaluateExpression(_listExpressionElement);
    }

    public CLASS_ELEMENT GetEnableElement_Next(int iStartIndex, ref int iElementIndex)
    {
        for (int i = iStartIndex; i < p_listElement.Count; i++)
        {
            if (p_listElement[i].bEnable)
            {
                iElementIndex = i;
                return p_listElement[i];
            }
        }

        return null;
    }

    public virtual bool Check_IsError()
    {
        int iBracketOpenCount_Total = 0;
        int iBracketCloseCount_Total = 0;

        CLASS_ELEMENT pEnableElement_Last = null;
        for (int i = 0; i < p_listElement.Count; i++)
        {
            CLASS_ELEMENT pElement = GetEnableElement_Next(i, ref i);
            if (pElement.iBracket_Start_Count > 0)
            {
                iBracketOpenCount_Total += pElement.iBracket_Start_Count;

                int iBracketOpenCount_Current = 1;
                int iBracketCloseCount_Current = 0;
                for (int j = i + 1; j < p_listElement.Count; j++)
                {
                    CLASS_ELEMENT pElement_B = GetEnableElement_Next(j, ref j);
                    if (pElement_B == null)
                        break;

                    if (pElement_B.iBracket_Start_Count > 0)
                        iBracketOpenCount_Current += pElement_B.iBracket_Start_Count;

                    if (pElement_B.iBracket_Finish_Count > 0)
                        iBracketCloseCount_Current += pElement_B.iBracket_Finish_Count;

                    if (iBracketOpenCount_Current == iBracketCloseCount_Current)
                        break;
                }
            }

            if (pElement.iBracket_Finish_Count > 0)
                iBracketCloseCount_Total += pElement.iBracket_Finish_Count;

            pEnableElement_Last = pElement;
        }

        if (pEnableElement_Last != null && pEnableElement_Last.IRelateByOther_IsRequireOtherItem())
            return true;

        return iBracketOpenCount_Total != iBracketCloseCount_Total;
    }

    // ========================================================================================

    abstract protected TYPE_RESULT Calculate_Relation(TYPE_RESULT pResult_Left, TYPE_ITEM_RELATE pRelateType, TYPE_RESULT pResult_Right);
    virtual protected bool Check_IsPriority_Relation(TYPE_ITEM_RELATE pRelateType) { return false; }

    // ========================================================================================

    private void Add_To_ExpressionList(List<ExpressionElement> listExpressionElement, CLASS_ELEMENT pEnableElement_Prev, CLASS_ELEMENT pEnableElement_Next, bool bIsFirstElement)
    {
        if(bIsFirstElement)
        {
            for (int i = 0; i < pEnableElement_Prev.iBracket_Start_Count; i++)
                listExpressionElement.Add(new ExpressionElement(RelateTypeWrapper.Open_Parenthesis));

            listExpressionElement.Add(new ExpressionElement(pEnableElement_Prev));
        }

        if (pEnableElement_Prev.IRelateByOther_IsRequireOtherItem())
        {
            listExpressionElement.Add(new ExpressionElement(new RelateTypeWrapper(pEnableElement_Prev.pRelateType)));

            for (int i = 0; i < pEnableElement_Next.iBracket_Start_Count; i++)
                listExpressionElement.Add(new ExpressionElement(RelateTypeWrapper.Open_Parenthesis));
            listExpressionElement.Add(new ExpressionElement(pEnableElement_Next));

            for (int i = 0; i < pEnableElement_Next.iBracket_Finish_Count; i++)
                listExpressionElement.Add(new ExpressionElement(RelateTypeWrapper.Close_Parenthesis));

            if (bIsFirstElement)
            {
                for (int i = 0; i < pEnableElement_Next.iBracket_Finish_Count; i++)
                    listExpressionElement.Add(new ExpressionElement(RelateTypeWrapper.Close_Parenthesis));
            }
        }
    }

    public IEnumerator<CLASS_ELEMENT> GetEnumerator()
    {
        return p_listElement.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return p_listElement.GetEnumerator();
    }
}

// ================================================================================================================================================================================
#region EditorCode
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RelatedByElementsListBase<,,>))]
public class RelatedByElementsList_Drawer<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> : PropertyDrawer
    where CLASS_ELEMENT : CRelateByOther<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT>
{
    const float const_fToggleWidth = 30f;
    const float const_fDrawBracketButton_Padding = 10f;
    const float const_fDrawBracketButton_Gap = 10f;

    public class OnGetReorderableList : ObservableCollection_ChainData<SerializedProperty, ReorderableList>
    {
        public OnGetReorderableList()
        {
            Subscribe += OnGetReorderableList_Default;
        }

        private void OnGetReorderableList_Default(SerializedProperty pProperty, ReorderableList pValue_Origin, ref ReorderableList pValue_Current)
        {
            pValue_Current = RelatedByElementsList_Drawer<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT>.Get_ReorderableList(pProperty);
        }
    }

    static public OnGetReorderableList p_Event_OnGetReorderableList = new OnGetReorderableList();
    ReorderableList _listInput;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty pSP_List = Get_List_SerializedProperty(property);
        if (_listInput == null || SerializedProperty.EqualContents(_listInput.serializedProperty, pSP_List) == false)
            _listInput = p_Event_OnGetReorderableList.DoNotify(property, _listInput);

        bool bIsError = GetDrawTargetList(property).Check_IsError();
        if (bIsError)
        {
            EditorGUI.HelpBox(position, "Error - Check Bracket", MessageType.Error);
            position.y += EditorCodeHelper.singleLineHeight * 2;
        }

        Color pColorOrigin = GUI.color;
        GUI.color = bIsError ? new Color(pColorOrigin.r, pColorOrigin.g * 0.5f, pColorOrigin.b * 0.5f) : pColorOrigin;
        _listInput.DoList(position);
        property.serializedObject.ApplyModifiedProperties();
        GUI.color = pColorOrigin;
    }

    static public ReorderableList Get_ReorderableList(SerializedProperty property)
    {
        RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> listDrawTarget = GetDrawTargetList(property);
        SerializedProperty pSP_List = Get_List_SerializedProperty(property);

        ReorderableList listInput = new ReorderableList(property.serializedObject, pSP_List, true, true, true, true);
        listInput.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var pPropertyElement = listInput.serializedProperty.GetArrayElementAtIndex(index);
            CLASS_ELEMENT pElement = EditorCodeHelper.GetTargetObjectOfProperty(pPropertyElement) as CLASS_ELEMENT;
            if (pElement == null)
                return;

            rect.width -= 15f;
            float fRectWidth_Origin = rect.width;
            rect.x -= EditorGUI.indentLevel * 15f;
            rect.width = const_fToggleWidth - 15f;
            pElement.bEnable = EditorGUI.Toggle(rect, pElement.bEnable);
            rect.x += const_fToggleWidth;
            rect.width = fRectWidth_Origin - rect.width;

            EditorCodeHelper.PropertyField(rect, pPropertyElement, listDrawTarget.GetElementDisplayName(pElement));
            rect.y += EditorGUI.GetPropertyHeight(pPropertyElement, true);

            int iIndetOrigin = EditorGUI.indentLevel;
            if (pPropertyElement.isExpanded)
            {
                EditorGUI.indentLevel++;

                ValueDropdownList<TYPE_ITEM_RELATE> listRelateType = pElement.IRelateByOther_GetEditorDisplayNameList();
                pElement.pRelateType = EditorCodeHelper.DrawEnumPopup(rect, "Related Type", pElement.pRelateType, listRelateType);
                rect.y += EditorCodeHelper.singleLineHeight * 2;

                float fRectOriginX = rect.x;
                float fBracketCount_LabelWidth = EditorCodeHelper.CalcSize($"\"(\" Count :00") + 40f;
                rect.width = fBracketCount_LabelWidth;
                EditorCodeHelper.LabelField(rect, $"\"(\" Count :{pElement.iBracket_Start_Count.ToString()}");
                rect.x += fBracketCount_LabelWidth;

                float fLabelWidth = EditorCodeHelper.CalcSize("Add \"(\"") + const_fDrawBracketButton_Padding;
                rect.width = fLabelWidth;
                rect.height = EditorCodeHelper.singleLineHeight;

                if (GUI.Button(rect, "Add \"(\""))
                {
                    pElement.iBracket_Start_Count++;
                }
                rect.x += fLabelWidth + const_fDrawBracketButton_Gap;

                fLabelWidth = EditorCodeHelper.CalcSize("Remove \"(\"") + const_fDrawBracketButton_Padding;
                rect.width = fLabelWidth;
                if (pElement.IRelateByOther_IsBracket_Start())
                {
                    if (GUI.Button(rect, "Remove \"(\""))
                    {
                        pElement.iBracket_Start_Count--;
                    }
                }
                rect.y += EditorCodeHelper.singleLineHeight;
                rect.x = fRectOriginX;
                rect.width = fBracketCount_LabelWidth;
                EditorCodeHelper.LabelField(rect, $"\")\" Count :{pElement.iBracket_Finish_Count.ToString()}");
                rect.x += fBracketCount_LabelWidth;

                fLabelWidth = EditorCodeHelper.CalcSize("Add \")\"") + const_fDrawBracketButton_Padding;
                rect.width = fLabelWidth;
                if (GUI.Button(rect, "Add \")\""))
                {
                    pElement.iBracket_Finish_Count++;
                }
                rect.x += fLabelWidth + const_fDrawBracketButton_Gap;

                fLabelWidth = EditorCodeHelper.CalcSize("Remove \")\"") + const_fDrawBracketButton_Padding;
                rect.width = fLabelWidth;
                if (pElement.IRelateByOther_IsBracket_Finish())
                {
                    if (GUI.Button(rect, "Remove \")\""))
                    {
                        pElement.iBracket_Finish_Count--;
                    }
                }
                rect.width = fRectWidth_Origin;
                rect.x = fRectOriginX;
                rect.y += EditorCodeHelper.singleLineHeight;
            }
            EditorGUI.indentLevel = iIndetOrigin;
        };

        listInput.drawHeaderCallback = (Rect rect) =>
        {
            string strDisplayName = $"[{property.displayName}]= {listDrawTarget.ToString()}";
            EditorCodeHelper.LabelField(rect, strDisplayName);
        };

        listInput.elementHeightCallback = (int index) =>
        {
            var element = listInput.serializedProperty.GetArrayElementAtIndex(index);
            return GetElementHeight(element);
        };

        listInput.onAddCallback = (ReorderableList list) =>
        {
            int iAddedIndex = list.serializedProperty.arraySize;

            list.serializedProperty.arraySize++;
            list.index = iAddedIndex;
            var element = list.serializedProperty.GetArrayElementAtIndex(iAddedIndex);

            CLASS_ELEMENT pElementDummy_ForFieldName = null;
            element.FindPropertyRelative(nameof(pElementDummy_ForFieldName.bEnable)).boolValue = true;
        };

        return listInput;
    }

    static protected RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> GetDrawTargetList(SerializedProperty property)
    {
        return EditorCodeHelper.GetTargetObjectOfProperty(property) as RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT>;
    }

    private static SerializedProperty Get_List_SerializedProperty(SerializedProperty property)
    {
        RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> plist;
        return property.FindPropertyRelative(nameof(plist.p_listElement));
    }

    public ValueDropdownList<CLASS_ELEMENT> GetValueDropdownList(RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> listDrawTarget, List<CLASS_ELEMENT> list, CLASS_ELEMENT pElementIgnore)
    {
        ValueDropdownList<CLASS_ELEMENT> listReturn = new ValueDropdownList<CLASS_ELEMENT>();
        listReturn.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            CLASS_ELEMENT pElement = list[i];
            if (pElement.Equals(pElementIgnore) == false)
                listReturn.Add($"{i}).{listDrawTarget.GetElementDisplayName(pElement)}", pElement);
        }

        if (listReturn.Count == 0)
        {
            listReturn.Add($"Require Other Element", default(CLASS_ELEMENT));
        }

        return listReturn;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float fHeight = EditorCodeHelper.singleLineHeight * 4;
        RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT> listDrawTarget = EditorCodeHelper.GetTargetObjectOfProperty(property) as RelatedByElementsListBase<CLASS_ELEMENT, TYPE_ITEM_RELATE, TYPE_RESULT>;
        SerializedProperty plistProperty = Get_List_SerializedProperty(property);
        if (plistProperty != null)
        {
            if(listDrawTarget.Check_IsError())
                fHeight += EditorCodeHelper.singleLineHeight * 2;

            for (int i = 0; i < plistProperty.arraySize; i++)
            {
                SerializedProperty pProperty = plistProperty.GetArrayElementAtIndex(i);
                if (pProperty.isExpanded)
                    fHeight += GetElementHeight(pProperty);
                else
                    fHeight += EditorCodeHelper.singleLineHeight;
            }
        }

        return fHeight;
    }

    static private float GetElementHeight(SerializedProperty element)
    {
        float fHeight = EditorCodeHelper.singleLineHeight;
        if (element.isExpanded)
            fHeight += EditorGUI.GetPropertyHeight(element, true) + EditorCodeHelper.singleLineHeight * 3;

        return fHeight;
    }

    static public float GetElementHeight_Expanded(SerializedProperty element)
    {
        float fHeight = EditorCodeHelper.singleLineHeight;
        fHeight += EditorGUI.GetPropertyHeight(element, true) + EditorCodeHelper.singleLineHeight * 3;

        return fHeight;
    }
}
#endif
#endregion