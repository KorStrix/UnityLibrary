using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

#endif

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
#pragma warning disable CS0672 // 멤버가 사용되지 않는 멤버를 재정의합니다.

#if ODIN_INSPECTOR
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class Rename_InspectorAttribute : Attribute
{
    public string strInspectorName;
    public bool bIsEditPossibleInspector;

    /// <summary>
    /// 기본적으로 인스펙터에 노출시킵니다.
    /// </summary>
    /// <param name="strInpectorName">인스펙터에 노출시킬 이름</param>
    /// <param name="bIsEditPossibleInspector">에디터에서 수정가능 유무</param>
    public Rename_InspectorAttribute(string text, bool bIsEditPossibleInspector = true)
    {
        strInspectorName = text;
        this.bIsEditPossibleInspector = bIsEditPossibleInspector;
    }
}
#else

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class Rename_InspectorAttribute : PropertyAttribute
{
    public string strInspectorName;
    public bool bIsEditPossibleInspector;

    /// <summary>
    /// 기본적으로 인스펙터에 노출시킵니다.
    /// </summary>
    /// <param name="strInpectorName">인스펙터에 노출시킬 이름</param>
    /// <param name="bIsEditPossibleInspector">에디터에서 수정가능 유무</param>
    public Rename_InspectorAttribute(string strInpectorName, bool bIsEditPossibleInspector = true)
    {
        this.strInspectorName = strInpectorName;
        this.bIsEditPossibleInspector = bIsEditPossibleInspector;
    }
}
#endif

#if UNITY_EDITOR
#if ODIN_INSPECTOR
[OdinDrawer]
[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class CEditorInspector_Attribute_Rename : OdinAttributeDrawer<Rename_InspectorAttribute>
{
    /// <summary>
    /// Draws the attribute.
    /// </summary>
    protected override void DrawPropertyLayout(InspectorProperty property, Rename_InspectorAttribute attribute, GUIContent label)
    {
        var context = property.Context.Get<StringMemberHelper>(this, "StringContext", (StringMemberHelper)null);

        if (context.Value == null)
            context.Value = new StringMemberHelper(property.ParentType, attribute.strInspectorName);

        if (context.Value.ErrorMessage != null)
            SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);

        if (label == null)
            property.Label = null;
        else
        {
            property.Label = label;
            property.Label.text = context.Value.GetString(property);
        }

        this.CallNextDrawer(property, property.Label);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUI.enabled = Attribute.bIsEditPossibleInspector;
        base.DrawPropertyLayout(label);
        GUI.enabled = true;
    }
}

#else
[CustomPropertyDrawer(typeof(Rename_InspectorAttribute))]
public class CEditorInspector_Attribute_Rename : PropertyDrawer
{
    Rename_InspectorAttribute pAttributeTarget;

    public override void OnGUI(Rect position,
                   SerializedProperty property, GUIContent label)
    {
        pAttributeTarget = (Rename_InspectorAttribute)attribute;
        // Vector2 vecSize = CalculateSize(pAttributeTarget);

        // Todo - 인스펙터 이름이 길어지면 재조정
        // Todo - Array 등 복수형 자료에서도 적용해야 함
        label.text = pAttributeTarget.strInspectorName;

        GUI.enabled = pAttributeTarget.bIsEditPossibleInspector;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        pAttributeTarget = (Rename_InspectorAttribute)attribute;
        Vector2 vecSize = CalculateSize(pAttributeTarget);

        return vecSize.y;
    }

    Vector2 CalculateSize(Rename_InspectorAttribute attribute)
    {
        return GUI.skin.label.CalcSize(new GUIContent(attribute.strInspectorName));
    }
}
#endif
#endif

#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
#pragma warning restore CS0672 // 멤버가 사용되지 않는 멤버를 재정의합니다.