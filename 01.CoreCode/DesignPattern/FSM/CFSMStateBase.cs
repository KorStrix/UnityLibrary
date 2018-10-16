abstract public class CStateBase<FSM, ENUM_STATE>
    where ENUM_STATE : System.IFormattable, System.IConvertible, System.IComparable
{
    protected FSM _pFSMOwner;
    protected ENUM_STATE _eState;  public ENUM_STATE p_eState {  get { return _eState; } }

    // ========================= [Division Line] ========================= //

    public void EventStateInit(FSM pOwner, ENUM_STATE eState)
    {
        _pFSMOwner = pOwner;
        _eState = eState;
        OnStateInit();
    }

    public void EventStateEnter(CStateBase<FSM, ENUM_STATE> pPrevState)
    {
        OnStateEnter(pPrevState);
    }

    public void EventStateUpdate()
    {
        OnStateUpdate();
    }

    public void EventStateLeave(CStateBase<FSM, ENUM_STATE> pNextState)
    {
        OnStateLeave(pNextState);
    }

    public void EventStateInterrupted(CStateBase<FSM, ENUM_STATE> pStateInterrupt)
    {
        OnStateInterrupted(pStateInterrupt);
    }

    public void EventStateResumeInterrupt()
    {
        OnStateResumeInterrupt();
    }

    // ========================= [Division Line] ========================= //

    protected virtual void OnStateInit() { }
    protected virtual void OnStateEnter(CStateBase<FSM, ENUM_STATE> pPrevState) { }
    protected virtual void OnStateUpdate() { }
    protected virtual void OnStateLeave(CStateBase<FSM, ENUM_STATE> pNextState) { }
    protected virtual void OnStateInterrupted(CStateBase<FSM, ENUM_STATE> pStateInterrupt) { }
    protected virtual void OnStateResumeInterrupt() { }
}
