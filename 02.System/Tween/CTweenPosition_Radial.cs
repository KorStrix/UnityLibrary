#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-30 오후 5:11:32
 *	기능 : 
 *	
 *	차일드 오브젝트를 프리팹을 등록 후,
 *	차일드 오브젝트에 붙일 스크립트에 CTweenPosition_Radial.ITweenPosRadial_Listener를 구현합니다.
 *	차일드 오브젝트의 수가 변경될 경우 Set ChildCount 함수를 사용하거나, ChildCount 필드를 직접 변경합니다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class CTweenPosition_Radial : CTweenBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public interface ITweenPosRadial_Listener
    {
        void ITweenPosRadial_Listener_OnStartTween(int iChildIndex);
    }

    /* public - Field declaration            */

    [DisplayName("카피하여 트윈할 차일드 오브젝트")]
    public GameObject p_iChildPrefab;
    [DisplayName("트윈 진행 각도범위 180이면 12시 기준으로 6시까지")]
    public float p_fRaidalRangeAngle = 360f;
    [DisplayName("트윈 시작 오프셋, 0일 땐 12시방향부터 오른쪽으로")]
    public float p_fRaidalStartAngle = 0f;
    [DisplayName("Radial 개수")]
    public int p_iChildCount = 5;
    [DisplayName("Tween Start")]
    public float p_fDistance_Start = 0f;
    [DisplayName("Tween Dest")]
    public float p_fDistance_Dest = 10f;

    /* protected & private - Field declaration         */

    List<Transform> _listChildEmpty_Instance = new List<Transform>();
    List<Transform> _listChildEmpty_Managing = new List<Transform>();

    List<GameObject> _listChildPrefab_Instance = new List<GameObject>();
    List<GameObject> _listChildPrefab_Managing = new List<GameObject>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSet_ChildCount(int iChildCount)
    {
        p_iChildCount = iChildCount;
    }

    public List<Transform> GetManagingChildren_List()
    {
        return _listChildEmpty_Managing;
    }

    public int GetChildIndex_ClosestDirection(Vector3 vecDirection)
    {
        if (p_iChildCount <= 1)
            return 0;

        int iClosestIndex = 0;
        float fAngleGap = p_fRaidalRangeAngle / p_iChildCount;
        float fClosestAngle = float.MaxValue;
        for (int i = 0; i < p_iChildCount; i++)
        {
            float fAngleDelta = (((i * fAngleGap) + p_fRaidalStartAngle)) * Mathf.Deg2Rad;
            Vector3 vecAngleDirection = new Vector3(Mathf.Sin(fAngleDelta), Mathf.Cos(fAngleDelta), 0f);

            float fAngle = Vector3.Angle(vecDirection, vecAngleDirection);
            if (fAngle < fClosestAngle)
            {
                fClosestAngle = fAngle;
                iClosestIndex = i;
            }
        }

        return iClosestIndex;
    }

    public Transform GetChildTransform_ClosestDirection(Vector3 vecDirection)
    {
        int iChildIndex = GetChildIndex_ClosestDirection(vecDirection);
        return _listChildEmpty_Managing[iChildIndex];
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        UpdateListManagingChild(p_iChildCount);
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        UpdateListManagingChild(p_iChildCount);
    }

    public override void OnEditorButtonClick_SetDestValue_IsCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEditorButtonClick_SetStartValue_IsCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInitTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    public override void OnReleaseTween_EditorOnly()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnSetTarget(GameObject pObjectNewTarget)
    {
    }

    protected override void OnTweenStart(ETweenDirection eTweenDirection)
    {
        base.OnTweenStart(eTweenDirection);

        UpdateListManagingChild(p_iChildCount);
        for(int i = 0; i < _listChildPrefab_Managing.Count; i++)
            _listChildPrefab_Managing[i].SendMessage("ITweenPosRadial_Listener_OnStartTween", i, SendMessageOptions.DontRequireReceiver);
    }

    protected override void OnTween(float fProgress_0_1)
    {
        float fAngleGap = p_fRaidalRangeAngle / p_iChildCount;
        for (int i = 0; i < _listChildEmpty_Managing.Count; i++)
        {
            float fAngleDelta = (((i * fAngleGap) + p_fRaidalStartAngle)) * Mathf.Deg2Rad;
            Transform pTransformChild = _listChildEmpty_Managing[i].transform;

            Vector3 vecDirection = new Vector3(Mathf.Sin(fAngleDelta), Mathf.Cos(fAngleDelta), 0f);
            vecDirection *= p_fDistance_Start * (1f - fProgress_0_1) + p_fDistance_Dest * fProgress_0_1;

            try
            {
                pTransformChild.localPosition = vecDirection;
            }
            catch
            {
                pTransformChild.localPosition = vecDirection;
            }
        }
    }

