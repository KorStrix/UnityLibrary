#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-12 오후 12:29:36
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ScriptableObject_Extension
{
    static public string ConvertJson(this ScriptableObject pScriptableObject)
    {
        return JsonUtility.ToJson(pScriptableObject);
    }

    static public void WriteJson<T>(this T pScriptableObject, string strJsonText)
        where T : ScriptableObject
    {
        JsonUtility.FromJsonOverwrite(strJsonText, pScriptableObject);
    }
}