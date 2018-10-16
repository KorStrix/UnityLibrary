using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class Rename_InspectorAttribute : Attribute
{
    public string strInspectorName;
    public bool bIsEditPossibleInspector;

    public Rename_InspectorAttribute(string text)
    {
        strInspectorName = text;
    }

    public Rename_InspectorAttribute(string text, bool bIsEditPossibleInspector)
    {
        strInspectorName = text;
        this.bIsEditPossibleInspector = bIsEditPossibleInspector;
    }
}
#else
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class Rename_InspectorAttribute : PropertyAttribute
{
    public string strInspectorName;
    public bool bIsEditPossibleInspector;

    /// <summary>
    /// 기본적으로 인스펙터에 노출시킵니다.
    /// </summary>
    /// <param name="strInpectorName">인스펙터에 노출시킬 이름</param>
    /// <param name="bIsEditPossibleInspector">에디터에서 수정가능 유무</param>
    public Rename_InspectorAttribute(string strInpectorName, bool bIsEditPossibleInspector = true)
    {
        this.strInspectorName = strInpectorName;
        this.bIsEditPossibleInspector = bIsEditPossibleInspector;
    }
}
#endif