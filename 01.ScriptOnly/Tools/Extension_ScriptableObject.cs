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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public static class Extension_ScriptableObject
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

#region Test
#if UNITY_EDITOR

[System.Serializable]
public class ScriptableObjectTest : ScriptableObject
{
    public int iTest;
    public string strTest;
}

public class 스크립테이블오브젝트_테스트
{
    [Test]
    public void Json파싱테스트()
    {
        ScriptableObjectTest pObjectTest = ScriptableObjectTest.CreateInstance<ScriptableObjectTest>();
        pObjectTest.iTest = 1;
        pObjectTest.strTest = "테스트";

        Assert.AreEqual(pObjectTest.iTest, 1);
        Assert.AreEqual(pObjectTest.strTest, "테스트");

        string strJsonText = pObjectTest.ConvertJson();

        pObjectTest.iTest = 2;
        pObjectTest.strTest = "Test";

        Assert.AreEqual(pObjectTest.iTest, 2);
        Assert.AreEqual(pObjectTest.strTest, "Test");

        pObjectTest.WriteJson(strJsonText);

        Assert.AreEqual(pObjectTest.iTest, 1);
        Assert.AreEqual(pObjectTest.strTest, "테스트");
    }
}

#endif
#endregion Test