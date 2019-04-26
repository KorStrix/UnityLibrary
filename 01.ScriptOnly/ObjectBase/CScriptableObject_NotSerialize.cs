#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-22 오후 8:48:54
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CScriptableObject_NotSerialize : ScriptableObject
{
    public void Event_OnAwake()
    {
        OnAwake();
    }

    virtual protected void OnAwake() { }
}