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

    private const float const_fWallJumpDuration = 0.2f;

    /* enum & struct declaration                */

    public enum EUpdateMode
    {
        Update,
        FixedUpdate,
        Manual
    }

    /* public - Field declaration            */

    /// <summary>
    /// Prev, Current
    /// </summary>
    public CObserverSubject<ECharacterControllerState, ECharacterControllerState> p_Event_OnChangePlatformerState { get; private set; } = new CObserverSubject<ECharacterControllerState, ECharacterControllerState>();
    public CObserverSubject<bool> p_Event_OnFalling { get; private set; } = new CObserverSubject<bool>();
    public CObserverSubject<bool> p_Event_OnWallSliding { get; private set; } = new CObserverSubject<bool>();
    public CObserverSubject<bool> p_Event_OnGround { get; private set; } = new CObserverSubject<bool>();
    public CObserverSubject<bool> p_Event_OnLadder { get => p_pLogicLadder?.p_Event_OnLadder; }
    public CObserverSubject<float> p_Event_OnMove { get; private set; } = new CObserverSubject<float>();

    public HashSet<Collider2D> p_setCharacterBody { get; private set; } = new HashSet<Collider2D>();

    public ECharacterControllerState p_ePlatformerState_Current { get; private set; }
    public ECharacterControllerState p_ePlatformerState_Prev { get; private set; }

    public bool p_bFaceDirection_IsRight { get; private set; }
    public bool p_bIsGround { get; private set; }
    public bool p_bIsCrouch { get; private set; }
    public bool p_bIsMoving { get; private set; }
    public bool p_bIsJumping { get; private set; }
    public bool p_bIsSlopeSliding { get; private set; }
    public bool p_bIsLock_Move { get; private set; }
    public bool p_bIsLock_Jump { get; private set; }
    public bool p_bIsClimbingLadder { get => p_pLogicLadder.p_bIsClimbLadder; }

    public bool p_bLeftDirection_IsBlocked_All { get; private set; }
    public bool p_bRightDirection_IsBlocked_All { get; private set; }

    public bool p_bLeftDirection_IsBlocked { get; private set; }
    public bool p_bRightDirection_IsBlocked { get; private set; }

    [SerializeField]
    [Rename_Inspector("디폴트 캐릭터 방향이 오른쪽인지")]
    private bool p_bDefault_FaceDirection_IsRight = false;

    [Header("움직임 옵션")]
    [Rename_Inspector("움직임 로직 SO")]
    public CCharacterController2D_MoveLogicBase p_pLogicMove;

    [Space(10)]
    [Rename_Inspector("점프 로직 SO")]
    public CCharacterController2D_JumpLogicDefault p_pLogicJump;

    [Space(10)]
    [Rename_Inspector("사다리 로직 SO")]
    public CCharacterController2D_LadderLogicBase p_pLogicLadder;

    [Space(10)]
    [Header("앉기 옵션")]
    [Rename_Inspector("앉기를 사용하는지")]
    public bool p_bUseCrouching = false;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseCrouching")] 
#endif
    [UnityEngine.Range(0, 1)]
    [Rename_Inspector("앉아있을 때 속도 감소 비율")]
    public float p_fCrouchSpeed = .36f;
    [Rename_Inspector("천장 체크범위")]
    public float p_fCeilingRadius = .01f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseCrouching")]
#endif
    [SerializeField]
    [GetComponentInChildren("Collider_OnCrouch", true, false)]
    [Rename_Inspector("\"Collider_OnCrouch\" 앉았을 때 몸통용 자식 컬라이더")]
    public CapsuleCollider2D p_pCapsuleCollider_OnCrouch { get; protected set; }
    [SerializeField]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseCrouching")]
#endif
    [GetComponentInChildren("ColliderLeft_OnCrouch", true, false)]
    [Rename_Inspector("\"ColliderLeft_OnCrouch\" 앉았을 때 왼벽 체크용 자식 컬라이더")]
    protected List<BoxCollider2D> _listBoxColliderLeft_OnCrouch = new List<BoxCollider2D>();
    [SerializeField]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseCrouching")]
#endif
    [GetComponentInChildren("ColliderRight_OnCrouch", true, false)]
    [Rename_Inspector("\"ColliderRight_OnCrouch\" 앉았을 때 오른벽 체크용 자식 컬라이더")]
    protected List<BoxCollider2D> _listBoxColliderRight_OnCrouch = new List<BoxCollider2D>();


    [Space(10)]
    [Header("경사면 슬라이딩 옵션")]
    [Rename_Inspector("경사면 슬라이딩 유무")]
    public bool p_bUseSlopeSliding = false;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseSlopeSliding")]
#endif
    [Rename_Inspector("경사면 발동 각도")]
    public float p_fSlopeAngle = 45f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseSlopeSliding")] 
#endif
    [Rename_Inspector("경사면 슬라이딩 중 점프할 때 추가 속도 (기본 점프힘에 추가 힘)")]
    public Vector2 p_vecSlidingJump = new Vector2(0f, 0f);
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseSlopeSliding")]
#endif
    [Rename_Inspector("경사면 슬라이딩 점프 딜레이")]
    public float p_fSlopeSlidingJump_Delay = 0.2f;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bUseSlopeSliding")]
