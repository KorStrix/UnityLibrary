#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-19 오후 5:34:55
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System;
using System.Reflection;

/// <summary>
/// 
/// </summary>
public abstract class CommandListBase : ScriptableObject
{
    protected abstract System.Type _pGetDrivenType { get; }

    public IEnumerable GetCommandList()
    {
        List<string> listString = new List<string>();
        Type[] arrInnerClassType = _pGetDrivenType.GetNestedTypes(BindingFlags.Public);
        for (int i = 0; i < arrInnerClassType.Length; i++)
        {
            if(arrInnerClassType[i].IsAbstract == false)
                listString.Add(arrInnerClassType[i].Name);
        }

        return listString.ToArray();
    }

    public Type GetCommandType_OrNull(string strFriendlyTypeName)
    {
        Type[] arrInnerClassType = _pGetDrivenType.GetNestedTypes(BindingFlags.Public);
        for (int i = 0; i < arrInnerClassType.Length; i++)
        {
            if (string.Equals(strFriendlyTypeName, arrInnerClassType[i].Name))
                return arrInnerClassType[i];
        }

        return null;
    }
}