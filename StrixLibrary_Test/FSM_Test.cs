using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class FSM_Test : CObjectBase
    {
        public enum EState
        {
            State_Idle, State_Attack, State_Guard, State_Loop_1, State_Loop_2,
        }

        public abstract class TestStateBase : IState<EState, TestStateBase>
        {
            public CFSM<EState, TestStateBase> p_pFSMOwner { get; set; }
            public CObjectBase p_pScriptOwner { get; set; }

            virtual public void OnAwake_State(CObjectBase pScriptOwner, CFSM<EState, TestStateBase> pFSMOwner)
            {
                Debug.Log(IDictionaryItem_GetKey() + " " + nameof(OnAwake_State) + " pScriptOwner : " + pScriptOwner + " pFSMOwner : " + pFSMOwner);
            }

            virtual public IEnumerator OnStart_State(TestStateBase pPrevState, EStateStartType eStateStartType)
            {
                int iWaitFrame = 0;
                while (iWaitFrame++ < 3)
                {
                    Debug.Log(IDictionaryItem_GetKey() + " eStateStartType : " + eStateStartType);
                    yield return null;
                }
            }

            virtual public void OnFinish_State(TestStateBase pNextState, EStateFinishType eStateFinishType)
            {
                if (pNextState != null)
                    Debug.Log(IDictionaryItem_GetKey() + nameof(OnFinish_State) + " eStateFinishType : " + eStateFinishType + " pNextState : " + pNextState.ToString());
                else
                    Debug.Log(IDictionaryItem_GetKey() + nameof(OnFinish_State) + " eStateFinishType : " + eStateFinishType + " pNextState : null");
            }

            public EState IDictionaryItem_GetKey()
            {
                return GetType().GetFriendlyName().ConvertEnum<EState>();
            }
        }

        public class State_Idle : TestStateBase { }
        public class State_Attack : TestStateBase { }
        public class State_Guard : TestStateBase { }

        public class State_Loop_1 : TestStateBase
        {

            public override IEnumerator OnStart_State(TestStateBase pPrevState, EStateStartType eStateStartType)
            {
                p_bIsLoop = true;

                yield return base.OnStart_State(pPrevState, eStateStartType);

                p_pFSMOwner.DoEnqueueState(EState.State_Loop_2, EStateInsertType.Waiting);
            }

            public override void OnFinish_State(TestStateBase pNextState, EStateFinishType eStateFinishType)
            {
                base.OnFinish_State(pNextState, eStateFinishType);

                p_bIsLoop = false;
            }
        }

        public class State_Loop_2 : TestStateBase
        {
            public override IEnumerator OnStart_State(TestStateBase pPrevState, EStateStartType eStateStartType)
            {
                p_bIsLoop = true;

                yield return base.OnStart_State(pPrevState, eStateStartType);

                p_pFSMOwner.DoEnqueueState(EState.State_Loop_1, EStateInsertType.Waiting);
            }

            public override void OnFinish_State(TestStateBase pNextState, EStateFinishType eStateFinishType)
            {
                base.OnFinish_State(pNextState, eStateFinishType);

                p_bIsLoop = false;
            }
        }


        [UnityTest]
        public IEnumerator State_Enumerator_Test()
        {
            CFSM<EState, TestStateBase> pFSM = Init_FSMTest(EState.State_Idle);
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Idle);

            // 1. Idle에서 2. Attack을 한다음 3. Guard를 순서대로 넣는다.
            pFSM.DoEnqueueState(EState.State_Attack, EStateInsertType.Waiting);
            pFSM.DoEnqueueState(EState.State_Guard, EStateInsertType.Waiting);
            // Wait이므로 현재는 Idle
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Idle);


            // 현재 상태(Idle)를 기다린 후에 체크하면 다음 상태인 2. Attack 상태가 된다.
            while (pFSM.p_eStateCurrent == EState.State_Idle)
                yield return null;

            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Attack);

            // Guard를 Interrupt하여 낀 후에 체크하면 현재 상태는 Guard가 된다.
            pFSM.DoEnqueueState(EState.State_Guard, EStateInsertType.Interrupt);
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Guard);


            // Guard를 기다리면 Interrupted당했던 2. Attack이 되돌아온다.
            while (pFSM.p_eStateCurrent == EState.State_Guard)
                yield return null;
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Attack);


            // Attack을 기다리면 3. Guard가 된다.
            while (pFSM.p_eStateCurrent == EState.State_Attack)
                yield return null;
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Guard);
        }

        static public bool p_bIsLoop = false;

        [UnityTest]
        public IEnumerator Controll_FSM_Inside_State_Test()
        {
            CFSM<EState, TestStateBase> pFSM = Init_FSMTest(EState.State_Attack);
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Attack);

            pFSM.DoEnqueueState(EState.State_Loop_1, EStateInsertType.Waiting);
            while (pFSM.p_eStateCurrent == EState.State_Attack)
                yield return null;
            Assert.AreEqual(pFSM.p_eStateCurrent, EState.State_Loop_1);
            Assert.AreEqual(p_bIsLoop, true);

            // Loop는 내부에서 FSM을 통해 다시 Loop를 Enqueue하고 있으므로 무한으로 돕니다.
            while (pFSM.p_eStateCurrent == EState.State_Loop_1)
                yield return null;
            Assert.AreEqual(p_bIsLoop, true);

            // 현재 State를 강제로 종료시킬 수 있습니다.
            pFSM.DoFinishCurrentState();
            Assert.AreEqual(p_bIsLoop, false);
        }


        private static CFSM<EState, TestStateBase> Init_FSMTest(EState eStateDefault)
        {
            GameObject pObject = new GameObject();
            CFSM<EState, TestStateBase> pFSM = new CFSM<EState, TestStateBase>();

            pFSM.DoInit(pObject.AddComponent<FSM_Test>(),
                new State_Idle(), new State_Attack(), new State_Guard(), new State_Loop_1(), new State_Loop_2());

            pFSM.DoEnqueueState(eStateDefault, EStateInsertType.Change);

            return pFSM;
        }
    }

}
