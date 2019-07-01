#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-22 오후 12:18:11
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CManagerInputEventSystem : CSingletonMonoBase<CManagerInputEventSystem>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("이벤트 카메라")]
    public Camera p_pEventCamera;
    [DisplayName("히트할 레이어")]
    public LayerMask p_pLayerMask_Hit;
    [DisplayName("Is 2D")]
    public bool p_bIs2D;

    [Space(10)]
    [DisplayName("히트 어레이 Capacity")]
    public int p_iHitArrayCapapcity = 10;

    public List<CRaycastHitWrapper> p_listLastHit { get; private set; }

    /* protected & private - Field declaration         */

    List<Transform> _listTransform_EnterAlready = new List<Transform>();
    List<Transform> _listTransform_EnterNew = new List<Transform>();
    List<Transform> _listTransform_ExitEnter = new List<Transform>();

    List<RaycastHit2D> _listHit_2D = new List<RaycastHit2D>();

    RaycastHit2D[] _arrHit_2D;
    RaycastHit[] _arrHit;

    Ray pRay_OnClick_ForDebug;
    int _iLastHitCount;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public Vector3 DoGetMousePos()
    {
        Vector2 vecMousePosition;
        if (CalculateMousePosition_And_IsInScreen(out vecMousePosition) == false)
            return Vector3.zero;

        return p_pEventCamera.ScreenToWorldPoint(new Vector3(vecMousePosition.x, vecMousePosition.y, p_pEventCamera.nearClipPlane));
    }

    public Vector3 DoRayCasting_MousePos_3D(Camera pCamera, LayerMask sLayerMask_Hit)
    {
        Vector2 vecMousePosition;
        if (CalculateMousePosition_And_IsInScreen(out vecMousePosition) == false)
            return Vector3.zero;

        RaycastHit pHitInfo;
        bool bIsHit = Physics.Raycast(pCamera.ScreenPointToRay(vecMousePosition), out pHitInfo, Mathf.Infinity, sLayerMask_Hit);

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Ray pRay = pCamera.ScreenPointToRay(vecMousePosition);
            if (bIsHit)
                Debug.DrawRay(pRay.origin, (Vector3)pHitInfo.point - pRay.origin, Color.red, 1f);
            else
                Debug.DrawRay(pRay.origin, pRay.direction * 1000f, Color.green, 1f);
        }

        if (bIsHit)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }


    public Vector3 DoRayCasting_2D(Vector2 vecPos, LayerMask sLayerMask_Hit)
    {
        return DoRayCasting_MousePos_2D(p_pEventCamera, sLayerMask_Hit);
    }

    public Vector3 DoRayCasting_MousePos_2D(LayerMask sLayerMask_Hit)
    {
        return DoRayCasting_MousePos_2D(p_pEventCamera, sLayerMask_Hit);
    }

    public Vector3 DoRayCasting_MousePos_3D(LayerMask sLayerMask_Hit)
    {
        return DoRayCasting_MousePos_3D(p_pEventCamera, sLayerMask_Hit);
    }

    public Vector3 DoRayCasting_MousePos_2D(Camera pCamera, LayerMask sLayerMask_Hit)
    {
        if (pCamera == null)
            return Vector3.zero;

        Vector2 vecMousePosition;
        if (CalculateMousePosition_And_IsInScreen(out vecMousePosition) == false)
            return Vector3.zero;

        return GetRayCasting_Pos_2D(pCamera, sLayerMask_Hit, vecMousePosition);
    }

    public List<RaycastHit2D> DoRayCasting_2D(LayerMask sLayerMask_Hit)
    {
        _listHit_2D.Clear();

        Vector2 vecMousePosition;
        if (CalculateMousePosition_And_IsInScreen(out vecMousePosition) == false)
            return _listHit_2D;

        int iHitCount = Physics2D.GetRayIntersectionNonAlloc(p_pEventCamera.ScreenPointToRay(vecMousePosition), _arrHit_2D, Mathf.Infinity, sLayerMask_Hit);

        for (int i = 0; i < iHitCount; i++)
            _listHit_2D.Add(_arrHit_2D[i]);

        return _listHit_2D;
    }

    public Vector3 DoRayCasting_MousePos()
    {
        Vector2 vecMousePosition;
        if (CalculateMousePosition_And_IsInScreen(out vecMousePosition) == false)
            return Vector3.zero;

        var pHitInfo = Physics2D.GetRayIntersection(p_pEventCamera.ScreenPointToRay(vecMousePosition), Mathf.Infinity);
        if (pHitInfo)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        p_listLastHit = new List<CRaycastHitWrapper>();
        InitCamera();

        _arrHit_2D = new RaycastHit2D[p_iHitArrayCapapcity];
        _arrHit = new RaycastHit[p_iHitArrayCapapcity];
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        yield return null;

        while (true)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            CalculateInputEvent_OnPC();
#elif UNITY_ANDROID
            CalculateInputEvent_OnMobile();
#endif

            yield return null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pRay_OnClick_ForDebug.origin, pRay_OnClick_ForDebug.origin + (pRay_OnClick_ForDebug.direction * Mathf.Infinity));
        if (p_bIs2D)
        {
            for (int i = 0; i < _iLastHitCount; i++)
            {
                RaycastHit2D pHit = _arrHit_2D[i];
                Gizmos.DrawSphere(pHit.point, 1f);
            }
        }
        else
        {
            for (int i = 0; i < _iLastHitCount; i++)
            {
                RaycastHit pHit = _arrHit[i];
                Gizmos.DrawSphere(pHit.point, 1f);
            }
        }
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private Vector3 GetRayCasting_Pos_2D(Camera pCamera, LayerMask sLayerMask_Hit, Vector2 vecMousePosition)
    {
        Ray pRay = pCamera.ScreenPointToRay(vecMousePosition);
        var pHitInfo = Physics2D.GetRayIntersection(pRay, Mathf.Infinity, sLayerMask_Hit);

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            if (pHitInfo)
                Debug.DrawRay(pRay.origin, (Vector3)pHitInfo.point - pRay.origin, Color.red, 1f);
            else
                Debug.DrawRay(pRay.origin, pRay.direction * 1000f, Color.green, 1f);
        }

        if (pHitInfo)
            return pHitInfo.point;
        else
            return Vector3.zero;
    }

    private void CalculateInputEvent_OnMobile()
    {
        p_listLastHit.Clear();
        _listTransform_EnterNew.Clear();
        _listTransform_ExitEnter.Clear();

        for (int j = 0; j < Input.touchCount; j++)
        {
            Touch pTouch = Input.GetTouch(j);
            bool bIsClick = pTouch.phase == TouchPhase.Ended;

            if (p_bIs2D)
                _iLastHitCount = Physics2D.GetRayIntersectionNonAlloc(p_pEventCamera.ScreenPointToRay(Input.mousePosition), _arrHit_2D, Mathf.Infinity, p_pLayerMask_Hit.value);
            else
                _iLastHitCount = Physics.RaycastNonAlloc(p_pEventCamera.ScreenPointToRay(Input.mousePosition), _arrHit, Mathf.Infinity, p_pLayerMask_Hit.value);

            for (int i = 0; i < _iLastHitCount; i++)
            {
                CRaycastHitWrapper pHit = _arrHit[i];
                p_listLastHit.Add(pHit);
                Transform pTransformHit = pHit.transform;

                if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                    Debug.Log(pTransformHit.name + " RayCast Hit bMouseClick: " + bIsClick, pTransformHit);

                if (bIsClick)
                {
                    var pClick = pTransformHit.GetComponent<IPointerClickHandler>();
                    if (pClick != null)
                        pClick.OnPointerClick(null);
                }

                _listTransform_EnterNew.Add(pHit.transform);
            }

            for (int i = 0; i < _listTransform_EnterAlready.Count; i++)
            {
                Transform pTransform = _listTransform_EnterAlready[i];
                if (_listTransform_EnterNew.Contains(pTransform))
                    _listTransform_EnterNew.Remove(pTransform);
                else
                    _listTransform_ExitEnter.Add(pTransform);
            }

            for (int i = 0; i < _listTransform_EnterNew.Count; i++)
            {
                var pEnter = _listTransform_EnterNew[i].GetComponent<IPointerEnterHandler>();
                if (pEnter != null)
                    pEnter.OnPointerEnter(null);
            }

            for (int i = 0; i < _listTransform_ExitEnter.Count; i++)
            {
                Transform pTransform = _listTransform_ExitEnter[i];
                _listTransform_EnterAlready.Remove(pTransform);

                var pExit = pTransform.GetComponent<IPointerExitHandler>();
                if (pExit != null)
                    pExit.OnPointerExit(null);

            }

            _listTransform_EnterAlready.AddRange(_listTransform_EnterNew);
        }
    }

    private void CalculateInputEvent_OnPC()
    {
        if (p_pEventCamera == null)
        {
            InitCamera();
            if (p_pEventCamera == null)
                return;
        }

        bool bIsClick = Input.GetMouseButtonUp(0);
        pRay_OnClick_ForDebug = p_pEventCamera.ScreenPointToRay(Input.mousePosition);
        if (p_bIs2D)
            _iLastHitCount = Physics2D.GetRayIntersectionNonAlloc(pRay_OnClick_ForDebug, _arrHit_2D, Mathf.Infinity, p_pLayerMask_Hit.value);
        else
            _iLastHitCount = Physics.RaycastNonAlloc(pRay_OnClick_ForDebug, _arrHit, Mathf.Infinity, p_pLayerMask_Hit.value);

        p_listLastHit.Clear();
        _listTransform_EnterNew.Clear();
        _listTransform_ExitEnter.Clear();
        for (int i = 0; i < _iLastHitCount; i++)
        {
            CRaycastHitWrapper pHit = _arrHit[i];
            if (pHit.transform == null)
                continue;

            p_listLastHit.Add(pHit);
            Transform pTransformHit = pHit.transform;

            if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.Log(pTransformHit.name + " RayCast Hit bMouseClick: " + bIsClick, pTransformHit);

            if (bIsClick)
            {
                var pClick = pTransformHit.GetComponent<IPointerClickHandler>();
                if (pClick != null)
                    pClick.OnPointerClick(null);
            }

            _listTransform_EnterNew.Add(pHit.transform);
        }

        for (int i = 0; i < _listTransform_EnterAlready.Count; i++)
        {
            Transform pTransform = _listTransform_EnterAlready[i];
            if (_listTransform_EnterNew.Contains(pTransform))
                _listTransform_EnterNew.Remove(pTransform);
            else
                _listTransform_ExitEnter.Add(pTransform);
        }

        for (int i = 0; i < _listTransform_EnterNew.Count; i++)
        {
            if (_listTransform_EnterNew[i] == null)
                continue;

            var pEnter = _listTransform_EnterNew[i].GetComponent<IPointerEnterHandler>();
            if (pEnter != null)
                pEnter.OnPointerEnter(null);
        }

        for (int i = 0; i < _listTransform_ExitEnter.Count; i++)
        {
            Transform pTransform = _listTransform_ExitEnter[i];
            _listTransform_EnterAlready.Remove(pTransform);

            var pExit = pTransform.GetComponent<IPointerExitHandler>();
            if (pExit != null)
                pExit.OnPointerExit(null);

        }

        _listTransform_EnterAlready.AddRange(_listTransform_EnterNew);
    }

    private void InitCamera()
    {
        if (p_pEventCamera == null)
        {
            Camera[] arrCamera = FindObjectsOfType<Camera>();
            for (int i = 0; i < arrCamera.Length; i++)
            {
                if (arrCamera[i].gameObject.tag == "MainCamera")
                {
                    p_pEventCamera = arrCamera[i];
                    break;
                }
            }

            if (p_pEventCamera == null)
                p_pEventCamera = arrCamera[0];
        }
    }

    private bool CalculateMousePosition_And_IsInScreen(out Vector2 vecMousePosition)
    {
        vecMousePosition = Input.mousePosition;
        return (0f < vecMousePosition.x && vecMousePosition.x < Screen.width && 0f < vecMousePosition.y && vecMousePosition.y < Screen.width);
    }

    #endregion Private
}