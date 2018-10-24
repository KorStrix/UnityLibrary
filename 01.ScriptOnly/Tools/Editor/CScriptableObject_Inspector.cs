#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-11 오전 11:08:12
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 
/// </summary>
#if !ODIN_INSPECTOR
[CustomEditor(typeof(CScriptableObject), true)]
public class CScriptableObject_Inspector : Editor
{
    public static string strJsonText;
    public static ScriptableObject pScriptableObject;
    public static System.Type pScriptableObject_Type;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Copy Chunk : ", pScriptableObject, pScriptableObject_Type, false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy ScriptableObject"))
        {
            pScriptableObject = target as ScriptableObject;
            pScriptableObject_Type = target.GetType();
            strJsonText = JsonUtility.ToJson(pScriptableObject);

            Debug.Log("Copy " + pScriptableObject.name);
        }

        if (GUILayout.Button("Paste ScriptableObject"))
        {
            if(pScriptableObject_Type != target.GetType())
            {
                Debug.LogError("타입이 달라서 Paste를 못합니다..");
                return;
            }

            JsonUtility.FromJsonOverwrite(strJsonText, target);
            Debug.Log("Paste " + target.name);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif