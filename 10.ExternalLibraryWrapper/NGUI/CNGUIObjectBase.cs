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

#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNGUIObjectBase : CUIObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	protected Dictionary<string, UIInput> _mapInput = null;
	protected Dictionary<string, UILabel> _mapLabel = null;
	protected Dictionary<string, UIButton> _mapButton = null;
	protected Dictionary<string, UISprite> _mapSprite = null;
	protected Dictionary<string, UIToggle> _mapUIToggle = null;
	protected Dictionary<string, UITexture> _mapUITexture = null;

	protected UIPanel _pUIPanel;

	/* private - Field declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoEditLabel<T_LabelName>( T_LabelName tLabelName, string strText )
	{
		FindUIElement( _mapLabel, tLabelName.ToString() ).text = strText;
	}

	/// <summary>
	/// 해당 프레임의 버튼들 중 1개만 활성화 시킵니다.
	/// </summary>
	/// 
	private List<UIButton> _listButton = new List<UIButton>();
	public void DoEnableButton_OnlyOne<ENUM_BUTTON>( ENUM_BUTTON eButtonEnable )
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton.Values.ToList( _listButton );

		for (int i = 0; i < _listButton.Count; i++)
			_listButton[i].isEnabled = strButtonName.Equals( _listButton[i].name );
	}

	public void DoEnableButton<ENUM_BUTTON>( ENUM_BUTTON eButtonEnable, bool bEnable )
	{
		string strButtonName = eButtonEnable.ToString();
		_mapButton[strButtonName].isEnabled = bEnable;
	}

	/// <summary>
	/// 해당 프레임의 버튼들을 전부 활성여부를 세팅합니다.
	/// </summary>
	public void DoEnableFrameButtons( bool bEnable )
	{
		if (_mapButton == null || _mapButton.Count == 0)
		{
			Debug.Log( "버튼이 없는데 DoEnableFrameButtons를 호출한다", this );
			return;
		}
		_mapButton.Values.ToList( _listButton );

		int iLen = _listButton.Count;
		for (int i = 0; i < iLen; i++)
			_listButton[i].isEnabled = bEnable;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	/// <summary>
	/// NGUI Sprite를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UISprite GetUISprite<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapSprite, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Label을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UILabel GetUILabel<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapLabel, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Button을 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIButton GetUIButton<ENUM>( ENUM eCompoName )
	{
		return FindUIElement( _mapButton, eCompoName.ToString() );
	}

	/// <summary>
	/// NGUI Input를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIInput GetUIInput<ENUM>( ENUM eCompoName )
	{
		return _mapInput[eCompoName.ToString()];
	}

	/// <summary>
	/// UI Toggle 를 오브젝트의 이름으로 찾아 얻어옵니다. (한번 찾은 후 캐싱), 자기 자신과 자식 하이어라키에서 모두 찾습니다.
	/// </summary>
	/// <param name="eCompoName">컴포넌트가 들어있는 오브젝트의 이름</param>
	/// <returns></returns>
	public UIToggle GetUIToggle<ENUM>( ENUM eCompoName )
	{
		return _mapUIToggle[eCompoName.ToString()];
	}

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
#endif