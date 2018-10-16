#if NGUI

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CUIEditorNGUIConetext : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	static private List<CNGUIPanelBase> _listSelectObject = new List<CNGUIPanelBase>();
	static private StringBuilder _pStrBuilder = new StringBuilder();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
	[MenuItem( "GameObject/StrixTool/UpdateUIObject", false, 15 )]
	static void UpdateUIObject()
	{
		Object[] arrSelectObject = Selection.objects;
		_listSelectObject.Clear();
		for (int i = 0; i < arrSelectObject.Length; i++)
		{
			GameObject pObjectCurrent = (GameObject)arrSelectObject[i];
			CNGUIPanelBase pUIObject = pObjectCurrent.GetComponent<CNGUIPanelBase>();
			if (pUIObject != null)
				_listSelectObject.Add( pUIObject );
		}

		for (int i = 0; i < _listSelectObject.Count; i++)
		{
			System.Type pClass = _listSelectObject[i].GetType();
			string strClassName = pClass.Name;
			string strFilePath = GetFilePath( strClassName );

			if (strFilePath == null)
			{
				Debug.LogWarning( "File Not Found " + strFilePath );
				continue;
			}

			string strScriptContents = File.ReadAllText( strFilePath, System.Text.Encoding.UTF8 );

			// Button Check - UI Button의 경우 1개라도 있으면 인터페이스와 Enum을 체크
			UIButton[] arrUIButton = _listSelectObject[i].GetComponentsInChildren<UIButton>(true);
			if(arrUIButton.Length != 0)
			{
				string strEnumUIButton = string.Format("IButton_OnClickListener<{0}.EUIButton>", strClassName); 

				// 없으면 Interface와 Enum을 추가
				if (strScriptContents.IndexOf( strEnumUIButton ) == -1)
				{
					// Class 선언부의 줄의 끝 찾기
					int iIndexString = strScriptContents.IndexOf( string.Format( "public class {0}", strClassName ), System.StringComparison.InvariantCultureIgnoreCase );
					int iIndexLineEnd = strScriptContents.IndexOf( "\n", iIndexString, System.StringComparison.InvariantCultureIgnoreCase );
					int iIndex_InheritEnd = strScriptContents.LastIndexOf( ":", iIndexString, iIndexLineEnd - iIndexString );

					// 버튼 리스너 인터페이스 상속
					// 무언가를 상속받는다면 다중 상속이므로 ,를 추가
					if(iIndex_InheritEnd != -1)
						strScriptContents = strScriptContents.Insert( iIndexLineEnd - 1, ", " + strEnumUIButton);
					else
						strScriptContents = strScriptContents.Insert( iIndexLineEnd - 1, strEnumUIButton );

					// Enum 선언부와 Enum 끝 찾기
					iIndexString = strScriptContents.IndexOf( "enum", System.StringComparison.InvariantCultureIgnoreCase );
					iIndexLineEnd = strScriptContents.IndexOf( "\n", iIndexString, System.StringComparison.InvariantCultureIgnoreCase );

					// Enum 선언
					strScriptContents = strScriptContents.Insert( iIndexLineEnd + 1, "public enum EUIButton {\n" + "}\n" );

					// 인터페이스 함수 구현 - 일단 위치는 Enum 밑으로..
					iIndexString = strScriptContents.IndexOf( "}\n", iIndexLineEnd ) + 2;
					strScriptContents = strScriptContents.Insert( iIndexString, "\npublic void IOnClick_Buttons( EUIButton eButtonName )\n" + "{\nDebug.Log(eButtonName);\n" + "}\n\n" );

					Debug.Log( strClassName + "에 Interface가 없어 추가했다" );
				}

				// Enum을 찾아서 일단 비운다..
				int iIndex_EnumStart = strScriptContents.IndexOf( "public enum EUIButton {", System.StringComparison.InvariantCultureIgnoreCase ) + 1;
				int iIndex_EnumEnd = strScriptContents.IndexOf( "}", iIndex_EnumStart );
				strScriptContents.Remove( iIndex_EnumStart, iIndex_EnumEnd - iIndex_EnumStart );

				_pStrBuilder.Length = 0;
				_pStrBuilder.Append( "\n" );
				for (int j = 0; j < arrUIButton.Length; j++)
				{
					_pStrBuilder.Append( arrUIButton[j].name );
					_pStrBuilder.Append( ",\n" );
				}

				strScriptContents = strScriptContents.Insert( iIndex_EnumEnd - 1, _pStrBuilder.ToString() );
			}
			File.WriteAllText( strFilePath, strScriptContents );
			Debug.Log( "Update Complete " + strClassName );
		}
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
	   자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
	   로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
	   찾기, 계산등 단순 로직(Simpe logic)         */

	static private string GetFilePath( string strClassName )
	{
		string strFilePath = string.Format( "{0}.cs", strClassName );
		string strAssetPath = Directory.GetCurrentDirectory() + "/Assets/";

		string[] arrFile = Directory.GetFiles( strAssetPath, strFilePath, SearchOption.AllDirectories );
		if (arrFile.Length != 0)
			return arrFile[0];
		else
			return null;
	}

}
#endif