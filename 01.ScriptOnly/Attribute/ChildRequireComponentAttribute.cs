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
using System.Linq;

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

    public ChildRequireComponentAttribute(System.Object pComponentName, bool bIsPrint_OnNotFound, int iOrder = 0)
        : base(nameof(ChildRequireComponentAttribute), iOrder)
    {
        this.strComponentName = pComponentName.ToString();
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
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
public class ChildRequireComponentAttribute : PropertyAttribute, IGetComponentAttribute
{
    public string strComponentName;
    public bool bIsPrint_OnNotFound;

    public ChildRequireComponentAttribute(System.Object pComponentName, bool bIsPrint_OntNotFound = true)
    {
        this.strComponentName = pComponentName.ToString();
        this.bIsPrint_OnNotFound = bIsPrint_OntNotFound;
    }

    public bool bIsPrint_OnNotFound_GetComponent => bIsPrint_OnNotFound;

    public object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        return SCManagerGetComponent.Event_GetComponentInChildren(pTargetMono, pElementType, true, true, strComponentName);
    }
}
#endif

#if UNITY_EDITOR
#if ODIN_INSPECTOR
[OdinDrawer]
[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class CChildRequireComponentAttribute_Drawer : OdinGroupDrawer<ChildRequireComponentAttribute>
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

        object pObjectFieldOwner = property.ParentValues[0];
        bool bTargetIsMono = property.ParentValues[0] is MonoBehaviour;
        if(bTargetIsMono)
            SCManagerGetComponent.DoUpdateGetComponentAttribute(property.ParentValues[0] as MonoBehaviour);
        else
        {
            pObjectFieldOwner = property.ParentValues[0];
            SCManagerGetComponent.DoUpdateGetComponentAttribute(property.Parent.ParentValues[0] as MonoBehaviour, property.ParentValues[0]);
        }


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
            var pAttribute = pPropertyChild.GetAttribute<ChildRequireComponentAttribute>();
            if (pAttribute != null && pAttribute.bIsPrint_OnNotFound_GetComponent == false)
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
    public enum ECheckState
    {
        NotYet,
        Checked,
        Fail,
    }

    /// <summary>
    /// A wrapper which returns the PropertyDrawer.attribute field as a HelpAttribute.
    /// </summary>
    ChildRequireComponentAttribute _pAttribute { get { return (ChildRequireComponentAttribute)attribute; } }
    
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        Color pColorOrigin = GUI.color;
        GUI.enabled = false;

        object pObjectFieldOwner = prop.serializedObject.targetObject;

        System.Type pTypeCurrentPropertyOwner = fieldInfo.ReflectedType;
        bool bTargetIsMono = pTypeCurrentPropertyOwner.IsSubclassOf(typeof(MonoBehaviour));
        if(bTargetIsMono)
        {
            SCManagerGetComponent.DoUpdateGetComponentAttribute(prop.serializedObject.targetObject as MonoBehaviour);
        }
        else
        {
            // Unity Editor의 PropertyDrawer에선 현재 그리고 있는 프로퍼티의 오너 인스턴스가 무조건 Monobehaviour를 통해 얻어오는데,
            // PropertyDrawer의 현재 그리고 있는 프로퍼티 오너가 Mono가 아닐 경우, Mono에서부터 현재 그리고 있는 프로퍼티 오너의 인스턴스를 찾아야 한다..

            // 프로퍼티의 경우 Play Mode가 Edit일 때 찾지 못하는 에러가 있음 ( Play 중일땐 정상 )
            int iIndex = prop.propertyPath.IndexOf('.');
            if(iIndex != -1)
            {
                string strFieldOwnerName = prop.propertyPath.Substring(0, iIndex);
                System.Type pMonobehaviourType = prop.serializedObject.targetObject.GetType();

                MemberInfo pMemberInfo_Owner = null;
                pMemberInfo_Owner = FindMember(strFieldOwnerName, pMemberInfo_Owner, pMonobehaviourType.GetFields(BindingFlags.Public | BindingFlags.Instance));
                if(pMemberInfo_Owner == null)
                    pMemberInfo_Owner = FindMember(strFieldOwnerName, pMemberInfo_Owner, pMonobehaviourType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                if (pMemberInfo_Owner == null)
                    pMemberInfo_Owner = FindMember(strFieldOwnerName, pMemberInfo_Owner, pMonobehaviourType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                if (pMemberInfo_Owner == null)
                    pMemberInfo_Owner = FindMember(strFieldOwnerName, pMemberInfo_Owner, pMonobehaviourType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance));

                if (pMemberInfo_Owner != null)
                {
                    pObjectFieldOwner = pMemberInfo_Owner.GetValue_Extension(prop.serializedObject.targetObject);
                    SCManagerGetComponent.DoUpdateGetComponentAttribute(prop.serializedObject.targetObject as MonoBehaviour, pObjectFieldOwner);
                }
            }
        }

        label.text = "'" + _pAttribute.strComponentName + "'";
        switch (CalculateCheckState(prop, pObjectFieldOwner))
        {
            case ECheckState.NotYet:
                GUI.color = Color.yellow;
                label.text = "Wait-" + label.text;
                break;

            case ECheckState.Checked:
                GUI.color = Color.green;
                label.text = "Check-" + label.text;
                break;

            case ECheckState.Fail:
                GUI.color = Color.red;
                label.text = "Fail-" + label.text;
                break;
        }

        EditorGUI.BeginProperty(position, label, prop);
        EditorGUI.PropertyField(position, prop, label);
        EditorGUI.EndProperty();

        GUI.enabled = true;
        GUI.color = pColorOrigin;

    }

    private static MemberInfo FindMember(string strFieldName, MemberInfo pMemberInfo_Me, MemberInfo[] arrMemberInfo)
    {
        foreach (var pMemberInfo in arrMemberInfo)
        {
            if (pMemberInfo.Name.Equals(strFieldName))
            {
                pMemberInfo_Me = pMemberInfo;
                break;
            }
        }

        return pMemberInfo_Me;
    }

    private ECheckState CalculateCheckState(SerializedProperty property, object pObjectFieldOwner)
    {
        ECheckState eCheckState;
        if (Check_IsFill_Member(property, pObjectFieldOwner))
        {
            eCheckState = ECheckState.Checked;
        }
        else
        {
            if (_pAttribute.bIsPrint_OnNotFound_GetComponent == false)
                eCheckState = ECheckState.NotYet;
            else
                eCheckState = ECheckState.Fail;
        }

        return eCheckState;
    }

    private bool Check_IsFill_Member(SerializedProperty property, object pObjectFieldOwner)
    {
        try
        {
            return this.fieldInfo.CheckValueIsNull(pObjectFieldOwner) == false;
        }
        catch
        {
            return false;
        }
    }
}
#endif
#endif