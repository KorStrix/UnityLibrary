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

public class CCompoCustomPhysics : CObjectBase
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

    public enum EPhysicsEventCustom
    {
        Enter,
        Stay,
        Exit
    }

    /* public - Field declaration            */

    public delegate void OnPhysicsEvent2D(List<Collider2D> listCollider, EPhysicsEventCustom ePhysicsEvent);
    public delegate void OnPhysicsEvent3D(List<Collider> listCollider, EPhysicsEventCustom ePhysicsEvent);

    public event OnPhysicsEvent2D p_Event_OnPhysicsEvent_Custom2D;
    public event OnPhysicsEvent3D p_Event_OnPhysicsEvent_Custom3D;

    [Rename_Inspector("디버깅")]
    public bool p_bIsDebuging;

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


    List<Collider2D> _listCollider2D_InCollider = new List<Collider2D>();
    List<Collider2D> _listCollider2D_NewInner = new List<Collider2D>();
    List<Collider2D> _listCollider2D_Exit = new List<Collider2D>();

    List<Collider> _listCollider3D_InCollider = new List<Collider>();
    List<Collider> _listCollider3D_NewInner = new List<Collider>();
    List<Collider> _listCollider3D_Exit = new List<Collider>();


    RaycastHit[] _arrHitInfo3D;
    RaycastHit2D[] _arrHitInfo2D;

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

    public void DoClear_InColliderList()
    {
        _listCollider2D_InCollider.Clear();
        _listCollider3D_InCollider.Clear();
    }

    public List<Collider2D> GetList_InCollider2D()
    {
        return _listCollider2D_InCollider;
    }

    public List<Collider> GetList_InCollider3D()
    {
        return _listCollider3D_InCollider;
    }

    public bool DoCheck_IsInner(Collider2D pCollider)
    {
        return _listCollider2D_InCollider.Contains(pCollider);
    }

    public bool DoCheck_IsInner(Collider pCollider)
    {
        return _listCollider3D_InCollider.Contains(pCollider);
    }
    public void DoLock_CalculatePhysics(bool bLock)
    {
        _bIsLock_CalculatePhysics = bLock;
    }

    public void DoCalculatePhysics_SphereCollider(float fRadius)
    {
        int iHitCount = Physics.SphereCastNonAlloc(p_pTransformTarget.position, fRadius, p_pTransformTarget.up, _arrHitInfo3D, Mathf.Infinity, p_pLayerMask);
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

        if(_pBoxCollider_Origin)
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
            _arrHitInfo2D = new RaycastHit2D[p_iHitInfoCount];
        else
            _arrHitInfo3D = new RaycastHit[p_iHitInfoCount];
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        if (_eColliderType == EColliderType.None)
            yield break;

        if (_bIs2D)
        {
            while (true)
            {
                yield return new WaitForSeconds(p_fPhysicsCheckDelay);

                if (_bIsLock_CalculatePhysics)
                    continue;

                CalculatePhysics_2D(_OnGetHit());
            }
        }
        else
        {
            while (true)
            {
                yield return new WaitForSeconds(p_fPhysicsCheckDelay);

                if (_bIsLock_CalculatePhysics)
                    continue;

                CalculatePhysics_3D(_OnGetHit());
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (p_bIsDebuging == false)
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
        for (int i = 0; i < _listCollider3D_InCollider.Count; i++)
        {
            Gizmos.DrawLine(vecPos, _listCollider3D_InCollider[i].transform.position);
            Gizmos.DrawWireSphere(_listCollider3D_InCollider[i].transform.position, 1f);
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
        _listCollider3D_NewInner.Clear();
        _listCollider3D_Exit.Clear();

        for (int i = 0; i < iHitCount; i++)
            _listCollider3D_NewInner.Add(_arrHitInfo3D[i].collider);

        OnCalculate_Inner_And_ExitCollider_3D(_listCollider3D_InCollider, _listCollider3D_NewInner, _listCollider3D_Exit);

        ExcutePhysicsEvent_3D(_listCollider3D_NewInner, EPhysicsEventCustom.Enter);
        ExcutePhysicsEvent_3D(_listCollider3D_InCollider, EPhysicsEventCustom.Stay);
        ExcutePhysicsEvent_3D(_listCollider3D_Exit, EPhysicsEventCustom.Exit);

        _listCollider3D_InCollider.AddRange(_listCollider3D_NewInner);

        for (int i = 0; i < _listCollider3D_Exit.Count; i++)
            _listCollider3D_InCollider.Remove(_listCollider3D_Exit[i]);

        if (p_bIsDebuging)
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Current - " + _listCollider3D_InCollider.ToStringList());
    }


    private void CalculatePhysics_2D(int iHitCount)
    {
        _listCollider2D_NewInner.Clear();
        _listCollider2D_Exit.Clear();

        for (int i = 0; i < iHitCount; i++)
            _listCollider2D_NewInner.Add(_arrHitInfo2D[i].collider);

        OnCalculate_Inner_And_ExitCollider_2D(_listCollider2D_InCollider, _listCollider2D_NewInner, _listCollider2D_Exit);

        ExcutePhysicsEvent_2D(_listCollider2D_NewInner, EPhysicsEventCustom.Enter);
        ExcutePhysicsEvent_2D(_listCollider2D_InCollider, EPhysicsEventCustom.Stay);
        ExcutePhysicsEvent_2D(_listCollider2D_Exit, EPhysicsEventCustom.Exit);

        _listCollider2D_InCollider.AddRange(_listCollider2D_NewInner);

        for (int i = 0; i < _listCollider2D_Exit.Count; i++)
            _listCollider2D_InCollider.Remove(_listCollider2D_Exit[i]);

        if (p_bIsDebuging)
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + " Current - " + _listCollider2D_InCollider.ToStringList());
    }




    private void ExcutePhysicsEvent_3D(List<Collider> listCollider, EPhysicsEventCustom ePhysicsEventCustom)
    {
        if (p_bIsDebuging)
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + ePhysicsEventCustom.ToString() + " - " + listCollider.ToStringList());

        if (listCollider.Count != 0 && p_Event_OnPhysicsEvent_Custom3D != null)
            p_Event_OnPhysicsEvent_Custom3D.Invoke(listCollider, ePhysicsEventCustom);
    }

    private void ExcutePhysicsEvent_2D(List<Collider2D> listCollider, EPhysicsEventCustom ePhysicsEventCustom)
    {
        if (p_bIsDebuging)
            Debug.Log(Time.realtimeSinceStartup.ToString("F2") + ePhysicsEventCustom.ToString() + " - " + listCollider.ToStringList());

        if (listCollider.Count != 0 && p_Event_OnPhysicsEvent_Custom3D != null)
            p_Event_OnPhysicsEvent_Custom2D.Invoke(listCollider, ePhysicsEventCustom);
    }



    private void SetCollider(Transform pTransformTarget, CircleCollider2D pCircleCollider2D)
    {
        p_pTransformTarget = pTransformTarget;

        _pCircleCollider2D_Current = pCircleCollider2D;
        _pCircleCollider2D_Current.enabled = false;

        _eColliderType = EColliderType.CircleCollider_2D;
        _OnGetHit = GetHit2D_CircleCollider;
    }

    private void SetCollider(Transform pTransformTarget, BoxCollider2D pBoxCollider2D)
    {
        p_pTransformTarget = pTransformTarget;

        _pBoxCollider2D_Current = pBoxCollider2D;
        _pBoxCollider2D_Current.enabled = false;

        _eColliderType = EColliderType.BoxCollider_2D;
        _OnGetHit = GetHit2D_BoxCollider;
    }

    private void SetCollider(Transform pTransformTarget, SphereCollider pSphereCollider)
    {
        p_pTransformTarget = pTransformTarget;

        _pSphereCollider_Current = pSphereCollider;
        _pSphereCollider_Current.enabled = false;

        _eColliderType = EColliderType.SphereCollider;
        _OnGetHit = GetHit3D_SphereCollider;
    }

    private void SetCollider(Transform pTransformTarget, BoxCollider pBoxCollider)
    {
        p_pTransformTarget = pTransformTarget;

        _pBoxCollider_Current = pBoxCollider;
        _pBoxCollider_Current.enabled = false;

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
        return Physics2D.BoxCastNonAlloc(p_pTransformTarget.position + (p_pTransformTarget.rotation * _pBoxCollider2D_Current.offset), _pBoxCollider2D_Current.size, 0f, Vector2.zero, _arrHitInfo2D, Mathf.Infinity, p_pLayerMask);
    }

    int GetHit2D_CircleCollider()
    {
        return Physics2D.CircleCastNonAlloc(p_pTransformTarget.position + (Vector3)_pCircleCollider2D_Current.offset, _pCircleCollider2D_Current.radius, Vector2.zero, _arrHitInfo2D, Mathf.Infinity, p_pLayerMask);
    }

    int GetHit3D_BoxCollider()
    {
        return Physics.BoxCastNonAlloc(p_pTransformTarget.position + _pBoxCollider_Current.center, CalculateVector3(p_pTransformTarget.lossyScale, _pBoxCollider_Current.size) * 0.5f, p_pTransformTarget.up, _arrHitInfo3D, p_pTransformTarget.rotation, Mathf.Infinity, p_pLayerMask);
    }

    int GetHit3D_SphereCollider()
    {
        return Physics.SphereCastNonAlloc(p_pTransformTarget.position + _pSphereCollider_Current.center, _pSphereCollider_Current.radius, p_pTransformTarget.up, _arrHitInfo3D, Mathf.Infinity, p_pLayerMask);
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test