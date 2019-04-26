#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-20 오후 1:23:01
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif
#endif

public abstract class CTransitionStateExecuter_Base<ENUM_STATE> : IExecuter
{
    virtual public int p_iExecuterOrder => 0;

#if ODIN_INSPECTOR
    [LabelText("해당 상태로 전환")]
    [HorizontalGroup]
#endif

    public ENUM_STATE eNextState;
#if ODIN_INSPECTOR
    [HideLabel, LabelWidth(80f)]
    [HorizontalGroup(80f)]
#endif
    public EStateInsertType eStateInsertType = EStateInsertType.Change;

    public abstract bool CheckIsTransition();
    public abstract string IHasName_GetName();

    public virtual void IExecuter_OnAwake(IExecuteContainer pContainer, CObjectBase pScriptOwner) { }
    public virtual void IExecuter_OnEnable(IExecuteContainer pContainer, CObjectBase pScriptOwner) { }

    public virtual void IExecuter_Check_IsInvalid_OnEditor(CObjectBase pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error) { }
}

public abstract class State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE> : IState<ENUM_STATE, CLASS_STATE>
    where CLASS_STATE : State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE>
    where CLASS_EXECUTER_ONSTART_STATE : class, IExecuter
    where CLASS_EXECUTER_TRANSITION_STATE : CTransitionStateExecuter_Base<ENUM_STATE>
{
    public CFSM<ENUM_STATE, CLASS_STATE> p_pFSMOwner { get; set; }
    public CObjectBase p_pScriptOwner { get; set; }

    //[Rename_Inspector("상태 이름")]
    //public ENUM_STATE p_eState;

#if ODIN_INSPECTOR
    [PropertyOrder(10)]
#endif
    [Rename_Inspector("상태 전환자 리스트")]
    public CExecuterContainer<CLASS_EXECUTER_TRANSITION_STATE> p_pExecuter_OnTransitionState = new CExecuterContainer<CLASS_EXECUTER_TRANSITION_STATE>();

#if ODIN_INSPECTOR
    [PropertyOrder(11)]
#endif
    [Space(5)]
    [Rename_Inspector("상태에 진입 했을 때 실행기능 리스트")]
    public CExecuterContainer<CLASS_EXECUTER_ONSTART_STATE> p_pExecuter_OnEnable = new CExecuterContainer<CLASS_EXECUTER_ONSTART_STATE>();

    protected CObjectBase _pScriptOwner;

    Coroutine _pCoroutine_ExecuteContainer;
    Coroutine _pCoroutine_Execute;

    // ========================================================================== //

    abstract public ENUM_STATE IDictionaryItem_GetKey();

    public IEnumerator OnStart_State(CLASS_STATE pPrevState, EStateStartType eStateStartType)
    {
        p_pExecuter_OnTransitionState.DoNotify_OnEnable();
        p_pExecuter_OnEnable.DoNotify_OnEnable();

        if (_pCoroutine_ExecuteContainer != null)
            _pScriptOwner.StopCoroutine(_pCoroutine_ExecuteContainer);
        _pCoroutine_ExecuteContainer = _pScriptOwner.StartCoroutine(OnStart_State());

        //if(pPrevState != null)
        //    Debug.Log(IDictionaryItem_GetKey().ToString() + " " + nameof(OnStart_State) + " pPrevState : " + pPrevState.IDictionaryItem_GetKey().ToString() + " eStateStartType : " + eStateStartType);
        //else
        //    Debug.Log(IDictionaryItem_GetKey().ToString() + " " + nameof(OnStart_State) + " pPrevState : Null / eStateStartType : " + eStateStartType);

        yield return OnStart_State_ExecuteContainer(pPrevState, eStateStartType);
    }

    public IEnumerator OnStart_State()
    {
        bool bIsUpdate = true;
        List<CLASS_EXECUTER_TRANSITION_STATE> listTransitionState = p_pExecuter_OnTransitionState.p_listExecute;
        while (bIsUpdate)
        {
            for (int i = 0; i < listTransitionState.Count; i++)
            {
                CLASS_EXECUTER_TRANSITION_STATE pTransitionState = listTransitionState[i];
                if (pTransitionState.CheckIsTransition())
                {
                    p_pFSMOwner.DoEnqueueState(pTransitionState.eNextState, pTransitionState.eStateInsertType);
                    bIsUpdate = false;
                    break;
                }
            }

            yield return null;
        }
    }

    abstract public IEnumerator OnStart_State_ExecuteContainer(CLASS_STATE pPrevState, EStateStartType eStateStartType);

    virtual public void OnAwake_State(CObjectBase pScriptOwner, CFSM<ENUM_STATE, CLASS_STATE> pFSMOwner)
    {
        _pScriptOwner = pScriptOwner;
        p_pExecuter_OnTransitionState.DoNotify_OnAwake(pScriptOwner);
        p_pExecuter_OnEnable.DoNotify_OnAwake(pScriptOwner);
    }

    virtual public void OnFinish_State(CLASS_STATE pNextState, EStateFinishType eStateFinishType)
    {
        if (_pCoroutine_ExecuteContainer != null)
            _pScriptOwner.StopCoroutine(_pCoroutine_ExecuteContainer);
    }
}


