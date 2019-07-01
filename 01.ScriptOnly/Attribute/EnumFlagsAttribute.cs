#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-06-21 오전 11:31:53
 *	개요 :
 *	출처 : https://wergia.tistory.com/104
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = UnityEditor.EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }
}

#endif