#if UNITY_EDITOR

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _listChildEmpty_Managing.Count; i++)
                Gizmos.DrawSphere(_listChildEmpty_Managing[i].position, 1f);
        }
    }

#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void UpdateListManagingChild(int iChildCount)
    {
        if (_listChildEmpty_Managing.Count != iChildCount)
        {
            p_iChildCount = iChildCount;

            ManagingChildInstance_EmptyObject(iChildCount);
            ManagingChild_EmptyObject(_listChildEmpty_Instance, _listChildEmpty_Managing, iChildCount);

            if (p_iChildPrefab != null && p_iChildPrefab.gameObject != null)
            {
                ManagingChildInstance_PrefabObject(iChildCount);
                ManagingChild_EmptyObject(_listChildPrefab_Instance, _listChildPrefab_Managing, iChildCount);
            }

            for (int i = 0; i < _listChildPrefab_Managing.Count; i++)
                _listChildPrefab_Managing[i].SetActive(true);

            for (int i = 0; i < _listChildEmpty_Managing.Count; i++)
            {
                _listChildEmpty_Managing[i].name = (i + 1).ToString();
                _listChildEmpty_Managing[i].gameObject.SetActive(true);
            }
        }
    }

    private void ManagingChildInstance_EmptyObject(int iChildCount)
    {
        if (_listChildEmpty_Instance.Count < iChildCount)
        {
            while (_listChildEmpty_Instance.Count < iChildCount)
            {
                GameObject pObjectChild = new GameObject();
                Transform pTransChild = pObjectChild.transform;
                pTransChild.SetParent(transform);

                _listChildEmpty_Instance.Add(pTransChild);
            }
        }
        for (int i = 0; i < _listChildEmpty_Instance.Count; i++)
            _listChildEmpty_Instance[i].gameObject.SetActive(false);

    }

    private void ManagingChildInstance_PrefabObject(int iChildCount)
    {
        if(p_iChildPrefab == null || string.IsNullOrEmpty(p_iChildPrefab.name))
        {
            Debug.LogError("ManagingChildInstance_PrefabObject", this);
            return;
        }

        if (_listChildPrefab_Instance.Count < iChildCount)
        {
            int iLoopCount = 0;
            while (_listChildPrefab_Instance.Count < iChildCount && iLoopCount++ < 100)
            {
                GameObject pObjectChild = Instantiate(p_iChildPrefab);
                _listChildPrefab_Instance.Add(pObjectChild);
                pObjectChild.transform.SetParent(_listChildEmpty_Instance[_listChildPrefab_Instance.Count - 1]);
                pObjectChild.transform.localPosition = Vector3.zero;
            }

            if(iLoopCount >= 100)
            {
                Debug.LogError(name + "ManagingChildInstance_PrefabObject - iLoopCount >= 100", this);
            }
        }
        for (int i = 0; i < _listChildPrefab_Instance.Count; i++)
            _listChildPrefab_Instance[i].SetActive(false);
    }

    private void ManagingChild_EmptyObject<T>(List<T> list_Instance, List<T> list_Managing, int iChildCount)
    {
        if (list_Managing.Count != iChildCount)
        {
            if (list_Managing.Count > iChildCount)
            {
                while (list_Managing.Count > iChildCount)
                {
                    list_Managing.Remove(list_Managing[list_Managing.Count - 1]);
                }
            }

            if (list_Managing.Count < iChildCount)
            {
                list_Managing.Clear();
                for (int i = 0; i < iChildCount; i++)
                    list_Managing.Add(list_Instance[i]);
            }
        }
    }

    #endregion Private
}