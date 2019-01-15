#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-13 오후 5:03:41
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if UNITY_EDITOR
using NUnit.Framework;
#endif

/// <summary>
/// 
/// </summary>
public class SCParserJson : CSingletonNotMonoBase<SCParserJson>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [System.Serializable]
    public class Wrapper_ForArray<T>
    {
        public T[] array;
    }

    /* protected & private - Field declaration         */


    static private StringBuilder _pStrBuilder = new StringBuilder();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public void DoWriteJson(string strFolderPath, string strFileName, System.Object pWriteObj)
    {
        strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
        string strFilePath = ExtractFilePath(strFileName, strFolderPath);
        string strJson = JsonUtility.ToJson(pWriteObj, true);
        _pStrBuilder.Length = 0;
        _pStrBuilder.Append(strJson);

        try
        {
            if (System.IO.File.Exists(strFolderPath) == false)
                System.IO.Directory.CreateDirectory(strFolderPath);

            using (StreamWriter sw = new StreamWriter(File.Open(strFilePath, FileMode.Create), Encoding.UTF8))
            {
                sw.Write(_pStrBuilder.ToString());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("경고 Json Write 에러 파일이름 : " + strFileName + " 에러 : " + e);
        }
    }

    static public void DoWriteJsonArray<T>(string strFolderPath, string strFileName, T[] pWriteObj)
    {
        strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
        string strFilePath = ExtractFilePath(strFileName, strFolderPath);
        
        _pStrBuilder.Length = 0;
        _pStrBuilder.Append("{\n    \"array\": [\n");

        for(int i = 0; i < pWriteObj.Length; i++)
        {
            _pStrBuilder.Append(JsonUtility.ToJson(pWriteObj[i], true));

            if (i == pWriteObj.Length - 1)
                _pStrBuilder.Append("\n");
            else
                _pStrBuilder.Append(",\n");
        }
        _pStrBuilder.Append("   ]\n}");

        try
        {
            if (System.IO.File.Exists(strFolderPath) == false)
                System.IO.Directory.CreateDirectory(strFolderPath);

            using (StreamWriter sw = new StreamWriter(File.Open(strFilePath, FileMode.Create), Encoding.UTF8))
            {
                sw.Write(_pStrBuilder.ToString());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("경고 Json Write 에러 파일이름 : " + strFileName + " 에러 : " + e);
        }
    }

    static public bool DoReadJson<T>(string strFolderPath, string strFileName, out T sData)
        where T : class, new()
    {
        bool bSuccess = true;
        sData = new T();

        try
        {
            string strText = "";
            strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
            string strFilePath = ExtractFilePath(strFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            JsonUtility.FromJsonOverwrite(strText, sData);
        }
        catch (System.Exception e)
        {
            sData = null; bSuccess = false;
            Debug.LogError("경고 Json Write 에러 파일이름 : " + strFileName + " 에러 : " + e);
        }

        if (sData == null)
            bSuccess = false;

        return bSuccess;
    }

    static public bool DoReadJson_SO<T>(string strFolderPath, string strFileName, out T sData)
    where T : ScriptableObject
    {
        bool bSuccess = true;
        sData = ScriptableObject.CreateInstance<T>();

        try
        {
            string strText = "";
            strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
            string strFilePath = ExtractFilePath(strFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            JsonUtility.FromJsonOverwrite(strText, sData);
        }
        catch (System.Exception e)
        {
            sData = null; bSuccess = false;
            Debug.LogError("경고 Json Write 에러 파일이름 : " + strFileName + " 에러 : " + e);
        }

        if (sData == null)
            bSuccess = false;

        return bSuccess;
    }


    static public bool DoReadJson_ScriptableObject<T>(string strFolderPath, string strFileName, out T sData)
        where T : ScriptableObject, new()
    {
        bool bSuccess = true;
        try
        {
            string strText = "";
            strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
            string strFilePath = ExtractFilePath(strFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            sData = ScriptableObject.CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(strText, sData);
        }
        catch { sData = null; bSuccess = false; }

        if (sData == null)
            bSuccess = false;

        return bSuccess;
    }

    static public bool DoReadJsonArray<T>(string strFolderPath, string strFileName, ref List<T> listOutData)
        where T : class, new()
    {
        bool bSuccess = true;
        listOutData.Clear();
        try
        {
            string strText = "";
            strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
            string strFilePath = ExtractFilePath(strFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(strText);
            for (int i = 0; i < wrapper.array.Length; i++)
            {
                if(wrapper.array[i] != null)
                    listOutData.Add(wrapper.array[i]);
            }
        }
        catch { bSuccess = false; }

        if (listOutData.Count == 0)
            bSuccess = false;

        return bSuccess;
    }

    static public bool DoReadJsonArray_ScriptableObject<T>(string strFolderPath, string strFileName, ref List<T> listOutData)
    where T : ScriptableObject, new()
    {
        bool bSuccess = true;
        listOutData.Clear();
        try
        {
            string strText = "";
            strFolderPath = Application.persistentDataPath + "/" + strFolderPath;
            string strFilePath = ExtractFilePath(strFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            strText = strText.Replace("]\n", "]").Replace("}\n", "}");
            int iIndex = strText.IndexOf("[") + 1;
            string strTextParsing = strText.Substring(iIndex);

            while(true)
            {
                int iIndexLast = strTextParsing.IndexOf("},");
                if (iIndexLast == -1)
                {
                    iIndexLast = strTextParsing.IndexOf("]}");
                    if (iIndexLast == -1)
                        break;

                    iIndexLast -= 1;
                }

                string strData = strTextParsing.Substring(0, iIndexLast + 1);
                T pData = ScriptableObject.CreateInstance<T>();
                JsonUtility.FromJsonOverwrite(strData, pData);

                strTextParsing = strTextParsing.Substring(iIndexLast + 2);
                listOutData.Add(pData);
            }
        }
        catch { bSuccess = false; }

        if (listOutData.Count == 0)
            bSuccess = false;

        return bSuccess;
    }


    //static public bool DoReadJsonArray<T>(WWW www, out T[] arrData)
    //{
    //    bool bSuccess = true;
    //    try
    //    {
    //        string encodedString = www.text;
    //        if (www.bytes.Length >= 3 &&
    //           www.bytes[0] == 239 && www.bytes[1] == 187 && www.bytes[2] == 191)   // UTF8 코드 확인
    //        {
    //            encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
    //        }

    //        Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(encodedString);
    //        arrData = wrapper.array;
    //    }
    //    catch { arrData = null; bSuccess = false; }

    //    return bSuccess;
    //}

    //static public bool DoReadJsonArray<T>(UnityEngine.Networking.UnityWebRequest www, out T[] arrData)
    //{
    //    bool bSuccess = true;
    //    try
    //    {
    //        string encodedString = www.downloadHandler.text;
    //        if (www.downloadHandler.data.Length >= 3 &&
    //           www.downloadHandler.data[0] == 239 && www.downloadHandler.data[1] == 187 && www.downloadHandler.data[2] == 191)   // UTF8 코드 확인
    //        {
    //            encodedString = Encoding.UTF8.GetString(www.downloadHandler.data, 3, www.downloadHandler.data.Length - 3);
    //        }

    //        Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(encodedString);
    //        arrData = wrapper.array;
    //    }
    //    catch { arrData = null; bSuccess = false; }

    //    return bSuccess;
    //}


    //static public bool DoReadJsonArray<T>(WWW www, ref List<T> listOutData)
    //{
    //    bool bSuccess = true;
    //    listOutData.Clear();
    //    try
    //    {
    //        string encodedString = www.text;
    //        if (www.bytes.Length >= 3 &&
    //           www.bytes[0] == 239 && www.bytes[1] == 187 && www.bytes[2] == 191)   // UTF8 코드 확인
    //        {
    //            encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
    //        }

    //        Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(encodedString);
    //        for (int i = 0; i < wrapper.array.Length; i++)
    //            listOutData.Add(wrapper.array[i]);
    //    }
    //    catch { listOutData = null; bSuccess = false; }

    //    return bSuccess;
    //}


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    static private string ExtractFilePath(string strFileName, string strFolderPath)
    {
        _pStrBuilder.Length = 0;
        _pStrBuilder.Append(strFolderPath);
        _pStrBuilder.Append("/");

        _pStrBuilder.Append(strFileName.ToString());
        _pStrBuilder.Append(".json");

        return _pStrBuilder.ToString();
    }

    #endregion Private

}


#if UNITY_EDITOR
#region Test

public class SCParserJson_Test
{
    private const string const_strFolderPath = "Test_SCParserJson";

    [System.Serializable]
    public class STestJson
    {
        public int _int = -1;
        public string _string = "string";
        public Vector3 _vector3 = Vector3.one;

        public void DoSetDataRandom()
        {
            _int = Random.Range(1, 1000);
            _string = _int.ToString() + " 한글 테스트";
            _vector3 = Vector3.one * _int;
        }
    }

    [System.Serializable]
    public class STestJson_ScriptableObject : CScriptableObject
    {
        public int _int = -1;
        public string _string = "string";
        public Vector3 _vector3 = Vector3.one;

        public void DoSetDataRandom()
        {
            _int = Random.Range(1, 1000);
            _string = _int.ToString() + " 한글 테스트";
            _vector3 = Vector3.one * _int;
        }
    }



    [Test]
    public void Test_Write_And_Read_SingleData()
    {
        STestJson pTestJson = new STestJson();
        STestJson pTestJson2 = new STestJson();
        pTestJson.DoSetDataRandom();

        Assert.IsFalse(pTestJson._int == pTestJson2._int);
        Assert.IsFalse(pTestJson._string == pTestJson2._string);
        Assert.IsFalse(pTestJson._vector3 == pTestJson2._vector3);

        SCParserJson.DoWriteJson(const_strFolderPath, "Test_Write_And_Read_SingleData", pTestJson);
        SCParserJson.DoReadJson(const_strFolderPath, "Test_Write_And_Read_SingleData", out pTestJson2);

        Assert.IsTrue(pTestJson._int == pTestJson2._int);
        Assert.IsTrue(pTestJson._string == pTestJson2._string);
        Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
    }

    [Test]
    public void Test_Write_And_Read_MultipleData()
    {
        List<STestJson> listTestJson = new List<STestJson>();
        List<STestJson> listTestJson2 = new List<STestJson>();

        for (int i = 0; i < 3; i++)
        {
            STestJson pTestJson = new STestJson();
            pTestJson.DoSetDataRandom();

            listTestJson.Add(pTestJson);
        }

        SCParserJson.DoWriteJsonArray(const_strFolderPath, "Test_Write_And_Read_MultipleData", listTestJson.ToArray());
        SCParserJson.DoReadJsonArray(const_strFolderPath, "Test_Write_And_Read_MultipleData", ref listTestJson2);

        for (int i = 0; i < 3; i++)
        {
            STestJson pTestJson = listTestJson[i];
            STestJson pTestJson2 = listTestJson2[i];

            Assert.IsTrue(pTestJson._int == pTestJson2._int);
            Assert.IsTrue(pTestJson._string == pTestJson2._string);
            Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
        }
    }

    [Test]
    public void Test_Write_And_Read_SingleScriptableData()
    {
        STestJson_ScriptableObject pTestJson = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
        STestJson_ScriptableObject pTestJson2 = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
        pTestJson.DoSetDataRandom();

        Assert.IsFalse(pTestJson._int == pTestJson2._int);
        Assert.IsFalse(pTestJson._string == pTestJson2._string);
        Assert.IsFalse(pTestJson._vector3 == pTestJson2._vector3);

        SCParserJson.DoWriteJson(const_strFolderPath, "Test_Write_And_Read_SingleScriptableData", pTestJson);
        SCParserJson.DoReadJson_ScriptableObject(const_strFolderPath, "Test_Write_And_Read_SingleScriptableData", out pTestJson2);

        Assert.IsTrue(pTestJson._int == pTestJson2._int);
        Assert.IsTrue(pTestJson._string == pTestJson2._string);
        Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
    }

    [Test]
    public void Test_Write_And_Read_MultipleScriptableData()
    {
        List<STestJson_ScriptableObject> listTestJson = new List<STestJson_ScriptableObject>();
        List<STestJson_ScriptableObject> listTestJson2 = new List<STestJson_ScriptableObject>();

        for (int i = 0; i < 3; i++)
        {
            STestJson_ScriptableObject pTestJson = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
            pTestJson.DoSetDataRandom();

            listTestJson.Add(pTestJson);
        }

        SCParserJson.DoWriteJsonArray(const_strFolderPath, "Test_Write_And_Read_MultipleScriptableData", listTestJson.ToArray());
        SCParserJson.DoReadJsonArray_ScriptableObject(const_strFolderPath, "Test_Write_And_Read_MultipleScriptableData", ref listTestJson2);

        for (int i = 0; i < 3; i++)
        {
            STestJson_ScriptableObject pTestJson = listTestJson[i];
            STestJson_ScriptableObject pTestJson2 = listTestJson2[i];

            Assert.IsTrue(pTestJson._int == pTestJson2._int);
            Assert.IsTrue(pTestJson._string == pTestJson2._string);
            Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
        }
    }
}

#endregion
#endif