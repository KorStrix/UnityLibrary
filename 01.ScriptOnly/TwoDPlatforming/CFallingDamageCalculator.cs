#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-29 오후 7:04:53
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CCharacterController2D))]
public class CFallingDamageCalculator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public ObservableCollection<float> p_Event_OnFallingDamage { get; private set; } = new ObservableCollection<float>();

    [Header("에디터용")]
    [DisplayName("낙사 최대 체력")]
    public int p_iMaximumHP_OnEditor = 15;

    [Space(5)]
    [Header("세팅용")]
    [DisplayName("데미지가 안들어가는 길이")]
    public float p_fDistance_IgnoreDamage = 3f;
    [DisplayName("거리 1의 길이")]
    public float p_fDistance_Per1 = 1f;
    [DisplayName("거리 1당 데미지")]
    public float p_fDamage_PerDistance = 1f;

    [Space(5)]
    [Header("프로그래머 세팅용")]
    [DisplayName("낙사 시작 위치")]
    public Vector3 p_vecFallingDamageStartPos = Vector3.zero;

    /* protected & private - Field declaration         */

    Vector3 _vecDirection_Gravity = Vector3.down;
    Vector3 _vecPos_Prev;

    bool _bIsGround_Current;
    [DisplayName("떨어진 거리", false)]
    [SerializeField]
    float _fFallingDistance_Last;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoReset_FallingDistance()
    {
        _fFallingDistance_Last = 0f;
        _vecPos_Prev = transform.position + p_vecFallingDamageStartPos;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        GetComponent<CCharacterController2D>().p_Event_OnGround.Subscribe += OnGround_Subscribe;
        GetComponent<CCharacterController2D>().p_Event_OnChangePlatformerState.Subscribe += OnChangePlatformerState_Subscribe;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        DoReset_FallingDistance();
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        Vector3 vecStartPos = transform.position + p_vecFallingDamageStartPos;
        if (_bIsGround_Current == false)
        {
            if (vecStartPos.y > _vecPos_Prev.y)
            {
                DoReset_FallingDistance();
            }
            else
            {
                float fOffsetY = vecStartPos.y - _vecPos_Prev.y;
                _fFallingDistance_Last += fOffsetY * -1f;
                _vecPos_Prev = vecStartPos;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        UnityEditor.Handles.Label(transform.position, "_fFallingDistance_Last : " + _fFallingDistance_Last);

        DrawGizmo_DamageLine();
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void OnGround_Subscribe(bool bIsGround)
    {
        if (_bIsGround_Current == bIsGround)
            return;

        _bIsGround_Current = bIsGround;
        if (bIsGround)
            Calculate_FallingDamage();
    }

    private void Calculate_FallingDamage()
    {
        float fRemainDistance = _fFallingDistance_Last - p_fDistance_IgnoreDamage;
        if (fRemainDistance < 0f)
            return;

        float fFallingDamage = (int)(fRemainDistance / p_fDistance_Per1) * p_fDamage_PerDistance;
        if (fFallingDamage <= 0f)
            return;

        p_Event_OnFallingDamage.DoNotify(fFallingDamage);

        DoReset_FallingDistance();
    }

    private void OnChangePlatformerState_Subscribe(CCharacterController2D.CharacterController_PlatformerState_Arg pArg)
    {
        if (pArg.eState_Prev != pArg.eState_Current && pArg.eState_Current == ECharacterControllerState.Falling)
            DoReset_FallingDistance();
    }

#if UNITY_EDITOR
    private void DrawGizmo_DamageLine()
    {
        Vector3 vecStartPos = transform.position + p_vecFallingDamageStartPos;
        Vector3 vecDrawLineStartPos = vecStartPos + (_vecDirection_Gravity * p_fDistance_IgnoreDamage);
        Vector3 vecDrawLineDestPos = Vector3.zero;

        Color sColorStart = Color.white;
        Gizmos.color = sColorStart;
        Gizmos.DrawLine(vecStartPos, vecDrawLineStartPos);

        float fMaximumDistance = ((p_iMaximumHP_OnEditor / p_fDamage_PerDistance) * p_fDistance_Per1) + p_fDistance_IgnoreDamage;
        float fDistanceRemain = fMaximumDistance - p_fDistance_IgnoreDamage;

        GUIStyle pGUIStyle = new GUIStyle();
        pGUIStyle.normal.textColor = sColorStart;
        int iDamage = 1;
        UnityEditor.Handles.Label(vecDrawLineStartPos, "Damage 1 Start", pGUIStyle);

        while (fDistanceRemain > 0f)
        {
            sColorStart.g = fDistanceRemain / fMaximumDistance;
            sColorStart.b = fDistanceRemain / fMaximumDistance;
            vecDrawLineDestPos = vecDrawLineStartPos + (_vecDirection_Gravity * p_fDistance_Per1);

            Gizmos.color = sColorStart;
            Gizmos.DrawLine(vecDrawLineStartPos, vecDrawLineDestPos);

            vecDrawLineStartPos = vecDrawLineDestPos;
            fDistanceRemain -= p_fDistance_Per1;

            pGUIStyle.normal.textColor = sColorStart;
            UnityEditor.Handles.Label(vecDrawLineDestPos, "Damage : " + (iDamage - 1).ToString() + " Dest & " + iDamage.ToString() + " Start ", pGUIStyle);
            iDamage++;
        }

        pGUIStyle.normal.textColor = Color.green;
        UnityEditor.Handles.Label(vecDrawLineDestPos, "Distance : " + fMaximumDistance, pGUIStyle);
    }
#endif

#endregion Private
}