using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-18 오후 5:30:33
// Description : 
// Edit Log    : 주의) 파싱할 struct와 sql 데이터 테이블의 이름이 같아야 합니다.
// ============================================ 

public enum EPHPName
{
    Get,
    Get_OrInsert,
    Get_Range,
    Get_RandomKey,

	Update_Set_ID,
	Update_Set_ID_DoubleKey,
	Update_Set_Custom,

	Update_Add,
    Update_Set_ServerTime,

    Check_Value,
    Check_Count,
    Check_Time,
    CheckCount_AndUpdateAdd,
    CheckValue_AndUpdateSet,

	DeleteInfo,

    Insert,
}

public delegate void delDBDelgate( string strTableName, StringPair[] arrParameter );
public delegate bool delDBRequest( bool bResult, int iRequestCount );
public delegate bool delDBRequest_WithText(bool bResult, int iRequestCount, string strText);
public delegate bool delDBRequest_Generic<T>( bool bResult, int iRequestCount, T pDataResult );
public delegate bool delDBRequest_GenericArray<T>( bool bResult, int iRequestCount, T[] arrDataResult );

public class CManagerNetworkDB_Project : CManagerNetworkDB_Base<CManagerNetworkDB_Project> { }

public class CManagerNetworkDB_Base<CLASS_Driven> : CSingletonNotMonoBase<CLASS_Driven>
    where CLASS_Driven : CManagerNetworkDB_Base<CLASS_Driven>, new()
{
	// ===================================== //
	// public - Variable declaration         //
	// ===================================== //

	public event delDBDelgate p_Event_DB_OnRequest_Start;
	public event delDBDelgate p_Event_DB_OnRequest_Finish;

	// ===================================== //
	// protected - Variable declaration      //
	// ===================================== //

	protected string _strURLPrefix = null;            // Example : http://URL/{0}.php
    protected string _strDBName = null;               // Example : ProjectName

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

	public void DoSetEventNull()
	{
		p_Event_DB_OnRequest_Start = null;
		p_Event_DB_OnRequest_Finish = null;
	}

	public void DoSetNetworkAddress(string strURLPrefix, string strDBName)
	{
		_strURLPrefix = strURLPrefix;
		_strDBName = strDBName;
    }

    public IEnumerator CoExcutePHP<ENUM_PHP_NAME>(string hID, ENUM_PHP_NAME ePHPName, string strTableName, delDBRequest OnFinishLoad = null, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoExcutePHP(hID, ePHPName.ToString(), strTableName, OnFinishLoad, arrParameter);
    }

    public IEnumerator CoExcuteAndGetValue<ENUM_PHP_NAME>(string hID, ENUM_PHP_NAME ePHPName, string strTableName, delDBRequest_WithText OnFinishLoad = null, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoExcuteAndGetValue(hID, ePHPName.ToString(), strTableName, OnFinishLoad, arrParameter);
    }

    public IEnumerator CoLoadDataFromServer_Json<ENUM_PHP_NAME, T>(string hID, ENUM_PHP_NAME ePHPName, delDBRequest_Generic<T> OnFinishLoad, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoLoadDataFromServer(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
    }

    public IEnumerator CoLoadDataFromServer_Json_Array<ENUM_PHP_NAME, T>(string hID, ENUM_PHP_NAME ePHPName, delDBRequest_GenericArray<T> OnFinishLoad, params StringPair[] arrParameter)
        where ENUM_PHP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        yield return CoLoadDataListFrom_Array(hID, ePHPName.ToString(), OnFinishLoad, arrParameter);
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    // ========================================================================== //

    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    protected IEnumerator CoExcutePHP(string hID, string strPHPName, string strTableName, delDBRequest OnFinishLoad = null, params StringPair[] arrParameter)
    {
		if (p_Event_DB_OnRequest_Start != null)
			p_Event_DB_OnRequest_Start( strTableName, arrParameter );

		int iRequestCount = 0;
		while(true)
		{
#if UNITY_2017_1
			UnityEngine.Networking.UnityWebRequest www = GetWWWNew( hID, strPHPName, strTableName, arrParameter );
			yield return www.SendWebRequest();
			bool bSuccess = www.error == null && (www.downloadHandler.text.Contains( "false" ) == false);

			if (bSuccess == false)
				Debug.Log( "DBParser Error " + www.downloadHandler.text + " php : " + strPHPName + " TableName : " + strTableName );
#else
			WWW www = GetWWW( hID, strPHPName, strTableName, arrParameter );
			yield return www;
			bool bSuccess = www.error == null && (www.text.Contains( "false" ) == false);

			if (bSuccess == false)
				Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName );
#endif

			// Debug.Log(strPHPName + " result : " + www.text);
			//Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName );

			if (OnFinishLoad == null)
				break;
			else if (OnFinishLoad( bSuccess, ++iRequestCount ))
				break;
		}

		if (p_Event_DB_OnRequest_Finish != null)
			p_Event_DB_OnRequest_Finish( strTableName, arrParameter );

		yield break;
    }

    protected IEnumerator CoExcuteAndGetValue(string hID, string strPHPName, string strTableName, delDBRequest_WithText OnFinishLoad = null, params StringPair[] arrParameter)
	{
		if (p_Event_DB_OnRequest_Start != null)
			p_Event_DB_OnRequest_Start( strTableName, arrParameter );

		int iRequestCount = 0;
		while(true)
		{
			WWW www = GetWWW( hID, strPHPName, strTableName, arrParameter );
			yield return www;

			bool bSuccess = www.error == null && (www.text.Contains( "false" ) == false);

			//Debug.Log(strPHPName + " result : " + www.text);

			if (bSuccess == false)
			{
				Debug.Log( "DBParser Error " + www.text + " php : " + strPHPName + " TableName : " + strTableName );
				for (int i = 0; i < arrParameter.Length; i++)
					Debug.Log( string.Format( "Key{0} : {1}, Value{2} : {3} ", i, arrParameter[i].strKey, i, arrParameter[i].strValue ) );
			}

			if (OnFinishLoad == null)
				break;
			else if (OnFinishLoad( bSuccess, ++iRequestCount , www.text))
				break;
		}

		if (p_Event_DB_OnRequest_Finish != null)
			p_Event_DB_OnRequest_Finish( strTableName, arrParameter );

		yield break;
    }

    protected IEnumerator CoLoadDataFromServer<T>(string hID, string strPHPName, delDBRequest_Generic<T> OnFinishLoad, params StringPair[] arrParameter)
	{
		T pData = default( T );
		string strTableName = typeof( T ).ToString();

		if (p_Event_DB_OnRequest_Start != null)
			p_Event_DB_OnRequest_Start( strTableName, arrParameter );

		int iRequestCount = 0;
		WWW www;
		bool bSuccess = true;
		while (true)
		{
			bSuccess = true;
			www = GetWWW( hID, strPHPName, strTableName, arrParameter );
			yield return www;

			//Debug.Log(strPHPName + " result : " + www.text);

			List<T> listOutData = new List<T>();
			bSuccess = www.error == null;
			try
			{
				bSuccess = SCManagerParserJson.DoReadJsonArray<T>( www, ref listOutData );
				pData = listOutData[0];
			}
			catch
			{
				bSuccess = false;
			}


			if (OnFinishLoad == null)
				break;
			else if (OnFinishLoad( bSuccess, ++iRequestCount, pData ))
				break;
		}
		
		if (bSuccess == false)
		{
			Debug.LogWarning( "[DBParser Error] RequestCount : " + iRequestCount + " php : " + strPHPName + " TableName : " + strTableName + " Error : " + www.text );
			for (int i = 0; i < arrParameter.Length; i++)
				Debug.LogWarning( string.Format( "Key{0} : {1}, Value{2} : {3} ", i, arrParameter[i].strKey, i, arrParameter[i].strValue ) );
		}

		if (p_Event_DB_OnRequest_Finish != null)
			p_Event_DB_OnRequest_Finish( strTableName, arrParameter );

		yield break;
    }

    public IEnumerator CoLoadDataListFrom_Array<T>(string hID, string strPHPName, delDBRequest_GenericArray<T> OnFinishLoad, params StringPair[] arrParameter)
	{
		string strTableName = typeof( T ).Name;

		if (p_Event_DB_OnRequest_Start != null)
			p_Event_DB_OnRequest_Start( strTableName, arrParameter );

		//UnityEngine.Networking.UnityWebRequest www;
		WWW www;
		int iRequestCount = 0;
		bool bSuccess;
		while (true)
		{
			bSuccess = true;
			www = GetWWW( hID, strPHPName, strTableName, arrParameter );
			yield return www;
			//www = GetWWWNew( hID, strPHPName, strTableName, arrParameter );
			//yield return www.SendWebRequest();

			T[] arrOutData = null;
			bSuccess = www.error == null;
			try
			{
				bSuccess = SCManagerParserJson.DoReadJsonArray<T>( www, out arrOutData );
			}
			catch
			{
				bSuccess = false;
			}

			if (OnFinishLoad == null)
				break;
			else if (OnFinishLoad( bSuccess, ++iRequestCount, arrOutData ))
				break;
		}

		if (bSuccess == false)
		{
			Debug.Log( "DBParser Warning " + www.text + " php : " + strPHPName + " TableName : " + strTableName + " iRequestCount : " + iRequestCount, null );
			//Debug.Log( "DBParser Warning " + www.downloadHandler.text + " php : " + strPHPName + " TableName : " + strTableName + " iRequestCount : " + iRequestCount, null );
			for (int i = 0; i < arrParameter.Length; i++)
				Debug.LogWarning( string.Format( "Key{0} : {1}, Value{2} : {3} ", i, arrParameter[i].strKey, i, arrParameter[i].strValue ) );
		}

		if (p_Event_DB_OnRequest_Finish != null)
			p_Event_DB_OnRequest_Finish( strTableName, arrParameter );

		yield break;
    }



	// ===================================== //
	// private - [Other] Function            //
	// 찾기, 계산 등의 비교적 단순 로직      //
	// ===================================== //

    private bool CheckIsValidDB()
    {
        if (string.IsNullOrEmpty(_strURLPrefix) || string.IsNullOrEmpty(_strDBName))
            return false;

        return true;
    }

	private UnityEngine.Networking.UnityWebRequest GetWWWNew( string hID, string strPHPName, string strTableName, params StringPair[] arrParameter )
	{
        if (CheckIsValidDB() == false)
            Debug.LogError("DB INI Require Setting");

        WWWForm form = new WWWForm();
        if (string.IsNullOrEmpty(hID) == false)
            form.AddField( "id", hID );

		form.AddField( "dbname", _strDBName );
		form.AddField( "table", strTableName );
		form.AddField( "paramcount", arrParameter.Length );

		for (int i = 0; i < arrParameter.Length; i++)
		{
			if (arrParameter[i].strKey == null)
			{
				form.AddField( "key" + i, "" );
				Debug.Log( string.Format( "Error key {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
				form.AddField( "key" + i, arrParameter[i].strKey );

			if (arrParameter[i].strValue == null)
			{
				form.AddField( "value" + i, "" );
				Debug.Log( string.Format( "Error value {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
				form.AddField( "value" + i, arrParameter[i].strValue );
		}

		return UnityEngine.Networking.UnityWebRequest.Post( string.Format( _strURLPrefix, strPHPName ), form );
	}	

	private WWW GetWWW(string hID, string strPHPName, string strTableName, params StringPair[] arrParameter)
    {
        if (CheckIsValidDB() == false)
            Debug.LogError("DB INI Require Setting strPHPName: " + strPHPName + " strTableName: " + strTableName);

        WWWForm form = new WWWForm();
        if (string.IsNullOrEmpty(hID) == false)
			form.AddField("id", hID);

        form.AddField("dbname", _strDBName);
        form.AddField("table", strTableName.ToLower());
        form.AddField("paramcount", arrParameter.Length);
        
        for (int i = 0; i < arrParameter.Length; i++)
        {
			if (arrParameter[i].strKey == null)
			{
				form.AddField( "key" + i, "" );
				Debug.Log( string.Format( "Error key {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
                form.AddField("key" + i, arrParameter[i].strKey, System.Text.Encoding.UTF8);

			if (arrParameter[i].strValue == null)
			{
				form.AddField( "value" + i, "" );
				Debug.Log( string.Format( "Error value {0} is null PHP : {1} Table : {2}", i, strPHPName, strTableName ) );
			}
			else
                form.AddField("value" + i, arrParameter[i].strValue, System.Text.Encoding.UTF8);
        }

        return new WWW(string.Format(_strURLPrefix, strPHPName), form);
    }
}
