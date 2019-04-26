#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-04 오후 8:58:26
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public class CTimer : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /// <summary>
    /// Setting Time, Remain Time
    /// </summary>
    public CObserverSubject<CTimer, float, float> p_Event_OnWorkingTimer { get; private set; } = new CObserverSubject<CTimer, float, float>();
    public CObserverSubject<CTimer> p_Event_OnFinishTimer { get; private set; } = new CObserverSubject<CTimer>();

#if ODIN_INSPECTOR
    [ShowInInspector]
    [Rename_Inspector("현재 작동중인지")]
#endif
    public bool p_bIsWorkingTimer { get; private set; } = false;

    [Rename_Inspector("세팅할 시간")]
    public float p_fSettingTime = 10f;
    [Rename_Inspector("루프 유무")]
    public bool p_bIsLoop = true;
    [Rename_Inspector("Enable 시 플레이")]
    public bool p_bIsPlayOnEnable = true;

#if ODIN_INSPECTOR
    [HideInEditorMode]
#endif
    [Space(10)]
    [Rename_Inspector("남은 시간", false)]
    private float p_fRemainTime;

    /* protected & private - Field declaration         */

    string strName_Origin;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoStartTimer()
    {
        DoStartTimer(p_fSettingTime);
    }

    public void DoStartTimer(float fSetTime)
    {
        p_fSettingTime = fSetTime;
        p_fRemainTime = fSetTime;
        DoSetEnable(true);
    }

    public void DoSetEnable(bool bEnable)
    {
        p_bIsWorkingTimer = bEnable;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

#if UNITY_EDITOR
    protected override void OnAwake()
    {
        base.OnAwake();

        strName_Origin = name;
    }
#endif

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if (p_bIsPlayOnEnable)
            DoStartTimer();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(p_bIsWorkingTimer)
        {
            p_fRemainTime -= Time.deltaTime;
            if (p_fRemainTime < 0f)
            {
                p_fRemainTime = 0f;

                p_Event_OnFinishTimer.DoNotify(this);
                DoSetEnable(p_bIsLoop);
                if (p_bIsLoop)
                    DoStartTimer();
            }
            else
                p_Event_OnWorkingTimer.DoNotify(this, p_fSettingTime, p_fRemainTime);
        }

#if UNITY_EDITOR
        if(p_bIsWorkingTimer)
            name = string.Format("{0}_타이머 동작중/{1}/{2}", strName_Origin, p_fRemainTime.ToString("F2"), p_fSettingTime.ToString("F2"));
        else
            name = string.Format("{0}_타이머 멈춤/{1}/{2}", strName_Origin, p_fRemainTime.ToString("F2"), p_fSettingTime.ToString("F2"));
#endif

    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}