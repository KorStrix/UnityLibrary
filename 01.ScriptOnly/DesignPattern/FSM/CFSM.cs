#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-04 오전 11:06:54
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum EStateStartType
{
    Changed,
    Interrupt,
    Interrupt_Resume,
    Waited,
}

public enum EStateFinishType
{
    External_Request,
    Interrupted,
    Finish,
}

public class WaitForWaitable : CustomYieldInstruction
{
    ICoroutineWaitable _pState;

    public WaitForWaitable(ICoroutineWaitable pState)
    {
        _pState = pState;
    }

    public override bool keepWaiting
    {
        get
        {
            return _pState.p_bIsWait;
        }
    }
}

public interface ICoroutineWaitable
{
    bool p_bIsWait { get; }
}

public interface IState<ENUM_STATE, CLASS_DERIVED_STATE> : IDictionaryItem<ENUM_STATE>
    where CLASS_DERIVED_STATE : class, IState<ENUM_STATE, CLASS_DERIVED_STATE>
{
    CFSM<ENUM_STATE, CLASS_DERIVED_STATE> p_pFSMOwner { get; set; }
    CObjectBase p_pScriptOwner { get; set; }

    void OnAwake_State(CObjectBase pScriptOwner, CFSM<ENUM_STATE, CLASS_DERIVED_STATE> pFSMOwner);

    /// <summary>
    /// 반복기가 끝나면 다음 스테이트로 넘어갑니다.
    /// </summary>
    /// <param name="pPrevState"></param>
    /// <returns></returns>
    IEnumerator OnStart_State(CLASS_DERIVED_STATE pPrevState, EStateStartType eStateStartType);
    void OnFinish_State(CLASS_DERIVED_STATE pNextState, EStateFinishType eStateFinishType);
}

public enum EStateInsertType
{
    Change,
    Interrupt,
    Waiting,
}

