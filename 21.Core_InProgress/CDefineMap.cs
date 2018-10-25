#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SDBInfoBase
{
	public string ID;
	public string strNickName;
	
	public void DoCopyDBInfo( SDBInfoBase pDBInfo )
	{
		if(pDBInfo == null)
		{
			Debug.LogWarning( strNickName + "DoCopyDBInfo == null" );
			return;
		}

		ID = pDBInfo.ID;
		strNickName = pDBInfo.strNickName;
	}
}

[System.Serializable]
public class SAppConfig
{
	public string strKey;
	public string strValue;

	static private Dictionary<string, string> _mapDataConfig_String = new Dictionary<string, string>();
	static private Dictionary<string, int> _mapDataConfig_Int = new Dictionary<string, int>();
	static private Dictionary<string, float> _mapDataConfig_Float = new Dictionary<string, float>();

	static public void SetData( SAppConfig[] arrData )
	{
		if (arrData == null)
		{
			Debug.LogWarning( "SGameData - arrData == null" );
			return;
		}

		for (int i = 0; i < arrData.Length; i++)
			_mapDataConfig_String.Add( arrData[i].strKey, arrData[i].strValue );
	}

	static public string GetString<Enum_FieldName>( Enum_FieldName eFieldName )
	{
		string strFieldName = eFieldName.ToString();
		if (_mapDataConfig_String.ContainsKey( strFieldName ))
			return _mapDataConfig_String[strFieldName];
		else
			return "";
	}

	static public int GetInt<Enum_FieldName>( Enum_FieldName eFieldName )
	{
		string strFieldName = eFieldName.ToString();
		if (_mapDataConfig_Int.ContainsKey( strFieldName ))
			return _mapDataConfig_Int[strFieldName];
		else
		{
			if (_mapDataConfig_String.ContainsKey( strFieldName ))
			{
				int iValue = int.Parse( _mapDataConfig_String[strFieldName] );
				_mapDataConfig_Int.Add( strFieldName, iValue );
				return iValue;
			}
			else
			{
				Debug.LogWarning( "GameData에 " + strFieldName + " 이 존재하지 않습니다." );
				return -1;
			}
		}
	}

	static public float GetFloat<Enum_FieldName>( Enum_FieldName eFieldName )
	{
		string strFieldName = eFieldName.ToString();
		if (_mapDataConfig_Float.ContainsKey( strFieldName ))
			return _mapDataConfig_Float[strFieldName];
		else
		{
			if (_mapDataConfig_String.ContainsKey( strFieldName ))
			{
				float fValue = float.Parse( _mapDataConfig_String[strFieldName] );
				_mapDataConfig_Float.Add( strFieldName, fValue );
				return fValue;
			}
			else
			{
				Debug.LogWarning( "GameData에 " + strFieldName + " 이 존재하지 않습니다." );
				return -1;
			}
		}
	}
}