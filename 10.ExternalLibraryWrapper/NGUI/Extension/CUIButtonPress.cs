using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-06 오후 1:56:09
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(UIButton))]
public class CUIButtonPress : CNGUIPanelBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    public event System.Action<bool> p_EVENT_OnPressOnOff;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private List<EventDelegate> _listOnPress = new List<EventDelegate>();

    private UIButton _pUIButton;
    private bool _bIsPress = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pUIButton = GetComponent<UIButton>();
        _listOnPress = _pUIButton.onClick;
        _pUIButton.onClick = null;
    }

    protected override void OnUIPress(bool bPress)
    {
        base.OnUIPress(bPress);

        _bIsPress = bPress;
    }

    protected override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        if (_bIsPress)
        {
            _pUIButton.SetState(UIButtonColor.State.Pressed, true);
            EventDelegate.Execute(_listOnPress);
        }
        else
            _pUIButton.SetState(UIButtonColor.State.Normal, true);

        if (p_EVENT_OnPressOnOff != null)
            p_EVENT_OnPressOnOff(_bIsPress);
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif