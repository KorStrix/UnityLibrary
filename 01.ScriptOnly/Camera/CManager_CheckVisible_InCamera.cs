#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-18 오후 4:03:20
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

public static class CameraExtensions
{
    public static Bounds GetBounds_Orthographic(this Camera pCamera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = pCamera.orthographicSize * 2f;

        return new Bounds(
                pCamera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    }

    // 참고한 링크
    // https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
    public static Bounds GetBounds_3D(this Camera pCamera, float fFarDistance)
    {
        var fFrustumHeight = 2f * fFarDistance * Mathf.Tan(pCamera.fieldOfView * .5f * Mathf.Deg2Rad);
        var fDistance = fFrustumHeight * .5f / Mathf.Tan(pCamera.fieldOfView * .5f * Mathf.Deg2Rad);
        var fFrustumWidth = fFrustumHeight * pCamera.aspect;

        Vector3 vecPos = pCamera.transform.position;
        vecPos.z += fFarDistance / 2f;

        return new Bounds(vecPos, new Vector3(fFrustumWidth, fFrustumHeight, fDistance));
    }
}

public interface IVisibleListener
{
    void IVisibleListener_OnVisible(bool bIsVisible);
}

[RequireComponent(typeof(Camera))]
public class CManager_CheckVisible_InCamera : CSingletonMonoBase<CManager_CheckVisible_InCamera>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EUpdateMode
    {
        Manual,
        Update,
        FixedUpdate
    }

    public enum EExcuteMode
    {
        TwoD_Physics,
        ThreeD_Physics, // WIP
    }

    /* public - Field declaration            */

    public Camera p_pCamera { get { return _pCamera; } }

    public int _iCapacity = 1024;

    public EUpdateMode p_eUpdateMode = EUpdateMode.Update;
    public EExcuteMode p_eExcuteMode = EExcuteMode.TwoD_Physics;

    public LayerMask p_sInCamera_CheckLayer;

    public float _fColliderSize_ScaleFactor = 1f;

    /* protected & private - Field declaration         */

    [GetComponent]
    Camera _pCamera = null;
    [SerializeField]
    CList_Enter_Stay_Exit<Collider2D> _listVisibleObject = new CList_Enter_Stay_Exit<Collider2D>();

    List<Collider> _listCollider = new List<Collider>();
    List<Collider2D> _listCollider_2D = new List<Collider2D>();

    Collider[] _arrColliderBuffer;
    Collider2D[] _arrColliderBuffer_2D;

    Plane[] _arrFrustumPlane = new Plane[6];

    Bounds _sBoundLast;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoUpdate_CheckVisible()
    {
        switch (p_eExcuteMode)
        {
            case EExcuteMode.TwoD_Physics:

                var listEnter = DoCheck_IsVisible_UsePhysics_2D(p_sInCamera_CheckLayer);
                _listVisibleObject.AddEnter(listEnter);

                for (int i = 0; i < _listVisibleObject.p_list_Enter.Count; i++)
                    _listVisibleObject.p_list_Enter[i].gameObject.SendMessage(nameof(IVisibleListener.IVisibleListener_OnVisible), true, SendMessageOptions.DontRequireReceiver);

                for (int i = 0; i < _listVisibleObject.p_list_Exit.Count; i++)
                    _listVisibleObject.p_list_Exit[i].gameObject.SendMessage(nameof(IVisibleListener.IVisibleListener_OnVisible), false, SendMessageOptions.DontRequireReceiver);

                break;

            //case EExcuteMode.ThreeD_Physics:
            //    DoCheck_IsVisible_UsePhysics(p_sCheckLayer_Mask);
            //    break;
        }
    }

    public void DoUpdateFrustumPlane()
    {
        GeometryUtility.CalculateFrustumPlanes(_pCamera, _arrFrustumPlane);
    }

    public bool DoCheck_IsVisible(Renderer pRendrer, bool bUpdate_FrustumPlane = true)
    {
        if (pRendrer == null)
            return false;

        if (bUpdate_FrustumPlane)
            DoUpdateFrustumPlane();

        return GeometryUtility.TestPlanesAABB(_arrFrustumPlane, pRendrer.bounds);
    }

    public List<Collider> DoCheck_IsVisible_UsePhysics(LayerMask pLayerMask_Detect)
    {
        Bounds sBounds = _pCamera.GetBounds_Orthographic();

        Vector3 vecSize = sBounds.size;
        Vector3 vecPosition = vecSize / 2f;

        int iHitCount = Physics.OverlapBoxNonAlloc(vecPosition, vecSize * _fColliderSize_ScaleFactor, _arrColliderBuffer, transform.rotation, pLayerMask_Detect);
        _listCollider.Clear();
        for (int i = 0; i < iHitCount; i++)
            _listCollider.Add(_arrColliderBuffer[i]);

        return _listCollider;
    }

    public List<Collider2D> DoCheck_IsVisible_UsePhysics_2D(LayerMask pLayerMask_Detect)
    {
        return DoCheck_IsVisible_UsePhysics_2D(_pCamera.GetBounds_Orthographic(), pLayerMask_Detect);
    }

    public List<Collider2D> DoCheck_IsVisible_UsePhysics_2D(Bounds sBound, LayerMask pLayerMask_Detect)
    {
        _sBoundLast = sBound;
        int iHitCount = Physics2D.OverlapBoxNonAlloc(sBound.center, sBound.size * _fColliderSize_ScaleFactor, transform.rotation.eulerAngles.z, _arrColliderBuffer_2D, pLayerMask_Detect);
        _listCollider_2D.Clear();
        for (int i = 0; i < iHitCount; i++)
            _listCollider_2D.Add(_arrColliderBuffer_2D[i]);

        return _listCollider_2D;
    }



    public List<Collider2D> GeVisibleObjectList_Enter() { return _listVisibleObject.p_list_Enter; }
    public List<Collider2D> GeVisibleObjectList_Stay() { return _listVisibleObject.p_list_Stay; }
    public List<Collider2D> GeVisibleObjectList_Exit() { return _listVisibleObject.p_list_Exit; }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _arrColliderBuffer = new Collider[_iCapacity];
        _arrColliderBuffer_2D = new Collider2D[_iCapacity];
        _listVisibleObject = new CList_Enter_Stay_Exit<Collider2D>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(p_eUpdateMode == EUpdateMode.Update)
            DoUpdate_CheckVisible();
    }

    private void FixedUpdate()
    {
        if (p_eUpdateMode == EUpdateMode.FixedUpdate)
            DoUpdate_CheckVisible();
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        if (_pCamera == null)
            _pCamera = GetComponent<Camera>();


        Vector3 vecSize = _sBoundLast.size;
        Vector3 vecPosition = vecSize / 2f;

        Matrix4x4 sCameraMatrix = new Matrix4x4();
        sCameraMatrix.SetTRS(Vector3.zero, _pCamera.transform.rotation, Vector3.one);
        Gizmos.matrix = sCameraMatrix;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_sBoundLast.center, vecSize);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test