#endif
    [Rename_Inspector("경사면 슬라이딩 조건 달성후 몇초 뒤에 발동할지")]
    public float p_fSlopeSlidingDelay = 0.2f;


    [Space(10)]
    [Header("월 슬라이딩 옵션")]
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
    public LayerMask p_sWhatIsTerrain;


    /* protected & private - Field declaration         */

    [GetComponent]
    public Rigidbody2D p_pRigidbody { get; protected set; }
    [GetComponent]
    public CapsuleCollider2D p_pCollider_Body { get; protected set; }
    [SerializeField]
    [GetComponentInChildren("CheckGround", true, false)]
    [Rename_Inspector("\"CheckGround\" - 바닥 체크용 자식 컬라이더")]
    public CapsuleCollider2D p_pCollider_GroundChecker { get; protected set; }

    [Space(10)]
    [Header("디버그용")]
    [SerializeField]
    [Rename_Inspector("\"CheckCeiling\" - 천장 체크용 자식 트렌스폼 ( 앉기 사용시 )", false)]
    [GetComponentInChildren("CheckCeiling", true, false)]
    protected Transform _pTransform_CeilingCheck = null;

    [SerializeField]
    [Rename_Inspector("\"ColliderLeft\" - 왼벽 체크용 자식 컬라이더", false)]
    [GetComponentInChildren("ColliderLeft", true, false)]
    protected List<BoxCollider2D> _listBoxCollider_LeftCheck = new List<BoxCollider2D>();
    [SerializeField]
    [Rename_Inspector("\"ColliderRight\" - 오른벽 체크용 자식 컬라이더", false)]
    [GetComponentInChildren("ColliderRight", true, false)]
    protected List<BoxCollider2D> _listBoxCollider_RightCheck = new List<BoxCollider2D>();
    [SerializeField]
    [Rename_Inspector("현재 언덕 경사면 각도", false)]
    protected float _fSlopeAngle = 0f;
    [SerializeField]
    [Rename_Inspector("현재 언덕 경사면 각도 ( 부호 포함 )", false)]
    protected float _fSlopeAngle_Signed = 0f;

    protected List<Collider2D> _listSideBlock_Wall = new List<Collider2D>(50);
    protected float p_fMoveVelocity;


    List<ICharacterController_Listener> _listCharacterControllerListener = new List<ICharacterController_Listener>();
    List<CCharacterController2D_LogicBase> _listLogic = new List<CCharacterController2D_LogicBase>();

    List<BoxCollider2D> _listBoxCollider_LeftCheck_Origin;
    List<BoxCollider2D> _listBoxCollider_RightCheck_Origin;

    List<Collider2D> _listColliderOverlap = new List<Collider2D>();

    Collider2D[] _arrHitColliders = new Collider2D[10];
    ContactPoint2D[] _arrContactPoint = new ContactPoint2D[10];

    Coroutine _pCoroutine_Falling;
    Coroutine _pCoroutine_SetLockMove_IsFalse;
    Collider2D _pColliderGround;

    Vector2 _vecOriginCollider_Offset;
    Vector2 _vecOriginCollider_Size;

    Vector2 _vecJumpAddForce;
    Vector2 _vecAddForce_Custom;

    Vector2 _vecCurrentSlopeNormal;

    int _iHitCount;

    float _fMoveDelta_0_1;
    float _fVerticalMoveDelta_0_1;
    float _fWaitSecond;
    float _fTime_WallSlidingStart = -1f;
    float _fJumpStartTime;

    bool _bCheckIsFalling;
    bool _bIsFalling;
    bool _bIsWallSliding;

    bool _bInputAddForce_Custom;
    bool _bInputSetForce_Custom;
    bool _bInputSetForce_Custom_Velocity_IsZero;
    bool _bInputJump;

    /// <summary>
    /// 이동 후 바로 언덕 슬라이딩을 할 경우
    ///  캐릭터가 이러한 지형에서 이 방향으로 이동할 때 -> __/
    ///  언덕 슬라이딩이 걸린다. 이동 후 한 프레임을 생략 후 2 프레임 이상 언덕에 있으면 그때 언덕 슬라이딩 발동
    /// </summary>
    float _fSkipSlopeSlidingElapsTime;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public Vector3 CalculateVector3(Vector3 vecX, Vector3 vecY)
    {
        Vector3 vecNewVector = vecX;
        vecNewVector.x *= vecY.x;
        vecNewVector.y *= vecY.y;
        vecNewVector.z *= vecY.z;

        return vecNewVector;
    }


    public void DoClear_AddforceCustom()
    {
        _bInputAddForce_Custom = false;
        _bInputSetForce_Custom = false;
    }

    public void DoUpdate_CharacterController()
    {
        Logic_CharacterControll();
    }

    public void DoSetLock_Move(bool bLockMove)
    {
        p_bIsLock_Move = bLockMove;
    }

    public void DoSetLock_Jump(bool bLockJump)
    {
        p_bIsLock_Jump = bLockJump;
    }

    public void DoSet_ChangeFaceDir(bool bDirection_IsRight)
    {
        p_bFaceDirection_IsRight = bDirection_IsRight;
        for (int i = 0; i < _listCharacterControllerListener.Count; i++)
            _listCharacterControllerListener[i].ICharacterController_Listener_OnChangeFaceDir(p_bFaceDirection_IsRight, p_bIsSlopeSliding, _fSlopeAngle_Signed);
    }

    public void DoSetVelocity_Custom(Vector2 vecVelocity, float fMoveLockSecond)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " DoSetVelocity_Custom : " + vecVelocity + " fMoveLockSecond : " + fMoveLockSecond, this);

        _bInputSetForce_Custom = true;
        _vecAddForce_Custom = vecVelocity;

        if (_fWaitSecond < fMoveLockSecond)
        {
            if (_pCoroutine_SetLockMove_IsFalse != null)
                StopCoroutine(_pCoroutine_SetLockMove_IsFalse);
            _pCoroutine_SetLockMove_IsFalse = StartCoroutine(CoDelaySetLockMove_IsFalse(fMoveLockSecond));
        }
    }

    public void DoAddForce_Custom(Vector2 vecAddForce, float fMoveLockSecond)
    {
        if(CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " DoAddForce_Custom : " + vecAddForce + " fMoveLockSecond : " + fMoveLockSecond, this);

        _bInputAddForce_Custom = true;
        _bInputSetForce_Custom_Velocity_IsZero = false;
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
        _bInputSetForce_Custom_Velocity_IsZero = true;
    }

    public void DoSetFalling(bool bFalling)
    {
        bool bFallingPrev = _bIsFalling;
        if(bFallingPrev != bFalling)
        {
            _bIsFalling = bFalling;
            p_Event_OnFalling.DoNotify(bFalling);
        }
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

    public void DoMoveLadder(float fMoveAmount_0_1)
    {
        bool bIsMoveLadder;
        int iCount = Calculate_GroundCollider(ref _arrHitColliders);
        p_pLogicLadder.DoCalculateLadder(GetCeilingOverlapCollider(), _pColliderGround, p_bIsGround, iCount, _arrHitColliders, ref fMoveAmount_0_1, out bIsMoveLadder);
        _fVerticalMoveDelta_0_1 = fMoveAmount_0_1;
        if (bIsMoveLadder)
            _bInputJump = false;
    }

    public void DoSet_Crouch(bool bInput_Crouch)
    {
        if (p_bUseCrouching)
        {
            if (!bInput_Crouch && p_bIsCrouch)
                bInput_Crouch = DoCheck_IsBlock_Ceiling();

            p_bIsCrouch = bInput_Crouch;
        }
    }

    public void DoSetInput_Jump()
    {
        Logic_IsInputJump(true);
    }

    public void DoMove(float fMoveAmount, bool bInput_Run)
    {
        if (p_bIsLock_Move)
            p_fMoveVelocity = 0f;
        else
            p_pLogicMove.DoCalculate_MoveVelocity(fMoveAmount, p_bIsCrouch, bInput_Run, p_fCrouchSpeed, ref p_fMoveVelocity);

        if (p_bIsMoving && bInput_Run && p_bIsCrouch == false)
            Set_MoveDelta(1f);
        if (p_bIsMoving && bInput_Run == false && _fMoveDelta_0_1 > p_pLogicMove.p_fMaxMoveDelta_OnWalking)
            Set_MoveDelta(p_pLogicMove.p_fMaxMoveDelta_OnWalking);

        p_bIsMoving = p_fMoveVelocity != 0f;
        if (p_pCapsuleCollider_OnCrouch)
            Logic_OnCrouch_And_Move();
    }

    public void DoMove(float fMoveAmount, bool bInput_Crouch, bool bInput_Run, bool bInput_Jump)
    {
        if (p_bUseCrouching)
        {
            if (!bInput_Crouch && p_bIsCrouch)
                bInput_Crouch = DoCheck_IsBlock_Ceiling();
        }

        if (p_bIsLock_Move)
            p_fMoveVelocity = 0f;
        else
            p_pLogicMove.DoCalculate_MoveVelocity(fMoveAmount, bInput_Crouch, bInput_Run, p_fCrouchSpeed, ref p_fMoveVelocity);

        Logic_IsInputJump(bInput_Jump);

        if (p_bIsMoving && bInput_Run && p_bIsCrouch == false)
            Set_MoveDelta(1f);
        if (p_bIsMoving && bInput_Run == false && _fMoveDelta_0_1 > p_pLogicMove.p_fMaxMoveDelta_OnWalking)
            Set_MoveDelta(p_pLogicMove.p_fMaxMoveDelta_OnWalking);

        p_bIsCrouch = bInput_Crouch;
        p_bIsMoving = p_fMoveVelocity != 0f;

        if (p_pCapsuleCollider_OnCrouch)
            Logic_OnCrouch_And_Move();
    }

    public bool DoCheck_IsBlock_Ceiling()
    {
        return Check_Collider_IsTerrain(GetCeilingOverlapCollider());
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        Collider2D[] arrColliderBody = GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < arrColliderBody.Length; i++)
            p_setCharacterBody.Add(arrColliderBody[i]);

        p_ePlatformerState_Current = (ECharacterControllerState)(-1);

        if (_listBoxCollider_LeftCheck.Count != 0)
        {
            _listBoxCollider_LeftCheck_Origin = _listBoxCollider_LeftCheck;
            for (int i = 0; i < _listBoxCollider_LeftCheck.Count; i++)
                _listBoxCollider_LeftCheck[i].gameObject.SetActive(false);

            _listBoxCollider_LeftCheck.Sort(SortBoxCollider_Height_Greater);
        }

        if (_listBoxCollider_RightCheck.Count != 0)
        {
            _listBoxCollider_RightCheck_Origin = _listBoxCollider_RightCheck;
            for (int i = 0; i < _listBoxCollider_RightCheck.Count; i++)
                _listBoxCollider_RightCheck[i].gameObject.SetActive(false);

            _listBoxCollider_RightCheck.Sort(SortBoxCollider_Height_Greater);
        }

        if (p_pCapsuleCollider_OnCrouch)
            p_pCapsuleCollider_OnCrouch.gameObject.SetActive(false);

        if (p_pCollider_GroundChecker)
            p_pCollider_GroundChecker.gameObject.SetActive(true);

        for (int i = 0; i < _listBoxColliderLeft_OnCrouch.Count; i++)
            _listBoxColliderLeft_OnCrouch[i].gameObject.SetActive(false);

        for (int i = 0; i < _listBoxColliderRight_OnCrouch.Count; i++)
            _listBoxColliderRight_OnCrouch[i].gameObject.SetActive(false);

        _listBoxColliderLeft_OnCrouch.Sort(SortBoxCollider_Height_Greater);
        _listBoxColliderRight_OnCrouch.Sort(SortBoxCollider_Height_Greater);

        _vecOriginCollider_Offset = p_pCollider_Body.offset;
        _vecOriginCollider_Size = p_pCollider_Body.size;

        if(Application.isPlaying)
        {
            _listLogic.Clear();
            Init_CharacterLogic(ref p_pLogicMove, typeof(CCharacterController2D_MoveLogic_Default));
            Init_CharacterLogic(ref p_pLogicJump, typeof(CCharacterController2D_JumpLogicDefault));
            Init_CharacterLogic(ref p_pLogicLadder, typeof(CCharacterController2D_LadderLogic_Default));

            for (int i = 0; i < _listLogic.Count; i++)
                _listLogic[i].DoInit(this, p_pLogicMove, p_pLogicLadder);
        }

        DoSet_ChangeFaceDir(p_bDefault_FaceDirection_IsRight);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float fMoveDelta_0_1 = _fMoveDelta_0_1;
        p_pLogicMove.Calculate_MoveDelta(ref fMoveDelta_0_1);
        Set_MoveDelta(fMoveDelta_0_1);

        if (p_eUpdateMode == EUpdateMode.Update)
            Logic_CharacterControll();
        Logic_SlopeSliding();
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

        return (p_bRightDirection_IsBlocked_All && p_fMoveVelocity > 0f) || (p_bLeftDirection_IsBlocked_All && p_fMoveVelocity < 0f);
    }

    virtual protected bool Check_IsSlopeSliding(Collider2D pColliderTerrain)
    {
        return true;
    }

    virtual protected void CalculateJump_OnWallSliding(out bool bIsJumping)
    {
        bIsJumping = true;
        Vector2 vecClimbJump = Vector2.zero;

        if ((p_bRightDirection_IsBlocked_All && p_fMoveVelocity > 0f) || (p_bLeftDirection_IsBlocked_All && p_fMoveVelocity < 0f))
        {
            if (p_vecWallJumpClimb.Equals(Vector3.zero) == false)
            {
                vecClimbJump = p_vecWallJumpClimb;
            }
        }
        else if (p_fMoveVelocity != 0f)
        {
            if (p_vecWallLeap.Equals(Vector3.zero) == false)
            {
                vecClimbJump = p_vecWallLeap;
            }
        }

        vecClimbJump.x *= Mathf.Sign(p_fMoveVelocity);
        if (vecClimbJump.y != 0f)
        {
            SetVelocity(new Vector2(p_pRigidbody.velocity.x, 0f));
            DoAddForce_Custom(vecClimbJump, p_fWallJump_Delay);
        }
        else
            bIsJumping = false;
    }

    virtual protected void CalculateJump_OnLadder(out bool bIsJumping)
    {
        p_pLogicLadder.DoCalculateJump(out bIsJumping, p_fMoveVelocity);
        if (bIsJumping)
            DoAddForce_Custom(new Vector2(p_fMoveVelocity * GetMoveSpeed(), p_pLogicJump.p_fJumpForce), 0f);
    }

    virtual protected void CalculateJump_OnSlopeSliding(out bool bIsJumping)
    {
        bIsJumping = true;

        float fAddJumpX = p_vecSlidingJump.x;
        Vector2 vecJump = p_vecSlidingJump;
        vecJump.y += p_pLogicJump.p_fJumpForce;
        vecJump *= _vecCurrentSlopeNormal;

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

        p_Event_OnChangePlatformerState.DoNotify(p_ePlatformerState_Prev, p_ePlatformerState_Current);
    }

    protected virtual float GetMoveSpeed()
    {
        return p_pLogicMove.GetMoveSpeed(_fMoveDelta_0_1);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            for (int i = 0; i < _listLogic.Count; i++)
                _listLogic[i].DoInit(this, p_pLogicMove, p_pLogicLadder);

        bool bIsDebug = CheckDebugFilter(EDebugFilter.Debug_Level_Core);
        for (int i = 0; i < _listLogic.Count; i++)
            _listLogic[i].OnDrawGizmo(bIsDebug);

        if (bIsDebug == false)
            return;

        if (p_pRigidbody == null)
            p_pRigidbody = GetComponent<Rigidbody2D>();

        float fPosYOffset = 1f;
        Vector3 vecPos = transform.position + new Vector3(1f, 1f);

        UnityEditor.Handles.Label(vecPos, "Movement State ---------------------------------------------");
        vecPos.y -= fPosYOffset;

        UnityEditor.Handles.Label(vecPos, " _vecAddForce_Custom : " + _vecAddForce_Custom);
        vecPos.y -= fPosYOffset;

        UnityEditor.Handles.Label(vecPos, "_fMoveDelta_0_1 : " + _fMoveDelta_0_1 + " p_fMoveVelocity : " + p_fMoveVelocity);
        vecPos.y -= fPosYOffset;

        UnityEditor.Handles.Label(vecPos, "_pRigidbody.velocity : " + p_pRigidbody.velocity);
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

        if (_bCheckIsFalling)
        {
            UnityEditor.Handles.Label(vecPos, "Check Falling Time : " + _fFallingTime.ToString("F2") + " Time : " + Time.time.ToString("F2"));
            vecPos.y -= fPosYOffset;
        }

        UnityEditor.Handles.Label(vecPos, "_bIsWallSliding : " + _bIsWallSliding + " _bLeftDirection_IsBlocked : " + p_bLeftDirection_IsBlocked_All + " _bRightDirection_IsBlocked : " + p_bRightDirection_IsBlocked_All);
        vecPos.y -= fPosYOffset * 2f;

        if (p_bUseSlopeSliding)
        {
            UnityEditor.Handles.Label(vecPos, "Slope Check State -----------------------------------------");
            vecPos.y -= fPosYOffset;

            UnityEditor.Handles.Label(vecPos, "_fSkipSlopeSlidingElapsTime : " + _fSkipSlopeSlidingElapsTime + " p_bIsSlopeSliding : " + p_bIsSlopeSliding + " _fSlopeAngle : " + _fSlopeAngle + " _fSlopeAngle_Signed : " + _fSlopeAngle_Signed + " _vecCurrentSlopeNormal : " + _vecCurrentSlopeNormal);
            vecPos.y -= fPosYOffset;
        }



        if (p_pCollider_GroundChecker == null || (p_bUseCrouching && _pTransform_CeilingCheck == null))
        {
            EventOnAwake();
            if (p_pCollider_GroundChecker == null || (p_bUseCrouching && _pTransform_CeilingCheck == null))
                return;
        }

        Collider2D pColliderTerrain = null;
        bool bHitGround = false;
        int iHitCount = Calculate_GroundCollider(ref _arrHitColliders);
        for (int i = 0; i < iHitCount; i++)
        {
            if (p_setCharacterBody.Contains(_arrHitColliders[i]) == false)
            {
                pColliderTerrain = _arrHitColliders[i];
                bHitGround = true;
                break;
            }
        }

        if (bHitGround)
        {
            Gizmos.color = Color.red;

            ContactPoint2D[] arrContactPoint = new ContactPoint2D[10];
            int iContactCount = Physics2D.GetContacts(pColliderTerrain, p_pCollider_GroundChecker, new ContactFilter2D(), arrContactPoint);
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
            if (Physics2D.OverlapCircle(_pTransform_CeilingCheck.position, p_fCeilingRadius, p_sWhatIsTerrain))
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_pTransform_CeilingCheck.position, p_fCeilingRadius);
        }


        Color pColorLight_Red = new Color(0.7f, 0f, 0f, 0.5f);
        Color pColorLight_Green = new Color(0f, 0.7f, 0f, 0.5f);

        if (_listBoxCollider_LeftCheck == null || _listBoxCollider_RightCheck == null)
            EventOnAwake_Force();

        DrawGizmo_BoxCollider(_listBoxCollider_LeftCheck, pColorLight_Red, pColorLight_Green);
        DrawGizmo_BoxCollider(_listBoxCollider_RightCheck, pColorLight_Red, pColorLight_Green);
    }

    private void DrawGizmo_BoxCollider(List<BoxCollider2D> listBoxCollider, Color pColorLight_Red, Color pColorLight_Green)
    {
        for (int i = 0; i < listBoxCollider.Count; i++)
        {
            BoxCollider2D pCollider = listBoxCollider[i];
            if (Physics2D.OverlapBox((Vector2)pCollider.transform.position + pCollider.offset, pCollider.size, 0f, p_sWhatIsTerrain))
                Gizmos.color = pColorLight_Red;
            else
                Gizmos.color = pColorLight_Green;
            Gizmos.DrawWireCube((Vector2)pCollider.transform.position + pCollider.offset, pCollider.size);
        }
    }
