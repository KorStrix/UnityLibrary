#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0414

[System.Serializable]
public class CInfoTabData
{
	public GameObject[] arrGoTab;
}

public class CUGUICategoryBase<ENUM_CATEGORY> : CUGUIPanelHasInputBase<ENUM_CATEGORY>
	where ENUM_CATEGORY : struct, System.IConvertible
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	[Header( "디버그용" )]
	[SerializeField] private ENUM_CATEGORY _eCurrentShow_Category;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	//[SerializeField] private ENUM_TAB _eFirstShowTab = default(ENUM_TAB);
	[SerializeField] private CInfoTabData[] _arrInfoTabData = new CInfoTabData[0];

	private Button[] _arrButton_Category;

	private int _iCount_Category;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetCategory(ENUM_CATEGORY eTabName)
	{
		ProcSetCategory(eTabName);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected
	/* protected - [abstract & virtual]         */

	public override void OnButtons_Click(ENUM_CATEGORY eTabName)
	{
		ProcSetCategory(eTabName);
	}

	protected virtual void OnClick_CategoryAll(ENUM_CATEGORY eTabName, Button pButton, bool bIsSameTab) { }
	protected virtual void OnClick_Category(ENUM_CATEGORY eTabName) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_arrButton_Category = GetComponentsInChildren<Button>(true);
		_iCount_Category = _arrButton_Category.Length;
	}

	#endregion Protected

	// ========================================================================== //

	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcSetCategory(ENUM_CATEGORY eTabName)
	{
		for (int i = 0; i < _iCount_Category; i++)
		{
			// 박싱이 일어난다 딕셔너리로 캐슁해야함
			bool bIsSameTab = (i == eTabName.ToInt32(null));

			Button pButton = _arrButton_Category[i];

			if (_arrInfoTabData.Length > 0)
			{
				GameObject[] arrGoTab = _arrInfoTabData[i].arrGoTab;

				int iLen = arrGoTab.Length;
				for (int j = 0; j < iLen; j++)
				{
					GameObject pGameObject = arrGoTab[j];
					pGameObject.SetActive(bIsSameTab);
				}
			}

			if (bIsSameTab)
				OnClick_Category(eTabName);

			OnClick_CategoryAll(eTabName, pButton, bIsSameTab);
		}

		_eCurrentShow_Category = eTabName;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
