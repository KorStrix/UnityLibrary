#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-22 오전 11:16:10
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
public class ExecuterList_Example_HowMove :
#if ODIN_INSPECTOR
    Sirenix.OdinInspector.SerializedMonoBehaviour
#else
    MonoBehaviour
#endif
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public abstract class Executer_HowMove : CExecuterBase
    {
        [GetComponent]
        protected ExecuterList_Example_HowMove _pExample_Owner = null;

        abstract public void Move(MonoBehaviour pCaller);
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("두발로 걷는 로직입니다.")]
#endif
    [RegistSubString("2발용 - 걷기")]
    public class TwoLeg_Walking : Executer_HowMove
    {
        [Range(0, 10)]
        public int WalkingSpeed_2Leg;
        public bool bIsError = false;

        public override void Move(MonoBehaviour pCaller)
        {
            Debug.Log(pCaller.name + " 두발로 걷는중.." + " Speed : " + WalkingSpeed_2Leg, pCaller);
        }

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            if (bIsError)
            {
                bIsInvalid_Default_IsFalse = bIsError;
                strErrorMessage_Default_Is_Error = "두발로 걷기 로직에 Error가 체크되었습니다.";
            }
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("두발로 뛰는 로직입니다.")]
#endif
    [RegistSubString("2발용 - 뛰기")]
    public class TwoLeg_Running : Executer_HowMove
    {
        public int RunningSpeed_2Leg;

        public override void Move(MonoBehaviour pCaller)
        {
            Debug.Log(pCaller.name + " 두발로 뛰는중.." + " Speed : " + RunningSpeed_2Leg, pCaller);
        }

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            if(RunningSpeed_2Leg == 0)
            {
                bIsInvalid_Default_IsFalse = true;
                strErrorMessage_Default_Is_Error = "현재 스피드가 0입니다.";
            }
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("네발로 걷는 로직입니다.")]
#endif
    [RegistSubString("4발용 - 걷기")]
    public class FourLeg_Walking : Executer_HowMove
    {
        [Range(0f, 10f)]
        public float WalkingSpeed_4Leg;

        public override void Move(MonoBehaviour pCaller)
        {
            Debug.Log(pCaller.name + " 네발로 걷는중.." + " Speed : " + WalkingSpeed_4Leg, pCaller);
        }

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            if (WalkingSpeed_4Leg == 0f)
            {
                bIsInvalid_Default_IsFalse = true;
                strErrorMessage_Default_Is_Error = "현재 스피드가 0입니다.";
            }
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("네발로 뛰는 로직입니다.")]
#endif
    [RegistSubString("4발용 - 뛰기")]
    public class FourLeg_Running : Executer_HowMove
    {
        public float RunningSpeed_4Leg;

        public override void Move(MonoBehaviour pCaller)
        {
            Debug.Log(pCaller.name + " 네발로 뛰는중.." + " Speed : " + RunningSpeed_4Leg, pCaller);
        }

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            if (RunningSpeed_4Leg == 0f)
            {
                bIsInvalid_Default_IsFalse = true;
                strErrorMessage_Default_Is_Error = "현재 스피드가 0입니다.";
            }
        }
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    [DisplayName("평소에 움직이는법")]
    [SerializeField]
    CExecuterList<Executer_HowMove> _pMoveExecuterList = new CExecuterList<Executer_HowMove>();

    [DisplayName("급할때 움직이는법")]
    [SerializeField]
    CExecuterList<Executer_HowMove> _pMoveExecuterList_OnHurry = new CExecuterList<Executer_HowMove>();

    // ========================================================================== //

#if ODIN_INSPECTOR
    [Button("Test Idle Move!")]
#endif
    private void IdleMove()
    {
        Debug.LogWarning(name + " 평소", this);

        for (int i = 0; i < _pMoveExecuterList.p_listExecute.Count; i++)
            _pMoveExecuterList.p_listExecute[i].Move(this);
    }

#if ODIN_INSPECTOR
    [Button("Test Hurry Move!")]
#endif
    private void HurryMove()
    {
        Debug.LogWarning(name + " 급할 때", this);

        for (int i = 0; i < _pMoveExecuterList_OnHurry.p_listExecute.Count; i++)
            _pMoveExecuterList_OnHurry.p_listExecute[i].Move(this);
    }
}