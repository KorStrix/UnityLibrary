#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-18 오후 4:25:47
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

public class CheckVisible_InCamera_Test : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public int iTestCube_Count = 10000;

    public float fRandomPosition_Range_X = 100f;
    public float fRandomPosition_Range_Y = 100f;
    public float fRandomPosition_Range_Z = 100f;

    public LayerMask CheckLayerMask;

    public float fFarDistance_On3D = 100f;

    [Header("2D 세팅")]
    public bool bIs2D = true;
    public Sprite pSprite_For2D;

    /* protected & private - Field declaration         */

    List<Renderer> _listRenderer = new List<Renderer>();
    Dictionary<Renderer, bool> _mapCheckResult = new Dictionary<Renderer, bool>();
    List<Collider2D> _listCollider_2D;
    List<Collider> _listCollider = new List<Collider>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        for (int i = 0; i < iTestCube_Count; i++)
        {
            GameObject pObjectBox = null;
            if (bIs2D)
            {
                pObjectBox = new GameObject();
                SpriteRenderer pSpriteRenderer = pObjectBox.AddComponent<SpriteRenderer>();
                pSpriteRenderer.sprite = pSprite_For2D;
                pObjectBox.AddComponent<BoxCollider2D>();

                pObjectBox.transform.localScale = Vector3.one * Random.Range(5f, 15f);
            }
            else
            {
                pObjectBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            float fX = Random.Range(-fRandomPosition_Range_X, fRandomPosition_Range_X);
            float fY = Random.Range(-fRandomPosition_Range_Y, fRandomPosition_Range_Y);
            float fZ = Random.Range(-fRandomPosition_Range_Z, fRandomPosition_Range_Z);

            pObjectBox.transform.position = new Vector3(fX, fY, fZ);
            pObjectBox.transform.SetParent(transform);

            pObjectBox.AddComponent<CheckVisible_IVisibleListener_Test>();
            pObjectBox.name = i.ToString();

            _listRenderer.Add(pObjectBox.GetComponent<Renderer>());
        }

        yield return YieldManager.GetWaitForSecond(0.5f);
        Update_CheckInVisibleGizmo(false);


        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UpdateGizmo_Original();
                yield return YieldManager.GetWaitForSecond(0.5f);

                Update_CheckInVisibleGizmo(false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UpdateGizmo_Original();
                yield return YieldManager.GetWaitForSecond(0.5f);

                Update_CheckInVisibleGizmo_UsePhysics();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UpdateGizmo_Original();
                yield return YieldManager.GetWaitForSecond(0.5f);

                Update_CheckInVisibleGizmo_UsePhysics_2D();
            }

            yield return null;
        }
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        for(int i = 0; i < _listRenderer.Count; i++)
            DestroyImmediate(_listRenderer[i]);

        _listRenderer.Clear();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Update_CheckInVisibleGizmo(bool bIsUpdate_Frustum)
    {
        CManager_CheckVisible_InCamera pCheckVisible = CManager_CheckVisible_InCamera.instance;
        if (bIsUpdate_Frustum == false)
            pCheckVisible.DoUpdateFrustumPlane();

        _mapCheckResult.Clear();
        for (int i = 0; i < _listRenderer.Count; i++)
        {
            Renderer pRenderer = _listRenderer[i];
            bool bResult = pCheckVisible.DoCheck_IsVisible(pRenderer, bIsUpdate_Frustum);

            _mapCheckResult.Add(pRenderer, bResult);
        }

        Invoke(nameof(UpdateGizmo_CheckResult), 1f);
    }

    private void Update_CheckInVisibleGizmo_UsePhysics()
    {
        Bounds pBound = CManager_CheckVisible_InCamera.instance.p_pCamera.GetBounds_Perspective(fFarDistance_On3D);
        _listCollider_2D = CManager_CheckVisible_InCamera.instance.DoCheck_IsVisible_UsePhysics_2D(pBound, CheckLayerMask);

        Invoke(nameof(UpdateGizmo_Collider_2D), 1f);
    }


    private void Update_CheckInVisibleGizmo_UsePhysics_2D()
    {
        _listCollider_2D = CManager_CheckVisible_InCamera.instance.DoCheck_IsVisible_UsePhysics_2D(CheckLayerMask);

        Invoke(nameof(UpdateGizmo_Collider_2D), 1f);
    }

    private void UpdateGizmo_CheckResult()
    {
        foreach(var pCheckReuslt in _mapCheckResult)
            UpdateGizmo(pCheckReuslt.Key, pCheckReuslt.Value);
    }

    private void UpdateGizmo_Original()
    {
        for (int i = 0; i < _listRenderer.Count; i++)
            _listRenderer[i].material.color = Color.white;
    }

    private void UpdateGizmo_Collider()
    {
        for (int i = 0; i < _listRenderer.Count; i++)
            UpdateGizmo(_listRenderer[i], false);

        for (int i = 0; i < _listCollider.Count; i++)
        {
            Renderer pRenderer = _listCollider[i].GetComponent<Renderer>();
            if (pRenderer != null)
                UpdateGizmo(pRenderer, true);
        }
    }

    private void UpdateGizmo_Collider_2D()
    {
        for (int i = 0; i < _listRenderer.Count; i++)
            UpdateGizmo(_listRenderer[i], false);

        for (int i = 0; i < _listCollider_2D.Count; i++)
        {
            Renderer pRenderer = _listCollider_2D[i].GetComponent<Renderer>();
            if (pRenderer != null)
                UpdateGizmo(pRenderer, true);
        }
    }

    private void UpdateGizmo(Renderer pRenderer, bool bResult)
    {
        pRenderer.material.color = bResult ? Color.green : Color.red;
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test