#endif

    // ========================================================================== //

#region Private

    private void Logic_SlopeSliding()
    {
        if (p_bUseSlopeSliding && _fSlopeAngle >= p_fSlopeAngle)
            _fSkipSlopeSlidingElapsTime += Time.deltaTime;
    }

    private void Logic_CharacterControll()
    {
        Logic_Jump();

        if (_bInputAddForce_Custom)
            Logic_AddForce_Custom();

        if (_bInputSetForce_Custom)
            Logic_SetForce_Custom();

        bool bSkipMove = _bInputJump || _bInputAddForce_Custom;
        bool IsPossible_Move = bSkipMove == false && p_bIsSlopeSliding == false && p_bIsLock_Move == false && CheckForward_IsBlock() == false && p_bIsClimbingLadder == false;
        if (IsPossible_Move)
            p_pLogicMove.DoMove(p_fMoveVelocity, GetMoveSpeed(), SetVelocity);

        if (_bIsWallSliding && p_pRigidbody.velocity.y < p_fWallSlideSpeedMax)
            SetVelocity(new Vector2(p_pRigidbody.velocity.x, p_fWallSlideSpeedMax));

        if (bSkipMove == false && _fJumpStartTime + Time.deltaTime < Time.time)
            Logic_IsGround();
        Logic_IsBlocked();
        SetState();
    }

    private void Logic_Jump()
    {
        if (_bInputJump)
        {
            p_pLogicJump.Logic_InputJump(ref _vecJumpAddForce, ref _bInputJump);
        }
    }

    private void Logic_IsGround()
    {
        if (p_pCollider_GroundChecker == null)
        {
            p_bIsGround = false;
            return;
        }

        _pColliderGround = null;

        bool bIsGround_Prev = p_bIsGround;
        bool bIsGround = false;
        int iHitCount = Calculate_GroundCollider(ref _arrHitColliders);
        for (int i = 0; i < iHitCount; i++)
        {
            Collider2D pCollider = _arrHitColliders[i];
            if (pCollider.isTrigger == false && Check_Collider_IsTerrain(pCollider))
            {
                bIsGround = true;
                OnGround(pCollider);
                break;
            }
        }

        if (bIsGround == false && (bIsGround_Prev && p_pLogicMove.AttachGround(out _pColliderGround) == false))
            OnAirborne();
    }

    private void Logic_AddForce_Custom()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " Before Logic_AddForce_Custom _vecAddForce_Custom : " + _vecAddForce_Custom + " _bInputSetForce_Custom_Velocity_IsZero : " + _bInputSetForce_Custom_Velocity_IsZero + " _pRigidbody.velocity : " + p_pRigidbody.velocity, this);

        _bInputAddForce_Custom = false;
        if(_bInputSetForce_Custom_Velocity_IsZero)
        {
            _bInputSetForce_Custom_Velocity_IsZero = false;
            SetVelocity(Vector3.zero);
        }

        p_pRigidbody.AddForce(_vecAddForce_Custom);

        // SetState();
    }

    private void Logic_SetForce_Custom()
    {
        // Debug.Log(name + " Logic_SetForce_Custom _vecAddForce_Custom : " + _vecAddForce_Custom, this);

        _bInputSetForce_Custom = false;
        SetVelocity(_vecAddForce_Custom);
    }


    private void Logic_IsBlocked()
    {
        _listSideBlock_Wall.Clear();

        List<Collider2D> listCollider = CheckIsOverlapTerrain(_listBoxCollider_LeftCheck);
        p_bLeftDirection_IsBlocked_All = listCollider.Count == _listBoxCollider_LeftCheck.Count;
        p_bLeftDirection_IsBlocked = listCollider.Count != 0;
        _listSideBlock_Wall.AddRange(listCollider);

        listCollider = CheckIsOverlapTerrain(_listBoxCollider_RightCheck);
        p_bRightDirection_IsBlocked_All = listCollider.Count == _listBoxCollider_RightCheck.Count;
        p_bRightDirection_IsBlocked = listCollider.Count != 0;
        _listSideBlock_Wall.AddRange(listCollider);

        bool bWallSliding_Prev = _bIsWallSliding;
        if (p_bLeftDirection_IsBlocked_All || p_bRightDirection_IsBlocked_All)
            _bIsWallSliding = Check_IsWallSliding();
        else
            _bIsWallSliding = false;

        if (_bIsWallSliding)
        {
            p_bIsJumping = false;
            _fTime_WallSlidingStart = Time.time;
        }

        if(bWallSliding_Prev != _bIsWallSliding)
            p_Event_OnWallSliding.DoNotify(_bIsWallSliding);
    }

    private void OnAirborne()
    {
        if(CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log("OnAirborne");

        p_bIsGround = false;
        p_Event_OnGround.DoNotify(p_bIsGround);
        _fSlopeAngle = 0f;

        Set_SlopeSliding(false);
        SetFalling_IfCondition_IsTrue();
    }

    private void OnGround(Collider2D pColliderTerrain)
    {
        //if(p_bIsGround == false)
        //{
        //    Debug.Log(name + " OnGround", this);
        //}

        _pColliderGround = pColliderTerrain;

        bool bIsGroundOrigin = p_bIsGround;
        p_bIsGround = true;
        p_Event_OnGround.DoNotify(p_bIsGround);
        p_bIsJumping = false;

        DoSetFalling(false);
        if (_pCoroutine_Falling != null)
            StopCoroutine(_pCoroutine_Falling);
        _bCheckIsFalling = false;

        if (p_bUseSlopeSliding)
        {
            UpdateSlopeAngle(pColliderTerrain);

            if(Check_IsSlopeSliding(pColliderTerrain))
                Set_SlopeSliding(_fSlopeAngle >= p_fSlopeAngle);
        }
    }

    private void SetVelocity(Vector2 vecVelocity)
    {
        p_pRigidbody.velocity = vecVelocity;
    }

    private void Set_SlopeSliding(bool bIsSlopeSliding)
    {
        if (bIsSlopeSliding && _fSkipSlopeSlidingElapsTime < p_fSlopeSlidingDelay)
            return;

        if (bIsSlopeSliding)
            p_bFaceDirection_IsRight = _fSlopeAngle_Signed > 0f;
        else
            _fSkipSlopeSlidingElapsTime = 0;

        p_bIsSlopeSliding = bIsSlopeSliding;
        DoSet_ChangeFaceDir(p_bFaceDirection_IsRight);
    }

    private void SetState()
    {
        if(p_bIsClimbingLadder)
        {
            if(_fVerticalMoveDelta_0_1 == 0f)
                OnChangeState(ECharacterControllerState.Ladder_Hold);
            else
                OnChangeState(ECharacterControllerState.Ladder_Climbing);
            return;
        }

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

        bool bIsMoving = p_bIsMoving && p_bIsGround;
        if (bIsMoving && p_pLogicMove.p_bIsPlayAnimation_OnForwardIsBlock == false)
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
        if (gameObject.activeSelf == false || _bCheckIsFalling)
            return;

        if (Check_IsFalling())
        {
            if (_pCoroutine_Falling != null)
                StopCoroutine(_pCoroutine_Falling);
            _pCoroutine_Falling = StartCoroutine(CoDelayFalling());
        }
    }

    float _fFallingTime;

    private IEnumerator CoDelayFalling()
    {
        _bCheckIsFalling = true;
        _fFallingTime = Time.time;

        float fElapseTime = p_pLogicJump.p_fTimeToFalling;
        while (fElapseTime > 0f)
        {
            yield return null;

            if (Check_IsFalling())
                DoSetFalling(true);
            else
            {
                _bCheckIsFalling = false;
                yield break;
            }

            fElapseTime -= Time.deltaTime;
        }

        _bCheckIsFalling = false;
    }

    private IEnumerator CoDelaySetLockMove_IsFalse(float fWaitSecond)
    {
        p_bIsLock_Move = true;
        _fWaitSecond = fWaitSecond;
        while (_fWaitSecond > 0f)
        {
            _fWaitSecond -= Time.deltaTime;
            yield return null;
        }

        p_bIsLock_Move = false;
    }

    private void Init_CharacterLogic<CharacterLogic>(ref CharacterLogic pLogicBase, System.Type pType) where CharacterLogic : CCharacterController2D_LogicBase
    {
        if (pLogicBase == null)
        {
            pLogicBase = ScriptableObject.CreateInstance(pType) as CharacterLogic;
            pLogicBase.name = "Default";
        }
        else
            pLogicBase = Instantiate(pLogicBase);

        _listLogic.Add(pLogicBase);
    }

    private bool Check_IsFalling()
    {
        return p_bIsGround == false && _bIsWallSliding == false;// && p_pCalculator.p_pCollisionInfo.climbingSlope == false;
    }

    private bool CheckForward_IsBlock()
    {
        //if (p_bFaceDirection_IsRight)
        //    return CheckIsOverlapTerrain(_listBoxCollider_RightCheck).Count == _listBoxCollider_RightCheck.Count;
        //else
        //    return CheckIsOverlapTerrain(_listBoxCollider_LeftCheck).Count == _listBoxCollider_LeftCheck.Count;

        if (p_bFaceDirection_IsRight)
            return CheckIsOverlapTerrain(_listBoxCollider_RightCheck).Count != 0;
        else
            return CheckIsOverlapTerrain(_listBoxCollider_LeftCheck).Count != 0;
    }

    private List<Collider2D> CheckIsOverlapTerrain(List<BoxCollider2D> listColliderCheck)
    {
        _listColliderOverlap.Clear();
        for(int i = 0; i < listColliderCheck.Count; i++)
        {
            BoxCollider2D pColliderCheck = listColliderCheck[i];
            Collider2D pCollider = Physics2D.OverlapBox((Vector2)pColliderCheck.transform.position + pColliderCheck.offset, pColliderCheck.size, 0f, p_sWhatIsTerrain);
            if (Check_Collider_IsTerrain(pCollider) && p_pLogicLadder.Check_IsPossibleLadder(pCollider) == false)
                _listColliderOverlap.Add(pCollider);
        }

        return _listColliderOverlap;
    }

    private void UpdateSlopeAngle(Collider2D pColliderTerrain)
    {
        _fSlopeAngle = 0f;
        _fSlopeAngle_Signed = 0f;
        _vecCurrentSlopeNormal = Vector3.zero;

        int iContactCount = Physics2D.GetContacts(pColliderTerrain, p_pCollider_GroundChecker, new ContactFilter2D(), _arrContactPoint);
        for (int i = 0; i < iContactCount; i++)
        {
            ContactPoint2D sContactPoint2D = _arrContactPoint[i];
            float fSlopeAngle = Vector2.Angle(sContactPoint2D.normal, Vector2.up);
            if (fSlopeAngle != 90f && _fSlopeAngle == 0f || _fSlopeAngle > fSlopeAngle)
            {
                _fSlopeAngle = fSlopeAngle;
                _fSlopeAngle_Signed = Vector2.SignedAngle(sContactPoint2D.normal, Vector2.up);
                _vecCurrentSlopeNormal = sContactPoint2D.normal;
            }
        }
    }

    private bool CheckRayCasting_IsHit(BoxCollider2D pCollider, float fPosX, Vector2 vecDirection)
    {
        Vector2 vecPos = Vector2.zero;
        vecPos.x = fPosX;
        vecPos.y = pCollider.transform.position.y + pCollider.offset.y - (pCollider.size.y / 2f);

        if (Physics2D.Raycast(vecPos, vecDirection, pCollider.size.x, p_sWhatIsTerrain))
        {
            vecPos.y = pCollider.transform.position.y + pCollider.offset.y + (pCollider.size.y / 2f);
            if (Physics2D.Raycast(vecPos, vecDirection, pCollider.size.x, p_sWhatIsTerrain))
            {
                return true;
            }
        }

        return false;
    }

    private void Logic_OnCrouch_And_Move()
    {
        if (p_bIsCrouch && p_bIsGround)
        {
            p_pCollider_Body.offset = p_pCapsuleCollider_OnCrouch.offset;
            p_pCollider_Body.size = p_pCapsuleCollider_OnCrouch.size;

            if (_listBoxColliderLeft_OnCrouch.Count != 0)
                _listBoxCollider_LeftCheck = _listBoxColliderLeft_OnCrouch;

            if (_listBoxColliderRight_OnCrouch.Count != 0)
                _listBoxCollider_RightCheck = _listBoxColliderRight_OnCrouch;
        }
        else
        {
            p_pCollider_Body.offset = _vecOriginCollider_Offset;
            p_pCollider_Body.size = _vecOriginCollider_Size;

            if (_listBoxColliderLeft_OnCrouch.Count != 0)
                _listBoxCollider_LeftCheck = _listBoxCollider_LeftCheck_Origin;

            if (_listBoxColliderRight_OnCrouch.Count != 0)
                _listBoxCollider_RightCheck = _listBoxCollider_RightCheck_Origin;
        }
    }

    private void Logic_IsInputJump(bool bInput_Jump)
    {
        if (p_bIsLock_Jump)
            return;

        if(p_pLogicJump.Check_IsPossible_JumpThroughtDown(_pColliderGround, p_bIsGround, p_bIsCrouch, bInput_Jump))
        {
            p_pLogicJump.OnJump_ThroughtDown(p_setCharacterBody, _pColliderGround, ref _bIsFalling);
            return;
        }

        // 월 슬라이딩은 인풋을 벽 반대방향으로 누를 경우 바로 false가 되기 때문에,
        // 시간을 체크하여 월 슬라이딩을 체크한지 한 프레임이 지나지 않았을 때도 허용
        bool bCheckIsWallSliding = _bIsWallSliding || _fTime_WallSlidingStart + const_fWallJumpDuration > Time.time;
        if (p_pLogicJump.Check_IsPossible_JumpNormal(bCheckIsWallSliding, p_bIsGround, p_bIsJumping, bInput_Jump) /*&& DoCheck_IsBlock_Ceiling() == false*/)
        {
            if (bCheckIsWallSliding)
                CalculateJump_OnWallSliding(out bInput_Jump);
            else if (p_bIsSlopeSliding)
            {
                // 경사면 점프일 때에는 2단점프 이상 허용 시 경사면을 그냥 점프로 넘어갈수있다.
                CalculateJump_OnSlopeSliding(out bInput_Jump);
                p_pLogicJump.DoIncreaseJumpCount();
            }
            else if (p_bIsClimbingLadder)
                CalculateJump_OnLadder(out bInput_Jump);
            else
                p_pLogicJump.OnJump_Normal(ref _vecJumpAddForce, ref bInput_Jump);

            _bInputJump = bInput_Jump;
            p_bIsJumping = bInput_Jump;

            if (_bInputJump)
            {
                _fJumpStartTime = Time.time;
                p_pLogicJump.DoIncreaseJumpCount();
                OnAirborne();
            }
        }
    }

    private int Calculate_GroundCollider(ref Collider2D[] arrHitColliders)
    {
        if (p_pCollider_GroundChecker == null)
            return 0;

        Transform pTransformGround = p_pCollider_GroundChecker.transform;
        Vector3 vecLossyScale = pTransformGround.lossyScale; 
        Vector3 vecPosition = pTransformGround.position + CalculateVector3(vecLossyScale, p_pCollider_GroundChecker.offset);

        // 딱 Ground 위치로 하면 Physics에서 딱 Collider만큼 Fixed Update마다 밀기 때문에 충돌 체크가 안된다.
        // ㄴ 이거때문에 다른 로직이 잘 안돌아가기 때문에 일단 보류
        // vecPosition.y -= 0.05f;

        return Physics2D.OverlapCapsuleNonAlloc(vecPosition, CalculateVector3(vecLossyScale, p_pCollider_GroundChecker.size), p_pCollider_GroundChecker.direction, 0f, arrHitColliders, p_sWhatIsTerrain);
    }

    private int SortBoxCollider_Height_Greater(BoxCollider2D pBoxCollider_X, BoxCollider2D pBoxCollider_Y)
    {
        return pBoxCollider_X.transform.position.y.CompareTo(pBoxCollider_Y.transform.position.y);
    }

    private Collider2D GetCeilingOverlapCollider()
    {
        if (_pTransform_CeilingCheck == null)
            return null;

        return Physics2D.OverlapCircle(_pTransform_CeilingCheck.position, p_fCeilingRadius, p_sWhatIsTerrain);
    }

    private bool Check_Collider_IsTerrain(Collider2D pCollider)
    {
        return pCollider != null && pCollider.isTrigger == false && p_setCharacterBody.Contains(pCollider) == false;
    }

    private void Set_MoveDelta(float fMoveDelta_0_1)
    {
        if(_fMoveDelta_0_1 != fMoveDelta_0_1)
        {
            _fMoveDelta_0_1 = fMoveDelta_0_1;
            p_Event_OnMove.DoNotify(fMoveDelta_0_1);
        }
    }

#endregion Private
}