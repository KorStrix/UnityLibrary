#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-21 오후 1:48:39
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.U2D;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CCharacterController2D : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EWallSlidingDetect
    {
        None,
        Collider,
        RayCasting,
    }

    public enum EUpdateMode
    {
        Update,
        FixedUpdate,
        Manual
    }

    /* public - Field declaration            */

    public ECharacterControllerState p_ePlatformerState_Current { get; private set; }
    public ECharacterControllerState p_ePlatformerState_Prev { get; private set; }

    public bool p_bFaceDirection_IsRight { get; private set; }
    public bool p_bIsGround { get; private set; }
    public bool p_bIsCrouch { get; private set; }
    public bool p_bIsMoving { get; private set; }
    public bool p_bIsJumping { get; private set; }
    public bool p_bIsSlopeSliding { get; private set; }

    public bool p_bLeftDirection_IsBlocked { get; private set; }
    public bool p_bRightDirection_IsBlocked { get; private set; }

    [SerializeField]
    [Rename_Inspector("디폴트 캐릭터 방향이 오른쪽인지")]
    private bool p_bDefault_FaceDirection_IsRight = false;

    [Header("움직임 옵션")]
    [Rename_Inspector("걷기 속도")]
    public float p_fSpeed_OnWalking = 10f;
    [Rename_Inspector("달리기 속도")]
    public float p_fSpeed_OnRunning = 20f;
    [Rename_Inspector("최대 걷기 속도까지 걸리는 시간")]
    public float p_fTimeToMaxSpeedApex = 0.5f;
    [Rename_Inspector("걷기로 도달하는 최대 Move Delta (1이 달리기)")]
    [UnityEngine.Range(0, 1)]
    public float p_fMaxMoveDelta_OnWalking = 0.5f;
    [Rename_Inspector("바라보는 방향 기준을 인풋이 아닌 속도를 기준으로 할 것인지")]
    public bool p_bLookAtVelocity = false;
    [Rename_Inspector("앞에 막힌 벽을 향해 움직일 때 움직이는 애니메이션을 할것인지")]
    public bool p_bIsPlayAnimation_OnForwardIsBlock = false;


    [Space(10)]
    [Header("제동 옵션")]
    [Rename_Inspector("제동 시 천천히 멈추기 유무")]
    public bool p_bIsSmoothMoveStop = false;
    [Rename_Inspector("제동 시간")]
    public float p_fTimeToStop = 0.1f;


    [Space(10)]
    [Header("공중관련 옵션")]
    [Rename_Inspector("점프 힘")]
    public float p_fJumpForce = 400f; 
    [Rename_Inspector("공중에서 조종 가능한지")]
    public bool p_bAirControl = true;
    [Rename_Inspector("떨어지는 상태까지 도달하는 시간")]
    public float p_fTimeToFalling = 1f;


    [Space(10)]
    [Header("앉기 옵션")]
    [Rename_Inspector("앉기를 사용하는지")]
    public bool p_bUseCrouching = false;
    [Rename_Inspector("앉아있을 때 속도 감소 비율")]
    [UnityEngine.Range(0, 1)]
    public float p_fCrouchSpeed = .36f; 
    [Rename_Inspector("천장 체크범위")]
    public float p_fCeilingRadius = .01f;
    [SerializeField]
    [GetComponentInChildren("Collider_OnCrouch", true, false)]
    [Rename_Inspector("\"Collider_OnCrouch\" 앉았을 때 몸통용 자식 컬라이더")]
    protected BoxCollider2D _pBoxCollider_OnCrouch = null;
    [SerializeField]
    [GetComponentInChildren("ColliderLeft_OnCrouch", true, false)]
    [Rename_Inspector("\"ColliderLeft_OnCrouch\" 앉았을 때 왼벽 체크용 자식 컬라이더")]
    protected BoxCollider2D _pBoxColliderLeft_OnCrouch = null;
    [SerializeField]
    [GetComponentInChildren("ColliderRight_OnCrouch", true, false)]
    [Rename_Inspector("\"ColliderRight_OnCrouch\" 앉았을 때 오른벽 체크용 자식 컬라이더")]
    protected BoxCollider2D _pBoxColliderRight_OnCrouch = null;


    [Space(10)]
    [Header("경사면 슬라이딩 옵션")]
    [Rename_Inspector("경사면 슬라이딩 유무")]
    public bool p_bUseSlopeSliding = false;
    [Rename_Inspector("경사면 발동 각도")]
    public float p_fSlopeAngle = 45f;
    [Rename_Inspector("경사면 슬라이딩 중 점프할 때 추가 속도 (기본 점프힘에 추가 힘)")]
    public Vector2 p_vecSlidingJump = new Vector2(0f, 0f);
    [Rename_Inspector("경사면 슬라이딩 점프 딜레이")]
    public float p_fSlopeSlidingJump_Delay = 0.2f;


    [Space(10)]
    [Header("월 슬라이딩 옵션")]
    [Rename_Inspector("월 슬라이딩 감지 모드")]
    public EWallSlidingDetect p_eWallSlidingDetect = EWallSlidingDetect.None;
    [Rename_Inspector("벽을 향해 점프할 때 추가  속도")]
    public Vector2 p_vecWallJumpClimb = new Vector2(50f, 150f);
    [Rename_Inspector("벽의 반대방향으로 점프할 때 추가  속도")]
    public Vector2 p_vecWallLeap = new Vector2(50f, 120f);
    [Rename_Inspector("월 슬라이딩 시 최대 낙하 속도")]
    public float p_fWallSlideSpeedMax = -50;
    [Rename_Inspector("월 점프 딜레이")]
    public float p_fWallJump_Delay = 0.2f;


    [Space(10)]
    [Header("기타 옵션")]
    [Rename_Inspector("업데이트 모드")]
    public EUpdateMode p_eUpdateMode = EUpdateMode.Update;
    [Rename_Inspector("터레인 레이어")]
    public LayerMask p_sWhatIsGround;


    /* protected & private - Field declaration         */

    [GetComponent]
    protected Rigidbody2D _pRigidbody = null;
    [GetComponent]
    protected BoxCollider2D _pCollider_Body = null;
    [GetComponent]
    protected CircleCollider2D _pCollider_Leg = null;

    [Space(10)]
    [Header("디버그용")]
    [SerializeField]
    [Rename_Inspector("\"CheckCeiling\" - 천장 체크용 자식 트렌스폼 ( 앉기 사용시 )", false)]
    [GetComponentInChildren("CheckCeiling", true, false)]
    protected Transform _pTransform_CeilingCheck = null;

    [SerializeField]
    [Rename_Inspector("\"ColliderLeft\" - 왼벽 체크용 자식 컬라이더", false)]
    [GetComponentInChildren("ColliderLeft", true, false)]
    protected BoxCollider2D _pBoxCollider_LeftCheck = null;
    [SerializeField]
    [Rename_Inspector("\"ColliderRight\" - 오른벽 체크용 자식 컬라이더", false)]
    [GetComponentInChildren("ColliderRight", true, false)]
    protected BoxCollider2D _pBoxCollider_RightCheck = null;
    [SerializeField]
    [Rename_Inspector("현재 언덕 경사면 각도", false)]
    protected float _fSlopeAngle = 0f;
    [SerializeField]
    [Rename_Inspector("현재 언덕 경사면 각도 ( 부호 포함 )", false)]
    protected float _fSlopeAngle_Signed = 0f;

    protected List<Collider2D> _listSideBlock_Wall = new List<Collider2D>();
    protected float p_fMoveVelocity { get; private set; }


    List<ICharacterController_Listener> _listCharacterControllerListener = new List<ICharacterController_Listener>();

    BoxCollider2D _pBoxCollider_LeftCheck_Origin = null;
    BoxCollider2D _pBoxCollider_RightCheck_Origin = null;

    Coroutine _pCoroutine_Falling;
    Coroutine _pCoroutine_SetLockMove_IsFalse;

    Vector2 _vecOriginCollider_Offset;
    Vector2 _vecOriginCollider_Size;

    Vector2 _vecJumpAddForce;
    Vector2 _vecAddForce_Custom;

    Vector2 _vecCurrentSlopeNormal;
    Queue<Vector2> _queueVelocityPrev = new Queue<Vector2>();

    // float _fJumpVelocity;
    float _fMoveDelta_0_1;
    float _fWaitSecond;
    float _fTime_WallSlidingStart = -1f;

    bool _bCheckIsFalling;
    bool _bIsFalling;
    bool _bIsWallSliding;
    bool _bIsLockMove;

    bool _bInputAddForce_Custom;
    bool _bInputAddForce_Custom_Velocity_IsZero;
    bool _bInputJump;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoUpdate_CharacterController()
    {
        Logic_CharacterControll();
    }

    public void DoSetMoveLock(bool bMoveLock)
    {
        _bIsLockMove = bMoveLock;
    }

    public void DoSet_ChangeFaceDir(bool bDirection_IsRight)
    {
        p_bFaceDirection_IsRight = bDirection_IsRight;
        for (int i = 0; i < _listCharacterControllerListener.Count; i++)
            _listCharacterControllerListener[i].ICharacterController_Listener_OnChangeFaceDir(bDirection_IsRight, p_bIsSlopeSliding, _fSlopeAngle_Signed);
    }

    public void DoAddForce_Custom(Vector2 vecAddForce, float fMoveLockSecond)
    {
        _bInputAddForce_Custom = true;
        _bInputAddForce_Custom_Velocity_IsZero = false;
        _vecAddForce_Custom = vecAddForce;

        if (_fWaitSecond < fMoveLockSecond)
        {
            if (_pCoroutine_SetLockMove_IsFalse != null)
                StopCoroutine(_pCoroutine_SetLockMove_IsFalse);
            _pCoroutine_SetLockMove_IsFalse = StartCoroutine(CoDelaySetLockMove_IsFalse(fMoveLockSecond));
        }
    }

    public void DoAddForce_Custom_Velocity_IsZero(Vector2 vecAddForce, float fMoveLockSecond)
    {
        DoAddForce_Custom(vecAddForce, fMoveLockSecond);
        _bInputAddForce_Custom_Velocity_IsZero = true;
    }

    public void DoSetFalling(bool bFalling)
    {
        _bIsFalling = bFalling;
    }

    public void DoAdd_ControllerListener(ICharacterController_Listener pListener)
    {
        if (_listCharacterControllerListener.Contains(pListener))
            return;

        _listCharacterControllerListener.Add(pListener);
    }

    public void DoRemove_ControllerListener(ICharacterController_Listener pListener)
    {
        _listCharacterControllerListener.Remove(pListener);
    }

    public void DoMove(float fMoveAmount, bool bInput_Run)
    {
        DoMove(fMoveAmount, false, bInput_Run, false);
    }

    public void DoMove(float fMoveAmount, bool bInput_Crouch, bool bInput_Run, bool bInput_Jump)
    {
        // If crouching, check to see if the character can stand up
        if(p_bUseCrouching)
        {
            if (!bInput_Crouch && p_bIsCrouch)
                bInput_Crouch = DoCheck_IsBlock_Ceiling();
        }

        if (_bIsLockMove)
        {
            p_fMoveVelocity = 0f;
        }
        else if (p_bIsGround || p_bAirControl)
        {
            p_fMoveVelocity = (bInput_Crouch ? fMoveAmount * p_fCrouchSpeed : fMoveAmount);

            if(p_bLookAtVelocity)
            {
                if (p_fMoveVelocity > 0)
                    DoSet_ChangeFaceDir(true);
                else if(p_fMoveVelocity < 0)
                    DoSet_ChangeFaceDir(false);
            }
            else
            {
                if ((p_fMoveVelocity > 0 && p_bFaceDirection_IsRight == false) ||
                    p_fMoveVelocity < 0 && p_bFaceDirection_IsRight)
                {
                    DoSet_ChangeFaceDir(!p_bFaceDirection_IsRight);
                }
            }
        }

        // 월 슬라이딩은 인풋을 벽 반대방향으로 누를 경우 바로 false가 되기 때문에,
        // 시간을 체크하여 월 슬라이딩을 체크한지 한 프레임이 지나지 않았을 때도 허용
        bool bCheckIsWallSliding = _bIsWallSliding || _fTime_WallSlidingStart + 0.15f > Time.time;
        if ((bCheckIsWallSliding || p_bIsGround) && p_bIsJumping == false && bInput_Jump && DoCheck_IsBlock_Ceiling() == false)
        {
            if (bCheckIsWallSliding)
                CalculateJump_OnWallSliding(out bInput_Jump);
            else if (p_bIsSlopeSliding)
                CalculateJump_OnSlopeSliding(out bInput_Jump);
            else
                _vecJumpAddForce.y = p_fJumpForce;

            _bInputJump = bInput_Jump;
            p_bIsJumping = bInput_Jump;

            if (bInput_Jump)
                OnAirborne();
        }

        if (p_bIsMoving && bInput_Run && p_bIsCrouch == false)
            _fMoveDelta_0_1 = 1f;
        
        p_bIsCrouch = bInput_Crouch;
        p_bIsMoving = p_fMoveVelocity != 0f;

        if (_pBoxCollider_OnCrouch)
        {
            if (p_bIsCrouch && p_bIsGround)
            {
                _pCollider_Body.offset = _pBoxCollider_OnCrouch.offset;
                _pCollider_Body.size = _pBoxCollider_OnCrouch.size;

                if (_pBoxColliderLeft_OnCrouch)
                    _pBoxCollider_LeftCheck = _pBoxColliderLeft_OnCrouch;

                if (_pBoxColliderRight_OnCrouch)
                    _pBoxCollider_RightCheck = _pBoxColliderRight_OnCrouch;
            }
            else
            {
                _pCollider_Body.offset = _vecOriginCollider_Offset;
                _pCollider_Body.size = _vecOriginCollider_Size;

                if (_pBoxColliderLeft_OnCrouch)
                    _pBoxCollider_LeftCheck = _pBoxCollider_LeftCheck_Origin;

                if (_pBoxColliderRight_OnCrouch)
                    _pBoxCollider_RightCheck = _pBoxCollider_RightCheck_Origin;
            }
        }
    }

    public bool DoCheck_IsBlock_Ceiling()
    {
        return Physics2D.OverlapCircle(_pTransform_CeilingCheck.position, p_fCeilingRadius, p_sWhatIsGround);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        // _fJumpVelocity = UpdateJumpVelocity(p_fJumpForce);
        p_ePlatformerState_Current = (ECharacterControllerState)(-1);

        if (_pBoxCollider_LeftCheck)
        {
            _pBoxCollider_LeftCheck_Origin = _pBoxCollider_LeftCheck;
            _pBoxCollider_LeftCheck.SetActive(false);
        }

        if (_pBoxCollider_RightCheck)
        {
            _pBoxCollider_RightCheck_Origin = _pBoxCollider_RightCheck;
            _pBoxCollider_RightCheck.SetActive(false);
        }

        if (_pBoxCollider_OnCrouch)
            _pBoxCollider_OnCrouch.SetActive(false);

        if (_pBoxColliderLeft_OnCrouch)
            _pBoxColliderLeft_OnCrouch.SetActive(false);

        if (_pBoxColliderRight_OnCrouch)
            _pBoxColliderRight_OnCrouch.SetActive(false);

        _vecOriginCollider_Offset = _pCollider_Body.offset;
        _vecOriginCollider_Size = _pCollider_Body.size;

        DoSet_ChangeFaceDir(p_bDefault_FaceDirection_IsRight);
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;
        Logic_Moving();

        if (p_eUpdateMode == EUpdateMode.Update)
            Logic_CharacterControll();

        if(p_bUseSlopeSliding && _pRigidbody.velocity.y < 0f)
        {
            if (_queueVelocityPrev.Count > 2)
                _queueVelocityPrev.Dequeue();

            _queueVelocityPrev.Enqueue(_pRigidbody.velocity);
        }
    }

    private void FixedUpdate()
    {
        if(p_eUpdateMode == EUpdateMode.FixedUpdate)
            Logic_CharacterControll();
    }

    /* protected - [abstract & virtual]         */

    virtual protected bool Check_IsWallSliding()
    {
        if (p_bIsGround)
            return false;

        switch (p_eWallSlidingDetect)
        {
            case EWallSlidingDetect.Collider:
                return (p_bRightDirection_IsBlocked && p_fMoveVelocity > 0f) || (p_bLeftDirection_IsBlocked && p_fMoveVelocity < 0f);

            case EWallSlidingDetect.RayCasting:
                {
                    if (p_bLeftDirection_IsBlocked && p_fMoveVelocity < 0f)
                        return CheckRayCasting_IsHit(_pBoxCollider_LeftCheck, _pBoxCollider_LeftCheck.transform.position.x + _pBoxCollider_LeftCheck.offset.x + (_pBoxCollider_LeftCheck.size.x / 2f), Vector2.left);

                    if (p_bRightDirection_IsBlocked && p_fMoveVelocity > 0f)
                        return CheckRayCasting_IsHit(_pBoxCollider_RightCheck, _pBoxCollider_RightCheck.transform.position.x + _pBoxCollider_RightCheck.offset.x - (_pBoxCollider_RightCheck.size.x / 2f), Vector2.right);

                    return false;
                }

            case EWallSlidingDetect.None:
            default:
                return false;
        }
    }

    private bool CheckRayCasting_IsHit(BoxCollider2D pCollider, float fPosX, Vector2 vecDirection)
    {
        // 왠지 모르게 Bounds가 잘 구해지지 않아서 직접 구현
        Vector2 vecPos = Vector2.zero;
        vecPos.x = fPosX;
        vecPos.y = pCollider.transform.position.y + pCollider.offset.y - (pCollider.size.y / 2f);

        //if (Physics2D.Raycast(vecPos, vecDirection, p_fWallSlidCheck_RayDistance, p_sWhatIsGround))
        if (Physics2D.Raycast(vecPos, vecDirection, pCollider.size.x, p_sWhatIsGround))
        {
            vecPos.y = pCollider.transform.position.y + pCollider.offset.y + (pCollider.size.y / 2f);
            //if (Physics2D.Raycast(vecPos, vecDirection, p_fWallSlidCheck_RayDistance, p_sWhatIsGround))
            if (Physics2D.Raycast(vecPos, vecDirection, pCollider.size.x, p_sWhatIsGround))
            {
                return true;
            }
        }

        return false;
    }

    virtual protected void CalculateJump_OnWallSliding(out bool bIsJumping)
    {
        bIsJumping = true;
        Vector2 vecClimbJump = Vector2.zero;

        if ((p_bRightDirection_IsBlocked && p_fMoveVelocity > 0f) || (p_bLeftDirection_IsBlocked && p_fMoveVelocity < 0f))
        {
            if (p_vecWallJumpClimb.Equals(Vector3.zero) == false)
            {
                if (p_eDebugFilter.ContainEnumFlag(EDebugFilter.Debug_Level_1))
                    Debug.Log(name + " WallJumpClimb", this);


                vecClimbJump = p_vecWallJumpClimb;
            }
        }
        else if (p_fMoveVelocity != 0f)
        {
            if (p_vecWallLeap.Equals(Vector3.zero) == false)
            {
                if (p_eDebugFilter.ContainEnumFlag(EDebugFilter.Debug_Level_1))
                    Debug.Log(name + " WallJumpLeap", this);

                vecClimbJump = p_vecWallLeap;
            }
        }

        if (p_bRightDirection_IsBlocked)
            vecClimbJump.x *= -1f;

        if (vecClimbJump.y != 0f)
        {
            _pRigidbody.velocity = new Vector2(_pRigidbody.velocity.x, 0f);
            DoAddForce_Custom(vecClimbJump, p_fWallJump_Delay);
        }
        else
            bIsJumping = false;
    }

    virtual protected void CalculateJump_OnSlopeSliding(out bool bIsJumping)
    {
        bIsJumping = true;

        float fAddJumpX = p_vecSlidingJump.x;
        if (_vecCurrentSlopeNormal.x < 0f)
            fAddJumpX *= -1f;

        Vector2 vecJump = _vecCurrentSlopeNormal * (new Vector2(fAddJumpX, p_vecSlidingJump.y + p_fJumpForce));
        // _pRigidbody.velocity = new Vector2(_pRigidbody.velocity.x, 0f);
        DoAddForce_Custom(vecJump, p_fSlopeSlidingJump_Delay);
    }

    virtual protected void OnChangeState(ECharacterControllerState eState)
    {
        //if (p_ePlatformerState_Current == eState)
        //    return;

        p_ePlatformerState_Prev = p_ePlatformerState_Current;
        p_ePlatformerState_Current = eState;
        for (int i = 0; i < _listCharacterControllerListener.Count; i++)
            _listCharacterControllerListener[i].ICharacterController_Listener_OnChangeState(p_ePlatformerState_Prev, p_ePlatformerState_Current);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_pRigidbody == null || _pCollider_Leg == null || (p_bUseCrouching && _pTransform_CeilingCheck == null))
            EventOnAwake_Force();

        if (p_eDebugFilter.ContainEnumFlag(EDebugFilter.Debug_Level_1))
        {
            bool bHitGround = false;
            Collider2D pColliderTerrain = null;
            Collider2D[] arrColliders = Physics2D.OverlapCircleAll((Vector2)transform.position + _pCollider_Leg.offset, _pCollider_Leg.radius, p_sWhatIsGround);
            for (int i = 0; i < arrColliders.Length; i++)
            {
                if (arrColliders[i].gameObject != gameObject)
                {
                    pColliderTerrain = arrColliders[i];
                    bHitGround = true;
                    break;
                }
            }

            if (bHitGround)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((Vector2)transform.position + _pCollider_Leg.offset, _pCollider_Leg.radius);


            if(bHitGround && p_bUseSlopeSliding)
            {
                Gizmos.color = Color.green;
                ContactPoint2D[] arrContactPoint = new ContactPoint2D[10];
                int iContactCount = Physics2D.GetContacts(pColliderTerrain, _pCollider_Leg, new ContactFilter2D(), arrContactPoint);
                for (int i = 0; i < iContactCount; i++)
                {
                    float fSlopeAngle = Vector2.Angle(arrContactPoint[i].normal, Vector2.up);
                    Gizmos.DrawWireSphere(arrContactPoint[i].point, 1f);
                    UnityEditor.Handles.Label(arrContactPoint[i].point, "fSlopeAngle : " + fSlopeAngle);
                    Gizmos.DrawRay(arrContactPoint[i].point, arrContactPoint[i].normal * 1f);
                }
            }


            if (p_bUseCrouching)
            {
                if (Physics2D.OverlapCircle(_pTransform_CeilingCheck.position, p_fCeilingRadius, p_sWhatIsGround))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_pTransform_CeilingCheck.position, p_fCeilingRadius);
            }


            if (p_eWallSlidingDetect == EWallSlidingDetect.Collider || p_eWallSlidingDetect == EWallSlidingDetect.RayCasting)
            {
                Color pColorLight_Red = new Color(0.7f, 0f, 0f, 0.5f);
                Color pColorLight_Green = new Color(0f, 0.7f, 0f, 0.5f);

                if (_pBoxCollider_LeftCheck == null || _pBoxCollider_RightCheck == null)
                    EventOnAwake_Force();

                if (Physics2D.OverlapBox((Vector2)_pBoxCollider_LeftCheck.transform.position + _pBoxCollider_LeftCheck.offset, _pBoxCollider_LeftCheck.size, 0f, p_sWhatIsGround))
                    Gizmos.color = pColorLight_Red;
                else
                    Gizmos.color = pColorLight_Green;
                Gizmos.DrawWireCube((Vector2)_pBoxCollider_LeftCheck.transform.position + _pBoxCollider_LeftCheck.offset, _pBoxCollider_LeftCheck.size);

                if (Physics2D.OverlapBox((Vector2)_pBoxCollider_RightCheck.transform.position + _pBoxCollider_RightCheck.offset, _pBoxCollider_RightCheck.size, 0f, p_sWhatIsGround))
                    Gizmos.color = pColorLight_Red;
                else
                    Gizmos.color = pColorLight_Green;
                Gizmos.DrawWireCube((Vector2)_pBoxCollider_RightCheck.transform.position + _pBoxCollider_RightCheck.offset, _pBoxCollider_RightCheck.size);
            }


            if (p_eWallSlidingDetect == EWallSlidingDetect.RayCasting)
            {
                // 왠지 모르게 Bounds가 잘 구해지지 않아서 직접 구현
                Vector2 vecRayPos = Vector2.zero;
                vecRayPos.x = _pBoxCollider_LeftCheck.transform.position.x + _pBoxCollider_LeftCheck.offset.x + (_pBoxCollider_LeftCheck.size.x / 2f);
                vecRayPos.y = _pBoxCollider_LeftCheck.transform.position.y + _pBoxCollider_LeftCheck.offset.y - (_pBoxCollider_LeftCheck.size.y / 2f);
                if (Physics2D.Raycast(vecRayPos, Vector2.left, _pBoxCollider_LeftCheck.size.x, p_sWhatIsGround))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(vecRayPos, Vector2.left * _pBoxCollider_LeftCheck.size.x);


                vecRayPos.y = _pBoxCollider_LeftCheck.transform.position.y + _pBoxCollider_LeftCheck.offset.y + (_pBoxCollider_LeftCheck.size.y / 2f);
                if (Physics2D.Raycast(vecRayPos, Vector2.left, _pBoxCollider_LeftCheck.size.x, p_sWhatIsGround))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(vecRayPos, Vector2.left * _pBoxCollider_LeftCheck.size.x);



                vecRayPos.x = _pBoxCollider_RightCheck.transform.position.x + _pBoxCollider_RightCheck.offset.x - (_pBoxCollider_RightCheck.size.x / 2f);
                vecRayPos.y = _pBoxCollider_RightCheck.transform.position.y + _pBoxCollider_RightCheck.offset.y - (_pBoxCollider_RightCheck.size.y / 2f);
                if (Physics2D.Raycast(vecRayPos, Vector2.right, _pBoxCollider_RightCheck.size.x, p_sWhatIsGround))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(vecRayPos, Vector2.right * _pBoxCollider_RightCheck.size.x);


                vecRayPos.y = _pBoxCollider_RightCheck.transform.position.y + _pBoxCollider_RightCheck.offset.y + (_pBoxCollider_RightCheck.size.y / 2f);
                if (Physics2D.Raycast(vecRayPos, Vector2.right, _pBoxCollider_RightCheck.size.x, p_sWhatIsGround))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(vecRayPos, Vector2.right * _pBoxCollider_RightCheck.size.x);
            }

            float fPosYOffset = 1f;
            Vector3 vecPos = transform.position + new Vector3(1f, 1f);


            UnityEditor.Handles.Label(vecPos, "Movement State ---------------------------------------------");
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, " _vecAddForce_Custom : " + _vecAddForce_Custom);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_fMoveDelta_0_1 : " + _fMoveDelta_0_1 + " p_fMoveVelocity : " + p_fMoveVelocity);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_pRigidbody.velocityPrev : " + _queueVelocityPrev.ToList().ToStringList());
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_pRigidbody.velocity : " + _pRigidbody.velocity);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_bInputAddForce_Custom : " + _bInputAddForce_Custom + " _vecAddForce_Custom : " + _vecAddForce_Custom);
            vecPos.y -= fPosYOffset * 2f;
            


            UnityEditor.Handles.Label(vecPos, "Platformer State ---------------------------------------------");
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_ePlatformerState_Flags_Prev : " + p_ePlatformerState_Prev);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_ePlatformerState_Flags_Current : " + p_ePlatformerState_Current);
            vecPos.y -= fPosYOffset * 2f;



            UnityEditor.Handles.Label(vecPos, "Platform Check State -----------------------------------------");
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_bIsGround : " + p_bIsGround + " _bIsCrouch : " + p_bIsCrouch + " _bIsMoving : " + p_bIsMoving + " _bIsJumping : " + p_bIsJumping);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_bCheckIsFalling : " + _bCheckIsFalling + " _bIsFalling : " + _bIsFalling);
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_bIsWallSliding : " + _bIsWallSliding + " _bLeftDirection_IsBlocked : " + p_bLeftDirection_IsBlocked + " _bRightDirection_IsBlocked : " + p_bRightDirection_IsBlocked);
            vecPos.y -= fPosYOffset * 2f;


            if(p_bUseSlopeSliding)
            {
                UnityEditor.Handles.Label(vecPos, "Slope Check State -----------------------------------------");
                vecPos.y -= fPosYOffset;

                UnityEditor.Handles.Label(vecPos, "p_bIsSlopeSliding : " + p_bIsSlopeSliding + " _fSlopeAngle : " + _fSlopeAngle + " _fSlopeAngle_Signed : " + _fSlopeAngle_Signed + " _vecCurrentSlopeNormal : " + _vecCurrentSlopeNormal);
                vecPos.y -= fPosYOffset;
            }
        }
    }
