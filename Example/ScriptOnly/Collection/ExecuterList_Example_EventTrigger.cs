#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-07-02 오후 6:37:31
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
public class ExecuterList_Example_EventTrigger :
#if ODIN_INSPECTOR
    Sirenix.OdinInspector.SerializedMonoBehaviour
#else
    MonoBehaviour
#endif
{
    public enum EffectName
    {
        불, 물, 풀
    }

    public enum SoundName
    {
        폭발음_A,
        폭발음_B,
        콰쾅,
    }

    [System.Flags]
    public enum UnitCategory
    {
        없음,
        인간 = 1 << 1,
        오크 = 1 << 2,
        언데드 = 1 << 3,
        엘프 = 1 << 4,

        모든유닛 = 인간 | 오크 | 언데드 | 엘프
    }

    [System.Serializable]
    public class PositionOption
    {
        public enum ETarget
        {
            자신,
            이벤트_호출자,
            특정_트렌스폼,
        }

        [DisplayName("타겟 설정")]
        public ETarget eTarget;

#if ODIN_INSPECTOR
        [ShowIf(nameof(Check_Is_ShowTargetObject))]
#endif
        public Transform pTarget;

        public string GetPosition(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            switch (eTarget)
            {
                case ETarget.자신: return "내 위치";
                case ETarget.이벤트_호출자: return "상대방 위치";
                case ETarget.특정_트렌스폼: return "특정 위치";
            }

            return "설정 안됨";
        }

        public bool Check_Is_ShowTargetObject()
        {
            return eTarget == ETarget.특정_트렌스폼;
        }
    }

    public abstract class EventCondition : CExecuterBase
    {
        abstract public void CheckCondition(MonoBehaviour pCaller, ref bool bCondition_Is_Met_Default_Is_False);
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("특정 유닛을 감지합니다.")]
#endif
    [RegistSubString("유닛 감지")]
    public class Event_Check_Is_InCollider : EventCondition
    {
        [DisplayName("유닛 카테고리")]
        public UnitCategory eUnitCategory;
        [DisplayName("감지할 컬라이더")]
        public Collider pColliderTarget;

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            bIsInvalid_Default_IsFalse = pColliderTarget == null;
            if (bIsInvalid_Default_IsFalse)
                strErrorMessage_Default_Is_Error = "충돌체를 넣어주세요.";
        }

        public override void CheckCondition(MonoBehaviour pCaller, ref bool bCondition_Is_Met_Default_Is_False)
        {
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("특정 오브젝트가 사라질 때.")]
#endif
    [RegistSubString("오브젝트가 사라질 때")]
    public class Event_Object_OnDisable : EventCondition
    {
        [DisplayName("타겟 오브젝트")]
        public GameObject pObjectTarget;

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            bIsInvalid_Default_IsFalse = pObjectTarget == null;
            if (bIsInvalid_Default_IsFalse)
                strErrorMessage_Default_Is_Error = "오브젝트를 넣어주세요.";
        }

        public override void CheckCondition(MonoBehaviour pCaller, ref bool bCondition_Is_Met_Default_Is_False)
        {
        }
    }


    public class EventExecuteArg
    {
        public int iSomthing;
        public MonoBehaviour pSomthing;
    }

    public abstract class EventExecuteBase : CExecuterBase
    {
        abstract public void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg);
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("로그를 출력합니다.")]
#endif
    [RegistSubString("디버그/로그 출력")]
    public class Execute_Print_Log : EventExecuteBase
    {
        [DisplayName("출력할 텍스트")]
        public string strPrintLog;

        public override void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            Debug.Log(strPrintLog);
        }

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            base.IExecuter_Check_IsInvalid_OnEditor(pScriptOwner, ref bIsInvalid_Default_IsFalse, ref strErrorMessage_Default_Is_Error);

            bIsInvalid_Default_IsFalse = string.IsNullOrEmpty(strPrintLog);
            if (bIsInvalid_Default_IsFalse)
                strErrorMessage_Default_Is_Error = "출력할 로그를 적어주세요";
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("이펙트를 출력합니다.")]
#endif
    [RegistSubString("연출/이펙트 출력")]
    public class Execute_PlayEffect : EventExecuteBase
    {
        [DisplayName("출력할 이펙트")]
        public EffectName eEffectName;

        public PositionOption pPosition;

        public override void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            Debug.Log("이펙트 출력!" + eEffectName + " 위치 : " + pPosition.GetPosition(pCaller, pArg));
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("사운드를 출력합니다.")]
#endif
    [RegistSubString("연출/사운드 플레이")]
    public class Execute_PlaySound : EventExecuteBase
    {
        [DisplayName("플레이할 사운드")]
        public SoundName eSoundName;

        public PositionOption pPosition;

        public override void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            Debug.Log("사운드 출력!" + eSoundName + " 위치 : " + pPosition.GetPosition(pCaller, pArg));
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("진입한 오브젝트에 데미지를 입힙니다.")]
#endif
    [RegistSubString("데미지 주기")]
    public class Execute_Damage : EventExecuteBase
    {
        [DisplayName("데미지 양")]
        public int iDamage;

        public override void IExecuter_Check_IsInvalid_OnEditor(MonoBehaviour pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
            bIsInvalid_Default_IsFalse = iDamage <= 0;
            if (bIsInvalid_Default_IsFalse)
                strErrorMessage_Default_Is_Error = "데미지가 0이하가 될수 없습니다.";

        }

        public override void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            Debug.Log("데미지 입히기!" + iDamage);
        }
    }

#if ODIN_INSPECTOR
    [TypeInfoBox("진입한 오브젝트에 데미지를 입힙니다.")]
#endif
    [RegistSubString("기능 켜기")]
    public class Execute_Switch_On : EventExecuteBase
    {
        public PositionOption pPosition;

        public override void ExecuteEvent(MonoBehaviour pCaller, EventExecuteArg pArg)
        {
            Debug.Log(pPosition.GetPosition(pCaller, pArg) + " 의 기능을 켰다!");
        }
    }

    [DisplayName("이벤트 조건 리스트")]
    [SerializeField]
    CExecuterList<EventCondition> _pEventConditionList = new CExecuterList<EventCondition>();

    [DisplayName("이벤트 실행 리스트")]
    [SerializeField]
    CExecuterList<EventExecuteBase> _pEventExecuteList = new CExecuterList<EventExecuteBase>();


    // ========================================================================== //

    #region Private

#if ODIN_INSPECTOR
    [Button("테스트 실행리스트 실행")]
#endif
    private void ExecuteEvent()
    {
        for (int i = 0; i < _pEventExecuteList.p_listExecute.Count; i++)
            _pEventExecuteList.p_listExecute[i].ExecuteEvent(this, null);
    }

    #endregion Private
}