/// <summary>
/// 
/// </summary>
public abstract class CFSMExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE> : CFSM<ENUM_STATE, CLASS_STATE>
    where CLASS_STATE : State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE>
    where CLASS_EXECUTER_ONSTART_STATE : class, IExecuter
    where CLASS_EXECUTER_TRANSITION_STATE : CTransitionStateExecuter_Base<ENUM_STATE>
{
    /* const & readonly declaration             */

/* enum & struct declaration                */

/* public - Field declaration            */

#if ODIN_INSPECTOR
    [Indent(-1)]
    [ValueDropdown(nameof(GetStateList), IsUniqueList = true, ExcludeExistingValuesInList = true)]
    [ListDrawerSettings(ShowIndexLabels = false, ShowItemCount = false, OnBeginListElementGUI = nameof(BeginDrawListElement), OnEndListElementGUI = nameof(EndDrawListElement), DraggableItems = false, Expanded = false)]
#endif
    public List<CLASS_STATE> p_listState;

    /* protected & private - Field declaration         */

    List<CLASS_STATE> _listState_Fixed = new List<CLASS_STATE>();
#if ODIN_INSPECTOR
    ValueDropdownList<CLASS_STATE> _list_ForPrint = new ValueDropdownList<CLASS_STATE>();
#endif

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInit_FSMExecuter(CObjectBase pOwner)
    {
        base.DoInit(pOwner, p_listState.ToArray());
    }

#if ODIN_INSPECTOR
    public ValueDropdownList<CLASS_STATE> GetStateList()
    {
        if (_listState_Fixed == null)
            _listState_Fixed = new List<CLASS_STATE>();
        _listState_Fixed.Clear();
        OnSetStateList(ref _listState_Fixed);

        if (_list_ForPrint == null)
            _list_ForPrint = new ValueDropdownList<CLASS_STATE>();
        _list_ForPrint.Clear();

        for (int i = 0; i < _listState_Fixed.Count; i++)
            _list_ForPrint.Add(_listState_Fixed[i].IDictionaryItem_GetKey().ToString(), _listState_Fixed[i]);

        return _list_ForPrint;
    }
#endif

    //public void DoInit(CObjectBase pOwner)
    //{
    //    base.DoInit(pOwner, p_listState.ToArray());
    //}

    //public override void DoInit(CObjectBase pOwner, params CLASS_STATE[] arrState)
    //{
    //    base.DoInit(pOwner, p_listState.ToArray());
    //}


    // ========================================================================== //

    /* protected - Override & Unity API         */

    /* protected - [abstract & virtual]         */

    abstract protected void OnSetStateList(ref List<CLASS_STATE> list_ForPrint_DefaultCount_IsZero);

    // ========================================================================== //

#region Private

    private void BeginDrawListElement(int index)
    {
        if (p_listState[index] == null)
            return;
#if ODIN_INSPECTOR && UNITY_EDITOR
        SirenixEditorGUI.BeginBox(this.p_listState[index].IDictionaryItem_GetKey().ToString());
#endif
    }

    private void EndDrawListElement(int index)
    {
#if ODIN_INSPECTOR && UNITY_EDITOR
        SirenixEditorGUI.EndBox();
#endif
    }

#endregion Private
}

#if ODIN_INSPECTOR
#if UNITY_EDITOR

[DrawerPriority(0, 0, 1)]
public class State_ExecuteContainer_Drawer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE> : OdinValueDrawer<CLASS_STATE>
    where CLASS_STATE : State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE>
    where CLASS_EXECUTER_ONSTART_STATE : class, IExecuter
    where CLASS_EXECUTER_TRANSITION_STATE : CTransitionStateExecuter_Base<ENUM_STATE>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE> value = this.ValueEntry.SmartValue;

        var rect = EditorGUILayout.GetControlRect();

        // In Odin, labels are optional and can be null, so we have to account for that.
        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        if (value.p_pExecuter_OnTransitionState == null)
            value.p_pExecuter_OnTransitionState = new CExecuterContainer<CLASS_EXECUTER_TRANSITION_STATE>();
        this.ValueEntry.Property.Children["p_pExecuter_OnTransitionState"].Draw();

        if (value.p_pExecuter_OnEnable == null)
            value.p_pExecuter_OnEnable = new CExecuterContainer<CLASS_EXECUTER_ONSTART_STATE>();
        this.ValueEntry.Property.Children["p_pExecuter_OnEnable"].Draw();
    }
}

public class CFSMExecuteContainer_Drawer<CLASS_FSM_EXECUTE_CONTAINER, ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE> : OdinValueDrawer<CLASS_FSM_EXECUTE_CONTAINER>
    where CLASS_FSM_EXECUTE_CONTAINER : CFSMExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE>
    where CLASS_STATE : State_ExecuterContainer<ENUM_STATE, CLASS_STATE, CLASS_EXECUTER_ONSTART_STATE, CLASS_EXECUTER_TRANSITION_STATE>
    where CLASS_EXECUTER_ONSTART_STATE : class, IExecuter
    where CLASS_EXECUTER_TRANSITION_STATE : CTransitionStateExecuter_Base<ENUM_STATE>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        CLASS_FSM_EXECUTE_CONTAINER value = this.ValueEntry.SmartValue;

        var rect = EditorGUILayout.GetControlRect();

        // In Odin, labels are optional and can be null, so we have to account for that.
        if (label != null)
        {
            rect = EditorGUI.PrefixLabel(rect, label);
        }

        if (value.p_listState == null)
            value.p_listState = new List<CLASS_STATE>();

        if (value.p_listState.Count == 0)
        {
            var listDefault = value.GetStateList();
            for (int i = 0; i < listDefault.Count; i++)
                value.p_listState.Add(listDefault[i].Value);
        }

        this.ValueEntry.Property.Children["p_listState"].Draw(label);
    }
}

#endif
#endif