#endif

    // ========================================================================== //

    #region Private

    private void Logic_CharacterControll()
    {
        bool bSkipMove = _bInputJump || _bInputAddForce_Custom;
        if (_bInputJump)
            Logic_InputJump();

        if (_bInputAddForce_Custom)
            Logic_AddForce_Custom();

        if (bSkipMove == false && p_bIsSlopeSliding == false)
        {
            bool bBlocked_OnAir = p_bIsGround == false && CheckForward_IsBlock();
            if (_bIsLockMove == false && bBlocked_OnAir == false)
            {
                float fMoveSpeed = Mathf.Lerp(p_fSpeed_OnWalking, p_fSpeed_OnRunning, _fMoveDelta_0_1);
                _pRigidbody.velocity = new Vector2(p_fMoveVelocity * fMoveSpeed, _pRigidbody.velocity.y);
            }
        }

        if (_bIsWallSliding && _pRigidbody.velocity.y < p_fWallSlideSpeedMax)
            _pRigidbody.velocity = new Vector2(_pRigidbody.velocity.x, p_fWallSlideSpeedMax);

        if (bSkipMove == false)
            Logic_IsGround();
        Logic_IsBlocked();
        SetState();
    }

    private void Logic_Moving()
    {
        if (p_bIsMoving && _fMoveDelta_0_1 < p_fMaxMoveDelta_OnWalking)
        {
            _fMoveDelta_0_1 += (1f / p_fTimeToMaxSpeedApex) * Time.deltaTime;
            if (_fMoveDelta_0_1 > p_fMaxMoveDelta_OnWalking)
                _fMoveDelta_0_1 = p_fMaxMoveDelta_OnWalking;
        }

        if (p_bIsMoving == false)
        {
            if (p_bIsSmoothMoveStop)
            {
                if (_fMoveDelta_0_1 > 0f)
                    _fMoveDelta_0_1 -= (1f / p_fTimeToStop) * Time.deltaTime;
            }
            else
                _fMoveDelta_0_1 = 0f;
        }
    }

    private void Logic_IsGround()
    {
        bool bIsGround = false;
        Collider2D[] arrColliders = Physics2D.OverlapCircleAll((Vector2)_pCollider_Leg.transform.position + _pCollider_Leg.offset, _pCollider_Leg.radius, p_sWhatIsGround);
        for (int i = 0; i < arrColliders.Length; i++)
        {
            if (arrColliders[i].gameObject != gameObject)
            {
                bIsGround = true;
                OnGround(arrColliders[i]);
                break;
            }
        }

        if (bIsGround == false)
            OnAirborne();
    }

    private void Logic_InputJump()
    {
        float fJumpLimit = UpdateJumpVelocity(_vecJumpAddForce.y);
        if (_pRigidbody.velocity.y < fJumpLimit)
            _pRigidbody.AddForce(_vecJumpAddForce);

        _vecJumpAddForce = Vector2.zero;
        _bInputJump = false;

        // SetState();
    }

    private void Logic_AddForce_Custom()
    {
        _bInputAddForce_Custom = false;
        if(_bInputAddForce_Custom_Velocity_IsZero)
        {
            _bInputAddForce_Custom_Velocity_IsZero = false;
            _pRigidbody.velocity = Vector3.zero;
        }

        _pRigidbody.AddForce(_vecAddForce_Custom);

        // SetState();
    }

    private void Logic_IsBlocked()
    {
        _listSideBlock_Wall.Clear();
        Collider2D pColliderLeft = Physics2D.OverlapBox((Vector2)_pBoxCollider_LeftCheck.transform.position + _pBoxCollider_LeftCheck.offset, _pBoxCollider_LeftCheck.size, 0f, p_sWhatIsGround);
        Collider2D pColliderRight = Physics2D.OverlapBox((Vector2)_pBoxCollider_RightCheck.transform.position + _pBoxCollider_RightCheck.offset, _pBoxCollider_RightCheck.size, 0f, p_sWhatIsGround);

        p_bLeftDirection_IsBlocked = pColliderLeft;
        p_bRightDirection_IsBlocked = pColliderRight;

        if (pColliderLeft)
            _listSideBlock_Wall.Add(pColliderLeft);
        if (pColliderRight)
            _listSideBlock_Wall.Add(pColliderRight);

        if (p_bLeftDirection_IsBlocked || p_bRightDirection_IsBlocked)
            _bIsWallSliding = Check_IsWallSliding();
        else
            _bIsWallSliding = false;

        if (_bIsWallSliding)
        {
            p_bIsJumping = false;
            _fTime_WallSlidingStart = Time.time;
        }
    }

    private void OnAirborne()
    {
        p_bIsGround = false;
        _fSlopeAngle = 0f;

        Set_SlopeSliding(false);
        SetFalling_IfCondition_IsTrue();
    }

    private void OnGround(Collider2D pColliderTerrain)
    {
        bool bIsGroundOrigin = p_bIsGround;
        p_bIsGround = true;
        p_bIsJumping = false;

        DoSetFalling(false);
        if(p_bUseSlopeSliding)
        {
            UpdateSlopeAngle(pColliderTerrain);
            Set_SlopeSliding(_fSlopeAngle >= p_fSlopeAngle);

            if (bIsGroundOrigin == false && _queueVelocityPrev.Count > 0)
                _pRigidbody.velocity = _queueVelocityPrev.Dequeue();
        }
    }

    private void Set_SlopeSliding(bool bIsSlopeSliding)
    {
        if (bIsSlopeSliding)
            p_bFaceDirection_IsRight = _fSlopeAngle_Signed > 0f;
        p_bIsSlopeSliding = bIsSlopeSliding;

        for (int i = 0; i < _listCharacterControllerListener.Count; i++)
            _listCharacterControllerListener[i].ICharacterController_Listener_OnChangeFaceDir(p_bFaceDirection_IsRight, bIsSlopeSliding, _fSlopeAngle_Signed);
    }

    private void SetState()
    {
        if (p_bIsSlopeSliding)
        {
            OnChangeState(ECharacterControllerState.Slope_Sliding);
            return;
        }

        if (_bIsWallSliding)
        {
            OnChangeState(ECharacterControllerState.Wall_Sliding);
            return;
        }

        if (_bIsFalling)
        {
            OnChangeState(ECharacterControllerState.Falling);
            return;
        }

        if (p_bIsJumping)
        {
            //if (_iJumpCount == 1)
            OnChangeState(ECharacterControllerState.Jumping);
            //else
            //    OnChangeState(ECharacterControllerState.MultipleJumping);
            return;
        }

        bool bIsMoving = p_bIsMoving;
        if (bIsMoving && p_bIsPlayAnimation_OnForwardIsBlock == false)
            bIsMoving = CheckForward_IsBlock() == false;

        if (bIsMoving)
        {
            if (p_bIsCrouch)
                OnChangeState(ECharacterControllerState.CrouchWalking);
            else
            {
                if (_fMoveDelta_0_1.Equals(1f))
                    OnChangeState(ECharacterControllerState.Running);
                else
                    OnChangeState(ECharacterControllerState.Walking);
            }
        }
        else
        {
            if (p_bIsCrouch)
                OnChangeState(ECharacterControllerState.Crouching);
            else
                OnChangeState(ECharacterControllerState.Standing);
            return;
        }
    }

    private void SetFalling_IfCondition_IsTrue()
    {
        if (gameObject.activeSelf == false)
            return;
        if (_bCheckIsFalling)
            return;

        if (Check_IsFalling())
        {
            if (_pCoroutine_Falling != null)
                StopCoroutine(_pCoroutine_Falling);
            _pCoroutine_Falling = StartCoroutine(CoDelayFalling());
        }
    }

    private IEnumerator CoDelayFalling()
    {
        _bCheckIsFalling = true;

        yield return new WaitForSeconds(p_fTimeToFalling);

        if (Check_IsFalling())
            DoSetFalling(true);

        _bCheckIsFalling = false;
    }

    private IEnumerator CoDelaySetLockMove_IsFalse(float fWaitSecond)
    {
        _bIsLockMove = true;
        _fWaitSecond = fWaitSecond;
        while (_fWaitSecond > 0f)
        {
            _fWaitSecond -= Time.deltaTime;
            yield return null;
        }

        _bIsLockMove = false;
    }

    private bool Check_IsFalling()
    {
        return p_bIsGround == false && _bIsWallSliding == false;// && p_pCalculator.p_pCollisionInfo.climbingSlope == false;
    }

    private float UpdateJumpVelocity(float fJumpVelocity)
    {
        return _pRigidbody.mass * p_fJumpForce * Time.fixedDeltaTime;
    }

    private bool CheckForward_IsBlock()
    {
        return ((p_fMoveVelocity > 0f && p_bRightDirection_IsBlocked) || (p_fMoveVelocity < 0f && p_bLeftDirection_IsBlocked));
    }

    private void UpdateSlopeAngle(Collider2D pColliderTerrain)
    {
        ContactPoint2D[] arrContactPoint = new ContactPoint2D[10];
        int iContactCount = Physics2D.GetContacts(pColliderTerrain, _pCollider_Leg, new ContactFilter2D(), arrContactPoint);

        _fSlopeAngle = 0f;
        _fSlopeAngle_Signed = 0f;
        _vecCurrentSlopeNormal = Vector3.zero;

        for (int i = 0; i < iContactCount; i++)
        {
            float fSlopeAngle = Vector2.Angle(arrContactPoint[i].normal, Vector2.up);
            if (_fSlopeAngle == 0f || _fSlopeAngle > fSlopeAngle)
            {
                _fSlopeAngle = fSlopeAngle;
                _fSlopeAngle_Signed = Vector2.SignedAngle(arrContactPoint[i].normal, Vector2.up);
                _vecCurrentSlopeNormal = arrContactPoint[i].normal;
            }
        }
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test