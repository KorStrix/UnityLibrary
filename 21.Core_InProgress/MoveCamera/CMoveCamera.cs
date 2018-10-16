#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자    : 
 *	작성자	   : KJH
 *	만든 날짜 : 2017-9-29 17:04:41
 *	
 *	기능     : 카메라 움직임, 확대
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CCompoEventTouch))]
public class CMoveCamera : CObjectBase
{
	/* const & readonly declaration             */

	private const int const_iMaxArrayMoveVelocity = 4;
	private const int const_iMaxArrayMoveVelMinusOne = const_iMaxArrayMoveVelocity - 1;

	/* enum & struct declaration                */

	public enum ECameraAxis
	{
		XY_2D,
		XZ_3D
	}

	#region Field
	/* public - Field declaration            */

	[Header("카메라 이동좌표")]
	[SerializeField] private ECameraAxis p_eCameraAxis = ECameraAxis.XY_2D;

	[Header("카메라 이동제한 (수평)")]
	[SerializeField] private int p_iLimitMoveHorizontal = 720;

	[Header("카메라 이동제한 (수직)")]
	[SerializeField] private int p_iLimitMoveVertical = 1280;

	[Header("카메라 줌 (최소)")]
	[SerializeField] private int p_iCameraZoomMin = 5;

	[Header("카메라 줌 (최대)")]
	[SerializeField] private int p_iCameraZoomMax = 10;

	[Header("카메라 줌 부드러움 수치")]
	[SerializeField] private float p_fZoomSmoothFactor = 5f;

	[Header("카메라 이동 부드러움 수치")]
	[SerializeField] private float p_fCameraMoveSmoothFactor = 10f;

	[Header("드래그가 끝난 후 멈추는 속도배율")]
	[SerializeField] private float p_fDragStoppedMultiplier = 5f;

	[Header("Plane 높이")]
	[SerializeField] private int p_iPlaneHeight = 0;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private CCompoEventTouch _pEventTouch;

	private Camera _pCamera;
	private Transform _pTransCamera;

	private Plane _sPlaneCurrentAxis;
	private Vector3 _v3StartCameraPos;
	private Vector3 _v3PrevCameraPos;
	private Vector3 _v3LastDragVelocity;

	private Vector3[] _arrMoveVelocity = new Vector3[const_iMaxArrayMoveVelocity];

	private int _iAddIndex;

	private float _fLastDragStoppedTime;

	//private float _fStartPinchDistance;
	private float _fStartPinchZoomSize;
	private float _fLerpPinchDistance;

	private bool _bIsCameraOrtho;

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

	private void OnStartDrag(Vector3 v3StartTouchPos)
	{
		ProcResetMoveVelocity();

		_v3StartCameraPos = _pTransCamera.position;
		_v3PrevCameraPos = _v3StartCameraPos;

		_v3LastDragVelocity = Vector3.zero;
	}

	private void OnUpdateDrag(Vector3 v3StartTouchPos, Vector3 v3CurrentTouchPos)
	{
		// Ray 의 첫번째 지점. (터치한 첫번째 좌표 부분부터)
		Vector3 v3IntersectPosOne = PrimitiveHelper.GetPlaneRaycastPos(_sPlaneCurrentAxis, _pCamera.ScreenPointToRay(v3StartTouchPos));
		// Ray 의 두번째 지점. (움직이는 두번째 좌표 까지)
		Vector3 v3IntersectPosTwo = PrimitiveHelper.GetPlaneRaycastPos(_sPlaneCurrentAxis, _pCamera.ScreenPointToRay(v3CurrentTouchPos));

		// 두개의 좌표를 계산 - 마지막 카메라 좌표까지
		Vector3 v3DragOffset = _v3StartCameraPos - (v3IntersectPosTwo - v3IntersectPosOne);
		if (p_fCameraMoveSmoothFactor > 0f)
			_pTransCamera.position = Vector3.Lerp(_pTransCamera.position, v3DragOffset, Time.deltaTime * p_fCameraMoveSmoothFactor);
		else
			_pTransCamera.position = v3DragOffset;

		_pTransCamera.position = ProcClampCamPosition(_pTransCamera.position);

		// 매 프레임마다 이전좌표 현재좌표 계산후 저장 (드래그 끝난 후 자동 드래그때문)
		ProcAddMoveVelocity((_v3PrevCameraPos - _pTransCamera.position) / Time.deltaTime);

		_v3PrevCameraPos = _pTransCamera.position;
	}

	private void OnFinishDrag(Vector3 v3LastTouchPos)
	{
		_v3LastDragVelocity = CalcAverageDragVelocity();
		_fLastDragStoppedTime = Time.time;

		OnUpdate(); // 때고나서 한번 살짝 끊기는현상 방지
    }

    private void OnStartPinch(float fDistance)
	{
		_fLerpPinchDistance = fDistance;
		//_fStartPinchDistance = fDistance;

		if (_bIsCameraOrtho)
			_fStartPinchZoomSize = _pCamera.orthographicSize;
		else
		{
			Vector3 v3CenterRayPos = PrimitiveHelper.GetPlaneRaycastPos(_sPlaneCurrentAxis, _pCamera.ScreenPointToRay(PrimitiveHelper.GetCenterByResolution()));
			float fCameraToPlaneDistance = (v3CenterRayPos - _pTransCamera.position).magnitude;

			_fStartPinchZoomSize = fCameraToPlaneDistance;
		}
	}

