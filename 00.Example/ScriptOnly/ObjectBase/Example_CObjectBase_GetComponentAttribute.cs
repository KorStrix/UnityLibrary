#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
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

public class Example_CObjectBase_GetComponentAttribute : CObjectBase
{
    public enum EObjectName
    {
        Text1,
        Text2,
        Text3,

        Text_Same,
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    [GetComponent]
    public Strix.IInterfaceTest_A pInterface_Me;

    [GetComponentInChildren("Text1")]
    public Text pText_1;
    [GetComponentInChildren("Text2")]
    public Text pText_2;
    [GetComponentInChildren("Text3")]
    public Text pText_3;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    [GetComponentInChildren]
    public Strix.IInterfaceTest_A pInterface_Child;

    [GetComponentInChildren]
    public Text[] arrText;
    [GetComponentInChildren]
    public List<Text> listText;
    [GetComponentInChildren("Text_Same")]
    public List<Text> listTextSame;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    [GetComponentInChildren]
    public Dictionary<string, Strix.IInterfaceTest_A> Dictionary_Key_IsString_Value_IsInterface;


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
        yield return YieldManager.GetWaitForSecond(2f);

        Debug.Log("OnEnableObjectCoroutine - After 2 Sec - And Disable Self");

        // gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

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
