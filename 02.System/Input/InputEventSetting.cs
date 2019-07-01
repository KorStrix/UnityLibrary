#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-13 오후 4:05:49
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
[CreateAssetMenu(menuName = "StrixSO/" + nameof(InputEventSetting))]
public class InputEventSetting : ScriptableObject
{
    public CommandListBase pCommandList;
    public InputInfoList listCommandInfo = new InputInfoList();

    public void DoInit(CommandExecuter_Input pManagerCommand)
    {
        foreach (var pCommandInfo in listCommandInfo)
            pCommandInfo.pCommand?.DoInitCommand(pManagerCommand);
    }
}

#region EditorCode
#if UNITY_EDITOR

[CustomEditor(typeof(InputEventSetting))]
public class InputEventSetting_Drawer : Editor
{
    InputEventSetting pTarget { get { return (InputEventSetting)target; } }

    public override void OnInspectorGUI()
    {
        var pProperty_CommadList = this.serializedObject.FindProperty(nameof(pTarget.pCommandList));
        EditorGUILayout.PropertyField(pProperty_CommadList, new GUIContent("CommandList SO"));

        var pProperty_CommadInfoList = this.serializedObject.FindProperty(nameof(pTarget.listCommandInfo));
        EditorGUILayout.PropertyField(pProperty_CommadInfoList);

        if (pTarget.listCommandInfo != null && pTarget.listCommandInfo.pCommandListTarget != pTarget.pCommandList)
            pTarget.listCommandInfo.pCommandListTarget = pTarget.pCommandList;
    }
}

#endif
#endregion