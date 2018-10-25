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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CUGUIDropdownItem : CObjectBase, IPointerEnterHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private System.Action<int, CUGUIDropDown.SDropDownData, string> _OnPointerEnter;
	private int _iOwnerID;

	[Header("For Debug")] [SerializeField]
	private CUGUIDropDown.SDropDownData _pData;
	private Text _pText;
	private Toggle _pToggle;
	private Image _pImage;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoInitItem( int iOwnerID, System.Action<int, CUGUIDropDown.SDropDownData, string> OnPointerEnter )
	{
		_iOwnerID = iOwnerID;
		_OnPointerEnter = OnPointerEnter;
	}

	public void DoSetDropDownData(CUGUIDropDown.SDropDownData pData)
	{
		_pData = pData;
	}

	public void DoSetIsHeader(Sprite pSpriteHeader, Color pColorHeader)
	{
		_pToggle.enabled = false;
		_pImage.sprite = pSpriteHeader;
		_pText.color = pColorHeader;
	}

	public string GetText()
	{
		return _pText.text;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	public void OnPointerEnter( PointerEventData eventData )
	{
		if(_OnPointerEnter != null)
			_OnPointerEnter( _iOwnerID, _pData, _pText.text );
	}

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

        this.GetComponentInChildren( out _pText );
        this.GetComponentInChildren( out _pToggle );
		_pImage = _pToggle.targetGraphic.GetComponent<Image>();
	}

	protected override void OnStart()
	{
		base.OnStart();

		IDropDownInitializer pDropDownInitializer = GetComponentInParent<IDropDownInitializer>();
		if (pDropDownInitializer == null)
		{
			Debug.LogWarning( "Error" );
			return;
		}

		pDropDownInitializer.IDropDownInitializer_Regist_DropDownItem( this );

	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
