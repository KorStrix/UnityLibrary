using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-27 오전 6:19:28
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIInGame_Ray : CUIInGameBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoInitRay(Transform pTarget)
    {
        _pTargetTransform = pTarget;

        gameObject.SetActive(true);
        StartCoroutine(CoSyncToTarget_Position());
        StartCoroutine(CoSyncToTarget_Rotation());
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

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif