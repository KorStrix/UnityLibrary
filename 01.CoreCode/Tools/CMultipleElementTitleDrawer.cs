#region Header
/* ============================================ 
 *	설계자 : https://forum.unity.com/threads/how-to-change-the-name-of-list-elements-in-the-inspector.448910/
 *	작성자 : 
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CAttributeMultipleElementTitle : PropertyAttribute
{
	public string strFieldName;
	public CAttributeMultipleElementTitle( string strFieldName )
	{
		this.strFieldName = strFieldName;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( CAttributeMultipleElementTitle ) )]
public class CMultipleElementTitleDrawer : PropertyDrawer
{
	public override float GetPropertyHeight( SerializedProperty property,
									GUIContent label )
	{
		return EditorGUI.GetPropertyHeight( property, label, true );
	}
	protected virtual CAttributeMultipleElementTitle Atribute
	{
		get { return (CAttributeMultipleElementTitle)attribute; }
	}
	SerializedProperty TitleNameProp;
	public override void OnGUI( Rect position,
							  SerializedProperty property,
							  GUIContent label )
	{
		string FullPathName = property.propertyPath + "." + Atribute.strFieldName;
		TitleNameProp = property.serializedObject.FindProperty( FullPathName );
		string newlabel = GetTitle();
		if (string.IsNullOrEmpty( newlabel ))
			newlabel = label.text;
		EditorGUI.PropertyField( position, property, new GUIContent( newlabel, label.tooltip ), true );
	}

	private string GetTitle()
	{
		switch (TitleNameProp.propertyType)
		{
			case SerializedPropertyType.Generic:
				break;
			case SerializedPropertyType.Integer:
				return TitleNameProp.intValue.ToString();
			case SerializedPropertyType.Boolean:
				return TitleNameProp.boolValue.ToString();
			case SerializedPropertyType.Float:
				return TitleNameProp.floatValue.ToString();
			case SerializedPropertyType.String:
				return TitleNameProp.stringValue;
			case SerializedPropertyType.Color:
				return TitleNameProp.colorValue.ToString();
			case SerializedPropertyType.ObjectReference:
				return TitleNameProp.objectReferenceValue.ToString();
			case SerializedPropertyType.LayerMask:
				break;
			case SerializedPropertyType.Enum:
				return TitleNameProp.enumNames[TitleNameProp.enumValueIndex];
			case SerializedPropertyType.Vector2:
				return TitleNameProp.vector2Value.ToString();
			case SerializedPropertyType.Vector3:
				return TitleNameProp.vector3Value.ToString();
			case SerializedPropertyType.Vector4:
				return TitleNameProp.vector4Value.ToString();
			case SerializedPropertyType.Rect:
				break;
			case SerializedPropertyType.ArraySize:
				break;
			case SerializedPropertyType.Character:
				break;
			case SerializedPropertyType.AnimationCurve:
				break;
			case SerializedPropertyType.Bounds:
				break;
			case SerializedPropertyType.Gradient:
				break;
			case SerializedPropertyType.Quaternion:
				break;
			default:
				break;
		}
		return "";
	}
}
#endif