	private void OnUpdatePinch(float fDistance)
	{
		float fZoomAverage = PrimitiveHelper.GetCalcReverseFloat(_fStartPinchZoomSize, fDistance);
		fZoomAverage = Mathf.Clamp(fZoomAverage, p_iCameraZoomMin, p_iCameraZoomMax);

		if (_bIsCameraOrtho)
			_pCamera.orthographicSize = fZoomAverage;
		else
		{
			// 터치 2개 중앙부터 Plane 까지 쏜 좌표값 구하기
			Vector3 v3CenterRayPos = PrimitiveHelper.GetPlaneRaycastPos(_sPlaneCurrentAxis, _pCamera.ScreenPointToRay(PrimitiveHelper.GetCenterByResolution()));

			_pTransCamera.position = v3CenterRayPos - _pTransCamera.forward * fZoomAverage;
		}

		if (p_fZoomSmoothFactor > 0f)
			_fLerpPinchDistance = Mathf.Lerp(_fLerpPinchDistance, fDistance, Time.deltaTime * p_fZoomSmoothFactor);
		else
			_fLerpPinchDistance = fDistance;
	}

    /* protected - Override & Unity API         */

    public override void OnUpdate(ref bool bCheckUpdateCount)
	{
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        // 드래깅 끝났을때 자동 스크롤
        float fSqrMagnitudeAutoDrag = _v3LastDragVelocity.sqrMagnitude;
		if (fSqrMagnitudeAutoDrag > float.Epsilon)
		{
			float fElapsedTime = (Time.time - _fLastDragStoppedTime);
			float fLerpProgress = Mathf.Lerp(0f, 1f, Mathf.Clamp01(fElapsedTime));

			Vector3 v3AutoDrag = _v3LastDragVelocity.normalized * fLerpProgress * p_fDragStoppedMultiplier;
			if (v3AutoDrag.sqrMagnitude < fSqrMagnitudeAutoDrag)
				_v3LastDragVelocity -= v3AutoDrag;
			else
				_v3LastDragVelocity = Vector3.zero;

			_pTransCamera.position = ProcClampCamPosition(_pTransCamera.position - _v3LastDragVelocity * Time.deltaTime);
		}
	}

	protected override void OnAwake() 
	{
		base.OnAwake();

        this.GetComponent(out _pEventTouch);
        this.GetComponent(out _pCamera);

		_pTransCamera = _pCamera.transform;

		_pEventTouch.p_EVENT_OnStartDrag += OnStartDrag;
		_pEventTouch.p_EVENT_OnUpdateDrag += OnUpdateDrag;
		_pEventTouch.p_EVENT_OnFinishDrag += OnFinishDrag;

		_pEventTouch.p_EVENT_OnStartPinch += OnStartPinch;
		_pEventTouch.p_EVENT_OnUpdatePinch += OnUpdatePinch;
		// Plane ( normal, height )
		// Plane 은 normal 좌표 방향 기준으로 무한임, 카메라 움직임의 경우 카메라가 바라보는
		// 첫번째 지점부터 레이를 쏴서 Plane 을 맞춘후 그 거리 및 좌표를 확인하는 용도.
		_sPlaneCurrentAxis = (p_eCameraAxis == ECameraAxis.XY_2D)
							 ? new Plane(Vector3.back, p_iPlaneHeight) // 2D 좌표 기준으로 Plane 이 뒤쪽으로
							 : new Plane(Vector3.up, -p_iPlaneHeight); // 3D 좌표 기준으로 Plane 이 아래쪽으로

		_bIsCameraOrtho = _pCamera.orthographic;
	}

	#endregion Protected

	// ===========================ww=============================================== //

	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcAddMoveVelocity(Vector3 v3Velocity)
	{
		// 6 > 5 -> 5 = velocity
		if (_iAddIndex > const_iMaxArrayMoveVelMinusOne)
			ProcRemoveAtMoveVelocity(0); // 5 -> 4, 5 = Vector.zero

		_arrMoveVelocity[_iAddIndex] = v3Velocity;
		_iAddIndex++; // 6
	}

	private void ProcRemoveAtMoveVelocity(int iRemoveID)
	{
		for (int i = iRemoveID; i < const_iMaxArrayMoveVelMinusOne; i++)
			_arrMoveVelocity[i] = _arrMoveVelocity[i + 1];

		_arrMoveVelocity[const_iMaxArrayMoveVelMinusOne] = Vector3.zero;

		_iAddIndex--;
	}

	private void ProcResetMoveVelocity()
	{
		for (int i = 0; i < const_iMaxArrayMoveVelocity; i++)
			_arrMoveVelocity[i] = Vector3.zero;

		_iAddIndex = 0;
	}

	private Vector3 ProcClampCamPosition(Vector3 v3CurrentPos)
	{
		if (p_eCameraAxis == ECameraAxis.XY_2D)
		{
			v3CurrentPos.x = Mathf.Clamp(v3CurrentPos.x, -p_iLimitMoveHorizontal, p_iLimitMoveHorizontal);
			v3CurrentPos.y = Mathf.Clamp(v3CurrentPos.y, -p_iLimitMoveVertical, p_iLimitMoveVertical);
		}
		else
		{
			v3CurrentPos.x = Mathf.Clamp(v3CurrentPos.x, -p_iLimitMoveHorizontal, p_iLimitMoveHorizontal);
			v3CurrentPos.z = Mathf.Clamp(v3CurrentPos.z, -p_iLimitMoveVertical, p_iLimitMoveVertical);
		}

		return v3CurrentPos;
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private Vector3 CalcAverageDragVelocity()
	{
		Vector3 v3CalcVelocity = Vector3.zero;
		for (int i = 0; i < const_iMaxArrayMoveVelMinusOne; i++)
		{
			// 배열에 있는 모든 Velocity 를 더한다.
			v3CalcVelocity += _arrMoveVelocity[i];
		}

		// 그리고 총 개수로 평균 Velocity 구하기
		v3CalcVelocity /= const_iMaxArrayMoveVelocity;

		return v3CalcVelocity;
	}


#endregion Private
}
