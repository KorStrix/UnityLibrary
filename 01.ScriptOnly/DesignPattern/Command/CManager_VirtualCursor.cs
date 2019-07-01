#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-29 오후 4:36:00
 *	개요 : 가상의 커서. 게임 내 마우스 커서를 대신합니다.

 ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CManager_VirtualCursor : CSingletonMonoBase<CManager_VirtualCursor>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    [DisplayName("마우스와 가상커서 위치 오프셋")]
    public Vector2 p_vecMousePosOffset;
    [DisplayName("카메라 레이 Hit 레이어")]
    public LayerMask p_pWorldPosition_Z_HitLayer;

    [Space(5)]
    [DisplayName("커서 이동속도")]
    public float p_fMoveSpeed = 10f;

    [DisplayName("마우스 포지션에 따른 업데이트 잠금")]
    public bool p_bIsLock_MousePosition = false;

    /* protected & private - Field declaration         */

    CManagerInputEventSystem _pManagerInputSystem;
    Vector2 vecMousePos_Prev;

    [GetComponentInChildren]
    List<Renderer> _listRenderer = new List<Renderer>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoEnable_Renderer(bool bEnable)
    {
        for (int i = 0; i < _listRenderer.Count; i++)
            _listRenderer[i].enabled = bEnable;
    }

    public void DoMoveCursor(Vector2 vecDirection)
    {
        Vector2 vecNewPos = position + (vecDirection * p_fMoveSpeed);
        Vector3 vecViewPoint = _pManagerInputSystem.p_pEventCamera.WorldToViewportPoint(vecNewPos);

        if (0f > vecViewPoint.x || vecViewPoint.x > 1f ||
           0f > vecViewPoint.y || vecViewPoint.y > 1f)
        {
            vecViewPoint.x = Mathf.Clamp01(vecViewPoint.x);
            vecViewPoint.y = Mathf.Clamp01(vecViewPoint.y);

            vecNewPos = _pManagerInputSystem.p_pEventCamera.ViewportToWorldPoint(vecViewPoint);
        }

        position = vecNewPos;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pManagerInputSystem = CManagerInputEventSystem.instance;
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        if(p_bIsLock_MousePosition == false)
        {
            Vector2 vecMousePosition = _pManagerInputSystem.DoRayCasting_MousePos_2D(p_pWorldPosition_Z_HitLayer);
            if(vecMousePosition != vecMousePos_Prev)
            {
                position = vecMousePosition + p_vecMousePosOffset;
                vecMousePos_Prev = vecMousePosition;
            }
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}