#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EStateTouch
{
	None,
	Dragging,
	Pinching
}

public class CCompoEventTouch : CObjectBase
{
	/* const & readonly declaration             */

#if (UNITY_EDITOR || UNITY_STANDALONE)
	private const string const_strMouseScrollWheel = "Mouse ScrollWheel";
#endif

	/* enum & struct declaration                */

	#region Field

	/* public - Field declaration            */

	[Header("드래그가 시작되는 한계치")]
	[SerializeField] private float p_fDragStartDistanceThreshold = 0.1f;

	public event System.Action<Vector3> p_EVENT_OnStartDrag;
	public event System.Action<Vector3, Vector3> p_EVENT_OnUpdateDrag;
	public event System.Action<Vector3> p_EVENT_OnFinishDrag;

	public event System.Action<float> p_EVENT_OnStartPinch;
	public event System.Action<float> p_EVENT_OnUpdatePinch;
	public event System.Action p_EVENT_OnFinishPinch;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private EStateTouch _eStateTouch; public EStateTouch p_eStateTouch { get { return _eStateTouch; } }

	private Vector3 _v3DragStartPos;
	private Vector3 _v3LastTouchOnePos;

#if (UNITY_EDITOR || UNITY_STANDALONE)
	private float _fPinchDistance;
	private float _fLerpPinchDistance;
#endif

    #endregion Field

    #region Public

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    public override void OnUpdate(float fTimeScale_Individual)
    {
        int iTouchCount = 0;

#if (UNITY_EDITOR || UNITY_STANDALONE)

		iTouchCount = Input.GetMouseButton(0) ? 1 : 0;

		_fPinchDistance = Mathf.Clamp(_fPinchDistance + Input.GetAxis(const_strMouseScrollWheel) / 2f, 0.1f, 100f);
		_fLerpPinchDistance = Mathf.Lerp(_fLerpPinchDistance, _fPinchDistance, Time.deltaTime * 20f);

		if (Mathf.Approximately(_fPinchDistance, _fLerpPinchDistance) == false)
		{
			iTouchCount = 2;
		}

#elif (UNITY_ANDROID || UNITY_IOS)

		iTouchCount = Input.touchCount;

#endif

		if (iTouchCount == 1)
		{
			Vector3 v3CurrentTouchOnePos = Vector3.zero;

#if (UNITY_EDITOR || UNITY_STANDALONE)

			v3CurrentTouchOnePos = Input.mousePosition;

#elif (UNITY_ANDROID || UNITY_IOS)

			Touch pTouch = Input.touches[0];
			v3CurrentTouchOnePos = pTouch.position;

#endif
			if (_eStateTouch != EStateTouch.Dragging)
			{
				float fDistByResolution = PrimitiveHelper.GetDistanceByResolution(v3CurrentTouchOnePos, _v3DragStartPos);
				if (fDistByResolution > p_fDragStartDistanceThreshold)
				{
					_eStateTouch = EStateTouch.Dragging;
					_v3DragStartPos = v3CurrentTouchOnePos;

					ProcOnStartDrag();

				}
			}
			else
			{
				ProcOnUpdateDrag(v3CurrentTouchOnePos);
				_v3LastTouchOnePos = v3CurrentTouchOnePos;
			}
		}
		else if (iTouchCount == 2)
		{
			float fPinchDistance = 0f;

#if (UNITY_EDITOR || UNITY_STANDALONE)

			fPinchDistance = _fLerpPinchDistance;

#elif (UNITY_ANDROID || UNITY_IOS)

			Touch pTouchOne = Input.touches[0];
			Touch pTouchTwo = Input.touches[1];

			Vector3 v3TouchPosOne = pTouchOne.position;
			Vector3 v3TouchPosTwo = pTouchTwo.position;
			//Vector3 v3CenterPos = PrimitiveHelper.GetCenterPos(v3TouchPosOne, v3TouchPosTwo);

			fPinchDistance = PrimitiveHelper.GetDistanceByResolutionSqrt(v3TouchPosOne, v3TouchPosTwo);

#endif

			if (_eStateTouch != EStateTouch.Pinching)
			{
				ProcOnStartPinch(fPinchDistance);
				_eStateTouch = EStateTouch.Pinching;
			}

			ProcOnUpdatePinch(fPinchDistance);
		}
		else
		{
			if (_eStateTouch == EStateTouch.Dragging)
				ProcOnFinishDrag();
			else if (_eStateTouch == EStateTouch.Pinching)
				ProcOnFinishPinch();

			if (_eStateTouch != EStateTouch.None)
				_eStateTouch = EStateTouch.None;
		}
	}

#endregion Protected

	// ========================================================================== //

#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcOnStartDrag()
	{
		if (p_EVENT_OnStartDrag != null)
			p_EVENT_OnStartDrag(_v3DragStartPos);
	}

	private void ProcOnUpdateDrag(Vector3 v3CurrentTouchPos)
	{
		if (p_EVENT_OnUpdateDrag != null)
			p_EVENT_OnUpdateDrag(_v3DragStartPos, v3CurrentTouchPos);
	}

	private void ProcOnFinishDrag()
	{
		if (p_EVENT_OnFinishDrag != null)
			p_EVENT_OnFinishDrag(_v3LastTouchOnePos);
	}

	private void ProcOnStartPinch(float fDistance)
	{
		if (p_EVENT_OnStartPinch != null)
			p_EVENT_OnStartPinch(fDistance);
	}

	private void ProcOnUpdatePinch(float fDistance)
	{
		if (p_EVENT_OnUpdatePinch != null)
			p_EVENT_OnUpdatePinch(fDistance);
	}

	private void ProcOnFinishPinch()
	{
		if (p_EVENT_OnFinishPinch != null)
			p_EVENT_OnFinishPinch();
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

#endregion Private
}
