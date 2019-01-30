#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-15 오후 5:45:04
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RangeAttribute = UnityEngine.RangeAttribute;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CSineVFXLaser : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Header("레이져 히트 옵션")]
    [Rename_Inspector("2D Physics를 사용하는지")]
    public bool _bUsePhysics2D = true;
    [Rename_Inspector("히트  레이어 마스크")]
    public LayerMask pLayerMask_Hit;

    [Space(10)]
    [Header("레이져 모양 옵션")]
    [Rename_Inspector("레이져 넓이")]
    public float fLaserWidth = 5f;
    [Rename_Inspector("레이져 최대 길이")]
    public float fLaserLength = 100f;
    [Rename_Inspector("레이져 지속시간")]
    public float fLaserDurationSec = 1f;

    [Space(10)]
    public AnimationCurve shaderProgressCurve;
    public AnimationCurve lineWidthCurve;

    [Space(10)]
    [Header("레이져 서브 이펙트 옵션")]
    [Rename_Inspector("레이 시작시 이펙트")]
    public GameObject pObjectStartPrefab;

    [Space(5)]
    [Rename_Inspector("레이 닿은 곳의 폭발 이펙트")]
    public GameObject pObjectExplosionPrefab;
    [UnityEngine.Range(0f, 1f)]
    [Rename_Inspector("폭발 이펙트 발동 최소 레이져 진행도 0 ~ 1")]
    public float fPlayExplosion_Progress_Min = 0f;
    [Range(0f, 1f)]
    [Rename_Inspector("폭발 이펙트 발동 최대 레이져 진행도 0 ~ 1")]
    public float fPlayExplosion_Progress_Max = 0.8f;

    [Space(5)]
    public float moveHitToSource = 0.5f;

    [Space(10)]
    [Header("레이져 플레이 옵션")]
    [Rename_Inspector("플레이 시 초반 딜레이")]
    public float fStartDelay = 0f;
    [Rename_Inspector("Enable 시 자동 플레이")]
    public bool bIsPlay_OnEnable = true;

    [Space(10)]
    [Header("레이져 루프 옵션")]
    [Rename_Inspector("반복 유무")]
    public bool bIsLoop;
    [Rename_Inspector("반복 시작 Progress")]
    public float fProgress_OnLoopStart = 0.2f;
    [Rename_Inspector("반복 끝 Progress")]
    public float fProgress_OnLoopFinish = 0.8f;


    /* protected & private - Field declaration         */

    private Vector3[] particleSpawnPositions;

    private LineRenderer _pLineRenderer;
    private Vector3 _vecPosition_ForExplosion;

    private int _iHitCount;
    private float AnimationProgress;
    private bool _bPlayExplosionEffect = false;


    [SerializeField]
    [Rename_Inspector("현재 진행 상태", false)]
    float _fProgress_0_1;

    [SerializeField]
    [Rename_Inspector("딜레이", false)]
    float _fDelay;

    [SerializeField]
    [Rename_Inspector("충돌된 길이", false)]
    private float _fHitLength;

    Transform _pTransformStart;
    Transform _pTransformExplosion;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayLaser()
    {
        _bPlayExplosionEffect = false;
        _fDelay = fStartDelay;
        _fProgress_0_1 = 0f;
        _iHitCount = 0;

        if (_pTransformStart)
            _pTransformStart.gameObject.SetActive(true);

        if (_pTransformExplosion)
            _pTransformExplosion.gameObject.SetActive(false);

        _pLineRenderer.material.SetFloat("_Progress", 0f);
        _pLineRenderer.widthMultiplier = 0f;
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pLineRenderer = GetComponent<LineRenderer>();
        _pLineRenderer.useWorldSpace = true;
        if (_pLineRenderer.positionCount < 2)
            _pLineRenderer.positionCount = 2;

        _fHitLength = 0;

        if (pObjectStartPrefab)
        {
            _pTransformStart = Instantiate(pObjectStartPrefab, transform).transform;
            _pTransformStart.gameObject.SetActive(false);
        }

        if (pObjectExplosionPrefab)
        {
            _pTransformExplosion = Instantiate(pObjectExplosionPrefab, transform).transform;
            _pTransformExplosion.gameObject.SetActive(false);
        }

        particleSpawnPositions = new Vector3[(int)(fLaserLength * 2f)];
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (bIsPlay_OnEnable)
            DoPlayLaser();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_fDelay > 0f)
        {
            _fDelay -= Time.deltaTime;
            return;
        }

        if (_fProgress_0_1 < 1f || bIsLoop)
        {
            _fProgress_0_1 += Time.deltaTime * (1 / fLaserDurationSec);
            if(bIsLoop)
            {
                if (_fProgress_0_1 > fProgress_OnLoopFinish)
                    _fProgress_0_1 = fProgress_OnLoopStart;
            }

            DrawLine();
            CastLaserRay();

            if (_fProgress_0_1 >= 1f && bIsLoop == false)
                OnFinishLaser();
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void OnFinishLaser()
    {
        if (_pTransformExplosion)
            _pTransformExplosion.gameObject.SetActive(false);

        if (_pTransformStart)
            _pTransformStart.gameObject.SetActive(false);
    }

    // Updating and Fading
    void DrawLine()
    {
        float fProgress = shaderProgressCurve.Evaluate(this._fProgress_0_1);
        _pLineRenderer.material.SetFloat("_Progress", fProgress);

        float fWidth = lineWidthCurve.Evaluate(this._fProgress_0_1);
        _pLineRenderer.widthMultiplier = fWidth * fLaserWidth;

        bool bActiveExplosionEffect = _bPlayExplosionEffect && fPlayExplosion_Progress_Min <=_fProgress_0_1 && _fProgress_0_1 <= fPlayExplosion_Progress_Max;
        if (_pTransformExplosion)
        {
            _pTransformExplosion.gameObject.SetActive(bActiveExplosionEffect);
            _pTransformExplosion.position = _vecPosition_ForExplosion;
        }
    }

    // Initialize Laser Line
    void CastLaserRay()
    {
        CRaycastHitWrapper hit;
        if (_bUsePhysics2D)
        {
            hit = Physics2D.Raycast(transform.position, transform.up, fLaserLength, pLayerMask_Hit);
        }
        else
        {
            RaycastHit hit3D;
            Physics.Raycast(transform.position, transform.up, out hit3D, fLaserLength, pLayerMask_Hit);
            hit = hit3D;
        }

        if(hit)
        {
            _vecPosition_ForExplosion = Vector3.MoveTowards(hit.point, transform.position, moveHitToSource);
            _fHitLength = hit.distance;
            _iHitCount = Mathf.RoundToInt(hit.distance * 2);
            _bPlayExplosionEffect = true;
        }
        else
        {
            _fHitLength = 0f;
            _iHitCount = 0;
            _bPlayExplosionEffect = false;
        }


        _pLineRenderer.SetPosition(0, transform.position);
        if (hit)
            _pLineRenderer.SetPosition(1, transform.position + (transform.up * _fHitLength));
        else
            _pLineRenderer.SetPosition(1, transform.position + (transform.up * fLaserLength));
    }


    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test