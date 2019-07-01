#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-21 오후 1:50:21
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CCharacterController2D_UserControl : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool p_bMoveIsLock { get; protected set; }
    public bool p_bIsJump { get; private set; }

    /* protected & private - Field declaration         */

    [GetComponent]
    private CCharacterController2D _pCharacterController = null;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_MoveIsLock(bool bLock)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log(ConsoleProWrapper.ConvertLog_ToCore(name + " SetMoveUnlock " + bLock), this);

        p_bMoveIsLock = bLock;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pCharacterController = GetComponent<CCharacterController2D>();
        _pCharacterController.EventOnAwake();
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        if (!p_bIsJump)
        {
            p_bIsJump = Input.GetButtonDown("Jump");
        }

    }

    private void FixedUpdate()
    {
        bool bIsCrouch = Input.GetKey(KeyCode.S);
        bool bIsRunning = Input.GetKey(KeyCode.LeftShift);
        float fHorizontal = Input.GetAxis("Horizontal");
        if (p_bMoveIsLock)
            _pCharacterController.DoMove(0f, false, false, false);
        else
            _pCharacterController.DoMove(fHorizontal, bIsCrouch, bIsRunning, p_bIsJump);

        p_bIsJump = false;
    }
    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}