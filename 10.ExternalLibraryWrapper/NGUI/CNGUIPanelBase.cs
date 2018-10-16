#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
abstract public class CNGUIPanelBase : CNGUIObjectBase, IUIPanel
{
	[SerializeField]
	private List<UIPanel> _listPanelFixedDepth = new List<UIPanel>();

	[SerializeField]
	private bool _bIsAlwaysShow;
	[SerializeField]
	private bool _bIsFixedSortOrder;

	private List<UIPanel> _listPanelChildren = new List<UIPanel>();

	public bool p_bIsAlwaysShow
	{
		get
		{
			return _bIsAlwaysShow;
		}
	}

	public bool p_bIsFixedSortOrder
	{
		get
		{
			return _bIsFixedSortOrder;
		}
	}

	public int p_iHashCode
	{
		get
		{
			return _iHashCode;
		}
	}

	public IManagerUI p_pManagerUI
	{
		get
		{
			return _pManagerUI;
		}
	}

	private IManagerUI _pManagerUI;
	private int _iHashCode;

	// ========================== [ Division ] ========================== //

	public void IUIPanel_Init( IManagerUI pManagerUI, int iHashCode )
	{
		_pManagerUI = pManagerUI;
		_iHashCode = iHashCode;
	}

	public void IUIPanel_SetOrder( int iSortOrder )
	{
		_pUIPanel.depth = iSortOrder;
		_pUIPanel.sortingOrder = Mathf.FloorToInt( iSortOrder * 0.1f );

		int iChildPanelDepth = iSortOrder + 1;
		for (int i = 0; i < _listPanelChildren.Count; i++)
		{
			if (_listPanelFixedDepth.Contains( _listPanelChildren[i] ) == false)
				_listPanelChildren[i].depth = iChildPanelDepth;
		}
	}

	// ========================== [ Division ] ========================== //

	protected override void OnAwake()
    {
        base.OnAwake();

		_pUIPanel = GetComponent<UIPanel>();
		if (_pUIPanel == null)
			_pUIPanel = p_pGameObjectCached.AddComponent<UIPanel>();

		GetComponentsInChildren( _listPanelChildren );
		_listPanelChildren.Remove( _pUIPanel );
    }

	public IEnumerator IUIPanel_OnShowPanel_PlayingAnimation( int iSortOrder )
	{
		yield return StartCoroutine( OnShowPanel_PlayingAnimation( iSortOrder ) );
	}

	public IEnumerator IUIPanel_OnHidePanel_PlayingAnimation()
	{
		yield return StartCoroutine( OnHidePanel_PlayingAnimation() );
	}

	// ========================== [ Division ] ========================== //

	virtual protected IEnumerator OnShowPanel_PlayingAnimation( int iSortOrder ) { yield return null; }
	virtual protected IEnumerator OnHidePanel_PlayingAnimation() { yield return null; }

}
#endif