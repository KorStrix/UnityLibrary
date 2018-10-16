using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-27 오전 6:15:23
   Description : 
   Edit Log    : 
   ============================================ */

public class CUIInGameBase : CNGUIPanelBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    static protected Camera g_pCamera_UI;
    static protected Camera g_pCamera_InGame;
    
    protected Transform _pTargetTransform;
    protected Vector3 _vecUIPosOffset;

    /* private - Variable declaration           */
    [SerializeField]
    private bool _bAttach3DObject = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public static void DoInit(Camera pUICamera, Camera pInGameCamera)
    {
        g_pCamera_UI = pUICamera; g_pCamera_InGame = pInGameCamera;
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

    protected IEnumerator CoSyncToTarget_Position()
    {
        while (true)
        {
            if (_bAttach3DObject == false)
            {
                Vector3 v3UIpos = g_pCamera_UI.ViewportToWorldPoint(g_pCamera_InGame.WorldToViewportPoint(_pTargetTransform.position + _vecUIPosOffset));
                _pTransformCached.position = new Vector3(v3UIpos.x, v3UIpos.y, 0);
            }
            else
                _pTransformCached.position = _pTargetTransform.position;

            yield return null;
        }
    }

    protected IEnumerator CoSyncToTarget_Rotation()
    {
        while (true)
        {
            Vector3 vecForward = _pTargetTransform.forward;
            Vector3 vecForwardConvert = new Vector3(vecForward.x, vecForward.z, 0f);
            _pTransformCached.rotation = Quaternion.LookRotation(vecForwardConvert);

            yield return null;
        }
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif