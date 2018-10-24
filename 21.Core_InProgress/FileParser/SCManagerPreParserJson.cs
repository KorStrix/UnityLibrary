#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
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

public class SCManagerPreParserJson<T_Key, Class_PreParsingType>
	where T_Key : System.IConvertible,  System.IComparable
	where Class_PreParsingType : class
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	static public Dictionary<T_Key, Class_PreParsingType> _mapPreParsingObject = new Dictionary<T_Key, Class_PreParsingType>();
	static private SCManagerParserJson _pManagerParserJson;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInit(MonoBehaviour pBaseClass, string strFolderPath)
	{
		_mapPreParsingObject.Clear();
		if (_pManagerParserJson == null)
			_pManagerParserJson = SCManagerParserJson.DoMakeInstance( pBaseClass, strFolderPath, EResourcePath.Resources );

		List<KeyValuePair<string, TextAsset>> listResource = _pManagerParserJson.p_mapResourceOrigin.ToList();
		System.Type pType = typeof( T_Key );
		if (pType.IsEnum)
		{
			for (int i = 0; i < listResource.Count; i++)
			{
				KeyValuePair<string, TextAsset> pKey = listResource[i];
				T_Key tKey = pKey.Key.ConvertEnum<T_Key>();
				Class_PreParsingType pData;
				if (_pManagerParserJson.DoReadJson_FromResource( tKey, out pData ))
					_mapPreParsingObject.Add( tKey, pData );
				else
				{
					System.Type pTypeClass = typeof( Class_PreParsingType );
					Debug.LogWarningFormat( "[Error] SCManagerPreParserJson<{0}, {1}>.Parsing Fail Key : {2}", pType.Name, pTypeClass, tKey );
				}
			}
		}
		else
		{
			for (int i = 0; i < listResource.Count; i++)
			{
				KeyValuePair<string, TextAsset> pKey = listResource[i];
				T_Key tKey = (T_Key)(object)pKey.Key;
				Class_PreParsingType pData;
				if (_pManagerParserJson.DoReadJson_FromResource( tKey, out pData ))
					_mapPreParsingObject.Add( tKey, pData );
			}
		}
	}

	public Class_PreParsingType GetData(T_Key tKey)
	{
		if (_mapPreParsingObject.ContainsKey_PrintOnError( tKey ))
			return _mapPreParsingObject[tKey];
		else
			return default( Class_PreParsingType );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */
	
	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
