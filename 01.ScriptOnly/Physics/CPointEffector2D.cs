#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-11 오전 10:28:47
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

[RequireComponent(typeof(CPhysicsTrigger))]
public class CPointEffector2D : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public struct SPointTarget
    {
        public Rigidbody2D pRigidbody;
        public Vector2 vecVelocityOrigin;
        public float fDistanceOrigin;

        public float GetDistanceDelta_0_1(float fCurrentDistance)
        {
            return 1 - Mathf.Clamp01(fCurrentDistance / fDistanceOrigin);
        }
    }

    /* public - Field declaration            */

    [Rename_Inspector("힘 크기")]
    public float p_fForceMagnitude = 10f;
    [Rename_Inspector("최소 힘 크기")]
    public float p_fForceMagnitude_Min = 1f;
    [Rename_Inspector("최대 속도(Addforce 결과)")]
    public float p_fVelocity_Max = 1f;

    [Rename_Inspector("가까우면 무시할 지 체크할 거리")]
    public float p_fDistance_Shorter_OnIgnore = 1f;

    /* protected & private - Field declaration         */

    [GetComponent]
    CPhysicsTrigger _pCustomTrigger = null;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    Dictionary<Collider2D, SPointTarget> _mapEffectorTarget = new Dictionary<Collider2D, SPointTarget>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pCustomTrigger.p_Event_OnPhysicsEvent_Custom2D += _pCustomTrigger_p_Event_OnPhysicsEvent_Custom2D;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _mapEffectorTarget.Clear();
    }

    private void _pCustomTrigger_p_Event_OnPhysicsEvent_Custom2D(List<Collider2D> listCollider, CPhysicsTrigger.EPhysicsEventCustom ePhysicsEvent)
    {
        if (ePhysicsEvent == CPhysicsTrigger.EPhysicsEventCustom.Enter || ePhysicsEvent == CPhysicsTrigger.EPhysicsEventCustom.Stay)
        {
            Logic_PointEffector(listCollider, Time.deltaTime);
        }
        else if (ePhysicsEvent == CPhysicsTrigger.EPhysicsEventCustom.Exit)
        {
            Remove_CheckTimeList(listCollider);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Logic_PointEffector(List<Collider2D> listCollider,  float fDeltaTime)
    {
        Vector2 vecCurrentPosition = transform.position;
        for (int i = 0; i < listCollider.Count; i++)
        {
            Collider2D pCollider = listCollider[i];
            SPointTarget sPointTarget;
            if (GetPointTarget(pCollider, vecCurrentPosition, out sPointTarget) == false)
                continue;

            Rigidbody2D pRigidbody = sPointTarget.pRigidbody;
            float fDistanceCurrent = Vector3.Distance(pRigidbody.position, vecCurrentPosition);
            if (fDistanceCurrent <= p_fDistance_Shorter_OnIgnore)
            {
                pRigidbody.velocity = Vector2.zero;
                continue;
            }

            float fDistanceDelta_0_1 = sPointTarget.GetDistanceDelta_0_1(fDistanceCurrent);
            Vector2 vecForceDirection = (pRigidbody.position - vecCurrentPosition).normalized;

            float fForceMagnitude = p_fForceMagnitude * fDistanceDelta_0_1;
            if (fForceMagnitude.Check_ABSValue_IsGreater(p_fForceMagnitude_Min) == false)
                fForceMagnitude = p_fForceMagnitude_Min;

            float fVelocity = (fForceMagnitude * fDeltaTime) / pRigidbody.mass;
            if (fVelocity.Check_ABSValue_IsGreater(p_fVelocity_Max))
                fVelocity = Mathf.Abs(p_fVelocity_Max) * Mathf.Sign(fForceMagnitude);

            pRigidbody.velocity = vecForceDirection * fVelocity;
        }
    }

    private bool GetPointTarget(Collider2D pCollider, Vector2 vecCurrentPosition, out SPointTarget pPointTarget)
    {
        if (_mapEffectorTarget.ContainsKey(pCollider) == false)
        {
            Rigidbody2D pRigidbody = pCollider.GetComponent<Rigidbody2D>();
            if (pRigidbody == null)
            {
                pPointTarget = default(SPointTarget);
                return false;
            }

            SPointTarget sPointTargetNew = new SPointTarget();
            sPointTargetNew.pRigidbody = pRigidbody;
            sPointTargetNew.vecVelocityOrigin = pRigidbody.velocity;
            sPointTargetNew.fDistanceOrigin = Vector3.Distance(pRigidbody.position, vecCurrentPosition);

            _mapEffectorTarget.Add(pCollider, sPointTargetNew);
        }

        pPointTarget = _mapEffectorTarget[pCollider];
        return true;
    }

    private void Remove_CheckTimeList(List<Collider2D> listCollider)
    {
        for (int i = 0; i < listCollider.Count; i++)
        {
            Collider2D pCollider = listCollider[i];
            Rigidbody2D pRigidbody = pCollider.GetComponent<Rigidbody2D>();
            if (pRigidbody == null)
                continue;

            if (_mapEffectorTarget.ContainsKey(pCollider))
                _mapEffectorTarget.Remove(pCollider);
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test