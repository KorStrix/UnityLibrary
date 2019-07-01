#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-03 오후 1:20:34
 *	기능 : RaycastHit을 계산하여 Hit이 되면 Event로 알려줍니다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CRaycastHitCalculator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EUpdateMode
    {
        Update,
        FixedUpdate,
        Menual,
    }

    /* public - Field declaration            */

    [DisplayName("레이 길이")]
    public float p_fHitDistance = 10f;
    [DisplayName("레이 방향")]
    public Vector3 p_vecRayDirection = new Vector3(0f, 1f, 0f);
    [DisplayName("히트 체크할 LayerMask")]
    public LayerMask p_pLayerMask;
    [DisplayName("업데이트 모드")]
    public EUpdateMode p_eUpdateMode = EUpdateMode.Update;
    [DisplayName("Hit Capacity")]
    [SerializeField]
    private int _iHitCapacity = 10;
    [DisplayName("2D ?")]
    public bool p_bIs2D = false;
    [DisplayName("Check Trigger")]
    public bool p_bIncludeTrigger = false;

    public ObservableCollection<List<RaycastHit>> p_Event_OnHit { get { return _OnHit; } }
    public ObservableCollection<List<RaycastHit2D>> p_Event_OnHit_2D { get { return _OnHit_2D; } }

    /* protected & private - Field declaration         */

    ObservableCollection<List<RaycastHit>> _OnHit = new ObservableCollection<List<RaycastHit>>();
    ObservableCollection<List<RaycastHit2D>> _OnHit_2D = new ObservableCollection<List<RaycastHit2D>>();

    List<RaycastHit> _listHit;
    List<RaycastHit2D> _listHit2D;

    List<Collider> _listCollider_Enter;
    List<Collider> _listCollider_Stay;
    List<Collider> _listCollider_Exit;

    List<Collider2D> _listCollider2D_Enter;
    List<Collider2D> _listCollider2D_Stay;
    List<Collider2D> _listCollider2D_Exit;

    RaycastHit[] _arrHit;
    RaycastHit2D[] _arrHit2D;

    int _iHitCount;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public int DoCalculateRaycast()
    {
        if(p_bIs2D)
        {
            _iHitCount = Physics2D.RaycastNonAlloc(transform.position, transform.rotation * p_vecRayDirection, _arrHit2D, p_fHitDistance, p_pLayerMask);
            _listHit2D.Clear();
            _listCollider2D_Enter.Clear();
            for (int i = 0; i < _iHitCount; i++)
            {
                if (p_bIncludeTrigger == false && _arrHit2D[i].collider.isTrigger == false)
                    continue;

                _listHit2D.Add(_arrHit2D[i]);
                _listCollider2D_Enter.Add(_arrHit2D[i].collider);
            }
            Calculate_Enter_And_ExitCollider(_listCollider2D_Enter, _listCollider2D_Stay, _listCollider2D_Exit);

            if (_iHitCount != 0)
                _OnHit_2D.DoNotify(_listHit2D);
        }
        else
        {
            _iHitCount = Physics.RaycastNonAlloc(transform.position, transform.rotation * p_vecRayDirection, _arrHit, p_fHitDistance, p_pLayerMask);
            _listHit.Clear();
            _listCollider_Enter.Clear();
            for (int i = 0; i < _iHitCount; i++)
            {
                if (p_bIncludeTrigger == false && _arrHit[i].collider.isTrigger == false)
                    continue;

                _listHit.Add(_arrHit[i]);
                _listCollider_Enter.Add(_arrHit[i].collider);
            }
            Calculate_Enter_And_ExitCollider(_listCollider_Enter, _listCollider_Stay, _listCollider_Exit);

            if (_iHitCount != 0)
                _OnHit.DoNotify(_listHit);
        }

        return _iHitCount;
    }

    public List<Collider2D> GetColliderList_2D_Enter() { return _listCollider2D_Enter; }
    public List<Collider2D> GetColliderList_2D_Stay() { return _listCollider2D_Stay; }
    public List<Collider2D> GetColliderList_2D_Exit() { return _listCollider2D_Exit; }

    public List<Collider> GetColliderList_Enter() { return _listCollider_Enter; }
    public List<Collider> GetColliderList_Stay() { return _listCollider_Stay; }
    public List<Collider> GetColliderList_Exit() { return _listCollider_Exit; }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        if (p_bIs2D)
        {
            _arrHit2D = new RaycastHit2D[_iHitCapacity];
            _listHit2D = new List<RaycastHit2D>(_iHitCapacity);

            _listCollider2D_Enter = new List<Collider2D>(_iHitCapacity);
            _listCollider2D_Stay = new List<Collider2D>(_iHitCapacity);
            _listCollider2D_Exit = new List<Collider2D>(_iHitCapacity);
        }
        else
        {
            _arrHit = new RaycastHit[_iHitCapacity];
            _listHit = new List<RaycastHit>(_iHitCapacity);

            _listCollider_Enter = new List<Collider>(_iHitCapacity);
            _listCollider_Stay = new List<Collider>(_iHitCapacity);
            _listCollider_Exit = new List<Collider>(_iHitCapacity);
        }
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        if (p_eUpdateMode == EUpdateMode.Update)
            DoCalculateRaycast();
    }

    private void FixedUpdate()
    {
        if (p_eUpdateMode == EUpdateMode.FixedUpdate)
            DoCalculateRaycast();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;
        
        if(_iHitCount != 0)
        {
            Gizmos.color = Color.red;
            if(p_bIs2D)
                Gizmos.DrawRay(transform.position, (Vector3)_arrHit2D[0].point - transform.position);
            else
                Gizmos.DrawRay(transform.position, _arrHit[0].point - transform.position);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.rotation * p_vecRayDirection * p_fHitDistance);
        }
    }
#endif

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void Calculate_Enter_And_ExitCollider<T>(List<T> list_NewInner, List<T> list_InCollider, List<T> list_Exit)
        where T : UnityEngine.Component
    {
        list_Exit.Clear();

        for (int i = 0; i < list_InCollider.Count; i++)
        {
            var pCollider = list_InCollider[i];
            if (list_NewInner.Contains(pCollider))
                list_NewInner.Remove(pCollider);
            else
            {
                list_InCollider.Remove(pCollider);
                list_Exit.Add(pCollider);
            }
        }

        list_InCollider.AddRange(list_NewInner);
    }

    #endregion Private
}