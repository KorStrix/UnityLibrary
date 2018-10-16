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

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CCharacterController2D_UserControl : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public bool p_bMoveIsLock { get; protected set; }

    /* protected & private - Field declaration         */

    [GetComponent]
    private CCharacterController2D _pCharacterController = null;
    private bool _bIsJump;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_MoveIsLock(bool bLock)
    {
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

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (!_bIsJump)
        {
            // Read the jump input in Update so button presses aren't missed.
            _bIsJump = Input.GetButtonDown("Jump");
        }

    }

    private void FixedUpdate()
    {
        // Read the inputs.
        bool bIsCrouch = Input.GetKey(KeyCode.S);
        bool bIsRunning = Input.GetKey(KeyCode.LeftShift);
        float fHorizontal = Input.GetAxis("Horizontal");
        if (p_bMoveIsLock)
            _pCharacterController.DoMove(0f, false, false, false);
        else
            _pCharacterController.DoMove(fHorizontal, bIsCrouch, bIsRunning, _bIsJump);

        _bIsJump = false;
    }
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