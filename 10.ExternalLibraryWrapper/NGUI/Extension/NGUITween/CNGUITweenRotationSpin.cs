using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-05-20 오전 8:47:49
   Description : 
   Edit Log    : 
   ============================================ */

public class CNGUITweenRotationSpin : TweenRotation
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    protected override void OnUpdate(float factor, bool isFinished)
    {
        cachedTransform.localRotation = Quaternion.Euler(Vector3.Slerp(from, to, factor));
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif