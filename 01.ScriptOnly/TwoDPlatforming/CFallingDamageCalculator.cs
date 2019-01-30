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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CFallingDamageCalculator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Header("에디터용")]
    [Rename_Inspector("낙사 최대 체력")]
    public int p_iMaximumHP_OnEditor = 10;

    [Header("세팅용")]
    [Rename_Inspector("터레인 레이어")]
    public LayerMask p_pLayer_Terrain;
    [Rename_Inspector("데미지가 안들어가는 길이")]
    public float p_fDistance_IgnoreDamage = 3f;
    [Rename_Inspector("거리 1의 길이")]
    public float p_fDistance_Per1 = 1f;
    [Rename_Inspector("거리 1당 데미지")]
    public float p_fDamage_PerDistance = 1f;

    /* protected & private - Field declaration         */

    Vector3 _vecDirection_Gravity = Vector3.down;
    bool _bIsGround_Current;
    float _fFallingDistance_Last;


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        GetComponent<CCharacterController2D>().p_Event_OnGround.Subscribe += P_Event_OnGround_Subscribe;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(_bIsGround_Current == false)
        {
            RaycastHit2D sHit = Physics2D.Raycast(transform.position, _vecDirection_Gravity, float.MaxValue);
            if(sHit)
                _fFallingDistance_Last = Vector3.Distance(transform.position, sHit.point);

        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void P_Event_OnGround_Subscribe(bool bIsGround)
    {
        if (_bIsGround_Current == bIsGround)
            return;

        _bIsGround_Current = bIsGround;
        if (bIsGround)
            Calculate_FallingDamage();

        Debug.Log(name + " bIsGround : " + bIsGround);
    }

    private void Calculate_FallingDamage()
    {


        _fFallingDistance_Last = 0f;
    }

    private void OnDrawGizmos()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        Vector3 vecDrawLineStartPos = transform.position + (_vecDirection_Gravity * p_fDistance_IgnoreDamage);
        Color sColorStart =  Color.white;
        Gizmos.DrawLine(transform.position, vecDrawLineStartPos);

        float fMaximumDistance = ((p_iMaximumHP_OnEditor / (p_fDamage_PerDistance)) * p_fDistance_Per1) +  p_fDistance_IgnoreDamage;
        float fDistanceRemain = fMaximumDistance - p_fDistance_IgnoreDamage;

        GUIStyle pGUIStyle = new GUIStyle();
        int iDamage = 1;
        while (fDistanceRemain > 0f)
        {
            sColorStart.g = fDistanceRemain / fMaximumDistance;
            sColorStart.b = fDistanceRemain / fMaximumDistance;
            Vector3 vecDrawLineDestPos = transform.position + (_vecDirection_Gravity * p_fDistance_Per1 * (fMaximumDistance - fDistanceRemain));

            Gizmos.color = sColorStart;
            Gizmos.DrawLine(vecDrawLineStartPos, vecDrawLineDestPos);

            vecDrawLineStartPos = vecDrawLineDestPos;
            fDistanceRemain -= p_fDistance_Per1;

            pGUIStyle.normal.textColor = sColorStart;
            UnityEditor.Handles.Label(vecDrawLineDestPos, iDamage.ToString(), pGUIStyle);
            iDamage++;
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test