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

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// ScriptableObject를 Custom Editor할 수 없어서
/// Custom Editor를 위한 ScriptableObject 래핑 클래스
/// </summary>
[System.Serializable]
public class CScriptableObject :
#if ODIN_INSPECTOR
    SerializedScriptableObject
#else
    ScriptableObject
#endif
{
    public static string strJsonText;
    public static ScriptableObject pScriptableObject;
    public static System.Type pScriptableObject_Type;

    // ==================================================================

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

    public bool p_bExecute_Awake_OnPlay { get; private set; } = false;

    public void Event_OnAwake()
    {
        if (Application.isPlaying == false ||
          (Application.isPlaying && p_bExecute_Awake_OnPlay == false))
            OnAwake(Application.isPlaying);
    }

    protected virtual void OnAwake(bool bAppIsPlaying)
    {
        if (bAppIsPlaying)
            p_bExecute_Awake_OnPlay = true;

        // Debug.Log(GetType().GetFriendlyName() + nameof(OnAwake) + " bAppIsPlaying : " + bAppIsPlaying + " p_bExecute_Awake_OnPlay : " + p_bExecute_Awake_OnPlay);
    }

    /// <summary>
    /// OnEnable은 이상하게 Editor에서 Play를 누른 직후에 호출된다.
    /// Applciation.isPlaying의 경우 false로 들어온다.
    /// </summary>
    private void OnEnable()
    {
        p_bExecute_Awake_OnPlay = false;
        OnAwake(false);
    }

    protected virtual void OnDestroy_Singleton() { }
}