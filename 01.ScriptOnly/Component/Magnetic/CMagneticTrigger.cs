#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-18 오후 4:38:00
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IMagneticListener
{
    void IMagneticListener_OnStartMagnetic(ref bool bIsStartMagnetic);
    void IMagneticListener_OnArriveMagnetic();
    void IMagneticListener_OnExitMagnetic();
}

public class CMagneticTrigger : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EMagneticType
	{
		Push = -1,
        Pull = 1,
    }

    private struct SMagneticInfo
	{
		public Transform pTransCached { get; private set; }
        public IMagneticListener iMagneticListener { get; private set; }
        public float fMagneticStartTime { get; private set; }
        public float fRandomSpeed { get; private set; }
        public bool bIsStartMagnetic { get; private set; }

        private Rigidbody _pRigidbody;
        private Rigidbody2D _pRigidbody2D;

        public SMagneticInfo(Transform pTransCached, IMagneticListener iMagneticListener, float fRandomSpeed)
        {
            this.pTransCached = pTransCached;
            this.iMagneticListener = iMagneticListener;
            this.fMagneticStartTime = Time.time;
            this.fRandomSpeed = fRandomSpeed;

            this._pRigidbody = pTransCached.GetComponent<Rigidbody>();
            this._pRigidbody2D = pTransCached.GetComponent<Rigidbody2D>();

            bool bIsStartMagnetic = true;
            iMagneticListener.IMagneticListener_OnStartMagnetic(ref bIsStartMagnetic);
            this.bIsStartMagnetic = bIsStartMagnetic;

        }

        public void AddForce(Vector3 vecForce)
        {
            if (_pRigidbody)
                _pRigidbody.AddForce(vecForce, ForceMode.Force);

            if (_pRigidbody2D)
                _pRigidbody2D.AddForce(vecForce, ForceMode2D.Force);
        }
        
        public void Event_OnDestroy()
        {
            iMagneticListener.IMagneticListener_OnArriveMagnetic();
        }

        public SMagneticInfo ResetStartTime()
        {
            this.fMagneticStartTime = Time.time;

            return this;
        }
    }

    #region Field

    /* public - Field declaration            */

    public CObserverSubject<Transform> p_Event_OnMagneticFinish { get; private set; } = new CObserverSubject<Transform>();

    [Header("SphereCollider or CircleCollider 필요")]
    [Rename_Inspector("자력에 물리를 사용할 것인지")]
    public bool p_bUsePhysics = false;
	[Rename_Inspector("자석 반응 타입")]
    public EMagneticType p_eMagnetReactType = EMagneticType.Pull;
    [Rename_Inspector("자력이 멈추는 최소 거리(Pull Only)")]
    public float p_fStopDistance = 1f;

    [Space(10)]
    [Header("최종자력: 거리별자력 + 최소자력 <= 최대자력")]
    [Rename_Inspector("거리별 자력")]
    public float p_fMagnetPower = 0.1f;
    [Rename_Inspector("최소 자력")]
    public float p_fMagnetPower_Min = 0.01f;
    [Rename_Inspector("최대 자력")]
    public float p_fMagnetPower_Max = 5f;

    [Space(10)]
    [Header("랜덤 설정")]
    [Rename_Inspector("랜덤자력 최소 배율")]
    [Range(0.5f, 3f)]
    public float p_fMagnetPower_Random_Min = 0.5f;
    [Range(0.5f, 3f)]
    public float p_fMagnetPower_Random_Max = 1.5f;

    [Space(10)]
    [Header("곡선 당기기 설정")]
    [Rename_Inspector("곡선으로 적용할지")]
    public bool p_bUseCurvePull = false;
    [Rename_Inspector("곡선의 완만도")]   
    public float p_fCurvePower = 1f;
    [Rename_Inspector("곡선 당기기를 어느 거리에서 할지(%)")]
    [Range(0f, 1f)]
    public float p_fRangePercent_EffectCurve = 0.5f;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    private Dictionary<int, SMagneticInfo> _mapMagneticPool = new Dictionary<int, SMagneticInfo>();

    bool _bIs2D;
    float _fSqrMagneticRange;

    #endregion Field

    #region Public

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/


    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        SphereCollider pCollider3D = GetComponent<SphereCollider>();
        if (pCollider3D)
        {
            _bIs2D = false;
            _fSqrMagneticRange = pCollider3D.radius * pCollider3D.radius;
        }
        else
        {
            CircleCollider2D pCollider2D = GetComponent<CircleCollider2D>();
            if (pCollider2D)
            {
                _bIs2D = true;
                _fSqrMagneticRange = pCollider2D.radius * pCollider2D.radius;
            }
            else
                Debug.LogError(name + " Error - 이 컴포넌트는 SphereCollider 혹은 CircleCollider를 필요로 합니다", this);
        }
    }

    private void OnTriggerEnter(Collider pCollider) { OnEnter(pCollider.GetHashCode(), pCollider.transform); }
    private void OnTriggerEnter2D(Collider2D pCollider) { OnEnter(pCollider.GetHashCode(), pCollider.transform); }

    private void OnTriggerStay(Collider pCollider) { OnStay(pCollider.GetHashCode(), pCollider.transform); }
    private void OnTriggerStay2D(Collider2D pCollider) { OnStay(pCollider.GetHashCode(), pCollider.transform); }

    private void OnTriggerExit(Collider pCollider) { OnExit(pCollider.GetHashCode()); }
    private void OnTriggerExit2D(Collider2D pCollider) { OnExit(pCollider.GetHashCode()); }

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    private void OnEnter(int iHashCode, Transform pTrans)
    {
        IMagneticListener iMagneticListener = pTrans.GetComponent<IMagneticListener>();
        if (iMagneticListener == null)
            return;

        if (_mapMagneticPool.ContainsKey(iHashCode) == false)
            _mapMagneticPool.Add(iHashCode, new SMagneticInfo(pTrans, iMagneticListener, Random.Range(p_fMagnetPower_Random_Min, p_fMagnetPower_Random_Max)));
        else
            _mapMagneticPool[iHashCode] = _mapMagneticPool[iHashCode].ResetStartTime();
    }


    private void OnStay(int iHashCode, Transform pTrans)
    {
        if (_mapMagneticPool.ContainsKey(iHashCode) == false)
            return;

        SMagneticInfo sMagneticInfo = _mapMagneticPool[iHashCode];
        Vector3 vecDirection = transform.position - pTrans.position;
        Vector3 vecMagneticForce = CalculateMagneticForce(ref sMagneticInfo, vecDirection, p_bUseCurvePull);
        Vector3 vecNewPosition = pTrans.position + vecMagneticForce;
        float fAfterSimulateDistance = (transform.position - vecNewPosition).sqrMagnitude;

        if (CheckIs_FinishMagnetic(vecDirection, fAfterSimulateDistance))
        {
            if (sMagneticInfo.bIsStartMagnetic && p_eMagnetReactType == EMagneticType.Pull)
                pTrans.position = transform.position;

            sMagneticInfo.Event_OnDestroy();
            _mapMagneticPool.Remove(iHashCode);
            p_Event_OnMagneticFinish.DoNotify(pTrans);
        }
        else
        {
            if(sMagneticInfo.bIsStartMagnetic)
            {
                if (p_bUsePhysics)
                    sMagneticInfo.AddForce(vecMagneticForce);
                else
                    pTrans.position = vecNewPosition;
            }
        }
    }

    private Vector3 CalculateMagneticForce(ref SMagneticInfo sMagneticInfo, Vector3 vecDirection, bool bUseCurve)
    {
        float fDistancePower;
        if (p_eMagnetReactType == EMagneticType.Pull)
            fDistancePower = (_fSqrMagneticRange - vecDirection.sqrMagnitude);
        else
            fDistancePower = vecDirection.sqrMagnitude * p_fMagnetPower;

        float fMagnetPower = fDistancePower + p_fMagnetPower_Min;
        if (fMagnetPower > p_fMagnetPower_Max)
            fMagnetPower = p_fMagnetPower_Max;

        vecDirection.Normalize();
        if (bUseCurve)
            vecDirection = CalculateCurve(ref sMagneticInfo, vecDirection);

        return (vecDirection * fMagnetPower * sMagneticInfo.fRandomSpeed * Time.deltaTime) * (int)p_eMagnetReactType;
    }

    private Vector3 CalculateCurve(ref SMagneticInfo sMagneticInfo, Vector3 vecDirection)
    {
        // Trigger 경계선에 가깝다면 일단 그대로 빨려들게 하기
        // 이걸 적용 안하면 경계선에서 자꾸 밀린다.
        if (Mathf.Abs(vecDirection.sqrMagnitude - _fSqrMagneticRange) < vecDirection.sqrMagnitude * p_fRangePercent_EffectCurve)
            return vecDirection;

        if (sMagneticInfo.fMagneticStartTime + p_fCurvePower > Time.time)
        {
            if (_bIs2D)
                CalculateCurve_2D(ref sMagneticInfo, ref vecDirection);
            else
                CalculateCurve_3D(ref sMagneticInfo, ref vecDirection);
        }

        return vecDirection;
    }

    private void CalculateCurve_3D(ref SMagneticInfo sMagneticInfo, ref Vector3 vecDirection)
    {
        float fX = Mathf.Abs(vecDirection.x);
        float fY = Mathf.Abs(vecDirection.y);
        float fZ = Mathf.Abs(vecDirection.z);

        if (fX > fY)
        {
            if (fX > fZ)
                vecDirection.x *= CalculateCurveSpeed(ref sMagneticInfo);
            else
                vecDirection.z *= CalculateCurveSpeed(ref sMagneticInfo);
        }
        else
        {
            if (fY > fZ)
                vecDirection.y *= CalculateCurveSpeed(ref sMagneticInfo);
            else
                vecDirection.z *= CalculateCurveSpeed(ref sMagneticInfo);
        }
    }

    private void CalculateCurve_2D(ref SMagneticInfo sMagneticInfo, ref Vector3 vecDirection)
    {
        float fX = Mathf.Abs(vecDirection.x);
        float fY = Mathf.Abs(vecDirection.y);

        if (fX > fY)
            vecDirection.x *= CalculateCurveSpeed(ref sMagneticInfo);
        else
            vecDirection.y *= CalculateCurveSpeed(ref sMagneticInfo);
    }

    private bool CheckIs_FinishMagnetic(Vector3 vecDirection, float fAfterSimulateDistance)
    {
        if (p_eMagnetReactType == EMagneticType.Pull)
            return vecDirection.sqrMagnitude < p_fStopDistance * p_fStopDistance;
        else
            return fAfterSimulateDistance > _fSqrMagneticRange;
    }

    private float CalculateCurveSpeed(ref SMagneticInfo sMagneticInfo)
    {
        float fProgress_0_1 = (Time.time - sMagneticInfo.fMagneticStartTime) / p_fCurvePower;
        if(fProgress_0_1 < 0.5f)
            return ((1 - fProgress_0_1) * -2f) + 1f;
        else
            return (fProgress_0_1 * 2f) - 1f;
    }

    private void OnExit(int iHashCode)
    {
        if(_mapMagneticPool.ContainsKey(iHashCode))
        {
            _mapMagneticPool[iHashCode].iMagneticListener.IMagneticListener_OnExitMagnetic();
            _mapMagneticPool.Remove(iHashCode);
        }
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
