#if NGUI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 2:35:02
   Description : 
   Edit Log    : 
   ============================================ */
   
public class CUI_Joystick : CNGUIPanelBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /// <summary>
    /// bool    IsInputJoystick //
    /// Vector3 vecInnerPadDirection //
    /// float   fDistance_InnerPad
    /// </summary>
    public event System.Action<bool, Vector3, float> p_EVENT_Jostick;

    [SerializeField]
    private Transform _pTrans_InnerPad = null;
    [SerializeField]
    private float _fDistance_Limit = 0.2f;
    [SerializeField]
    private float _fDistance_Current = 0.0f;

    /* private - Field declaration           */

    private Camera _pUICam = null;
    private UICamera.MouseOrTouch _pCurrentTouch;

    private Vector3 _vecDirectionCurrent = new Vector3(0, 0, 0);
    private bool _bIsPress = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        UIRoot pRoot = GetComponentInParent<UIRoot>();
        if(pRoot == null)
        {
            Debug.LogWarning(name + "Joystick은 UIRoot 자식에 있어야 합니다.", this);
            return;
        }

		if(GetComponent<Collider>() == null && GetComponent<Collider2D>() == null)
		{
			Debug.LogWarning( name + "Joystick은 Collider가 있어야 합니다.", this );
			return;
		}

		_pUICam = pRoot.GetComponentInChildren<Camera>();
    }

	private void FixedUpdate()
	{

	//}



	//protected override void OnUpdate()
 //   {
 //       base.OnUpdate();

        Vector3 vecPos_Background = _pTransformCached.position;
        bool bInputJoystick = _bIsPress && _pCurrentTouch != null;
        if (bInputJoystick)
        {
            // Touch 코드
            Vector3 vecPos_Touch = _pCurrentTouch.pos;
            Vector3 vecPos_Pad = _pUICam.ScreenToWorldPoint(new Vector3(vecPos_Touch.x, vecPos_Touch.y, 0));

            // 최대 Touch 거리를 넘어섰을 경우 최대거리로 제한
            if ((vecPos_Pad - vecPos_Background).magnitude > _fDistance_Limit)
                vecPos_Pad = vecPos_Background + (vecPos_Pad - vecPos_Background).normalized * _fDistance_Limit;

            _pTrans_InnerPad.position = vecPos_Pad;
        }
        else
            _pTrans_InnerPad.position = _pTransformCached.position;

        // 방향벡터 얻기
        Vector3 vecDirection = _pTrans_InnerPad.position - vecPos_Background;
        _fDistance_Current = vecDirection.magnitude;

        vecDirection.z = vecDirection.y;
        vecDirection.y = 0f;
        _vecDirectionCurrent = vecDirection.normalized;

        if (p_EVENT_Jostick != null)
            p_EVENT_Jostick(bInputJoystick, _vecDirectionCurrent, _fDistance_Current / _fDistance_Limit);
    }

    protected override void OnUIPress(bool bPress)
    {
        base.OnUIPress(bPress);

        // 패드가 눌렸을 경우 터치의 충돌여부 판단과 충돌된 터치번호를 얻음
        _bIsPress = bPress;
        if (_bIsPress == false)
        {
            _pCurrentTouch = null;
            return;
        }

        _pCurrentTouch = UICamera.currentTouch;
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif