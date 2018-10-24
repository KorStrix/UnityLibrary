#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-04 오전 10:01:40
 *	기능 : 
 *	
 *	txt 파일을 파싱하는 Static Class 입니다.

 *  텍스트 형식은
 *	이름 = 값
 *	이며,
 *	// 주석내용
 *	을 통해 주석을 지원합니다
 
 * 	코드 내 class의 경우 
 * 	class Test
 * 	{
 * 	    [Key("텍스트파일 내 이름")]
 * 	    int Value;
 * 	}
 * 	
 * 	로 사용하시면 됩니다.
  
 ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using System.Linq;

using FileInfo = System.Reflection.FieldInfo;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class KeyAttribute : Attribute
{
    public string strKeyName { get; private set; }

    public KeyAttribute(string strKeyName)
    {
        this.strKeyName = strKeyName;
    }
}

#if NET_4_6
public class SCManagerParserText
{
    // PropertyInfo, FieldInfo 를 동시에 담을만한 것이 API에 존재하지 않는다.
    // 그래서 커스텀 선언
    #region Struct MemberWrapper
    public struct SMemberWrapper
    {
        PropertyInfo pPropertyInfo;
        FieldInfo pFieldInfo;

        public SMemberWrapper(PropertyInfo pPropertyInfo)
        {
            this.pPropertyInfo = pPropertyInfo;
            this.pFieldInfo = null;
        }

        public SMemberWrapper(FieldInfo pFieldInfo)
        {
            this.pPropertyInfo = null;
            this.pFieldInfo = pFieldInfo;
        }

        public void SetValue(object pTargetInstance, string strValue)
        {
            if (pPropertyInfo != null)
            {
                string strFieldType = pPropertyInfo.PropertyType.Name;
                strFieldType = strFieldType.ToLower();
                switch (strFieldType)
                {
                    case "boolean":
                        bool b;
                        pPropertyInfo.SetValue(pTargetInstance, bool.TryParse(strValue, out b) ? b : false);
                        break;

                    case "int32":
                        int n;
                        pPropertyInfo.SetValue(pTargetInstance, int.TryParse(strValue, out n) ? n : 0);
                        break;

                    case "double":
                        double d;
                        pPropertyInfo.SetValue(pTargetInstance, double.TryParse(strValue, out d) ? d : 0);
                        break;

                    case "string":
                        pPropertyInfo.SetValue(pTargetInstance, strValue);
                        break;

                    case "single":
                        float f;
                        pPropertyInfo.SetValue(pTargetInstance, float.TryParse(strValue, out f) ? f : 0);
                        break;
                }
            }
            else if (pFieldInfo != null)
            {
                string strFieldType = pFieldInfo.FieldType.Name;
                strFieldType = strFieldType.ToLower();
                switch (strFieldType)
                {
                    case "boolean":
                        bool b;
                        pFieldInfo.SetValue(pTargetInstance, bool.TryParse(strValue, out b) ? b : false);
                        break;

                    case "int32":
                        int n;
                        pFieldInfo.SetValue(pTargetInstance, int.TryParse(strValue, out n) ? n : 0);
                        break;

                    case "double":
                        double d;
                        pFieldInfo.SetValue(pTargetInstance, double.TryParse(strValue, out d) ? d : 0);
                        break;

                    case "string":
                        pFieldInfo.SetValue(pTargetInstance, strValue);
                        break;

                    case "single":
                        float f;
                        pFieldInfo.SetValue(pTargetInstance, float.TryParse(strValue, out f) ? f : 0);
                        break;
                }
            }
        }
    }
    #endregion SMemberWrapper

    private static readonly char[] const_arrTrimKey = new char[] { '\t', '\r', ' ' };
    private static readonly char[] const_arrTrimValue = new char[] { '\t', '\r' };

    static Dictionary<string, SMemberWrapper> _mapMemberInfoTemp = new Dictionary<string, SMemberWrapper>();

    static public T ParsingObject<T>(string strText)
        where T : new()
    {
        T pDeserializeObject = new T();
        if (string.IsNullOrEmpty(strText))
        {
            Debug.LogError(typeof(SCManagerParserText).Name + "Text가 비어있습니다" + strText);
            return pDeserializeObject;
        }

        System.Type pTypeTarget = typeof(T);
        ProcDefineFieldDictionary(pTypeTarget);

        string[] arrLine = strText.Split('\n');
        for (int i = 0; i < arrLine.Length; i++)
        {
            string strTargetLine = arrLine[i];
            if (CheckIsValid(strTargetLine) == false)
                continue;

            string strKey, strValue;
            if (ExtractKeyValue(strTargetLine, out strKey, out strValue) == false)
                continue;

            SMemberWrapper pMemberWrapper;
            if (_mapMemberInfoTemp.TryGetValue(strKey, out pMemberWrapper) == false)
                continue;

            pMemberWrapper.SetValue(pDeserializeObject, strValue);
        }

        return pDeserializeObject;
    }

    #region Protected & Private

