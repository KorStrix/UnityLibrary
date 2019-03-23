#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-07-23 오후 7:14:44
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

public class CPhysicsTrigger : CObjectBase, IPhysicsWrapper
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EColliderType
    {
        None,

        SphereCollider,
        BoxCollider,

        GreaterIs2D,

        CircleCollider_2D,
        BoxCollider_2D,
    }

    /* public - Field declaration            */

    public event OnPhysicsEvent2D p_Event_IPhysicsWrapper_OnPhysicsEvent_2D;
    public event OnPhysicsEvent3D p_Event_IPhysicsWrapper_OnPhysicsEvent_3D;

    [Rename_Inspector("컬라이더 On")]
    public bool p_bColliderOn = false;

    [Rename_Inspector("타겟 트렌스폼", false)]
    public Transform p_pTransformTarget;

    [Rename_Inspector("물리 체크 TimeDelta")]
    public float p_fPhysicsCheckDelay = 0.02f;

    [Rename_Inspector("히트 Array Capcity")]
    public int p_iHitInfoCount = 10;

    [Rename_Inspector("히트 레이어 마스크")]
    public LayerMask p_pLayerMask;

    /* protected & private - Field declaration         */

    [SerializeField]
    [Rename_Inspector("컬라이더 타입", false)]
    EColliderType _eColliderType;

    [GetComponent]
    BoxCollider2D _pBoxCollider2D_Origin = null;
    [GetComponent]
    CircleCollider2D _pCircleCollider2D_Orign = null;
    [GetComponent]
    BoxCollider _pBoxCollider_Origin = null;
    [GetComponent]
    SphereCollider _pSphereCollider_Orign = null;

    [Header("[2D]")]
    [Rename_Inspector("새로 충돌된 컬라이더 (Enter)")]
    [SerializeField]
    List<Collider2D> _listCollider2D_Enter = new List<Collider2D>();
    [Rename_Inspector("충돌 중인 컬라이더 (Stay)")]
    [SerializeField]
    List<Collider2D> _listCollider2D_Stay = new List<Collider2D>();
    [Rename_Inspector("충돌 중인 컬라이더 (Exit)")]
    [SerializeField]
    List<Collider2D> _listCollider2D_Exit = new List<Collider2D>();

    [Space(10)]
    [Header("[3D]")]
    [Rename_Inspector("새로 충돌된 컬라이더 (Enter)")]
    [SerializeField]
    List<Collider> _listCollider3D_Enter = new List<Collider>();
    [Rename_Inspector("충돌 중인 컬라이더 (Stay)")]
    [SerializeField]
    List<Collider> _listCollider3D_Stay = new List<Collider>();
    [Rename_Inspector("충돌 중인 컬라이더 (Exit)")]
    [SerializeField]
    List<Collider> _listCollider3D_Exit = new List<Collider>();


    Collider[] _arrCollider3D;
    Collider2D[] _arrCollider2D;

    delegate int OnGetHit();
    OnGetHit _OnGetHit;

    BoxCollider2D _pBoxCollider2D_Current;
    CircleCollider2D _pCircleCollider2D_Current;
    BoxCollider _pBoxCollider_Current;
    SphereCollider _pSphereCollider_Current;

    bool _bIs2D;
    bool _bIsLock_CalculatePhysics = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public Vector3 CalculateVector3(Vector3 vecX, Vector3 vecY)
    {
        Vector3 vecNewVector = vecX;
        vecNewVector.x *= vecY.x;
        vecNewVector.y *= vecY.y;
        vecNewVector.z *= vecY.z;

        return vecNewVector;
    }

    static public float CalculateAbsMax(Vector2 vector)
    {
        return Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    public void DoClear_InColliderList()
    {
        _listCollider2D_Enter.Clear();
        _listCollider2D_Stay.Clear();
        _listCollider2D_Exit.Clear();

        _listCollider3D_Enter.Clear();
        _listCollider3D_Stay.Clear();
        _listCollider3D_Exit.Clear();
    }

    public List<Collider2D> GetColliderList_2D_Enter() { return _listCollider2D_Enter; }
    public List<Collider2D> GetColliderList_2D_Stay() { return _listCollider2D_Stay; }
    public List<Collider2D> GetColliderList_2D_Exit() { return _listCollider2D_Exit; }

    public List<Collider> GetColliderList_3D_Enter() { return _listCollider3D_Enter; }
    public List<Collider> GetColliderList_3D_Stay() { return _listCollider3D_Stay; }
    public List<Collider> GetColliderList_3D_Exit() { return _listCollider3D_Exit; }

    public bool DoCheck_IsInner(Collider2D pCollider)
    {
        return _listCollider2D_Stay.Contains(pCollider);
    }

    public bool DoCheck_IsInner(Collider pCollider)
    {
        return _listCollider3D_Enter.Contains(pCollider);
    }
    public void DoLock_CalculatePhysics(bool bLock)
    {
        _bIsLock_CalculatePhysics = bLock;
    }

    public void DoCalculatePhysics_SphereCollider(float fRadius)
    {
        int iHitCount = Physics.OverlapSphereNonAlloc(p_pTransformTarget.position, fRadius, _arrCollider3D, p_pLayerMask);
        CalculatePhysics(iHitCount);
    }

    public void DoExcute_CalculatePhysics()
    {
        CalculatePhysics(_OnGetHit());
    }

    public void DoRevertOriginTarget()
    {
        if (_pBoxCollider2D_Origin)
            SetCollider(transform, _pBoxCollider2D_Origin);

        if (_pCircleCollider2D_Orign)
            SetCollider(transform, _pCircleCollider2D_Orign);

        if (_pBoxCollider_Origin)
            SetCollider(transform, _pBoxCollider_Origin);

        if (_pSphereCollider_Orign)
            SetCollider(transform, _pSphereCollider_Orign);
    }

    public void DoChangeCollider(Transform pTransform, BoxCollider2D pBoxCollider)
    {
        SetCollider(pTransform, pBoxCollider);
    }

    public void DoChangeCollider(Transform pTransform, CircleCollider2D pCircleCollider)
    {
        SetCollider(pTransform, pCircleCollider);
    }

    public int GetHit2D_BoxCollider(Vector2 vecPosition)
    {
        return Physics2D.OverlapBoxNonAlloc(vecPosition + _pBoxCollider2D_Current.offset, _pBoxCollider2D_Current.size, 0f, _arrCollider2D, p_pLayerMask);
    }



    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnReset()
    {
        base.OnReset();

        p_pTransformTarget = transform;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        _eColliderType = EColliderType.None;
        DoRevertOriginTarget();

        _bIs2D = _eColliderType > EColliderType.GreaterIs2D;
        if (_bIs2D)
            _arrCollider2D = new Collider2D[p_iHitInfoCount];
        else
            _arrCollider3D = new Collider[p_iHitInfoCount];
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        if (_eColliderType == EColliderType.None)
            yield break;

        if (_bIs2D)
        {
            while (true)
            {
                yield return YieldManager.GetWaitForSecond(p_fPhysicsCheckDelay);

                if (_bIsLock_CalculatePhysics)
                    continue;

                CalculatePhysics_2D(_OnGetHit());
            }
        }
        else
        {
            while (true)
            {
                yield return YieldManager.GetWaitForSecond(p_fPhysicsCheckDelay);

                if (_bIsLock_CalculatePhysics)
                    continue;

                CalculatePhysics_3D(_OnGetHit());
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        if(Application.isPlaying == false)
            EventOnAwake();

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.green;
        if (_pBoxCollider2D_Current)
            Gizmos.DrawWireCube(p_pTransformTarget.rotation * _pBoxCollider2D_Current.offset, _pBoxCollider2D_Current.size);

        if (_pCircleCollider2D_Current)
            Gizmos.DrawWireSphere(_pCircleCollider2D_Current.offset, _pCircleCollider2D_Current.radius);

        if (_pBoxCollider_Current)
            Gizmos.DrawWireCube(_pBoxCollider_Current.center, _pBoxCollider_Current.size);

        if (_pSphereCollider_Current)
            Gizmos.DrawWireSphere(_pSphereCollider_Current.center, _pSphereCollider_Current.radius);

        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.red;
        Vector3 vecPos = transform.position;
        for (int i = 0; i < _listCollider3D_Enter.Count; i++)
        {
            Gizmos.DrawLine(vecPos, _listCollider3D_Enter[i].transform.position);
            Gizmos.DrawWireSphere(_listCollider3D_Enter[i].transform.position, 1f);
        }
    }
#endif

    /* protected - [abstract & virtual]         */

    virtual protected void OnCalculate_Inner_And_ExitCollider_2D(List<Collider2D> list_InCollider_Already, List<Collider2D> list_InCollider_New, List<Collider2D> list_ExitCollider)
    {
        for (int i = 0; i < list_InCollider_Already.Count; i++)
        {
            Collider2D pCollider = list_InCollider_Already[i];
            if (list_InCollider_New.Contains(pCollider))
                list_InCollider_New.Remove(pCollider);
            else
                list_ExitCollider.Add(pCollider);
        }
    }

    virtual protected void OnCalculate_Inner_And_ExitCollider_3D(List<Collider> list_InCollider_Already, List<Collider> list_InCollider_New, List<Collider> list_ExitCollider)
    {
        for (int i = 0; i < list_InCollider_Already.Count; i++)
        {
            Collider pCollider = list_InCollider_Already[i];
            if (list_InCollider_New.Contains(pCollider))
                list_InCollider_New.Remove(pCollider);
            else
                list_ExitCollider.Add(pCollider);
        }
    }


    // ========================================================================== //

    #region Private

    private void CalculatePhysics(int iHitCount)
    {
        if (_bIs2D)
            CalculatePhysics_2D(iHitCount);
        else
            CalculatePhysics_3D(iHitCount);
    }

    private void CalculatePhysics_3D(int iHitCount)
    {
        _listCollider3D_Stay.Clear();
        _listCollider3D_Exit.Clear();
        for(int i = 0; i < iHitCount; i++)
            _listCollider3D_Stay.Add(_arrCollider3D[i]);

        OnCalculate_Inner_And_ExitCollider_3D(_listCollider3D_Enter, _listCollider3D_Stay, _listCollider3D_Exit);

        ExcutePhysicsEvent_3D(_listCollider3D_Stay, EPhysicsEvent.Enter);
        ExcutePhysicsEvent_3D(_listCollider3D_Enter, EPhysicsEvent.Stay);
        ExcutePhysicsEvent_3D(_listCollider3D_Exit, EPhysicsEvent.Exit);

        _listCollider3D_Enter.AddRange(_listCollider3D_Stay);

        for (int i = 0; i < _listCollider3D_Exit.Count; i++)
            _listCollider3D_Enter.Remove(_listCollider3D_Exit[i]);

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Current - " + _listCollider3D_Enter.ToStringList());
    }


    private void CalculatePhysics_2D(int iHitCount)
    {
        _listCollider2D_Enter.Clear();
        _listCollider2D_Exit.Clear();
        for (int i = 0; i < iHitCount; i++)
            _listCollider2D_Enter.Add(_arrCollider2D[i]);

        OnCalculate_Inner_And_ExitCollider_2D(_listCollider2D_Stay, _listCollider2D_Enter, _listCollider2D_Exit);

        ExcutePhysicsEvent_2D(_listCollider2D_Enter, EPhysicsEvent.Enter);
        ExcutePhysicsEvent_2D(_listCollider2D_Stay, EPhysicsEvent.Stay);
        ExcutePhysicsEvent_2D(_listCollider2D_Exit, EPhysicsEvent.Exit);

        _listCollider2D_Stay.AddRange(_listCollider2D_Enter);

        for (int i = 0; i < _listCollider2D_Exit.Count; i++)
            _listCollider2D_Stay.Remove(_listCollider2D_Exit[i]);

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(" Current - " + _listCollider2D_Stay.ToStringList());
    }




    private void ExcutePhysicsEvent_3D(List<Collider> listCollider, EPhysicsEvent ePhysicsEventCustom)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(ePhysicsEventCustom.ToString() + " - " + listCollider.ToStringList());

        if (listCollider.Count != 0 && p_Event_IPhysicsWrapper_OnPhysicsEvent_3D != null)
            p_Event_IPhysicsWrapper_OnPhysicsEvent_3D.Invoke(listCollider, ePhysicsEventCustom);
    }

    private void ExcutePhysicsEvent_2D(List<Collider2D> listCollider, EPhysicsEvent ePhysicsEventCustom)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(ePhysicsEventCustom.ToString() + " - " + listCollider.ToStringList());

        if (listCollider.Count != 0 && p_Event_IPhysicsWrapper_OnPhysicsEvent_2D != null)
            p_Event_IPhysicsWrapper_OnPhysicsEvent_2D.Invoke(listCollider, ePhysicsEventCustom);
    }



    private void SetCollider(Transform pTransformTarget, CircleCollider2D pCircleCollider2D)
    {
        p_pTransformTarget = pTransformTarget;

        _pCircleCollider2D_Current = pCircleCollider2D;
        _pCircleCollider2D_Current.enabled = p_bColliderOn;

        _eColliderType = EColliderType.CircleCollider_2D;
        _OnGetHit = GetHit2D_CircleCollider;
    }

    private void SetCollider(Transform pTransformTarget, BoxCollider2D pBoxCollider2D)
    {
        p_pTransformTarget = pTransformTarget;

        _pBoxCollider2D_Current = pBoxCollider2D;
        _pBoxCollider2D_Current.enabled = p_bColliderOn;

        _eColliderType = EColliderType.BoxCollider_2D;
        _OnGetHit = GetHit2D_BoxCollider;
    }

    private void SetCollider(Transform pTransformTarget, SphereCollider pSphereCollider)
    {
        p_pTransformTarget = pTransformTarget;

        _pSphereCollider_Current = pSphereCollider;
        _pSphereCollider_Current.enabled = p_bColliderOn;

        _eColliderType = EColliderType.SphereCollider;
        _OnGetHit = GetHit3D_SphereCollider;
    }

    private void SetCollider(Transform pTransformTarget, BoxCollider pBoxCollider)
    {
        p_pTransformTarget = pTransformTarget;

        _pBoxCollider_Current = pBoxCollider;
        _pBoxCollider_Current.enabled = p_bColliderOn;

        _eColliderType = EColliderType.BoxCollider;
        _OnGetHit = GetHit3D_BoxCollider;
    }

    private void SetCollider(Transform pTransformTarget, ref Collider2D pColliderOrigin, Collider2D pColliderCurrent)
    {
        pColliderCurrent.enabled = false;

        p_pTransformTarget = pTransformTarget;
        pColliderOrigin = pColliderCurrent;
    }



    int GetHit2D_BoxCollider()
    {
        Vector3 vecLossyScale = p_pTransformTarget.lossyScale;
        Vector3 vecPosition = p_pTransformTarget.position + (p_pTransformTarget.rotation * CalculateVector3(p_pTransformTarget.lossyScale, _pBoxCollider2D_Current.offset));

        if (vecLossyScale.x > 0f && vecLossyScale.y > 0f)
            return Physics2D.OverlapBoxNonAlloc(vecPosition, CalculateVector3(vecLossyScale, _pBoxCollider2D_Current.size), p_pTransformTarget.rotation.eulerAngles.z, _arrCollider2D, p_pLayerMask);
        else
        {
            Vector3 vecSize = CalculateVector3(vecLossyScale.Get_Abs(), _pBoxCollider2D_Current.size);
            return Physics2D.OverlapBoxNonAlloc(vecPosition, vecSize, p_pTransformTarget.rotation.eulerAngles.z, _arrCollider2D, p_pLayerMask);
        }
    }

    int GetHit2D_CircleCollider()
    {
        return Physics2D.OverlapCircleNonAlloc(p_pTransformTarget.position + CalculateVector3(p_pTransformTarget.lossyScale, _pCircleCollider2D_Current.offset), CalculateAbsMax((Vector2)p_pTransformTarget.lossyScale) * _pCircleCollider2D_Current.radius, _arrCollider2D, p_pLayerMask);
    }

    int GetHit3D_BoxCollider()
    {
        return Physics.OverlapBoxNonAlloc(p_pTransformTarget.position + _pBoxCollider_Current.center, CalculateVector3(p_pTransformTarget.lossyScale, _pBoxCollider_Current.size) * 0.5f, _arrCollider3D, p_pTransformTarget.rotation, p_pLayerMask);
    }

    int GetHit3D_SphereCollider()
    {
        return Physics.OverlapSphereNonAlloc(p_pTransformTarget.position + _pSphereCollider_Current.center, _pSphereCollider_Current.radius, _arrCollider3D, p_pLayerMask);
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test