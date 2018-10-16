#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : KJH
   Date        : 2017-03-04 오전 12:20:30
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIPopupShared_DebugConsole_NGUI : CNGUIPanelBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private UITextList _pUITextList;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void OnCallbackUnity_DebugLog(string strLog, string strStackTrace, LogType eLogType)
	{
		string strLogType = "";

		switch (eLogType)
		{
			case LogType.Error:
				strLogType = "[에러] "; break;
			case LogType.Warning:
				strLogType = "[경고] "; break;
			case LogType.Log:
				strLogType = "[로그] "; break;
			case LogType.Exception:
				strLogType = "[심각한 에러] "; break;
		}

		string strDebugLog = string.Format("[시간]{0} -> {1}\n{2}\n[스택 트레이스]\n{3}", Time.time, strLogType, strLog, strStackTrace);
		_pUITextList.Add(strDebugLog);
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pUITextList = GetComponentInChildren<UITextList>();
		if (_pUITextList == null)
			Debug.LogWarning("UITextList 가 없습니다.");

		Application.logMessageReceived += OnCallbackUnity_DebugLog;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif