using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : parkjonghwa                             
   Date        : 2017-02-09 오후 5:50:51
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(UISprite))]
public class CUIButtonMultiToggle : CNGUIPanelBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
    [System.Serializable]
    public class SButtonToggle {
        public string strSpriteName;
        public List<EventDelegate> listEvent;
    }
    public enum EButtonToggleOption
    {
       ToggleChange,
       ButtonChange
    }
    /* public - Variable declaration            */

    public List<SButtonToggle> listEvent;
    public int iDefaultState;
    public EButtonToggleOption _eButtonToggleOption;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    private UISprite _pUISprite;

    public string _strSpriteCurrent = "Test";
    private int _iState_Current;
    private int _iState_Prev;
 
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
    public void DoChangeToggle()
    {
        CalculateNextIndex();
        ProcUpdateToggle();
    }

    public void DoChangeToggle(int iIndex)
    {
        ProcSetIndex(iIndex);
        ProcUpdateToggle();
    }

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

        _pUISprite = GetComponent<UISprite>();

        ProcSetIndex(iDefaultState);
        _iState_Prev = iDefaultState;
        ProcUpdateToggle();
    }

    protected override void OnUIClick()
    {
        base.OnUIClick();

        DoChangeToggle();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcUpdateToggle()
    {
        if (_eButtonToggleOption == EButtonToggleOption.ButtonChange)
        {
            EventDelegate.Execute(listEvent[_iState_Prev].listEvent);
            _pUISprite.spriteName = listEvent[_iState_Current].strSpriteName;
        }
        else if (_eButtonToggleOption == EButtonToggleOption.ToggleChange)
        {
            if (++_iState_Prev == listEvent.Count)
                _iState_Prev = 0;
            EventDelegate.Execute(listEvent[_iState_Prev].listEvent);
            _pUISprite.spriteName = listEvent[_iState_Current].strSpriteName;
        }
    }
    
    private void CalculateNextIndex()
    {
        _iState_Prev = _iState_Current;

        if (++_iState_Current >= listEvent.Count)
            _iState_Current = 0;
    }

    private void ProcSetIndex(int iIndex)
    {
        if (iIndex > listEvent.Count)
        {
            Debug.LogWarning(string.Format("인덱스 초과 {0}", iIndex), this);
            iIndex = 0;
        }

        _iState_Prev = _iState_Current;
        _iState_Current = iIndex;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}
#endif