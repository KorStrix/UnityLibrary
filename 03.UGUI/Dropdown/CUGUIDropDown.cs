#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class CUGUIDropDown : Dropdown
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */
	
	[System.Serializable]
	public class SDropDownData : IDictionaryItem<string>
	{
		public string strName;
		public bool bIsHeader;

		public SDropDownData(string strName, bool bIsHeader)
		{
			this.strName = strName; this.bIsHeader = bIsHeader;
		}

		public string IDictionaryItem_GetKey()
		{
			return strName;
		}
	}

	#region Field
	/* public - Field declaration            */
	
	[Header("Item 헤더용")]
	[SerializeField]
	public Sprite p_pSpriteBG_OnHeader;
	[SerializeField]
	public Color p_pColorItemText_OnHeader;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Dictionary<string, SDropDownData> _mapData = new Dictionary<string, SDropDownData>();

	#endregion Field
	#region Public
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoClearData()
	{
		value = 0;
		_mapData.Clear();
		ClearOptions();
	}

	public void DoAddOptions(List<SDropDownData> listData)
	{
		if (listData == null) return;

		_mapData.DoAddItem(listData, false);

		List<string> listText = new List<string>();
		for(int i = 0; i < listData.Count; i++)
		{
			listText.Add(listData[i].strName);
		}

		AddOptions(listText);
		ProcIgnoreHeader();
	}

	public SDropDownData GetData(string strItemText)
	{
        if (_mapData.ContainsKey_PrintOnError(strItemText))
            return _mapData[strItemText];
        else
            return null;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
	#endregion Public
	// ========================================================================== //
	#region Protected
	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);

		OnShowDropdown();
	}

	public override void OnSubmit(BaseEventData eventData)
	{
		base.OnSubmit(eventData);

		OnShowDropdown();
	}

	#endregion Protected
	// ========================================================================== //
	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void OnShowDropdown()
	{
		CUGUIDropdownItem[] arrDropdownItem = GetComponentsInChildren<CUGUIDropdownItem>();
		for(int i = 0; i < arrDropdownItem.Length; i++)
		{
			CUGUIDropdownItem pDropDownItem = arrDropdownItem[i];
			SDropDownData pDropDownData;
			if (_mapData.TryGetValue(pDropDownItem.GetText(), out pDropDownData))
			{
				pDropDownItem.DoSetDropDownData(pDropDownData);
				if (pDropDownData.bIsHeader)
					pDropDownItem.DoSetIsHeader(p_pSpriteBG_OnHeader, p_pColorItemText_OnHeader);
			}
		}

		ProcIgnoreHeader();
	}

	private void ProcIgnoreHeader()
	{
		if (options.Count == 0)
		{
			captionText.text = "Empty";
			return;
		}

		try
		{
			if (_mapData.ContainsKey( options[value].text ))
			{
				if (_mapData[options[value].text].bIsHeader)
					value++;

				if (value >= options.Count)
					value = 0;

				captionText.text = options[value].text;
			}
		}
		catch
		{
			if (_mapData.ContainsKey( options[value].text ))
			{
				if (_mapData[options[value].text].bIsHeader)
					value++;

				if (value >= options.Count)
					value = 0;

				captionText.text = options[value].text;
			}
		}

	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */
	#endregion Private
}
