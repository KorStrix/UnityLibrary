#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-24 오후 1:22:20
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// 
/// </summary>
public class ColorAttribute : PropertyAttribute
{
    public Color pColor;

    public ColorAttribute(float fRed_0_1, float fGreen_0_1, float fBlue_0_1, float fAlpha_0_1 = 1f)
    {
        this.pColor = new Color(fRed_0_1, fGreen_0_1, fBlue_0_1, fAlpha_0_1);
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ColorAttribute))]
public class ColorAttributeDrawer : PropertyDrawer
{
    ColorAttribute _pAttribute { get { return (ColorAttribute)attribute; } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Color pColorOrigin = GUI.color;
        GUI.color = _pAttribute.pColor;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.color = pColorOrigin;
    }
}

#endif