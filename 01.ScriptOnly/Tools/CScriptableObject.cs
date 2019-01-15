#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-11 오전 11:15:33
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject를 Custom Editor할 수 없어서
/// Custom Editor를 위한 ScriptableObject 래핑 클래스
/// </summary>
[System.Serializable]
public class CScriptableObject : ScriptableObject
{
    public static string strJsonText;
    public static ScriptableObject pScriptableObject;
    public static System.Type pScriptableObject_Type;

#if UNITY_EDITOR && ODIN_INSPECTOR
    [Sirenix.OdinInspector.ButtonGroup("버튼")]
    [Sirenix.OdinInspector.Button("Copy ScriptableObject")]
    public void CopyObject()
    {
        pScriptableObject = this;
        pScriptableObject_Type = this.GetType();
        strJsonText = JsonUtility.ToJson(pScriptableObject);

        Debug.Log("Copy " + pScriptableObject.name);
    }

    [Sirenix.OdinInspector.ButtonGroup("버튼")]
    [Sirenix.OdinInspector.Button("Paste ScriptableObject")]
    public void PasteObject()
    {
        if (pScriptableObject_Type != this.GetType())
        {
            Debug.LogError("타입이 달라서 Paste를 못합니다..");
            return;
        }

        JsonUtility.FromJsonOverwrite(strJsonText, this);
        Debug.Log("Paste " + this.name);
    }
#endif
}