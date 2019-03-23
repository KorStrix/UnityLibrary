#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-21 오후 3:01:23
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECharacterControllerState
{
    Standing,
    Walking,
    Running,

    Crouching,
    CrouchWalking,

    Slope_Sliding,
    Wall_Sliding,

    Jumping,
    MultipleJumping,

    Falling,

    LedgeGrab,

    Ladder_Hold,
    Ladder_Climbing,
}

public interface ICharacterController_Listener
{
    void ICharacterController_Listener_OnChangeState(ECharacterControllerState eStatePrev, ECharacterControllerState eStateChanged);
    void ICharacterController_Listener_OnChangeFaceDir(bool bDirection_IsRight, bool bIsSlopSliding, float fSlopeAngleSigned);
}

