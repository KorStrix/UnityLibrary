using UnityEngine;
using System.Collections;

public class CFollowObject : CObjectBase
{
    public enum EFollowPos
    {
        All,
        X,
        XY,
        XZ,
        Y,
        YZ,
        Z
    }

    public enum EFollowMode
    {
        ControlOutside,
        FixedUpdate,
        Update,   
    }

    public event System.Action<bool> p_Event_OnArrive_SmoothFollow;

	[Header("쫓아가기 옵션")]
    [SerializeField]
    private EFollowPos _eFollowPos = EFollowPos.All;
    [SerializeField]
    private Transform _pTransTarget = null;

    [SerializeField]
    private bool _bUseDistanceOffset = true;

    public EFollowMode p_eFollowMode = EFollowMode.Update;

    [Header("부드러운 따라가기 관련 옵션")]
    [Rename_Inspector("부드러운 따라가기를 할건지")]
	public bool p_bIsSmoothFollow = false;

    [Rename_Inspector("부드러운 따라가기를 했을 때 도착 판정 거리")]
    public float p_fCondition_ArriveDistance = 1f;

    [Rename_Inspector("스무스 팔로우 모드일 때 현재 거리", false)]
    [SerializeField]
    private float _fArriveDistance;

    //[Rename_Inspector("스무스 팔로우 모드일 때 근접했는지", false)]
    //[SerializeField]
    //private bool _bArrive_OnSmooth;

    [SerializeField] [Range(0, 1f)]
	private float _fSmoothFollowDelta = 0.1f;

	[Header("흔들기 옵션")]
	[SerializeField]
    private float _fShakeMinusDelta = 0.1f;

    [Rename_Inspector("현재 따라가는 중인지", false)]
    [SerializeField]
	private bool _bIsFollow = false;

	private Vector3 _vecAwakePos;
	private Vector3 _vecOriginPos;
    private Vector3 _vecTargetOffset;
    private float _fRemainShakePow;

    private bool _bFollowX;
    private bool _bFollowY;
    private bool _bFollowZ;

    // ========================== [ Division ] ========================== //

	public void DoSetPos_OnAwake()
	{
		transform.position = _vecAwakePos;
	}

    public void DoShakeObject(float fShakePow)
    {
        if(_fRemainShakePow <= 0f)
        {
            //Debug.Log("Shake Start CurrentPos : "  + _pTransformCashed.position + " Offset : " + _vecTargetOffset);
            _vecOriginPos = transform.position;
        }

        _fRemainShakePow = fShakePow;
    }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button]
    public void DoInitTarget(Transform pTarget)
    {
        _pTransTarget = pTarget;
		DoResetFollowOffset();
	}
#endif

    public void DoResetFollowOffset()
	{
		if (_pTransTarget == null) return;

        if (_bUseDistanceOffset)
            _vecTargetOffset = _pTransTarget.position - transform.position;
        else
            _vecTargetOffset = Vector3.zero;

        _bFollowX = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.X || _eFollowPos == EFollowPos.XY || _eFollowPos == EFollowPos.XZ;
		_bFollowY = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.Y || _eFollowPos == EFollowPos.XY || _eFollowPos == EFollowPos.YZ;
		_bFollowZ = _eFollowPos == EFollowPos.All || _eFollowPos == EFollowPos.Z || _eFollowPos == EFollowPos.XZ || _eFollowPos == EFollowPos.YZ;
	}

	public void DoSetFollow(bool bFollow)
	{
		_bIsFollow = bFollow;
	}

    public void DoUpdateFollow()
    {
		if (_bIsFollow == false) return;
		if (_pTransTarget == null) return;

        Vector3 vecFollowPos = transform.position;
        Vector3 vecTargetPos = _pTransTarget.position;

		if(p_bIsSmoothFollow)
			ProcFollow_Smooth( ref vecFollowPos, vecTargetPos );
		else
			ProcFollow_Normal( ref vecFollowPos, vecTargetPos );

		vecFollowPos = ProcShake( vecFollowPos );
        transform.position = vecFollowPos;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

		_vecAwakePos = transform.position;
		if (_pTransTarget != null)
		{
			DoInitTarget( _pTransTarget );
			DoSetFollow( true );
		}
	}

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (p_eFollowMode == EFollowMode.Update)
        {
            DoUpdateFollow();
        }
    }

    private void FixedUpdate()
    {
        if (p_eFollowMode == EFollowMode.FixedUpdate)
            DoUpdateFollow();
    }

	// ========================== [ Division ] ========================== //

	private void ProcFollow_Smooth( ref Vector3 vecFollowPos, Vector3 vecTargetPos )
	{
		Vector3 vecDestPos = vecFollowPos;
		ProcFollow_Normal( ref vecDestPos, vecTargetPos );
		vecFollowPos = Vector3.Lerp(transform.position, vecDestPos, _fSmoothFollowDelta );
        _fArriveDistance = Vector3.Distance(vecFollowPos, vecDestPos);

        bool bArrive = _fArriveDistance <= p_fCondition_ArriveDistance;
        // _bArrive_OnSmooth = bArrive;
        if (p_Event_OnArrive_SmoothFollow != null)
            p_Event_OnArrive_SmoothFollow(bArrive);
    }

	private void ProcFollow_Normal( ref Vector3 vecFollowPos, Vector3 vecTargetPos )
	{
		if (_eFollowPos != EFollowPos.All)
		{
			if (_bFollowX)
				vecFollowPos.x = vecTargetPos.x - _vecTargetOffset.x;

			if (_bFollowY)
				vecFollowPos.y = vecTargetPos.y - _vecTargetOffset.y;

			if (_bFollowZ)
				vecFollowPos.z = vecTargetPos.z - _vecTargetOffset.z;
		}
		else
			vecFollowPos = vecTargetPos - _vecTargetOffset;
	}

	private Vector3 ProcShake(Vector3 vecFollowPos )
	{
		if (_fRemainShakePow > 0f)
		{
			_fRemainShakePow -= _fShakeMinusDelta;
			if (_fRemainShakePow <= 0f)
			{
				if (_bFollowX == false)
					vecFollowPos.x = _vecOriginPos.x;

				if (_bFollowY == false)
					vecFollowPos.y = _vecOriginPos.y;

				if (_bFollowZ == false)
					vecFollowPos.z = _vecOriginPos.z;
			}
			else
			{
				Vector3 vecShakePos = PrimitiveHelper.RandomRange( vecFollowPos.AddFloat( -_fRemainShakePow ), vecFollowPos.AddFloat( _fRemainShakePow ) );
				//if (_bFollowX) vecFollowPos.x = vecShakePos.x;
				//if (_bFollowY) vecFollowPos.y = vecShakePos.y;
				//if (_bFollowZ) vecFollowPos.z = vecShakePos.z;

				vecFollowPos = vecShakePos;
			}
		}

		return vecFollowPos;
	}
}
