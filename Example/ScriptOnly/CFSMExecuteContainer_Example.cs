#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-21 오후 4:03:52
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public class CFSMExecuteContainer_Example : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EState
    {
        TestState_1,
        TestState_12,
        TestState_2,
        TestState_22,
        TestState_3,
        TestState_4
    }

    public class FSMExecuteContainer_Test : CFSMExecuterContainer<EState, TestStateBase, ExecuterBase, TransitionStateBase>
    {
        protected override void OnSetStateList_IfCountZero_IsError(ref List<TestStateBase> _list_ForPrint)
        {
            _list_ForPrint.Add(new TestState_1());
            _list_ForPrint.Add(new TestState_2());
            _list_ForPrint.Add(new TestState_4());
        }
    }

    public abstract class TestStateBase : State_ExecuterContainer<EState, TestStateBase, ExecuterBase, TransitionStateBase>
    {
        public override EState IDictionaryItem_GetKey()
        {
            return GetType().GetFriendlyName().ConvertEnum<EState>();
        }

        public override IEnumerator OnStart_State_ExecuteContainer(TestStateBase pPrevState, EStateStartType eStateStartType)
        {
            yield break;
        }
    }

    public class TestState_1 : TestStateBase
    {
    }

    public class TestState_12 : TestStateBase
    {
    }

    public class TestState_2 : TestStateBase
    {
    }

    public class TestState_22 : TestStateBase
    {
    }

    public class TestState_3 : TestStateBase
    {
    }

    public class TestState_4 : TestStateBase
    {
    }

    public abstract class TransitionStateBase : CTransitionStateExecuter_Base<EState>
    {
        public override string IHasName_GetName()
        {
            return this.ToStringSub();
        }
    }

    [RegistSubString("Somthing1")]
    public class TransitionState_Somthing : TransitionStateBase
    {
        public override bool CheckIsTransition()
        {
            return false;
        }
    }

    [RegistSubString("Somthing2")]
    public class TransitionState_Somthing2 : TransitionStateBase
    {
        public override bool CheckIsTransition()
        {
            return false;
        }
    }

    public abstract class ExecuterBase : IExecuter
    {
        virtual public int p_iExecuterOrder => 0;

        public IExecuterList p_pOwnerList { get; set; }

        public void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
        }

        public void IExecuter_OnAwake(IExecuterList pContainer, MonoBehaviour pScriptOwner)
        {
        }

        public void IExecuter_OnDestroy(IExecuterList pContainer, MonoBehaviour pScriptOwner)
        {
        }

        public void IExecuter_OnDisable(IExecuterList pContainer, MonoBehaviour pScriptOwner)
        {
        }

        public void IExecuter_OnEnable(IExecuterList pContainer, MonoBehaviour pScriptOwner)
        {
        }

        public string IHasName_GetName()
        {
            return this.ToStringSub();
        }
    }

    [RegistSubString("Executer1")]
    public class Executer1 : ExecuterBase
    {
    }

    [RegistSubString("Executer2")]
    public class Executer2 : ExecuterBase
    {
    }

    /* public - Field declaration            */

    public FSMExecuteContainer_Test Container;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}