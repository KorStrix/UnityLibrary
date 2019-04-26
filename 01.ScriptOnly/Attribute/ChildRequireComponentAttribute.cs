#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-05 오후 3:25:09
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

#endif

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
#pragma warning disable CS0672 // 멤버가 사용되지 않는 멤버를 재정의합니다.

public class PropertyGroupAttribute_For_Build : Attribute
{
    public PropertyGroupAttribute_For_Build(string strGroup, int iOrder)
    {
    }
}

#if ODIN_INSPECTOR
public class ChildRequireComponentAttribute :
#if UNITY_EDITOR
    PropertyGroupAttribute,
#else
    PropertyGroupAttribute_For_Build,
#endif
    IGetComponentAttribute
{
    public bool bIsPrint_OnNotFound_GetComponent => bIsPrint_OnNotFound;

    public string strComponentName;
    public bool bIsPrint_OnNotFound;
    

    public ChildRequireComponentAttribute(System.Object pComponentName, int iOrder = 0)
        : base(nameof(ChildRequireComponentAttribute), iOrder)
    {
        this.strComponentName = pComponentName.ToString();
        this.bIsPrint_OnNotFound = true;
    }

    public object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        object pObjectArray = pTargetMono.GetComponentsInChildren_SameName(strComponentName, pElementType, true);
        if (pElementType == typeof(GameObject))
            return Convert_TransformArray_To_GameObjectArray(pTargetMono, pObjectArray);
        else
            return pObjectArray; ;
    }

    protected GameObject[] Convert_TransformArray_To_GameObjectArray(MonoBehaviour pTargetMono, object pObject)
    {
        object[] arrObject = pObject as object[];
        GameObject[] arrObjectReturn = new GameObject[arrObject.Length];
        for (int i = 0; i < arrObject.Length; i++)
            arrObjectReturn[i] = (arrObject[i] as Transform).gameObject;
        return arrObjectReturn;
    }
}

#else

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class ChildRequireComponentAttribute : GetComponentInChildrenAttribute
{
    public string strRequireComent;
    public bool bIsEditPossibleInspector;

    public ChildRequireComponentAttribute(System.Object pComponentName, bool bIsEditPossibleInspector = true)
        : base(pComponentName)
    {
        this.strRequireComent = "'" + pComponentName.ToString() + "' - Require In Children Object";
        this.bIsEditPossibleInspector = bIsEditPossibleInspector;
    }
}
#endif

