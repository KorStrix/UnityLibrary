using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-03-03 오후 11:00:06
   Description : 
   Edit Log    : 
   ============================================ */
   
[RequireComponent(typeof(CCompoEventSystemChecker))]
public class CManagerUIShared_UGUI : CManagerUGUIBase<CManagerUIShared_UGUI, CManagerUIShared_UGUI.EFrame>
{
	/* const & readonly declaration             */

	private const float const_fShakeSpeed = 4;
	private const float const_fConsoleDelay = 0.5f;

	/* enum & struct declaration                */
	
	public enum EFrame
	{
		CUIFrameShared_Loading,

		CUIPopupShared_DebugConsole_UGUI,
		CUIPopupShared_LoginBar
	}

    /* public - Variable declaration            */

    public bool bIsDefaultShow_DebugPopup = false;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private CUIPopupShared_DebugConsole_UGUI _pUIPopup_DebugConsole = null;
	private float fConsoleDelay;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

    public bool CheckIsAlreadyShowConsole()
    {
		return _pUIPopup_DebugConsole.isActiveAndEnabled;
    }

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */
	
	protected override void OnAwake()
	{
		base.OnAwake();

#if CUSTOMLOG
		Strix.Debug.SetFileNameType(Strix.Debug.EFileNameType.OnMinute);
		Strix.Debug.SetFileExportType(Strix.Debug.EFlagFileExportType.CSV, Strix.Debug.EFlagFileExportType.TXT);
		Application.logMessageReceived += Strix.Debug.OnUnityDebugLogCallBack;
#endif

		_pUIPopup_DebugConsole = GetUIPanel<CUIPopupShared_DebugConsole_UGUI>();
		Input.gyro.enabled = true;
	}

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 순간 가속도 흔들기 힘이 특정치 이상일 때
        if ((Input.acceleration.magnitude > 5 && fConsoleDelay < Time.time) ||
			 Application.isEditor && Input.GetKeyDown(KeyCode.BackQuote))
		{
			DoShowHide_Panel( EFrame.CUIPopupShared_DebugConsole_UGUI, !CheckIsAlreadyShowConsole() );

			fConsoleDelay = Time.time + const_fConsoleDelay;
		}
	}

	protected override void OnDefaultPanelShow()
	{
        if(bIsDefaultShow_DebugPopup)
            DoShowHide_Panel(EFrame.CUIPopupShared_DebugConsole_UGUI, true );
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