/// <summary>
/// 
/// </summary>
public class CFSM<ENUM_STATE, CLASS_STATE>
    where CLASS_STATE : class, IState<ENUM_STATE, CLASS_STATE>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class CStateContainer: IDictionaryItem<ENUM_STATE>, ICoroutineWaitable
    {
        public CObjectBase p_pScriptOwner;
        public CLASS_STATE p_pStateInstance { get; private set; }
        public ENUM_STATE p_eStateName { get; private set; }
        public bool p_bIsWait { get; private set; }

        public CStateContainer(CObjectBase pScriptOwner, CFSM<ENUM_STATE, CLASS_STATE> pFSMOwner, CLASS_STATE pState)
        {
            p_pScriptOwner = pScriptOwner;
            p_pStateInstance = pState;
            p_eStateName = pState.IDictionaryItem_GetKey();

            p_pStateInstance.p_pScriptOwner = pScriptOwner;
            p_pStateInstance.p_pFSMOwner = pFSMOwner;

            p_pStateInstance.OnAwake_State(pScriptOwner, pFSMOwner);
        }

        public IEnumerator EventStart(CStateContainer pPrevState, EStateStartType eStateStartType, System.Action<CLASS_STATE> OnFinishState)
        {
            p_bIsWait = true;

            if (pPrevState != null)
                yield return p_pStateInstance.OnStart_State(pPrevState.p_pStateInstance, eStateStartType);
            else
                yield return p_pStateInstance.OnStart_State(null, eStateStartType);

            OnFinishState(p_pStateInstance);
        }

        public void EventFinish(CStateContainer pNextState, EStateFinishType eStateFinishType)
        {
            p_bIsWait = false;

            if(pNextState != null)
                p_pStateInstance.OnFinish_State(pNextState.p_pStateInstance, eStateFinishType);
            else
                p_pStateInstance.OnFinish_State(null, eStateFinishType);
        }

        public Coroutine StartCoroutine(IEnumerator pRoutine)
        {
            return p_pScriptOwner.StartCoroutine(pRoutine);
        }

        // ========================= [Division Line] ========================= //

        public ENUM_STATE IDictionaryItem_GetKey()
        {
            return p_eStateName;
        }
    }

    public struct FSM_Arg
    {
        public ENUM_STATE eState;
        public CLASS_STATE pState;

        public FSM_Arg(ENUM_STATE eState, CLASS_STATE pState)
        {
            this.eState = eState;
            this.pState = pState;
        }
    }


    /* public - Field declaration            */

    public ObservableCollection<FSM_Arg> p_Event_OnChangeState { get; private set; } = new ObservableCollection<FSM_Arg>();

    public ENUM_STATE p_eStateCurrent { get; private set; }
    public CStateContainer p_pStateContainer_Current { get; private set; }
    public CLASS_STATE p_pStateCurrent
    {
        get
        {
            if (p_pStateContainer_Current == null)
                return null;
            else
                return p_pStateContainer_Current.p_pStateInstance;
        }
    }


    public List<CLASS_STATE> p_listStateInstance { get; private set; } = new List<CLASS_STATE>();

    /* protected & private - Field declaration         */

    protected Dictionary<ENUM_STATE, CStateContainer> _mapState = new Dictionary<ENUM_STATE, CStateContainer>();
    protected Stack<ENUM_STATE> _stack_Interrupt = new Stack<ENUM_STATE>();
    protected Queue<ENUM_STATE> _queue_Waiting = new Queue<ENUM_STATE>();
    protected Stack<ENUM_STATE> _stack_PrevState = new Stack<ENUM_STATE>();

    protected CObjectBase _pOwner;

    Dictionary<ENUM_STATE, GameObject> _mapDebugObject = new Dictionary<ENUM_STATE, GameObject>();
    Transform _pTransform_Debug;
    Coroutine _pCoroutine;

    bool _bIsExecute_Once;
    bool _bIsDebug = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void CraeteInstance()
    {
        _mapState = new Dictionary<ENUM_STATE, CStateContainer>();
        _stack_Interrupt = new Stack<ENUM_STATE>();
        _queue_Waiting = new Queue<ENUM_STATE>();
        _stack_PrevState = new Stack<ENUM_STATE>();
        _mapDebugObject = new Dictionary<ENUM_STATE, GameObject>();

        p_listStateInstance = new List<CLASS_STATE>();
        p_Event_OnChangeState = new ObservableCollection<FSM_Arg>();
    }

    virtual public void DoInit(CObjectBase pOwner, params CLASS_STATE[] arrState)
    {
        _pOwner = pOwner;
        _bIsExecute_Once = false;
        CraeteInstance();

        _mapState.Clear();
        for (int i = 0; i < arrState.Length; i++)
            _mapState.Add(new CStateContainer(pOwner, this, arrState[i]));
        
        p_listStateInstance.Clear();
        p_listStateInstance.AddRange(arrState);
    }

    public void DoSet_DebugMode(bool bIsDebug)
    {
        _bIsDebug = bIsDebug;        
    }

    public void DoClearState()
    {
        if (_bIsDebug)
            Debug.Log("FSM)" + _pOwner.name + " " + nameof(DoClearState), _pOwner);

        _bIsExecute_Once = false;
        _stack_Interrupt.Clear();
        _queue_Waiting.Clear();

        SetLeave_State(null, EStateFinishType.External_Request);
        p_pStateContainer_Current = null;
    }

    public void DoEnqueueState(ENUM_STATE eState, EStateInsertType eInsertType)
    {
        if (_bIsDebug)
            Debug.Log("FSM)" + _pOwner.name + " " + nameof(DoEnqueueState) + " eState : " + eState + " eInsertType : " + eInsertType, _pOwner);

        Insert_State(eState, eInsertType);
    }

    public void DoEnqueue_PrevState(EStateInsertType eInsertType)
    {
        if (_bIsDebug)
            Debug.Log("FSM)" + _pOwner.name + " " + nameof(DoEnqueue_PrevState) + " eInsertType : " + eInsertType, _pOwner);

        ENUM_STATE ePrevState = _stack_PrevState.Pop();
        Insert_State(ePrevState, eInsertType);
    }

    public void DoFinishCurrentState()
    {
        if (_bIsDebug)
            Debug.Log("FSM)" + _pOwner.name + " " + nameof(DoFinishCurrentState), _pOwner);

        Finish_State(EStateFinishType.External_Request);
    }

    public TSTATE GetState<TSTATE>(ENUM_STATE eState) where TSTATE : class, CLASS_STATE
    {
        return _mapState[eState].p_pStateInstance as TSTATE;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    protected void SetCurrentState(ENUM_STATE eState, bool bNotExcute_IfSameState = true)
    {
        if (_bIsDebug)
            Debug.Log("FSM)" + _pOwner.name + " " + nameof(SetCurrentState) + " eState: "+ eState + " Try", _pOwner);

        CStateContainer pState = _mapState[eState];
        if ((bNotExcute_IfSameState && pState.Equals(p_pStateCurrent) == false) ||
             bNotExcute_IfSameState == false)
        {
            _bIsExecute_Once = true;
            p_eStateCurrent = eState;
            SetCurrentState(pState);

            if(pState == null)
                p_Event_OnChangeState.DoNotify(new FSM_Arg(eState, null));
            else
                p_Event_OnChangeState.DoNotify(new FSM_Arg(eState, pState.p_pStateInstance));

            if (_bIsDebug)
                Debug.Log("FSM)" + _pOwner.name + " " + nameof(SetCurrentState) + " eState: " + eState + " Success", _pOwner);

#if UNITY_EDITOR
            // if (_bIsDebug)
            UpdateDebugObject_InHierachy();
#endif
        }
    }

    private void Insert_State(ENUM_STATE eState, EStateInsertType eInsertType)
    {
        switch (eInsertType)
        {
            case EStateInsertType.Change:       Change_State(eState, _bIsExecute_Once); break;
            case EStateInsertType.Interrupt:    Interrupt_State(eState); break;
            case EStateInsertType.Waiting:      Waiting_State(eState); break;
        }
    }

    private void Finish_State(CLASS_STATE pState)
    {
        if(p_pStateCurrent == pState)
            Finish_State(EStateFinishType.Finish);
    }

    private void Finish_State(EStateFinishType eStateFinishType)
    {
        CStateContainer pPrevState = p_pStateContainer_Current;
        if (_stack_Interrupt.Count > 0)
        {
            ENUM_STATE eStateInterrupted = _stack_Interrupt.Pop();
            CStateContainer pNextState = _mapState[eStateInterrupted];

            SetLeave_State(pNextState, eStateFinishType);
            SetInterruptResume_State(pNextState, pPrevState);
            SetCurrentState(eStateInterrupted);
        }
        else if (_queue_Waiting.Count > 0)
        {
            ENUM_STATE pStateWaiting = _queue_Waiting.Dequeue();
            CStateContainer pNextState = _mapState[pStateWaiting];

            SetLeave_State(pNextState, eStateFinishType);
            SetStart_State(_mapState[pStateWaiting], pPrevState, EStateStartType.Waited);
            SetCurrentState(pStateWaiting);
        }
        else
        {
            SetLeave_State(null, eStateFinishType);
            SetCurrentState(null);
        }
    }

    /// <summary>
    /// 현재 상태를 종료시키고 새 상태로 바꿉니다.
    /// 새 상태가 끝나면 다음 상태로 넘어갑니다.
    /// </summary>
    /// <param name="eState"></param>
    private void Change_State(ENUM_STATE eState, bool bNotExcute_IfSameState)
    {
        if (eState.Equals(p_eStateCurrent) == false && _mapState.ContainsKey(eState))
            SetLeave_State(_mapState[eState], EStateFinishType.External_Request);

        if(_mapState.ContainsKey(eState))
        {
            SetStart_State(_mapState[eState], p_pStateContainer_Current, EStateStartType.Changed);
            SetCurrentState(eState, bNotExcute_IfSameState);
        }
    }

    /// <summary>
    /// 현재 상태를 강제로 종료시키고 무조건 새 상태가 끼어듭니다.
    /// 새 상태가 끝나면 다음 상태는 강제로 종료시킨 상태입니다.
    /// </summary>
    /// <param name="eState"></param>
    private void Interrupt_State(ENUM_STATE eState)
    {
        ENUM_STATE ePrevState = p_eStateCurrent;
        CStateContainer pPrevState = p_pStateContainer_Current;
        if (pPrevState != null && eState.Equals(p_eStateCurrent) == false)
        {
            SetLeave_State(_mapState[eState], EStateFinishType.Interrupted);
            _stack_Interrupt.Push(ePrevState);
        }
        SetStart_State(_mapState[eState], pPrevState, EStateStartType.Interrupt);
        SetCurrentState(eState);
    }

    /// <summary>
    /// 현재 상태가 있다면 기다립니다.
    /// </summary>
    /// <param name="eState"></param>
    private void Waiting_State(ENUM_STATE eState)
    {
        if (p_pStateCurrent == null)
        {
            SetStart_State(_mapState[eState], p_pStateContainer_Current, EStateStartType.Waited);
            SetCurrentState(eState);
        }
        else
            _queue_Waiting.Enqueue(eState);
    }



    protected void SetStart_State(CStateContainer pStartState, CStateContainer pPrevState, EStateStartType eStateStartType)
    {
        if (_pOwner.gameObject.activeSelf == false)
            return;

        if (_pCoroutine != null)
            _pOwner.StopCoroutine(_pCoroutine);

        _pCoroutine = _pOwner.StartCoroutine(pStartState.EventStart(pPrevState, eStateStartType, Finish_State));
    }

    protected void SetLeave_State(CStateContainer pNextState, EStateFinishType eStateFinishType)
    {
        if(p_pStateContainer_Current != null)
        {
            p_pStateContainer_Current.EventFinish(pNextState, eStateFinishType);
            if (_pCoroutine != null)
                _pOwner.StopCoroutine(_pCoroutine);

            _stack_PrevState.Push(p_eStateCurrent);
        }
    }

    protected void SetInterruptResume_State(CStateContainer pStartState, CStateContainer pInterruptedState)
    {
        if (_pCoroutine != null)
            _pOwner.StopCoroutine(_pCoroutine);

        _pCoroutine = _pOwner.StartCoroutine(pStartState.EventStart(pInterruptedState, EStateStartType.Interrupt_Resume, Finish_State));
    }

    private void UpdateDebugObject_InHierachy()
    {
        foreach (var pState in _mapState)
        {
            ENUM_STATE eState = pState.Key;
            if (_mapDebugObject.ContainsKey(eState) == false)
            {
                if (_pTransform_Debug == null)
                {
                    _pTransform_Debug = new GameObject(string.Format("[Debug]_FSM<{0},{1}>", typeof(ENUM_STATE).Name, typeof(CLASS_STATE).Name)).transform;
                    _pTransform_Debug.SetParent(_pOwner.transform);
                }

                GameObject pObjectState = new GameObject(eState.ToString());
                pObjectState.transform.SetParent(_pTransform_Debug);

                _mapDebugObject.Add(eState, pObjectState);
            }

            if (p_eStateCurrent.Equals(eState))
                _mapDebugObject[eState].name = "[Current]_" + eState.ToString();
            else
                _mapDebugObject[eState].name = eState.ToString();
        }
    }

    private void SetCurrentState(CStateContainer pState)
    {
        if (_bIsDebug)
        {
            if(pState != null)
                Debug.Log(nameof(SetCurrentState) + pState.IDictionaryItem_GetKey());
            else
                Debug.Log(nameof(SetCurrentState) + " is Null");
        }

        p_pStateContainer_Current = pState;
    }

    #endregion Private
}