#if UNITY_EDITOR
#if ODIN_INSPECTOR
[OdinDrawer]
[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class CChildRequireComponentAttribute_Drawer :
#if UNITY_EDITOR
    OdinGroupDrawer<ChildRequireComponentAttribute> // 그룹 드로어는 Editor Only이다..
#else
    OdinAttributeDrawer<ChildRequireComponentAttribute>
#endif
{
    public enum ECheckState
    {
        NotYet,
        Checked,
        Fail,
    }


    private class TitleContext
    {
        public StringMemberHelper TitleHelper;
        public StringMemberHelper SubtitleHelper;
    }

    public readonly string const_strTitle = "Require Child Component List";
    public const string const_strSubTitle = "자식 오브젝트 중에 동일한 오브젝트 이름의 컴포넌트를 찾습니다.";

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var property = this.Property;
        var attribute = this.Attribute;

        SCManagerGetComponent.DoUpdateGetComponentAttribute(property.ParentValues[0] as MonoBehaviour);

        PropertyContext<TitleContext> context;
        if (property.Context.Get(this, "Title", out context))
        {
            context.Value = new TitleContext()
            {
                TitleHelper = new StringMemberHelper(property, const_strTitle),
                SubtitleHelper = new StringMemberHelper(property, const_strSubTitle),
            };
        }

        if (property != property.Tree.GetRootProperty(0))
            EditorGUILayout.Space();

        SirenixEditorGUI.Title(const_strTitle, const_strSubTitle, TextAlignment.Left, true, true);

        GUI.enabled = false;
        GUIHelper.PushIndentLevel(EditorGUI.indentLevel + 1);
        foreach (var pPropertyChild in property.Children)
        {
            ChildRequireComponentAttribute pAttribute = pPropertyChild.Info.GetAttribute<ChildRequireComponentAttribute>();
            if(pAttribute != null)
            {
                pPropertyChild.Label.text = "`" + pAttribute.strComponentName + "`";

                ECheckState eCheckState = ECheckState.NotYet;
                eCheckState = CalculateCheckState(property, pPropertyChild);

                switch (eCheckState)
                {
                    case ECheckState.NotYet:
                        GUI.contentColor = Color.yellow;
                        pPropertyChild.Label.image = EditorIcons.UnityWarningIcon;
                        break;

                    case ECheckState.Checked:
                        GUI.contentColor = Color.green;
                        pPropertyChild.Label.image = EditorIcons.Checkmark.Raw;
                        break;

                    case ECheckState.Fail:
                        GUI.contentColor = Color.red;
                        pPropertyChild.Label.image = EditorIcons.X.Raw;
                        break;
                }
            }

            pPropertyChild.Draw(pPropertyChild.Label);
        }

        GUI.contentColor = Color.white;
        GUIHelper.PopIndentLevel();
        GUI.enabled = true;
    }

    private ECheckState CalculateCheckState(InspectorProperty property, InspectorProperty pPropertyChild)
    {
        ECheckState eCheckState;
        if (Check_IsFill_Member(property, pPropertyChild))
        {
            eCheckState = ECheckState.Checked;
        }
        else
        {
            if (Attribute.bIsPrint_OnNotFound_GetComponent == false)
                eCheckState = ECheckState.NotYet;
            else
                eCheckState = ECheckState.Fail;
        }

        return eCheckState;
    }

    private bool Check_IsFill_Member(InspectorProperty property, InspectorProperty pPropertyChild)
    {
        return pPropertyChild.Info.GetMemberInfo().CheckValueIsNull(property.ParentValues[0]) == false;
    }
}
#else
[CustomPropertyDrawer(typeof(ChildRequireComponentAttribute))]
public class CEditorInspector_ChildRequireComponentAttribute : PropertyDrawer
{
    // Used for top and bottom padding between the text and the HelpBox border.
    const int paddingHeight = 8;

    // Used to add some margin between the the HelpBox and the property.
    const int marginHeight = 2;

    //  Global field to store the original (base) property height.
    float baseHeight = 0;

    /// <summary>
    /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
    /// </summary>
    ChildRequireComponentAttribute pRequireChildComponent { get { return (ChildRequireComponentAttribute)attribute; } }


    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        // We store the original property height for later use...
        baseHeight = base.GetPropertyHeight(prop, label);

        // This stops icon shrinking if text content doesn't fill out the container enough.
        float minHeight = paddingHeight * 5;

        // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
        // window's currentViewWidth.
        var content = new GUIContent(pRequireChildComponent.strComponentName);
        var style = GUI.skin.GetStyle("helpbox");

        var height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

        // We add tiny padding here to make sure the text is not overflowing the HelpBox from the top
        // and bottom.
        height += marginHeight * 2;

        // If the calculated HelpBox is less than our minimum height we use this to calculate the returned
        // height instead.
        return height > minHeight ? height + baseHeight : minHeight + baseHeight;
    }


    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        GUI.enabled = false;

        label.text = "[" + pRequireChildComponent.strComponentName + "]";

        EditorGUI.BeginProperty(position, label, prop);

        var helpPos = position;
        helpPos.height -= baseHeight + marginHeight;

        position.y += helpPos.height + marginHeight;
        position.height = baseHeight;

        EditorGUI.PropertyField(position, prop, label);

        EditorGUI.EndProperty();
        GUI.enabled = true;
    }
}
#endif
#endif