    static void ProcDefineFieldDictionary(System.Type pTypeTarget)
    {
        _mapMemberInfoTemp.Clear();
        ProcAddField(pTypeTarget.GetFields());
        ProcAddField(pTypeTarget.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
        ProcAddField(pTypeTarget.GetProperties());
    }

    static void ProcAddField(IEnumerable<FieldInfo> arrField)
    {
        if (arrField.Count() == 0) return;

        foreach (FieldInfo pField in arrField)
        {
            SMemberWrapper sMemberWrapper = new SMemberWrapper(pField);
            KeyAttribute pKeyAttribute = GetKeyAttribute_OrNull(pField);
            if (pKeyAttribute != null)
                AddDictionary_MemberInfo(pKeyAttribute.strKeyName, ref sMemberWrapper);
            else
                AddDictionary_MemberInfo(pField.Name, ref sMemberWrapper);
        }
    }

    static void ProcAddField(IEnumerable<PropertyInfo> arrProperty)
    {
        if (arrProperty.Count() == 0) return;

        foreach (PropertyInfo pProperty in arrProperty)
        {
            SMemberWrapper sMemberWrapper = new SMemberWrapper(pProperty);
            KeyAttribute pKeyAttribute = GetKeyAttribute_OrNull(pProperty);
            if (pKeyAttribute != null)
                AddDictionary_MemberInfo(pKeyAttribute.strKeyName, ref sMemberWrapper);
            else
                AddDictionary_MemberInfo(pProperty.Name, ref sMemberWrapper);
        }
    }


    static KeyAttribute GetKeyAttribute_OrNull(MemberInfo pMember)
    {
        var arrAttribute = pMember.GetCustomAttributes();
        foreach (var pAttribute in arrAttribute)
        {
            KeyAttribute pAttributeKey = pAttribute as KeyAttribute;
            if (pAttributeKey != null)
                return pAttributeKey;

        }

        return null;
    }

    static bool ExtractKeyValue(string strTargetLine, out string strKey, out string strValue)
    {
        string[] arrLang = strTargetLine.Split('=');
        if (arrLang.Length < 2)
        {
            strKey = string.Empty;
            strValue = string.Empty;
            return false;
        }

        strKey = arrLang[0].TrimStart(const_arrTrimKey).TrimEnd(const_arrTrimKey);
        strValue = arrLang[1].TrimStart(const_arrTrimValue).TrimEnd(const_arrTrimValue).Replace("\\n", "\n");

        // 주석 이후 내용은 제거
        int iCommentIndex = strValue.IndexOf("//");
        if (iCommentIndex != -1)
            strValue = strValue.Substring(0, iCommentIndex);

        return true;
    }

    // SMemberWrapper Struct이므로 값복사를 막기위한 ref
    static void AddDictionary_MemberInfo(string strKey, ref SMemberWrapper sMemberWrapper)
    {
        if(_mapMemberInfoTemp.ContainsKey(strKey))
        {
            Debug.LogError(typeof(SCManagerParserText).Name + "중복되는 키값이 존재합니다. Key : " + strKey);
            return;
        }

        _mapMemberInfoTemp.Add(strKey, sMemberWrapper);
    }

    static bool CheckIsValid(string strTargetLine)
    {
        return (strTargetLine.StartsWith("//") == false &&
                 strTargetLine.Contains("="));
    }

    #endregion protected & private


}
#endif

#region Test
#if NET_4_6
#if UNITY_EDITOR
public class SCManagerParserText_Test
{
    /* 텍스트 내용

    형식은
    키 = 값
    입니다.
    위 내용은 클래스 파싱에 안들어갑니다

    Int = 1
    Float = 2

    c= 3

    스트링 = 스트링 스트링 // 주석처리도 되요
    Int_Private = 7

    // Int_Private = 8 
    위의 값도 주석처리 됩니다 

     */

    public static readonly string const_strTest =
        "형식은\r\n키 = 값\r\n입니다.\r\n" +
        "위 내용은 클래스 파싱에 안들어갑니다\r\n" +
        "Int = 1\r\n" +
        "Float = 2\r\n" +
        "c= 3\r\n" +
        "스트링 = 스트링 스트링 // 주석처리도 되요\r\n" +
        "Int_Private = 7\r\n" +
        "// Int_Private = 8\r\n" +
        "위의 값도 주석처리 됩니다\r\n";

    public class STestObject
    {
        public int Int;
        public float Float;
        [Key("c")]
        public string String;

        [Key("스트링")]
        public string Property_String { get; private set; }

        private int Int_Private = 0;

        [UnityEngine.TestTools.UnityTest]
        [Category("StrixLibrary")]
        public IEnumerator Test_ManagerParserText()
        {
            STestObject pTestObject = SCManagerParserText.ParsingObject<STestObject>(const_strTest);

            Assert.IsTrue(pTestObject.Int == 1);
            Assert.IsTrue(pTestObject.Property_String == " 스트링 스트링 ");
            Assert.IsTrue(pTestObject.Int_Private == 7);

            yield break;
        }
    }
}
#endif
#endif
#endregion