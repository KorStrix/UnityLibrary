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
public class CGridObject : CObjectBase
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

    public enum ERowColumnOption
    {
        None,
        Row,
        Column
    }


	/* public - Variable declaration            */

    [DisplayName("포지션 오프셋")]
	public Vector3 _vecLocalPosOffset = Vector3.zero;

    [Header("행열 옵션 관련")]
    [DisplayName("행열 옵션")]
    public ERowColumnOption p_eRowColumnOption = ERowColumnOption.None;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(_bIsEnable_RowColumn_IsRow))]
#endif
    [DisplayName("행의 개수")]
    public int p_iRowCount = 0;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(_bIsEnable_RowColumn_IsColumn))]
#endif
    [DisplayName("열의 개수")]
    public int p_iColumnCount = 0;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(_bIsEnable_RowColumnOption))]
#endif
    [DisplayName("다음 행열과의 갭")]
    public float p_fNextRowColumnOffset = 0f;


    [Space(10)]
    [Header("원형 옵션 관련")]
    [DisplayName("원형 옵션")]
    public ECircleOption _eCircleOption = ECircleOption.None;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(_bIsEnable_CircleOption))]
#endif
    [DisplayName("원형일 때 회전 값")]
    public Vector3 _vecRotate_OnCircle;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf(nameof(_bIsEnable_CircleOption))]
#endif
    [DisplayName("원형일 때 위치 값")]
    public Vector3 _vecPos_OnCircle;

    [Space(10)]
    [DisplayName("피벗을 중앙으로 할지")]
    public bool p_bPivotIsCenter = false;
    [DisplayName("Update때 항상 정렬 할지")]
    public bool p_bUseUpdateSort = false;

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private bool _bIsEnable_RowColumnOption { get { return p_eRowColumnOption != ERowColumnOption.None; } }
    private bool _bIsEnable_RowColumn_IsRow { get { return p_eRowColumnOption == ERowColumnOption.Row; } }
    private bool _bIsEnable_RowColumn_IsColumn { get { return p_eRowColumnOption == ERowColumnOption.Column; } }
    private bool _bIsEnable_CircleOption { get { return _eCircleOption != ECircleOption.None; } }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
#endif
    public void DoSetGrid()
    {
        Vector3 vecOffset = Vector3.zero;
        if (p_bPivotIsCenter)
            vecOffset = ((_vecLocalPosOffset * transform.childCount) / 2f) - (_vecLocalPosOffset / 2f);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pTransformChild = transform.GetChild(i);
            RectTransform pTransformRect = pTransformChild as RectTransform;

            Vector3 CalculatedPosition = (_vecLocalPosOffset * i) - vecOffset;
            if (pTransformRect)
                pTransformRect.localPosition = CalculatedPosition;
            else
                pTransformChild.localPosition = CalculatedPosition;

            if (_bIsEnable_RowColumnOption)
            {
                Vector3 vecRowColumnOffset = Calculate_RowColumnPosition(i);
                if (pTransformRect)
                    pTransformRect.localPosition += vecRowColumnOffset;
                else
                    pTransformChild.localPosition += vecRowColumnOffset;
            }

            if (_bIsEnable_CircleOption)
                Calculate_CircleOption(i, pTransformChild, pTransformRect);
        }

        CalculateSizeDelta_OnRectTransform();
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isEditor == false) return;
        if (transform.childCount == 0) return;

        DoSetGrid();
    }
#endif

    public override void OnUpdate(float fTimeScale_Individual)
    {
        if (p_bUseUpdateSort == false || transform.childCount == 0)
            return;

        DoSetGrid();
	}

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void Calculate_CircleOption(int iIndex, Transform pTransformChild, RectTransform pTransformRect)
    {
        pTransformChild.localRotation = Quaternion.Euler(_vecRotate_OnCircle * iIndex);
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

    private Vector3 Calculate_RowColumnPosition(int iIndex)
    {
        Vector3 vecRowColumnPosition = Vector3.zero;
        if (_bIsEnable_RowColumn_IsRow)
        {
            vecRowColumnPosition.x = (iIndex / p_iRowCount) * p_fNextRowColumnOffset;
            vecRowColumnPosition.y = -(_vecLocalPosOffset.y * ((iIndex / p_iRowCount) * p_iRowCount));
        }
        else if (_bIsEnable_RowColumn_IsColumn)
        {
            vecRowColumnPosition.x = -(_vecLocalPosOffset.x * ((iIndex / p_iColumnCount) * p_iColumnCount));
            vecRowColumnPosition.y = (iIndex / p_iColumnCount) * p_fNextRowColumnOffset;
        }

        return vecRowColumnPosition;
    }

    private void CalculateSizeDelta_OnRectTransform()
    {
        RectTransform pRectTransform = GetComponent<RectTransform>();
        if (pRectTransform == null)
            return;

        Vector2 vecSizeDelta = pRectTransform.sizeDelta;
        vecSizeDelta = _vecLocalPosOffset * transform.childCount;

        pRectTransform.sizeDelta = vecSizeDelta;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
