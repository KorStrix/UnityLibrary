#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-22 오전 6:59:36
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Example_CObjectBase : CObjectBase
{
    public enum EObjectName
    {
        Text1,
        Text2,
        Text3,
    }

    [GetComponentInChildren("Text1")]
    public Text pText_1;
    [GetComponentInChildren("Text2")]
    public Text pText_2;
    [GetComponentInChildren("Text3")]
    public Text pText_3;

    [GetComponentInChildren]
    public Text[] arrText;

    [GetComponentInChildren]
    public List<Text> listText;

    [GetComponentInChildren]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Dictionary<string, Text> Dictionary_Key_IsString_Value_IsComponent;

    [GetComponentInChildren]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Dictionary<EObjectName, Text> Dictionary_Key_IsEnum_Value_IsComponent;

    [GetComponentInChildren]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Dictionary<EObjectName, GameObject> Dictionary_Key_IsEnum_Value_IsGameObject;

    protected override void OnAwake()
    {
        base.OnAwake();

        PrintLog();

        Debug.LogWarning("mapText_Key_Is_String.Count : " + Dictionary_Key_IsString_Value_IsComponent.Count);
        Debug.LogWarning("mapText_Key_Is_Dictionary.Count : " + Dictionary_Key_IsEnum_Value_IsComponent.Count);
    }

    protected override void OnStart()
    {
        base.OnStart();

        PrintLog();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        PrintLog();
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("OnEnableObjectCoroutine - After 2 Sec - And Disable Self");

        // gameObject.SetActive(false);
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        PrintLog();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        PrintLog();
    }


    private void PrintLog()
    {
        System.Diagnostics.StackTrace pTrace = new System.Diagnostics.StackTrace();

        UnityEngine.Debug.Log(string.Format("{0} - CObjectBaseTest", pTrace.GetFrame(1).GetMethod()));
    }
}
