using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(CUGUIDropDown))]
public class CUGUIDropDownEditor : SelectableEditor
{
	private SerializedProperty m_Template;
	private SerializedProperty m_CaptionText;
	private SerializedProperty m_CaptionImage;
	private SerializedProperty m_ItemText;
	private SerializedProperty m_ItemImage;
	private SerializedProperty m_OnSelectionChanged;
	private SerializedProperty m_Value;
	private SerializedProperty m_Options;

	private SerializedProperty _pSpriteBG_OnHeader;
	private SerializedProperty _pColorItemText_OnHeader;

	protected override void OnEnable()
	{
		base.OnEnable();

		this.m_Template = base.serializedObject.FindProperty("m_Template");
		this.m_CaptionText = base.serializedObject.FindProperty("m_CaptionText");
		this.m_CaptionImage = base.serializedObject.FindProperty("m_CaptionImage");
		this.m_ItemText = base.serializedObject.FindProperty("m_ItemText");
		this.m_ItemImage = base.serializedObject.FindProperty("m_ItemImage");
		this.m_OnSelectionChanged = base.serializedObject.FindProperty("m_OnValueChanged");
		this.m_Value = base.serializedObject.FindProperty("m_Value");
		this.m_Options = base.serializedObject.FindProperty("m_Options");

		// 추가된 것
		_pSpriteBG_OnHeader = base.serializedObject.FindProperty("p_pSpriteBG_OnHeader");
		_pColorItemText_OnHeader = base.serializedObject.FindProperty("p_pColorItemText_OnHeader");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		base.serializedObject.Update();

		// 추가된 것
		EditorGUILayout.PropertyField(this._pSpriteBG_OnHeader, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this._pColorItemText_OnHeader, new GUILayoutOption[0]);

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(this.m_Template, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_CaptionText, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_CaptionImage, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_ItemText, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_ItemImage, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_Value, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_Options, new GUILayoutOption[0]);
		EditorGUILayout.PropertyField(this.m_OnSelectionChanged, new GUILayoutOption[0]);

		base.serializedObject.ApplyModifiedProperties();
	}
}
