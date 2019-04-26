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

using NUnit.Framework;
using UnityEngine.TestTools;

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

[Category("StrixLibrary")]
public class ToSubStringAttribute_Test
{
    public enum ETest
    {
        [RegistSubString("Test11")]
        Test1,
        Test2,
    }

    public class ClassToSubStringBase { }
    [RegistSubString("Test11")]
    public class ClassToSubString : ClassToSubStringBase { }
    public class ClassToSubString2: ClassToSubStringBase { }

    [Test]
    public void 이넘_투_서브스트링_테스트()
    {
        Assert.AreEqual(ETest.Test1.ToStringSub(), "Test11");
        Assert.AreEqual(ETest.Test2.ToStringSub(), "Test2");
    }

    [Test]
    public void 오브젝트_투_서브스트링_테스트()
    {
        Assert.AreEqual(new ClassToSubStringBase().ToStringSub(), new ClassToSubStringBase().ToString());
        Assert.AreEqual(new ClassToSubString().ToStringSub(), "Test11");
        Assert.AreEqual(new ClassToSubString2().ToStringSub(), new ClassToSubString2().ToString());
    }
}