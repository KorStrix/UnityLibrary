#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-06 오후 3:35:19
 *	기능 : 
 *	
 *	출처 : https://www.youtube.com/watch?v=2PZHOEAXrdY
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTrajectoryDrawer : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("점 원본")]
    public GameObject p_pObjectDot;
    [DisplayName("원본을 Disable할지")]
    public bool p_bDeactive_Origin = true;

    [Space(5)]
    [DisplayName("점 개수")]
    public int p_iNumberOfDots = 40;
    [DisplayName("점 사이 거리")]
    public float p_fDotSeparation = 3f;
    [DisplayName("최소 점 사이 거리")]
    public float p_fDotSeparation_Miniest = 0f;
    [DisplayName("원본과 떨어지는 거리")]
    public float p_fDotShift = 3f;

    [Space(5)]
    [DisplayName("도착지에 표기할 오브젝트")]
    public GameObject p_pObjectArrivePoint;
    [DisplayName("점을 그리는걸 멈추는 조건 충돌체 Layer")]
    public LayerMask p_sCheck_StopDrawLayerMask;

    /* protected & private - Field declaration         */

    RaycastHit2D[] arrHit = new RaycastHit2D[10];
    List<Transform> _listDots;
    GameObject _pObjectDotsRoot;
    Transform _pTransformArrivePoint_Copy;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoDrawTrajectory_Force(Rigidbody2D pRigidbody, Vector3 vecStartPos, Vector2 vecAddForce)
    {
        vecAddForce = (vecAddForce / pRigidbody.mass) * Time.fixedDeltaTime;
        DoDrawTrajectory_VelocityChange(pRigidbody.gravityScale, vecStartPos, vecAddForce);
    }

    public void DoDrawTrajectory_Impulse(Rigidbody2D pRigidbody, Vector3 vecStartPos, Vector2 vecAddForce)
    {
        vecAddForce /= pRigidbody.mass;
        DoDrawTrajectory_VelocityChange(pRigidbody.gravityScale, vecStartPos, vecAddForce);
    }

    public void DoDrawTrajectory_VelocityChange(float fGravityScale, Vector3 vecStartPos, Vector2 vecVelocity)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(name + " DoDrawTrajectory_VelocityChange - vecVelocity : " + vecVelocity + " gravity : " + Physics2D.gravity + " Time.fixedDeltaTime : " + Time.fixedDeltaTime, this);

        for (int i = 0; i < p_iNumberOfDots; i++)
            _listDots[i].gameObject.SetActive(false);

        RaycastHit2D sHitResult = default(RaycastHit2D);
        Vector2 vecPrevPos = vecStartPos;
        float fFixedDeltaTime = Time.fixedDeltaTime;
        for (int i = 0; i < p_iNumberOfDots; i++)
        {
            float fDotSeparation = p_fDotSeparation * i + p_fDotShift;
            float fX = vecStartPos.x + vecVelocity.x * fFixedDeltaTime * fDotSeparation;
            float fY = vecStartPos.y + vecVelocity.y * fFixedDeltaTime * fDotSeparation - (-(Physics2D.gravity.y * fGravityScale) / 2f * fFixedDeltaTime * fFixedDeltaTime * fDotSeparation * fDotSeparation);

            Vector2 vecNewPos = new Vector2(fX, fY);
            Vector2 vecDirection = vecNewPos - vecPrevPos;
            if (IsFinish_DrawTrajectory(vecPrevPos, vecDirection, ref sHitResult))
            {
                vecPrevPos = vecNewPos;
                break;
            }

            _listDots[i].position = new Vector3(fX, fY, _listDots[i].position.z);
            _listDots[i].gameObject.SetActive(true);
            vecPrevPos = vecNewPos;
        }

        if (_pTransformArrivePoint_Copy)
        {
            if (sHitResult)
                _pTransformArrivePoint_Copy.position = (Vector3)sHitResult.point;
            else
                _pTransformArrivePoint_Copy.position = vecPrevPos;
        }
    }

    public bool Check_IsDrawDots()
    {
        return _pObjectDotsRoot.activeInHierarchy;
    }

    public void DoActive_TrajectoryDots(bool bActive)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(name + " DoActive_TrajectoryDots - bActive : " + bActive, this);

        _pObjectDotsRoot.SetActive(bActive);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if(p_pObjectDot == null)
        {
            Debug.LogError(name + " 점 원본을 세팅해주어야 합니다.", this);
            return;
        }

        _pObjectDotsRoot = new GameObject("포물선 점들");
        _pObjectDotsRoot.SetActive(false);

        _listDots = new List<Transform>(p_iNumberOfDots);
        for (int i = 0; i < p_iNumberOfDots; i++)
        {
            GameObject pObjectDot = GameObject.Instantiate(p_pObjectDot);
            pObjectDot.name = string.Format("Dot_{0}", i);
            pObjectDot.transform.SetParent(_pObjectDotsRoot.transform);
            pObjectDot.SetActive(true);

            _listDots.Add(pObjectDot.transform);
        }

        if(p_pObjectArrivePoint)
        {
            _pTransformArrivePoint_Copy = GameObject.Instantiate(p_pObjectArrivePoint).transform;
            _pTransformArrivePoint_Copy.SetParent(_pObjectDotsRoot.transform);
            _pTransformArrivePoint_Copy.gameObject.SetActive(true);
        }

        if (p_bDeactive_Origin)
        {
            p_pObjectDot.SetActive(false);
            if (p_pObjectArrivePoint)
                p_pObjectArrivePoint.SetActive(false);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private bool IsFinish_DrawTrajectory(Vector2 vecPrevPos, Vector2 vecDirection, ref RaycastHit2D sHitResult)
    {
        bool bIsOut = false;
        int iHitCount = Physics2D.RaycastNonAlloc(vecPrevPos, vecDirection.normalized, arrHit, vecDirection.magnitude, p_sCheck_StopDrawLayerMask);
        for (int i = 0; i < iHitCount; i++)
        {
            if (arrHit[i].collider.isTrigger == false && arrHit[i].transform != transform)
            {
                sHitResult = arrHit[i];
                bIsOut = true;
                break;
            }
        }

        return bIsOut;
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test