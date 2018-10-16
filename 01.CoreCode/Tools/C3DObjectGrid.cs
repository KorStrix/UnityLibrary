using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-09 오후 9:54:07
   Description : 
   Edit Log    : 
   ============================================ */

[ExecuteInEditMode]
public class C3DObjectGrid : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum ECircleOption
	{
		None,
		Rotate_Circle,
		Rotate_Circle_Inverse_Y,
		Rotate_Circle_Inverse_Z,
	}

	/* public - Variable declaration            */

    [Rename_Inspector("포지션 오프셋")]
	public Vector3 _vecLocalPosOffset = Vector3.zero;

    [Header("원형 옵션 관련")]
    [Rename_Inspector("원형 옵션")]
    public ECircleOption _eCircleOption = ECircleOption.None;
    [Rename_Inspector("원형일 때 회전 값")]
    public Vector3 _vecRotate_OnCircle;
    [Rename_Inspector("원형일 때 위치 값")]
    public Vector3 _vecPos_OnCircle;

    [Rename_Inspector("오브젝트들의 한가운데를 중앙으로 할것인지")]
    public bool p_bPivotIsCenter = false;
    [Rename_Inspector("항상 자동 정렬시킬것인지")]
    public bool p_bUseUpdateSort = false;

    public bool p_bIsUpdateRectTransform = false;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoSetGrid()
	{
		Vector3 vecOffset = Vector3.zero;
		if (p_bPivotIsCenter)
			vecOffset = ((_vecLocalPosOffset * transform.childCount) / 2f) - (_vecLocalPosOffset / 2f);

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform pTransformChild = transform.GetChild( i );
            RectTransform pTransformRect = pTransformChild as RectTransform;

            pTransformChild.localPosition = (_vecLocalPosOffset * i) - vecOffset;
            if(pTransformRect)
                pTransformRect.localPosition = (_vecLocalPosOffset * i) - vecOffset;

            if (_eCircleOption != ECircleOption.None)
			{
				pTransformChild.localRotation = Quaternion.Euler( _vecRotate_OnCircle * i );
				Vector3 vecCurrentLocalPos = pTransformChild.localPosition;

				vecCurrentLocalPos += pTransformChild.forward * _vecPos_OnCircle.z;
				vecCurrentLocalPos += pTransformChild.up * _vecPos_OnCircle.y;
				vecCurrentLocalPos += pTransformChild.right * _vecPos_OnCircle.x;
				pTransformChild.localPosition = vecCurrentLocalPos;
                if (pTransformRect)
                    pTransformRect.localPosition = vecCurrentLocalPos;

                if (_eCircleOption == ECircleOption.Rotate_Circle_Inverse_Y)
				{
					Vector3 vecDirection = transform.position - pTransformChild.position;
					pTransformChild.up = vecDirection.normalized;
				}
				else if (_eCircleOption == ECircleOption.Rotate_Circle_Inverse_Z)
				{
					Vector3 vecDirection = transform.position - pTransformChild.position;
					pTransformChild.forward = vecDirection.normalized;
				}
			}
		}

		if (p_bIsUpdateRectTransform)
		{
			p_bIsUpdateRectTransform = false;
			RectTransform pTransform = GetComponent<RectTransform>();
			if (pTransform == null) return;

			Vector2 vecSizeDelta = pTransform.sizeDelta;
			vecSizeDelta = _vecLocalPosOffset * transform.childCount;

			pTransform.sizeDelta = vecSizeDelta;
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (Application.isPlaying)
		{
			enabled = false;
			//Debug.LogWarning("이 컴포넌트는 Editor 전용이기 때문에 실행시 자동으로 컴포넌트를 삭제합니다." + name, this);
			//DestroyObject(this);
		}

		if (transform.childCount == 0)
		{
			Debug.LogWarning( name + " 자식이 없어서 정렬을 못합니다. 부모 오브젝트에게 붙여주세요", this );
			return;
		}
	}

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isEditor == false) return;
        if (transform.childCount == 0) return;

        DoSetGrid();
    }
#endif

    public override void OnUpdate(ref bool bCheckUpdateCount)
	{
		base.OnUpdate(ref bCheckUpdateCount);

        if (p_bUseUpdateSort == false)
            return;
        if (transform.childCount == 0)
            return;

        bCheckUpdateCount = true;
        DoSetGrid();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
