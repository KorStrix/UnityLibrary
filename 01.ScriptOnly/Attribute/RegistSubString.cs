#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-15 오후 5:39:53
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RegistSubString : UnityEngine.PropertyAttribute
{
    public string strSubString;

    public RegistSubString(string strSubString)
    {
        this.strSubString = strSubString;
    }
}

public static class SCSubStringHelper
{
    static public string ToStringSub(this System.Enum eEnum)
    {
        string strString = eEnum.ToString();
        Type pType = eEnum.GetType();
        FieldInfo pFieldInfo = pType.GetField(strString);
        if(pFieldInfo != null)
        {
            RegistSubString pAttribute = pFieldInfo.GetCustomAttribute(typeof(RegistSubString), false) as RegistSubString;
            if(pAttribute != null)
                strString = pAttribute.strSubString;
        }

        return strString;
    }

    static public string ToStringSub(this object pClass)
    {
        string strString = pClass.ToString();
        RegistSubString pAttribute = pClass.GetType().GetCustomAttribute(typeof(RegistSubString), false) as RegistSubString;
        if (pAttribute != null)
            strString = pAttribute.strSubString;

        return strString;
    }
}
