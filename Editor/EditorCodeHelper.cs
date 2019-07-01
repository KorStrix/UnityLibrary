#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-07 오후 4:00:34
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public static class EditorCodeHelper
{
    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }

    // 참고 : https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
    public static object GetValue_Imp(object pObjectFieldOwner, string strFieldName)
    {
        if (pObjectFieldOwner == null)
            return null;
        var type = pObjectFieldOwner.GetType();

        while (type != null)
        {
            var f = type.GetField(strFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(pObjectFieldOwner);

            var p = type.GetProperty(strFieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(pObjectFieldOwner, null);

            type = type.BaseType;
        }
        return null;
    }

    public static object GetValue_Imp(object pObjectFieldOwner, string strFieldName, int iArrayIndex)
    {
        var enumerable = GetValue_Imp(pObjectFieldOwner, strFieldName) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= iArrayIndex; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }

    // =======================================================================================================

    static public float singleLineHeight { get { return EditorGUIUtility.singleLineHeight; } }

    static public void LabelField(string strLabel)
    {
        EditorGUILayout.LabelField(strLabel);
    }

    static public void LabelField(GUIContent pLabel)
    {
        EditorGUILayout.LabelField(pLabel);
    }

    static public void LabelField(Rect rect, string strLabel)
    {
        EditorGUI.LabelField(rect, strLabel);
    }

    static public void LabelField(Rect rect, GUIContent pLabel)
    {
        EditorGUI.LabelField(rect, pLabel);
    }

    static public void HelpBox(string strText, MessageType eMessageType)
    {
        EditorGUILayout.HelpBox(strText, eMessageType);
    }

    static public void BeginHorizontal()
    {
        EditorGUILayout.BeginHorizontal();
    }

    static public void EndHorizontal()
    {
        EditorGUILayout.EndHorizontal();
    }

    static public void PropertyField(Rect sDrawRect, SerializedProperty pProperty, GUIContent pGUIContent)
    {
        EditorGUI.PropertyField(sDrawRect, pProperty, pGUIContent, true);
    }

    static public void PropertyField(Rect sDrawRect, SerializedProperty pProperty, string strLabel)
    {
        EditorGUI.PropertyField(sDrawRect, pProperty, new GUIContent(strLabel), true);
    }
    
    static public float CalcSize(GUIContent pGUIContent)
    {
        return GUI.skin.label.CalcSize(pGUIContent).x;
    }

    static public float CalcSize(string strText)
    {
        return GUI.skin.label.CalcSize(new GUIContent(strText)).x;
    }

    static public bool Foldout(Rect rect, bool bFoldOut, string strText)
    {
        return EditorGUI.Foldout(rect, bFoldOut, strText);
    }

    static public bool Foldout(bool bFoldOut, string strText)
    {
        return EditorGUILayout.Foldout(bFoldOut, strText);
    }

    static public Vector2 BeginScrollView(Vector2 vecScrollPosition, bool bAlwaysShowHorizontal, bool bAlwaysShowVertical)
    {
        return GUILayout.BeginScrollView(vecScrollPosition, bAlwaysShowHorizontal, bAlwaysShowVertical);
    }

    static public void EndScrollView()
    {
        GUILayout.EndScrollView();
    }

    static public UnityEngine.Object ObjectField(string strLabel, UnityEngine.Object pUnityObject, System.Type pObjectType, bool bAllowSceneObjects, params GUILayoutOption[] arrGUILayoutOption)
    {
        return EditorGUILayout.ObjectField(strLabel, pUnityObject, pObjectType, bAllowSceneObjects, arrGUILayoutOption);
    }

    static public DERIVED_UNITY_OBJECT_CLASS ObjectField<DERIVED_UNITY_OBJECT_CLASS>(string strLabel, UnityEngine.Object pUnityObject, bool bAllowSceneObjects, params GUILayoutOption[] arrGUILayoutOption)
        where DERIVED_UNITY_OBJECT_CLASS : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(strLabel, pUnityObject, typeof(DERIVED_UNITY_OBJECT_CLASS), bAllowSceneObjects, arrGUILayoutOption) as DERIVED_UNITY_OBJECT_CLASS;
    }

    static public bool Button(string strButtonText, params GUILayoutOption[] options)
    {
        return GUILayout.Button(strButtonText, options);
    }

    static public T DrawEnumPopup<T>(Rect rect, string strLabelName, T pCurrentValue, ValueDropdownList<T> listEnum)
    {
        int iIndex = listEnum.Calculate_SelectIndex(pCurrentValue);
        rect.height = EditorCodeHelper.singleLineHeight;
        iIndex = EditorGUI.Popup(rect, strLabelName, iIndex, listEnum.GetNameList());

        if (iIndex < listEnum.Count)
            return listEnum[iIndex].Value;
        else
            return default(T);
    }
}
#endif