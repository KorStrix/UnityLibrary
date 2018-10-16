// 작성자 : Strix 
// 개요   : 유한 상태 머신(FSM) 부모 클래스.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0219

public enum EStateInsertType
{
    Change,
    Interrupt,
    Waiting,
}

abstract public class CFSMBase<FSM, ENUM_STATE, STATE> : CObjectBase
    where FSM : CFSMBase<FSM, ENUM_STATE, STATE>
    where ENUM_STATE : System.IFormattable, System.IConvertible, System.IComparable
    where STATE : CStateBase<FSM, ENUM_STATE>
{
    protected const int const_iLimitSaveCount_PrevState = 10;

    // ========================== [ Division ] ========================== //

    public ENUM_STATE p_eStateCurrent { get { return _eStateCurrent; } }
    public ENUM_STATE p_eStatePrev { get { return _stack_PrevState.Peek(); } }

    public event System.Action<ENUM_STATE> p_EVENT_OnChangeState;

    // ========================== [ Division ] ========================== //

    protected Dictionary<ENUM_STATE, STATE> _mapState = new Dictionary<ENUM_STATE, STATE>();
    protected Stack<ENUM_STATE> _stack_Interrupt = new Stack<ENUM_STATE>();
    protected Queue<ENUM_STATE> _queue_Waiting = new Queue<ENUM_STATE>();
    protected Stack<ENUM_STATE> _stack_PrevState = new Stack<ENUM_STATE>();

    protected STATE _pStateCurrent;
    protected ENUM_STATE _eStateCurrent;
    protected ENUM_STATE _eStateDefault;

    private bool _bIsInit = false;

    // ========================= [Division Line] ========================= //

    public void DoInitFSM(ENUM_STATE eStateDefault)
    {
        _bIsInit = true;
        _eStateDefault = eStateDefault;
        if(_pStateCurrent == null)
            DoStartState(eStateDefault, EStateInsertType.Change);
    }

    public void DoClearState()
    {
        _stack_Interrupt.Clear();
        _queue_Waiting.Clear();
        if(_pStateCurrent != null)
            _pStateCurrent.EventStateLeave(null);
        _pStateCurrent = null;
    }
    
    public void DoStartState(ENUM_STATE eState, EStateInsertType eInsertType)
    {
        ProcInsertState(eState, eInsertType);
    }

    public void DoStartState_Prev(EStateInsertType eInsertType)
    {
        ENUM_STATE ePrevState = _stack_PrevState.Pop();
        ProcInsertState(ePrevState, eInsertType);
    }

    public void DoEndCurrentState()
    {
        ProcEndState();
    }

    public TSTATE GetState<TSTATE>(ENUM_STATE eState) where TSTATE : CStateBase<FSM, ENUM_STATE>
    {
        return _mapState[eState] as TSTATE;
    }

    // ========================= [Division Line] ========================= //

    abstract protected void OnInitFSM_RegistState(Dictionary<ENUM_STATE, STATE> mapState);

    protected List<STATE> EventGetListAllState() { return _mapState.Values.ToList(); }

    protected override void OnAwake()
    {
        base.OnAwake();

        OnInitFSM_RegistState(_mapState);

        FSM pThis = this as FSM;

		IEnumerator<KeyValuePair<ENUM_STATE, STATE>> pEnum = _mapState.GetEnumerator();
		while(pEnum.MoveNext())
		{
			KeyValuePair<ENUM_STATE, STATE> pCurrent = pEnum.Current;
			pCurrent.Value.EventStateInit(pThis, pCurrent.Key);
		}
	}

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if(_bIsInit && _pStateCurrent == null)
            DoStartState(_eStateDefault, EStateInsertType.Change);
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate();
        bCheckUpdateCount = true;

        if (_pStateCurrent != null)
            _pStateCurrent.EventStateUpdate();
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        if(_bIsInit)
            DoClearState();
    }

    protected void OnState_Start(STATE pStartState, STATE pPrevState)
    {
        pStartState.EventStateEnter(pPrevState);
    }

    protected void OnState_Leave(STATE pLeaveState, STATE pNextState)
    {
        pLeaveState.EventStateLeave(pNextState);
    }

    protected void OnState_Interrupted(STATE pInterruptedState, STATE pInterruptState)
    {
        pInterruptedState.EventStateInterrupted(pInterruptState);
    }

    protected void OnState_ResumeInterrupt(STATE pResumeState)
    {
        pResumeState.EventStateResumeInterrupt();
    }

    // ========================= [Division Line] ========================= //

    protected void ProcSetCurrentState(ENUM_STATE eState, bool bNotExcuteIfSameState = true)
    {
        STATE pState = _mapState[eState];
        if ((bNotExcuteIfSameState && pState.Equals(_pStateCurrent) == false) || 
             bNotExcuteIfSameState == false)
        {
            _stack_PrevState.Push(_eStateCurrent);

            _eStateCurrent = eState;
            _pStateCurrent = pState;

            if(p_EVENT_OnChangeState != null)
                p_EVENT_OnChangeState(_eStateCurrent);
        }
    }

    // ========================= [Division Line] ========================= //

    private void ProcInsertState(ENUM_STATE eState, EStateInsertType eInsertType)
    {
        switch (eInsertType)
        {
            case EStateInsertType.Change:    ProcState_Change(eState); break;
            case EStateInsertType.Interrupt: ProcState_Interrupt(eState); break;
            case EStateInsertType.Waiting:   ProcState_Waiting(eState); break;
        }
    }

    private void ProcEndState()
    {
        if (_stack_Interrupt.Count > 0)
        {
            ENUM_STATE eStateInterrupted = _stack_Interrupt.Pop();
            STATE pPrevState = _pStateCurrent;
            if (pPrevState != null)
                OnState_Leave(pPrevState, _pStateCurrent);
            OnState_ResumeInterrupt(_mapState[eStateInterrupted]);
            ProcSetCurrentState(eStateInterrupted);
        }
        else if (_queue_Waiting.Count > 0)
        {
            ENUM_STATE pStateWaiting = _queue_Waiting.Dequeue();
            STATE pPrevState = _pStateCurrent;
            if (pPrevState != null)
                OnState_Leave(pPrevState, _pStateCurrent);
            OnState_Start(_pStateCurrent, pPrevState);
            ProcSetCurrentState(pStateWaiting);
        }
        else
        {
            STATE pPrevState = _pStateCurrent;
            if (pPrevState != null)
                OnState_Leave(pPrevState, null);
            _pStateCurrent = null;
        }
    }

    private void ProcState_Change(ENUM_STATE eState)
    {
        if (_pStateCurrent != null && eState.Equals(_eStateCurrent) == false)
            OnState_Leave(_pStateCurrent, _mapState[eState]);
        OnState_Start(_mapState[eState], _pStateCurrent);
        ProcSetCurrentState(eState, false);
    }

    private void ProcState_Interrupt(ENUM_STATE eState)
    {
        STATE pPrevState = _pStateCurrent;
        if (pPrevState != null && eState.Equals(_eStateCurrent) == false)
        {
            OnState_Leave(_pStateCurrent, _mapState[eState]);
            OnState_Interrupted(_pStateCurrent, _mapState[eState]);
            _stack_Interrupt.Push(pPrevState.p_eState);
        }
        OnState_Start(_mapState[eState], pPrevState);
        ProcSetCurrentState(eState);
    }

    private void ProcState_Waiting(ENUM_STATE eState)
    {
        if (_pStateCurrent == null)
        {
            OnState_Start(_mapState[eState], _pStateCurrent);
            ProcSetCurrentState(eState);
        }
        else
            _queue_Waiting.Enqueue(eState);
    }
}
