using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:21:49
   Description : 
   Edit Log    : 
   ============================================ */

public class CCompoEventTrigger : CObjectBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class CYield_IsWaitingEventTrigger : CustomYieldInstruction
    {
        static System.Func<bool> g_OnCheckIsWaiting;

        public static void SetIsWaiting(System.Func<bool> OnCheckIsWaiting)
        {
            g_OnCheckIsWaiting = OnCheckIsWaiting;
        }

        public override bool keepWaiting
        {
            get
            {
                if (g_OnCheckIsWaiting == null)
                    return false;
                else
                    return g_OnCheckIsWaiting();
            }
        }
    }

    [System.Flags]
    public enum EConditionTypeFlags
    {
        None = 0,

        OnAwake = 1 << 1,
        OnEnable = 1 << 2,
        OnDisable = 1 << 3,
        OnDestroy = 1 << 4,

        OnPress_True = 1 << 5,
        OnClick = 1 << 6,
        OnPress = 1 << 7,

        OnUIEvent_Show = 1 << 8,
		OnUIEvent_Hide = 1 << 9,

        OnUpdate = 1 << 10,
    }

    public enum EPhysicsEvent
    {
        Collision_Enter,
        Collision_Stay,
        Collision_Exit,

        Trigger_Enter,
        Trigger_Stay,
        Trigger_Exit,

    }

    /* public - Field declaration            */

	public event System.Action<bool> p_OnPress;

	[Rename_Inspector( "트리거 작동 조건" )]
	public EConditionTypeFlags p_eConditionType = EConditionTypeFlags.None;
	[Rename_Inspector("트리거 작동 시 처음 딜레이")]
	public float p_fDelayTrigger = 0f;

#if ODIN_INSPECTOR
    [ShowIf(nameof(CheckIsUpdate))]
#endif
    [Rename_Inspector("업데이트 시 타임 델타")]
    public float p_fUpdateTimeDelta = 0.02f;

	public UnityEngine.Events.UnityEvent p_listEvent = new UnityEvent();
    public event System.Action<GameObject> p_Event_IncludeThisObject;

    /* protected - Field declaration         */

    /* private - Field declaration           */
    
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void OnUIEvent_Show()
	{
		if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnUIEvent_Show))
			DoPlayEventTrigger();
	}

	public void OnUIEvent_Hide()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnUIEvent_Hide))
			DoPlayEventTrigger();
	}
	
	public void DoAddEventTrigger( UnityAction CallBack )
	{
		p_listEvent.AddListener( CallBack );
	}
	
	public void DoPlayEventTrigger()
	{
		if (this == null) return;

		if (p_fDelayTrigger != 0f)
            ProcDelayExcuteCallBack( ProcPlayEvent, p_fDelayTrigger );
		else
			ProcPlayEvent();
	}

    public void DoSetActiveObject_True()
    {
        gameObject.SetActive(true);
    }

    public void DoSetActiveObject_False()
    {
        gameObject.SetActive(false);
    }


    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    virtual protected void OnPlayEvent() { }

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnAwake))
			DoPlayEventTrigger();
	}

	protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnEnable))
	        DoPlayEventTrigger();
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnUpdate) == false)
            yield break;

        while (true)
        {
            DoPlayEventTrigger();

            yield return YieldManager.GetWaitForSecond(p_fUpdateTimeDelta);
        }
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
	{
		base.OnDisableObject(bIsQuitApplciation);

        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnDisable))
			DoPlayEventTrigger();
	}
	
	void OnClick()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnClick))
			DoPlayEventTrigger();
	}

	void OnPress( bool bPress )
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnPress))
		{
			DoPlayEventTrigger();
			if (p_OnPress != null)
				p_OnPress( bPress );
		}

        if (bPress)
		{
            if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnPress_True))
			{
				DoPlayEventTrigger();
				if (p_OnPress != null)
					p_OnPress( bPress );
			}
		}
	}
    


    private void OnDestroy()
	{
        if (p_eConditionType.ContainEnumFlag(EConditionTypeFlags.OnDestroy))
			DoPlayEventTrigger();
	}

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcPlayEvent()
	{
		p_listEvent.Invoke();
        if (p_Event_IncludeThisObject != null)
            p_Event_IncludeThisObject(gameObject);

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(name + " " + this.GetType().Name + " Play Event", this);

        OnPlayEvent();
	}

	public void OnPointerDown( PointerEventData eventData )
	{
		OnPress( true );
	}

	public void OnPointerUp( PointerEventData eventData )
	{
		OnPress( false );
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

    protected void ProcDelayExcuteCallBack(System.Action OnAfterDelayAction, float fDelaySec)
    {
        if (this != null && gameObject.activeInHierarchy)
            StartCoroutine(CoDelayActionEventTrigger(OnAfterDelayAction, fDelaySec));
    }

    private IEnumerator CoDelayActionEventTrigger(System.Action OnAfterDelayAction, float fDelaySec)
    {
        yield return YieldManager.GetWaitForSecond(fDelaySec);
        yield return new CYield_IsWaitingEventTrigger();
        OnAfterDelayAction();
    }

    private bool CheckIsUpdate()
    {
        return p_eConditionType == EConditionTypeFlags.OnUpdate;
    }
}
