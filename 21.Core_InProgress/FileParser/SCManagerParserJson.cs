using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-16 오후 7:06:31
// Description : Json 파싱, Struct는 시리얼라이즈가 되어야 하며, 접근에 제한을 받으면 안된다. Ex) Generic
//               퍼블릭 변수만 파싱 가능하다.
//               
// Edit Log    : 
// ============================================ 

// Json Format
// 단일형
// {
//      "_strCharacterName": "RedWoman",
//      "_strSpriteName": "f004",
// }

// 복수형
//{
//  "array": [
//    {
//      "_strCharacterName": "RedWoman",
//      "_strSpriteName": "f004",
//    },
//    {
//      "_strCharacterName": "Blueman",
//      "_strSpriteName": "f010",
//    }
//  ]
//} 

//[System.Serializable]
//public struct SCharacter
//{
//    public string _strCharacterName; 
//    public string _strSpriteName;
//}

#pragma warning disable 0649

public class SCManagerParserJson : SCManagerResourceBase<SCManagerParserJson, string, TextAsset>
{
	public const string const_strFolderName = "JsonData";

	[Serializable]
    public class Wrapper_ForArray<T>
    {
        public T[] array;
    }

    static private StringBuilder _pStrBuilder = new StringBuilder();

    // ========================== [ Division ] ========================== //

    static public bool DoReadJson_FromResource<ENUM_FILE_NAME, T>(string strFolderPath, ENUM_FILE_NAME eFileName, out T sData)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
        where T : class
    {
        bool bSuccess = true;
        try
        {
            string strText = "";

            string strFilePath = ExtractLocalFilePath(eFileName, strFolderPath);
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);

            sData = JsonUtility.FromJson<T>(strText);
        }
        catch { sData = null; bSuccess = false; }

        if (sData == null)
            bSuccess = false;

        return bSuccess;
    }


    static public bool DoReadJson<T>(string strJsonFormatText, out T sData)
    {
        bool bSuccess = true;
        try
        {
            sData = JsonUtility.FromJson<T>(strJsonFormatText);
        }
        catch { sData = default(T); bSuccess = false; }

        return bSuccess;
    }
    
    static public bool DoReadJsonArray<T>(WWW www, out T[] arrData)
    {
        bool bSuccess = true;
        try
        {
            string encodedString = www.text;
            if (www.bytes.Length >= 3 &&
               www.bytes[0] == 239 && www.bytes[1] == 187 && www.bytes[2] == 191)   // UTF8 코드 확인
            {
                encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
            }

            Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(encodedString);
            arrData = wrapper.array;
        }
        catch { arrData = null; bSuccess = false; }

        return bSuccess;
    }

	static public bool DoReadJsonArray<T>( UnityEngine.Networking.UnityWebRequest www, out T[] arrData )
	{
		bool bSuccess = true;
		try
		{
			string encodedString = www.downloadHandler.text;
			if (www.downloadHandler.data.Length >= 3 &&
			   www.downloadHandler.data[0] == 239 && www.downloadHandler.data[1] == 187 && www.downloadHandler.data[2] == 191)   // UTF8 코드 확인
			{
				encodedString = Encoding.UTF8.GetString( www.downloadHandler.data, 3, www.downloadHandler.data.Length - 3 );
			}

			Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>( encodedString );
			arrData = wrapper.array;
		}
		catch { arrData = null; bSuccess = false; }

		return bSuccess;
	}


	static public bool DoReadJsonArray<T>(WWW www, ref List<T> listOutData)
    {
        bool bSuccess = true;
        listOutData.Clear();
        try
        {
            string encodedString = www.text;
            if (www.bytes.Length >= 3 &&
               www.bytes[0] == 239 && www.bytes[1] == 187 && www.bytes[2] == 191)   // UTF8 코드 확인
            {
                encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
            }

            Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(encodedString);
            for (int i = 0; i < wrapper.array.Length; i++)
                listOutData.Add(wrapper.array[i]);
        }
        catch { listOutData = null; bSuccess = false; }

        return bSuccess;
    }

    //public bool DoReadJson_FromResource<ENUM_FILE_NAME, T>(string strFolderPath, ENUM_FILE_NAME eFileName, out T sData)
    //where ENUM_FILE_NAME : System.IConvertible, System.IComparable
    //where T : class
    //{
    //    bool bSuccess = true;
    //    try
    //    {
    //        string strText = "";

    //        string strFilePath = ExtractLocalFilePath(eFileName, strFolderPath);
    //        if (System.IO.File.Exists(strFilePath))
    //            strText = System.IO.File.ReadAllText(strFilePath);

    //        sData = JsonUtility.FromJson<T>(strText);
    //    }
    //    catch { sData = null; bSuccess = false; }

    //    if (sData == null)
    //        bSuccess = false;

    //    return bSuccess;
    //}

    public bool DoReadJson_FromResource<ENUM_FILE_NAME, T>(ENUM_FILE_NAME eFileName, out T sData)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
		where T : class
	{
        bool bSuccess = true;
        try
        {
            string strText = "";
            if (_eResourcePath == EResourcePath.Resources)
                strText = DoGetResource_Origin(eFileName.ToString()).text;
            else if (_eResourcePath == EResourcePath.PersistentDataPath)
            {
                string strFilePath = ExtractLocalFilePath(eFileName, _strFolderPath);
                if (System.IO.File.Exists(strFilePath))
                    strText = System.IO.File.ReadAllText(strFilePath);
            }

            sData = JsonUtility.FromJson<T>(strText);
        }
        catch { sData = null; bSuccess = false; }

        if (sData == null)
            bSuccess = false;

        return bSuccess;
    }

    public void DoReadJsonArray_FromResource<ENUM_FILE_NAME, T>(ENUM_FILE_NAME eFileName, out T[] arrOutData)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
    {
        string strText = "";

        if (_eResourcePath == EResourcePath.Resources)
            strText = DoGetResource_Origin(eFileName.ToString()).text;
        else if (_eResourcePath == EResourcePath.PersistentDataPath)
        {
            string strFilePath = ExtractLocalFilePath(eFileName, _strFolderPath );
            if (System.IO.File.Exists(strFilePath))
                strText = System.IO.File.ReadAllText(strFilePath);
        }

        Wrapper_ForArray<T> wrapper = JsonUtility.FromJson<Wrapper_ForArray<T>>(strText);
        arrOutData = wrapper.array;
    }

	public void DoReadJson_And_InitEnumerator<T_Value>( string strFileName, ref List<T_Value> listOutData )
	{
		T_Value[] arrData = DoReadJsonArray_FromResource<T_Value>( strFileName );
		listOutData.AddRange( arrData );
	}

	public void DoReadJson_And_InitEnumerator<T_Key, T_Value>( string strFileName, ref Dictionary<T_Key, T_Value> mapOutData )
		where T_Value : IDictionaryItem<T_Key>
	{
		T_Value[] arrData = DoReadJsonArray_FromResource<T_Value>( strFileName );
		mapOutData.DoAddItem( arrData );
	}

	public void DoReadJson_And_InitEnumerator<T_Key, T_Value>( string strFileName, ref Dictionary<T_Key, List<T_Value>> mapOutData )
	where T_Value : IDictionaryItem<T_Key>
	{
		T_Value[] arrData = DoReadJsonArray_FromResource<T_Value>( strFileName );
		mapOutData.DoAddItem( arrData );
	}

	public T[] DoReadJsonArray_FromResource<T>( string strFileName )
	{
		T[] arrReturn;
		DoReadJsonArray_FromResource<string, T>( strFileName, out arrReturn );
		return arrReturn;
	}

    public void DoWriteJson<ENUM_FILE_NAME>(ENUM_FILE_NAME eFileName, System.Object pWriteObj)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
    {
        string strFilePath = ExtractLocalFilePath(eFileName, _strFolderPath );
        _pStrBuilder.Length = 0;
        string strJson = JsonUtility.ToJson(pWriteObj, true);
        _pStrBuilder.Append(strJson);

        try
        {
            if (System.IO.File.Exists(_strFolderPath) == false)
                System.IO.Directory.CreateDirectory(_strFolderPath);

            using (StreamWriter sw = new StreamWriter(File.Open(strFilePath, FileMode.Create), Encoding.UTF8))
            {
                sw.Write(_pStrBuilder.ToString());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("경고 Json Write 에러 파일이름 : " + eFileName + " 에러 : " + e);
        }
    }

	static public void DoWriteJson<ENUM_FILE_NAME>( string strFolderPath, ENUM_FILE_NAME eFileName, System.Object pWriteObj )
		where ENUM_FILE_NAME : System.IConvertible, System.IComparable
	{
		string strFilePath = ExtractLocalFilePath( eFileName, strFolderPath );
		_pStrBuilder.Length = 0;
		string strJson = JsonUtility.ToJson( pWriteObj, true );
		_pStrBuilder.Append( strJson );

		try
		{
			if (System.IO.File.Exists( strFolderPath ) == false)
				System.IO.Directory.CreateDirectory( strFolderPath );

			using (StreamWriter sw = new StreamWriter( File.Open( strFilePath, FileMode.Create ), Encoding.UTF8 ))
			{
				sw.Write( _pStrBuilder.ToString() );
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning( "경고 Json Write 에러 파일이름 : " + eFileName + " 에러 : " + e );
		}
	}

	public void DoWriteJsonArray<ENUM_FILE_NAME, T>(ENUM_FILE_NAME eFileName, T[] pWriteObj)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
    {
        string strFilePath = ExtractLocalFilePath(eFileName, _strFolderPath );
        _pStrBuilder.Length = 0;

        Wrapper_ForArray<T> wrapper = new Wrapper_ForArray<T>();
        wrapper.array = pWriteObj;
        _pStrBuilder.Append(JsonUtility.ToJson(wrapper, true));

        try
        {
            if (System.IO.File.Exists(_strFolderPath) == false)
                System.IO.Directory.CreateDirectory(_strFolderPath);

            using (StreamWriter sw = new StreamWriter(File.Open(strFilePath, FileMode.Create), Encoding.UTF8))
            {
                sw.Write(_pStrBuilder.ToString());
            }
        }
        catch (System.Exception e)
        {
            // Debug.LogWarning("경고 Json Write 에러 파일이름 : " + eFileName + " 에러 : " + e);
        }
    }

    // ========================== [ Division ] ========================== //

    protected override void OnMakeClass(MonoBehaviour pBaseClass, ref bool bIsMultipleResource)
    {
        base.OnMakeClass(pBaseClass, ref bIsMultipleResource);

		_pStrBuilder.Length = 0;
		if (_eResourcePath == EResourcePath.PersistentDataPath)
            _pStrBuilder.Append(Application.persistentDataPath);
        else if (_eResourcePath == EResourcePath.StreamingAssets)
            _pStrBuilder.Append(Application.streamingAssetsPath);
		else if(_eResourcePath == EResourcePath.Resources)
			_pStrBuilder.Append( Application.dataPath + "/Resources" );

		_pStrBuilder.Append( "/" + _strResourceLocalPath );
#if UNITY_EDITOR
		if (Application.isEditor && _eResourcePath != EResourcePath.Resources)
        {
            DirectoryInfo pDirectoryInfo = new DirectoryInfo(_pStrBuilder.ToString());
            if (pDirectoryInfo.Exists == false)
			{
				Debug.Log( "pDirectoryInfo.Exists == false // Create : " + _pStrBuilder.ToString() );
				pDirectoryInfo.Create();
			}
		}
#endif

		_strFolderPath = _pStrBuilder.ToString();
    }

    protected override bool OnWWWToResource<TResource>(WWW www, ref TResource pResource)
    {
        bool bSuccess = true;
        try
        {
			string encodedString = www.text;
			if (www.bytes[0] == 239 && www.bytes[1] == 187 && www.bytes[2] == 191)   // UTF8 코드 확인)
				encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);

			pResource = JsonUtility.FromJson<TResource>(encodedString);
        }
        catch { bSuccess = false; }

        return bSuccess;
    }

    protected override bool OnWWWToResource_Array<TResource>(WWW www, ref TResource[] arrResource)
    {
        bool bSuccess = true;
        try
        {
            string encodedString = Encoding.UTF8.GetString(www.bytes, 3, www.bytes.Length - 3);
            Wrapper_ForArray<TResource> wrapper = JsonUtility.FromJson<Wrapper_ForArray<TResource>>(encodedString);
            arrResource = wrapper.array;
        }
        catch { bSuccess = false; }

        return bSuccess;
    }

    protected override string OnGetFileExtension()
    {
        return ".json";
    }

    // ========================== [ Division ] ========================== //

    static private string ExtractLocalFilePath<ENUM_FILE_NAME>(ENUM_FILE_NAME eFileName, string strFolderPath)
        where ENUM_FILE_NAME : System.IConvertible, System.IComparable
    {
        _pStrBuilder.Length = 0;
        _pStrBuilder.Append( strFolderPath );
        _pStrBuilder.Append("/");

        _pStrBuilder.Append(eFileName.ToString());
        _pStrBuilder.Append(".json");

        return _pStrBuilder.ToString();
    }
}
