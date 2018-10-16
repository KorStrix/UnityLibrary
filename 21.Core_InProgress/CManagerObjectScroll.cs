#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CManagerObjectScroll : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	public Camera p_pCameraMain;
	public Transform p_pTransTarget;
	[Range(-1f, 1f)]
	public float p_fScrollingSpeed = 1f;
	public float p_fGapOffset = 0f;
	public float p_fStartPosOffset = 0f;

	public int p_iPoolingCount = 3;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private List<CScrollObject> _listScrollObject_Origin = new List<CScrollObject>();
	private LinkedList<CScrollObject> _listScrollObject_Instance = new LinkedList<CScrollObject>();

	private CScrollObject _pScrollObject_Old;
	private CScrollObject _pScrollObject_Last;

	//private float _fTargetPos_Last;
	private float _fTargetPos_Origin;

	private float _fObjectWidth_Old;
	private float _fObjectWidth_Last;

	private float _fOriginPosX;
	private float _fCameraWidth;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	static public void DoClearPoolingObject()
	{
		CManagerPooling<string, CScrollObject>.instance.DoPushAll();
	}

	public void DoResetScroll()
	{
		if (p_pCameraMain == null)
			p_pCameraMain = Camera.main;
		// pCameraMain.orthographicSize는 pixelHeight와 같으면 Pixel Per Unit은 1이 된다.
		// PixelHeight와 pCameraMain.orthographicSize의 비율에 pCameraMain.pixelWidth을 곱하면 카메라의 영역이 나온다.
		_fCameraWidth = (p_pCameraMain.pixelWidth * (p_pCameraMain.orthographicSize / p_pCameraMain.pixelHeight) * 2f);

		IEnumerator<CScrollObject> pEnumerator = _listScrollObject_Instance.GetEnumerator();
		while(pEnumerator.MoveNext())
		{
			CManagerPooling<string, CScrollObject>.instance.DoPush( pEnumerator.Current );
		}
		_listScrollObject_Instance.Clear();

		_pScrollObject_Old = null;
		_pScrollObject_Last = null;

		Vector3 vecCurrentPos = transform.position;
		vecCurrentPos.x = _fOriginPosX;
        transform.position = vecCurrentPos;
		
		ProcGenerate_ScrollObject();
	}

	public void DoClear_InvisibleObject()
	{
		if (_pScrollObject_Last == null) return;

		float fTargetPos = p_pTransTarget.position.x;
		if (fTargetPos + _fCameraWidth > (_pScrollObject_Last.transform.position.x - _fObjectWidth_Last) / 2f)
		{
			//Debug.Log( name + _pScrollObject_Last  + "Clear" );
			CManagerPooling<string, CScrollObject>.instance.DoPush( _pScrollObject_Last );
		}
		//Debug.Log( name + " fTargetPos + _fCameraWidth : " + (fTargetPos + _fCameraWidth).ToString() + "[>] _pScrollObject_Last.p_vecPos.x - _fObjectWidth_Last : " + (_pScrollObject_Last.p_vecPos.x - _fObjectWidth_Last) );
	}

	public void DoGenerate_ScrollObject(string strObjectName)
	{
		//CScrollObject pObjectRandom = CManagerRandomTable<CScrollObject>.instance[GetInstanceID()].GetItem_AsMono( strObjectName );
		//ProcPlacement_ScrollObject( pObjectRandom );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
	   자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponentsInChildren( _listScrollObject_Origin );
		if (_listScrollObject_Origin.Count == 0)
		{
			Debug.Log( name + "_listScrollObject.Count == 0", this );
			enabled = false;
			return;
		}

		List<GameObject> listObject = new List<GameObject>();
		for (int i = 0; i < _listScrollObject_Origin.Count; i++)
		{
			if(_listScrollObject_Origin[i].p_fWidth == 0f)
			{
				Debug.LogWarning( name + "Scroll Object의 Width는 0이 될 수 없습니다. 해당 Scroll Object : " + _listScrollObject_Origin[i] .name, _listScrollObject_Origin[i] );
				gameObject.SetActive(false);
				return;
			}

			listObject.Add( _listScrollObject_Origin[i].gameObject );
		}

		//int iInstanceID = GetInstanceID();
		//CManagerRandomTable<CScrollObject>.instance[iInstanceID].DoClearRandomItemTable();
		//CManagerRandomTable<CScrollObject>.instance[iInstanceID].DoAddRandomItem_Range( _listScrollObject_Origin );
		CManagerPooling<string, CScrollObject>.instance.DoInitPoolingObject( listObject );
		CManagerPooling<string, CScrollObject>.instance.DoStartPooling( p_iPoolingCount );

		for (int i = 0; i < _listScrollObject_Origin.Count; i++)
			_listScrollObject_Origin[i].SetActive( false );

		if (p_pTransTarget != null)
			_fTargetPos_Origin = p_pTransTarget.position.x;

		_fOriginPosX = transform.position.x;
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		DoResetScroll();
	}

    public override void OnUpdate(ref bool bCheckUpdateCount)
	{
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (p_pTransTarget == null) return;

		float fTargetPos = p_pTransTarget.position.x;
		//if (_fTargetPos_Last == fTargetPos) return;

		//_fTargetPos_Last = fTargetPos;

		float fMoveTargetGap = _fTargetPos_Origin - fTargetPos;
		float fMoveAmount = _fOriginPosX + fMoveTargetGap * p_fScrollingSpeed;

		Vector3 vecPos = transform.position;
		vecPos.x = fMoveAmount;
        transform.position = vecPos;

		// 타겟의 위치 + 카메라 넓이가
		// 가장 우측의 스크롤 오브젝트의 위치 - 가장 우측의 오브젝트의 넓이보다 크다면
		// 가장 우측에 스크롤 오브젝트 새로 생성
		while (fTargetPos + _fCameraWidth > _pScrollObject_Last.transform.position.x - _fObjectWidth_Last)
		{
			//Debug.Log( "Target Pos : " + fTargetPos + " _fCameraWidth : " + _fCameraWidth + " _pScrollObject_Last.p_vecPos.x : " + _pScrollObject_Last.p_vecPos.x + " _fObjectWidth_Last: " + _fObjectWidth_Last );

			ProcGenerate_ScrollObject();
		}

        if (_pScrollObject_Old == null) return;

		// 타겟의 위치 - 카메라 넓이가
		// 가장 좌측의 스크롤 오브젝트의 위치 + 가장 좌측의 스크롤 오브젝트의 넓이보다 크다면
		// 좌측의 스크롤 오브젝트는 카메라에 비추지 않으므로 사라져야 한다.
		if (fTargetPos - _fCameraWidth > _pScrollObject_Old.transform.position.x + _fObjectWidth_Old)
		{
			// Debug.Log( "Target Pos : " + fTargetPos + " _fCameraWidth : " + _fCameraWidth + " _pScrollObject_Old.p_vecPos.x : " + _pScrollObject_Old.p_vecPos.x + " _fObjectWidth_Old : " + _fObjectWidth_Old );

			ProcDisable_ScrollObject();
		}
	}

	private void OnDestroy()
	{
		DoClearPoolingObject();
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private void ProcGenerate_ScrollObject()
	{
		//CScrollObject pObjectRandom = CManagerRandomTable<CScrollObject>.instance[GetInstanceID()].GetRandomItem();
		//ProcPlacement_ScrollObject( pObjectRandom );
	}

	private void ProcPlacement_ScrollObject( CScrollObject pObjectRandom )
	{
		CScrollObject pObjectRandomNew = CManagerPooling<string, CScrollObject>.instance.DoPop( pObjectRandom.p_strName );
		pObjectRandomNew.transform.SetParent( transform );

		Vector3 vecPos = pObjectRandom.transform.position;
		if (_pScrollObject_Last != null)
			vecPos.x = _pScrollObject_Last.transform.position.x + _fObjectWidth_Last + (pObjectRandomNew.p_fWidth * 0.5f) + p_fGapOffset;
		else
			vecPos.x = -((pObjectRandomNew.p_fWidth * 0.5f) + p_fGapOffset) + p_fStartPosOffset;

		// Debug.Log( "ProcGenerate_ScrollObject vecPos : " + vecPos + pObjectRandomNew.name, pObjectRandomNew );

		pObjectRandomNew.transform.position = vecPos;

		_fObjectWidth_Last = pObjectRandomNew.p_fWidth * 0.5f;
		_pScrollObject_Last = pObjectRandomNew;
		_listScrollObject_Instance.AddLast( pObjectRandomNew );

		if (_pScrollObject_Old == null)
			ProcUpdate_ScrollObject_Old();
	}

	private void ProcDisable_ScrollObject()
	{
		// Debug.Log( "ProcDisable_ScrollObject : " + _pScrollObject_Old.name, _pScrollObject_Old );
		
		CManagerPooling<string, CScrollObject>.instance.DoPush( _pScrollObject_Old );
		_listScrollObject_Instance.RemoveFirst();

		ProcUpdate_ScrollObject_Old();
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private void ProcUpdate_ScrollObject_Old()
	{
		_pScrollObject_Old = _listScrollObject_Instance.First.Value;
		_fObjectWidth_Old = _pScrollObject_Old.p_fWidth * 0.5f;

		// Debug.Log( "ProcUpdate_ScrollObject_Old : " + _pScrollObject_Old.name, _pScrollObject_Old );
	}

	#endregion Private
}
