#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-27 오전 10:34:20
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EAnimationEvent
{
    None = 0,
    AnimationStart = 1,

    AttackStart = 2,
    AttackFinish = 3,

    AnimationFinish = 4,
}

public delegate void OnPlayAnimation(string strAnimationName);
public delegate void OnFinishAnimation(string strAnimationName, bool bIsInterrupted);
public delegate void OnAnimationEvent(string strAnimationName, string strEventName);

public interface IAnimationController
{
    event OnAnimationEvent p_Event_OnAnimationEvent;


    void DoPlayAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, OnFinishAnimation OnFinishAnimation = null)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

    void DoPlayAnimation_Continuedly<ENUM_ANIMATION_NAME>(System.Action OnFinishAnimationAll, params ENUM_ANIMATION_NAME[] arrAnimName)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

    bool DoPlayAnimation_Loop<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

    void DoPlayAnimation_ForceChange_OnSameAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, OnFinishAnimation OnFinishAnimation = null)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;


    void DoSeekAnimation<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName, float fProgress_0_1);

    void DoStopAnimation();

    void DoResetAnimationEvent();


    bool DoCheckIsPlaying<ENUM_ANIMATION_NAME>(ENUM_ANIMATION_NAME eAnimName)
        where ENUM_ANIMATION_NAME : System.IConvertible, System.IComparable;

    void DoSetAnimationSpeed(float fSpeed);

    string GetCurrentAnimation();

}
