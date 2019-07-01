#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-07 오후 5:44:58
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CManagerMouseInput : CSingletonMonoBase<CManagerMouseInput>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public struct SMouseInputState
    {
        public Vector2 vecMousePos { get; private set; }
        public float fRecordTime { get; private set; }

        public SMouseInputState(Vector2 vecMousePos, float fRecordTime)
        {
            this.vecMousePos = vecMousePos;
            this.fRecordTime = fRecordTime;
        }
    }

    /* public - Field declaration            */

    public ObservableCollection<Vector2> p_Event_OnMouseFlick { get; private set; } = new ObservableCollection<Vector2>();

    [Header("감지거리는 스크린 사이즈 기준")]
    [DisplayName("스크린 사이즈", false)]
    [SerializeField]
    Vector2 _vecScreenSize = Vector2.zero;

    [Header("마우스 플릭 감지"), Space(10)]
    [DisplayName("마우스 플릭 감지를 할지")]
    public bool p_bIsCheckFlick = false;
    [DisplayName("플릭 감지 거리")]
    public float p_fDetectDistance_Flick = 100;
    [DisplayName("플릭 감지 초")]
    public float p_fDetectDuration_Flick = 0.075f;

    /* protected & private - Field declaration         */

    CFixedSizeList<SMouseInputState> _listMouseInputState = new CFixedSizeList<SMouseInputState>(500); // 0.02 * 500 = 10초

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override void OnUpdate(float fTimeScale_Individual)
    {
        Vector2 vecMousePos = Input.mousePosition;
        if(0f < vecMousePos.x && vecMousePos.x < Screen.width &&
           0f < vecMousePos.y && vecMousePos.y < Screen.height)
            _listMouseInputState.Add(new SMouseInputState(vecMousePos, Time.unscaledTime));

        if (p_bIsCheckFlick)
            Check_IsMouseFlick();
    }

#if UNITY_EDITOR
    private void Update()
    {
        _vecScreenSize.x = Screen.width;
        _vecScreenSize.y = Screen.height;
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Check_IsMouseFlick()
    {
        if (_listMouseInputState.Count < 2)
            return;

        bool bIsFlick = false;
        float fSqrCheckDistance = p_fDetectDistance_Flick * p_fDetectDistance_Flick;
        Vector2 vecTotalDirection = Vector2.zero;
        float fTime = Time.unscaledTime - _listMouseInputState[_listMouseInputState.Count - 1].fRecordTime;

        for (int i = _listMouseInputState.Count - 2; i > 0; i--)
        {
            SMouseInputState sCurrentInputState = _listMouseInputState[i];
            fTime += (_listMouseInputState[i + 1].fRecordTime - sCurrentInputState.fRecordTime);
            if (fTime > p_fDetectDuration_Flick)
                break;

            vecTotalDirection += (_listMouseInputState[i + 1].vecMousePos - sCurrentInputState.vecMousePos);
            if (vecTotalDirection.sqrMagnitude > fSqrCheckDistance)
            {
                bIsFlick = true;
                break;
            }
        }

        if (bIsFlick)
        {
            p_Event_OnMouseFlick.DoNotify(vecTotalDirection.normalized);

            if(CheckDebugFilter(EDebugFilter.Debug_Level_Core))
                Debug.Log(name + " Mouse Flick Direction : " + vecTotalDirection.normalized);
        }
    }

    #endregion Private
}