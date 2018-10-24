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
 *	기능 : UIPanel 관련
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IUIPanel
{
	bool p_bIsAlwaysShow { get; }
	bool p_bIsFixedSortOrder { get; }

	int p_iHashCode { get; }
	IManagerUI p_pManagerUI { get; }

	void IUIPanel_Init( IManagerUI pManagerUI, int iHashCode );
	void IUIPanel_SetOrder( int iSetSortOrder );
	IEnumerator IUIPanel_OnShowPanel_PlayingAnimation( int iSortOrder );
    void IUIPanel_OnShow();
    IEnumerator IUIPanel_OnHidePanel_PlayingAnimation();
    void IUIPanel_OnHide();
}

abstract public partial class CManagerUIBase<CLASS_Instance, ENUM_Panel_Name, CLASS_Panel, Class_Button> : CSingletonMonoBase<CLASS_Instance>
	where CLASS_Instance : CManagerUIBase<CLASS_Instance, ENUM_Panel_Name, CLASS_Panel, Class_Button>
	where ENUM_Panel_Name : System.IFormattable, System.IConvertible, System.IComparable
	where CLASS_Panel : CObjectBase, IUIPanel
{
	[System.Serializable]
	public class CUIPanelData
	{
		[SerializeField]
		private ENUM_Panel_Name _ePanelName;	public ENUM_Panel_Name p_ePanelName { get { return _ePanelName; } }
		[SerializeField]
		private CLASS_Panel _pPanel;	public CLASS_Panel p_pPanel {  get { return _pPanel; } }

		[SerializeField]
		private int _iCurrentSortOrder;

		private bool _bIsShowCurrent = false; public bool p_bIsShowCurrent { get { return _bIsShowCurrent; } }
		private bool _bIsPlayUIAnimation = false; public bool p_bIsPlayingUIAnimation { get { return _bIsPlayUIAnimation; } }

		private System.Action _OnFinishAnimation;

        private Coroutine _pCoroutine_ShowAnimation;
        private Coroutine _pCoroutine_HideAnimation;

        public CUIPanelData( ENUM_Panel_Name ePanelName, CLASS_Panel pPanel )
		{
			_ePanelName = ePanelName;
			_pPanel = pPanel;
		}

		public void DoSetFinishAnimationEvent(System.Action OnFinishAnimation)
		{
			_OnFinishAnimation = OnFinishAnimation;
		}

		public void DoShow()
		{
			_pPanel.gameObject.SetActive( true );

            StopAnimationCoroutine();
            _pCoroutine_ShowAnimation = _pPanel.StartCoroutine( CoProcShowPanel( _iCurrentSortOrder ) );
		}

		public void DoShow( int iSortOrder )
		{
			EventSetOrder( iSortOrder );
			DoShow();
		}

		public void DoHide(bool bAnimationPlay = true)
		{
			_bIsShowCurrent = false;
			if (_pPanel.gameObject.activeSelf == false) return;

            StopAnimationCoroutine();
            _pCoroutine_HideAnimation = _pPanel.StartCoroutine( CoProcHidePanel( true ) );
		}

		public void DoHide_IgnoreAnimation()
		{
			_bIsShowCurrent = false;
			if (_pPanel.gameObject.activeSelf == false) return;

            StopAnimationCoroutine();
            _pCoroutine_HideAnimation = _pPanel.StartCoroutine( CoProcHidePanel( false ) );
		}


		public void DoHide( int iSortOrder )
		{
			EventSetOrder( iSortOrder );
			DoHide();
		}

		public void EventSetOrder( int iSortOrder )
		{
			if (_pPanel.p_bIsFixedSortOrder) return;

			_iCurrentSortOrder = iSortOrder;
			_pPanel.IUIPanel_SetOrder( _iCurrentSortOrder );
		}

		public void SetActive(bool bActive)
		{
			_pPanel.gameObject.SetActive( bActive );
		}



		protected IEnumerator CoProcShowPanel( int iSortOrder )
		{
            // if (_bIsShowCurrent) yield break;

            _pPanel.gameObject.SendMessage("OnUIEvent_Show", SendMessageOptions.DontRequireReceiver);
            _bIsShowCurrent = true;
			_bIsPlayUIAnimation = true;
			yield return _pPanel.StartCoroutine( _pPanel.IUIPanel_OnShowPanel_PlayingAnimation( iSortOrder ) );
			_bIsPlayUIAnimation = false;

            if (_OnFinishAnimation != null)
			{
				_OnFinishAnimation();
				_OnFinishAnimation = null;
			}

            _pPanel.IUIPanel_OnShow();
        }

        protected IEnumerator CoProcHidePanel(bool bAnimationPlay)
		{
            _pPanel.gameObject.SendMessage("OnUIEvent_Hide", SendMessageOptions.DontRequireReceiver);
            if (bAnimationPlay)
			{
				_bIsPlayUIAnimation = true;
				yield return _pPanel.StartCoroutine( _pPanel.IUIPanel_OnHidePanel_PlayingAnimation() );
				_bIsPlayUIAnimation = false;
			}

			if (_OnFinishAnimation != null)
			{
				_OnFinishAnimation();
				_OnFinishAnimation = null;
			}

            _pPanel.IUIPanel_OnHide();
            _pPanel.gameObject.SetActive( false );
		}

        private void StopAnimationCoroutine()
        {
            if (_pCoroutine_HideAnimation != null)
                _pPanel.StopCoroutine(_pCoroutine_HideAnimation);
            if (_pCoroutine_ShowAnimation != null)
                _pPanel.StopCoroutine(_pCoroutine_ShowAnimation);
        }
    }
}