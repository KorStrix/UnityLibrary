using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 3:46:33
// Description : 
// Edit Log    : 
// ============================================ 

[System.Flags]
public enum EVolumeOff
{
	None = 0,
	SoundEffect = 1,
	BGM = 2,
	All,
}

[System.Serializable]
public class SINI_UserSetting
{
	public string strLanguage = "Korean";
	public float fVolumeEffect = 0.5f;
	public float fVolumeBGM = 0.5f;
	public EVolumeOff eVolumeOff = EVolumeOff.None;
	public bool bVibration;
	public string ID;
	public string strNickName;
}

[System.Serializable]
public class SINI_ApplicationSetting
{
	public int iBundleCode;

	public string strDBName;
	public string strPHPPrefix;

	public string[] arrLogIgnore_Level;
	public string[] arrLogIgnore_Writer;
}

public interface IDB_Insert
{
	StringPair[] IDB_Insert_GetField();
}

public struct StringPair
{
	public string strKey;
	public string strValue;

	static public StringPair Empty {  get { return new StringPair(); } }

	static public bool IsEmpty(StringPair pPair)
	{
		return string.IsNullOrEmpty( pPair.strKey ) && string.IsNullOrEmpty( pPair.strValue );
	}

	public StringPair( string strKey, string strValue )
	{
		this.strKey = strKey; this.strValue = strValue;
	}

	public StringPair( string strKey, object pValue )
	{
		this.strKey = strKey; this.strValue = pValue.ToString();
	}

	public StringPair( string strKey, System.Enum eEnum )
	{
		this.strKey = strKey; this.strValue = eEnum.GetHashCode().ToString();
	}
}

[RequireComponent( typeof( CCompoDontDestroyObj ) )]
public class CManagerFrameWorkBase<CLASS_Framework, ENUM_Scene_Name, ENUM_DataField> : CSingletonMonoBase<CLASS_Framework>
	where CLASS_Framework : CManagerFrameWorkBase<CLASS_Framework, ENUM_Scene_Name, ENUM_DataField>
	where ENUM_Scene_Name : System.IFormattable, System.IConvertible, System.IComparable
	where ENUM_DataField : System.IFormattable, System.IConvertible, System.IComparable
{
	private const string const_strLocalPath_INI = "INI";
	private const string const_strEmptySceneName = "Empty";

	public enum EINI_JSON_FileName
	{
		Sound,
		UserSetting,
		ApplicationSetting,
	}

	public enum ESceneLoadState
	{
		None,
		SceneLoadStart,
		SceneLoadFinish,
	}

	// ===================================== //
	// public - Variable declaration         //
	// ===================================== //

	static public CManagerNetworkDB_Project p_pNetworkDB { get { return CManagerNetworkDB_Project.instance; } }
	static public SCSceneLoader<ENUM_Scene_Name> p_pManagerScene { get { return _pManagerScene; } }

	static public event delDBDelgate p_Event_DB_OnRequest_Start
	{
		add { p_pNetworkDB.p_Event_DB_OnRequest_Start += value; }
		remove { p_pNetworkDB.p_Event_DB_OnRequest_Start -= value; }
	}

	static public event delDBDelgate p_Event_DB_OnRequest_Finish
	{
		add { p_pNetworkDB.p_Event_DB_OnRequest_Finish += value; }
		remove { p_pNetworkDB.p_Event_DB_OnRequest_Finish -= value; }
	}

	public static event System.Action<float> p_EVENT_OnLoadSceneProgress;
	public static event System.Action p_EVENT_OnStartLoadScene;
	public static event System.Action p_EVENT_OnFinishLoadScene;
	public static event System.Action p_EVENT_OnFinishPreLoadScene;

	public static System.Action p_EVENT_OnLoadFinish_LocalData;

	//private bool _bSuccessLoadScene; public bool p_bSuccessLoadScene { get { return _bSuccessLoadScene; } }

	// ===================================== //
	// protected - Variable declaration      //
	// ===================================== //

	static protected string _strCallBackRequest_SceneName;
	static protected System.Action _OnFinishLoad_Scene;

	protected SINI_UserSetting _pSetting_User; public SINI_UserSetting p_pUserSetting { get { return _pSetting_User; } }
	protected SINI_ApplicationSetting _pSetting_App; public SINI_ApplicationSetting p_pSetting_App { get { return _pSetting_App; } }

	// ===================================== //
	// private - Variable declaration        //
	// ===================================== //

	static protected SCSceneLoader<ENUM_Scene_Name> _pManagerScene;
	static protected SCManagerParserJson _pJsonParser_Persistent;
	static protected SCManagerParserJson _pJsonParser_StreammingAssets;
	static public SCManagerParserJson _pJsonParser_JsonData;

	//static private List<iTween> _listTween = new List<iTween>();

	protected string _strID; public string p_strID { get { return _strID; } }

	static private int _iLocalDataLoadingCount_Request;
	static private int _iLocalDataLoadingCount_Finish;

	// ========================================================================== //

	// ===================================== //
	// public - [Do] Function                //
	// 외부 객체가 요청                      //
	// ===================================== //

	static public void DoSetDBEventNull()
	{
		p_pNetworkDB.DoSetEventNull();
	}

	static public float GetFloat_InGameData( ENUM_DataField eDataField)
	{
		return SAppConfig.GetFloat( eDataField );
	}

	static public int GetInt_InGameData( ENUM_DataField eDataField )
	{
		return SAppConfig.GetInt( eDataField );
	}

	static public string GetString_GameData( ENUM_DataField eDataField )
	{
		return SAppConfig.GetString( eDataField );
	}

	static public void DoNetworkDB_CheckCount_IsEqualOrGreater<StructDB>( string strFieldName, object iCheckFieldCount, delDBRequest OnResult = null )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Check_Count, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iCheckFieldCount.ToString() ) ) );
	}

	static public void DoNetworkDB_UpdateAdd_If_CheckCount_IsEqualOrGreater<StructDB>( string strFieldName, object iCheckFieldCount, int iAddFieldCount, delDBRequest_WithText OnResult = null )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.CheckCount_AndUpdateAdd, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iCheckFieldCount.ToString() ), new StringPair( strFieldName, iAddFieldCount.ToString() ) ) );
	}

	static public void DoNetworkDB_UpdateAdd<StructDB>( string strFieldName, object iFieldCount, delDBRequest_WithText OnResult = null, params StringPair[] arrParams )
	{
		CheckIsContainField<StructDB>( strFieldName );

		if (arrParams.Length != 0)
		{
			StringPair[] arrNewParams = new StringPair[arrParams.Length + 1];
			arrNewParams[0] = new StringPair( strFieldName, iFieldCount );

			for (int i = 0; i < arrParams.Length; i++)
				arrNewParams[i + 1] = arrParams[i];

			instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.Update_Add, typeof( StructDB ).ToString(), OnResult, arrNewParams ) );
		}
		else
			instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( instance._strID, EPHPName.Update_Add, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, iFieldCount.ToString() ) ) );
	}


	static public void DoNetworkDB_GetRange_Orderby_HighToLow<StructDB>( string strFieldName, int iGetDataCount, delDBRequest_GenericArray<StructDB> OnFinishLoad = null )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json_Array( instance._strID, EPHPName.Get_Range, OnFinishLoad, new StringPair( strFieldName, iGetDataCount ) ) );
	}

	static public void DoNetworkDB_Get_Single<StructDB>( delDBRequest_Generic<StructDB> OnFinishLoad = null, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Get, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_GetOrInsert_Single<StructDB>( delDBRequest_Generic<StructDB> OnFinishLoad = null, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Get_OrInsert, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_GetRandomKey( string strCheckOverlapTableName, delDBRequest_WithText OnFinishLoad = null, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcuteAndGetValue( null, EPHPName.Get_RandomKey, strCheckOverlapTableName, OnFinishLoad, arrParams ) );
	}

	static public void DoNetworkDB_Get_Array<StructDB>( delDBRequest_GenericArray<StructDB> OnFinishLoad = null, params StringPair[] arrParams )
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json_Array( instance._strID, EPHPName.Get, OnFinishLoad, arrParams ) );
	}

	/// <summary>
	/// DB에 Generic에 있는 필드 값을 덮어 씌운다.
	/// </summary>
	/// <typeparam name="StructDB"></typeparam>
	/// <param name="strFieldName">Generic에 있는 필드 명</param>
	/// <param name="strSetFieldValue">Generic에 있는 필드에 덮어씌울 값</param>
	/// <param name="OnResult">결과 함수</param>
	static public void DoNetworkDB_Update_Set<StructDB>( string strFieldName, object strSetFieldValue, delDBRequest_Generic<StructDB> OnResult = null )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Update_Set_ID, OnResult, new StringPair( strFieldName, strSetFieldValue ) ) );
	}

	static public void DoNetworkDB_Update_Set_DoubleKey<StructDB>( string strFieldName, object strSetFieldValue, delDBRequest OnResult, StringPair pDoubleKey )
	{
		CheckIsContainField<StructDB>( strFieldName );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ID_DoubleKey, typeof( StructDB ).ToString(), OnResult,
			new StringPair[2] { pDoubleKey, new StringPair( strFieldName, strSetFieldValue ) } ) );
	}

	static public void DoNetworkDB_Update_Set_Custom<StructDB>( string strFieldName, object strSetFieldValue, delDBRequest OnResult, params StringPair[] arrField )
	{
		CheckIsContainField<StructDB>( strFieldName );

		StringPair[] arrPair = new StringPair[arrField.Length + 1];
		for (int i = 0; i < arrField.Length; i++)
			arrPair[i] = arrField[i];

		arrPair[arrField.Length] = new StringPair( strFieldName, strSetFieldValue.ToString() );

		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( null, EPHPName.Update_Set_Custom, typeof( StructDB ).ToString(), OnResult, arrPair ) );
	}

	static public void DoNetworkDB_Update_Set_Multi<StructDB>( delDBRequest OnResult, params StringPair[] arrParam )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ID, typeof( StructDB ).ToString(), OnResult, arrParam ) );
	}

	static public void DoNetworkDB_Update_Set_ServerTime<StructDB>( string strFieldName, delDBRequest OnResult )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Update_Set_ServerTime, typeof( StructDB ).ToString(), OnResult, new StringPair( strFieldName, "" ) ) );
	}

    static public void DoNetworkDB_Insert<StructDB>( delDBRequest OnResult, StructDB pStructDB )
		where StructDB : IDB_Insert
	{
        if(instance.isActiveAndEnabled)
            instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Insert, typeof( StructDB ).ToString(), OnResult, pStructDB.IDB_Insert_GetField() ) );
	}

	static public void DoNetworkDB_Insert_Get_InsertData<StructDB>( delDBRequest_Generic<StructDB> OnResult, StructDB pStructDB )
		where StructDB : IDB_Insert
	{
		instance.StartCoroutine( p_pNetworkDB.CoLoadDataFromServer_Json( instance._strID, EPHPName.Insert, OnResult, pStructDB.IDB_Insert_GetField() ) );
	}

	static public void DoNetworkDB_Delete<StructDB>( delDBRequest OnResult, params StringPair[] arrParam )
	{
		if (instance._strID != null)
			instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.DeleteInfo, typeof( StructDB ).ToString(), OnResult, arrParam ) );
		else
			Debug.Log( "Delete는 strID에 null이오면 안됩니다." );
	}

	static public void DoNetworkDB_Insert<StructDB>( delDBRequest OnResult, params StringPair[] arrParam )
	{
		instance.StartCoroutine( p_pNetworkDB.CoExcutePHP( instance._strID, EPHPName.Insert, typeof( StructDB ).ToString(), OnResult, arrParam ) );
	}

	public void DoShakeMobile()
	{
#if UNITY_ANDROID
		if (Application.isPlaying && _pSetting_User.bVibration)
			Handheld.Vibrate();
#endif
    }

    public void DoSetTestMode()
	{
		_strID = "Test";
	}

	// ===================================== //
	// public - [Event] Function             //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	static public void DoLoadScene(ENUM_Scene_Name eSceneName, LoadSceneMode eLoadSceneMode, System.Action OnFinishLoading = null)
	{
		SceneManager.LoadScene(eSceneName.ToString(), eLoadSceneMode);
		if (_OnFinishLoad_Scene == null && OnFinishLoading != null)
		{
			_OnFinishLoad_Scene = OnFinishLoading;
			_strCallBackRequest_SceneName = eSceneName.ToString();
		}
	}

	public void DoLoadSceneAsync( params ENUM_Scene_Name[] arrSceneName )
	{
		StartCoroutine(CoProcLoadSceneAsync(arrSceneName, false));
	}

	public void DoLoadSceneAsync( System.Action OnFinishLoadScene, params ENUM_Scene_Name[] arrSceneName )
	{
		p_EVENT_OnFinishLoadScene = OnFinishLoadScene;

		StartCoroutine(CoProcLoadSceneAsync(arrSceneName, false));
	}

	private IEnumerator CoProcLoadSceneAsync(ENUM_Scene_Name[] arrSceneName, bool bManualCall_EventOnFinishLoadScene)
	{
		// 로딩전 빈씬으로 메모리를 비워준다. 다음 로딩씬의 메모리가 너무 커서 프리징 걸릴수도있기때문에...
		yield return SceneManager.LoadSceneAsync(const_strEmptySceneName);

		System.GC.Collect();

		if (p_EVENT_OnStartLoadScene != null)
			p_EVENT_OnStartLoadScene();

		yield return null;

		Scene pScene_Empty = SceneManager.GetSceneByName(const_strEmptySceneName);

		int iMaxLoadScene = arrSceneName.Length;
		for (int i = 0; i < iMaxLoadScene; i++)
		{
			string strSceneName = arrSceneName[i].ToString();

			AsyncOperation pAsyncOperation = SceneManager.LoadSceneAsync(strSceneName, LoadSceneMode.Additive);
			Scene pCurrentLoadScene = SceneManager.GetSceneByName(strSceneName);

			while (pAsyncOperation.isDone == false)
			{
				float fTotalProgress = pAsyncOperation.progress + i;

				if (p_EVENT_OnLoadSceneProgress != null)
					p_EVENT_OnLoadSceneProgress(fTotalProgress / iMaxLoadScene);

				yield return null;
			}

			SceneManager.SetActiveScene(pCurrentLoadScene);

			GameObject[] arrGameObject = pScene_Empty.GetRootGameObjects();
			int iLen = arrGameObject.Length;

			for (int j = 0; j < iLen; j++)
			{
				GameObject pGameObject = arrGameObject[j];
				if (pGameObject.name.Equals("Main Camera")) continue;

				SceneManager.MoveGameObjectToScene(pGameObject, pCurrentLoadScene);
			}
		}

		yield return SceneManager.UnloadSceneAsync(const_strEmptySceneName);

		// 로딩 끝난후 먼저 실행되는 이벤트
		if (p_EVENT_OnFinishPreLoadScene != null)
			p_EVENT_OnFinishPreLoadScene();

		yield return new WaitForSecondsRealtime(0.5f);

		if (bManualCall_EventOnFinishLoadScene == false)
			EventCall_OnFinishLoadScene();

		p_EVENT_OnFinishPreLoadScene = null;
		p_EVENT_OnLoadSceneProgress = null;
		p_EVENT_OnStartLoadScene = null;

		//System.GC.Collect(System.GC.GetGeneration(this), System.GCCollectionMode.Optimized);
	}
    
	public void EventCall_OnFinishLoadScene()
	{
		if (p_EVENT_OnFinishLoadScene == null) return;

		p_EVENT_OnFinishLoadScene();
		p_EVENT_OnFinishLoadScene = null;
	}

	// ========================================================================== //

	// ===================================== //
	// protected - abstract & virtual        //
	// ===================================== //

	virtual protected void OnSceneLoaded( UnityEngine.SceneManagement.Scene pScene, UnityEngine.SceneManagement.LoadSceneMode eLoadMode ) { }
	virtual protected void OnLoadFinish_LocalData( ) { }

	// ===================================== //
	// protected - Unity API                 //
	// ===================================== //

	protected override void OnAwake()
	{
		base.OnAwake();

		_iLocalDataLoadingCount_Request = 0;
		_iLocalDataLoadingCount_Finish = 0;

		_pJsonParser_Persistent = SCManagerParserJson.DoMakeInstance( this, "", EResourcePath.PersistentDataPath );
		_pJsonParser_StreammingAssets = SCManagerParserJson.DoMakeInstance( this, const_strLocalPath_INI, EResourcePath.StreamingAssets );
		_pJsonParser_JsonData = SCManagerParserJson.DoMakeInstance( this, SCManagerParserJson.const_strFolderName, EResourcePath.Resources );

		_pManagerScene = new SCSceneLoader<ENUM_Scene_Name>();
		_pManagerScene.p_EVENT_OnSceneLoaded += ProcOnSceneLoaded;

        ProcParse_UserSetting();

        _iLocalDataLoadingCount_Request++;
        _iLocalDataLoadingCount_Request++;
        _iLocalDataLoadingCount_Request++;
        _pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource_Array<SINI_Sound>(EINI_JSON_FileName.Sound.ToString(), OnParseComplete_SoundSetting);
        _pJsonParser_StreammingAssets.DoStartCo_GetStreammingAssetResource<SINI_ApplicationSetting>( EINI_JSON_FileName.ApplicationSetting.ToString(), OnFinishParse_AppSetting );

		if (CManagerUILocalize.instance == null)
			CManagerUILocalize.EventMakeSingleton();

        CManagerUILocalize.instance.DoStartParse_Locale( CUIManagerLocalize_p_EVENT_OnChangeLocalize );
	}

	private void ProcParse_UserSetting()
	{
		if (_pJsonParser_Persistent.DoReadJson_FromResource( EINI_JSON_FileName.UserSetting, out _pSetting_User ) == false)
		{
			Debug.Log( "UserInfo - bParsingResult is Fail" );

			_pSetting_User = new SINI_UserSetting();
			_pJsonParser_Persistent.DoWriteJson( EINI_JSON_FileName.UserSetting, _pSetting_User );
		}

        CManagerSound.instance.DoSetVolumeEffect( _pSetting_User.fVolumeEffect );
        CManagerSound.instance.DoSetVolumeBGM( _pSetting_User.fVolumeBGM );
    }

	private void CUIManagerLocalize_p_EVENT_OnChangeLocalize()
	{
        // Debug.Log("CUIManagerLocalize_p_EVENT_OnChangeLocalize");

		SystemLanguage eCurLanguage = SystemLanguage.Unknown; // Application.systemLanguage
		try
		{
			eCurLanguage = (SystemLanguage)System.Enum.Parse( typeof( SystemLanguage ), _pSetting_User.strLanguage );
		}
		catch
		{
			List<SystemLanguage> listLocale = CManagerUILocalize.instance.p_listLocale;

			if (listLocale.Contains( eCurLanguage ) == false)
				eCurLanguage = SystemLanguage.English;

			Debug.LogWarning( "UserSetting에서 Enum 파싱에 실패해서 기본언어로 바꿨다" + _pSetting_User.strLanguage );

			_pSetting_User.strLanguage = eCurLanguage.ToString();
			_pJsonParser_Persistent.DoWriteJson( EINI_JSON_FileName.UserSetting, _pSetting_User );
		}

		Debug.Log( "Set Language " + eCurLanguage );

		CManagerUILocalize.instance.DoSet_Localize( eCurLanguage );
		CheckLoadFinish_LocalDataAll();
	}

	static private void ProcOnSceneLoaded( UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1 )
	{
		if (CManagerUILocalize.instance != null)
			CManagerUILocalize.instance.DoSetLocalize_CurrentScene();

		p_pManagerScene.EventCheckIsLoadComplete();

		instance.OnSceneLoaded( arg0, arg1 );
		if (_OnFinishLoad_Scene != null && _strCallBackRequest_SceneName != null && _strCallBackRequest_SceneName.CompareTo( arg0.name ) == 0)
		{
			_strCallBackRequest_SceneName = "";
			System.Action OnFinishCurrentScene = _OnFinishLoad_Scene;
			_OnFinishLoad_Scene = null;
			OnFinishCurrentScene();
		}
	}
    
	// ========================================================================== //

	// ===================================== //
	// private - [Proc] Function             //
	// 중요 로직을 처리                      //
	// ===================================== //

	static private void CheckLoadFinish_LocalDataAll()
	{
        // Debug.Log("CheckLoadFinish_LocalDataAll _iLocalDataLoadingCount_Finish : " + (_iLocalDataLoadingCount_Finish + 1) + " _iLocalDataLoadingCount_Request : " + _iLocalDataLoadingCount_Request);

        if (_iLocalDataLoadingCount_Request <= ++_iLocalDataLoadingCount_Finish)
		{
            if (p_EVENT_OnLoadFinish_LocalData != null)
				p_EVENT_OnLoadFinish_LocalData();

			instance.OnLoadFinish_LocalData();
		}
	}

	static private void CheckIsContainField<Struct>( string strFieldName )
	{
#if UNITY_EDITOR
		System.Type pType = typeof( Struct );
		System.Reflection.FieldInfo pField = pType.GetField( strFieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
		if (pField == null)
			Debug.LogError( "DB Error - Not Contain Field : " + strFieldName );
#endif
	}

	// ===================================== //
	// private - [Other] Function            //
	// 찾기, 계산 등의 비교적 단순 로직      //
	// ===================================== //

	private void OnParseComplete_SoundSetting( bool bSuccess, SINI_Sound[] arrSound )
	{
        // Debug.Log("OnParseComplete_SoundSetting");

		if (bSuccess)
            CManagerSound.instance.EventSetSoundOption( arrSound, _pSetting_User.fVolumeEffect, _pSetting_User.eVolumeOff );
        //		ENUM_Sound_Name[] arrSoundName = PrimitiveHelper.GetEnumArray<ENUM_Sound_Name>();
        //#if UNITY_EDITOR
        //		if (arrSound == null || arrSound.Length < arrSoundName.Length)
        //		{
        //			Debug.Log( "Sound INI의 내용과 Enum SoundName과 길이가 맞지 않아 재조정" );

        //			List<SINI_Sound> listINISound = arrSound == null ? new List<SINI_Sound>() : arrSound.ToList();
        //			Dictionary<string, SINI_Sound> mapINISound = new Dictionary<string, SINI_Sound>();
        //			mapINISound.DoAddItem( arrSound );

        //			for (int i = 0; i < arrSoundName.Length; i++)
        //			{
        //				string strSoundName = arrSoundName[i].ToString();
        //				if (mapINISound.ContainsKey( strSoundName ) == false)
        //					listINISound.Add( new SINI_Sound( strSoundName, 0.5f ) );
        //			}

        //_pJsonParser_StreammingAssets.DoWriteJsonArray(EINI_JSON_FileName.Sound, listINISound.ToArray());
        //		}
        //#endif

#if UNITY_EDITOR
        _pJsonParser_StreammingAssets.DoWriteJsonArray(EINI_JSON_FileName.Sound, new SINI_Sound[] { new SINI_Sound(), new SINI_Sound() });
#endif

        CheckLoadFinish_LocalDataAll();
	}

	private void OnFinishParse_AppSetting( bool bResult, SINI_ApplicationSetting sAppSetting )
	{
        // Debug.Log("OnFinishParse_AppSetting strDBName : " + sAppSetting.strDBName + " strPHPPrefix : " + sAppSetting.strPHPPrefix);

        _pSetting_App = sAppSetting;
		if (bResult == false)
		{
			Debug.Log("Error, AppSetting Is Null");
			_pJsonParser_StreammingAssets.DoWriteJson( EINI_JSON_FileName.ApplicationSetting, new SINI_ApplicationSetting() );
			return;
		}

		CManagerNetworkDB_Project.instance.DoSetNetworkAddress( sAppSetting.strPHPPrefix, sAppSetting.strDBName );

		//_sSetting_App = sAppSetting;
		//System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
		//for (int i = 0; i < _sSetting_App.arrLogIgnore_Writer.Length; i++)
		//{
		//	pStrBuilder.Append( _sSetting_App.arrLogIgnore_Writer[i] );
		//	DebugCustom.AddIgnore_LogWriterList( _sSetting_App.arrLogIgnore_Writer[i].ConvertEnum<ELogWriter>() );

		//	if (i != _sSetting_App.arrLogIgnore_Writer.Length - 1)
		//		pStrBuilder.Append( ", " );
		//}
		//if (Application.isEditor)
		//	Debug.Log( "OnFinishParse_AppSetting - Debug Ignore Writer List : " + pStrBuilder.ToString() );
		//pStrBuilder.Length = 0;
		//for (int i = 0; i < _sSetting_App.arrLogIgnore_Level.Length; i++)
		//{
		//	pStrBuilder.Append( _sSetting_App.arrLogIgnore_Level[i] );
		//	DebugCustom.AddIgnore_LogLevel( _sSetting_App.arrLogIgnore_Level[i] );

		//	if (i != _sSetting_App.arrLogIgnore_Level.Length - 1)
		//		pStrBuilder.Append( ", " );
		//}

#if UNITY_EDITOR
		int iBundleCode = UnityEditor.PlayerSettings.Android.bundleVersionCode;
		if (_pSetting_App.iBundleCode != iBundleCode)
		{
			_pSetting_App.iBundleCode = iBundleCode;
			_pJsonParser_StreammingAssets.DoWriteJson( EINI_JSON_FileName.ApplicationSetting, _pSetting_App );
		}
#endif

		CheckLoadFinish_LocalDataAll();
	}
}
