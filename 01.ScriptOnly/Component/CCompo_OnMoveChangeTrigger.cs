
#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-07 오후 1:06:17
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
public class CCompo_OnMoveChangeTrigger : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [Rename_Inspector("업데이트 시 타임 델타")]
    public float p_fUpdateTimeDelta = 0.02f;

    public UnityEngine.Events.UnityEvent p_listEvent = new UnityEvent();

    /* protected & private - Field declaration         */

    Vector3 _vecPosPrev;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        yield return null;

        while (true)
        {
            Vector3 vecCurrentPos = transform.position;
            if (vecCurrentPos.Equals(_vecPosPrev) == false)
            {
                if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                    Debug.Log(name + ConsoleProWrapper.ConvertLog_ToCore(name + " Excute Trigger "), this);

                p_listEvent.Invoke();
            }

            _vecPosPrev = vecCurrentPos;
            yield return new WaitForSeconds(p_fUpdateTimeDelta);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}