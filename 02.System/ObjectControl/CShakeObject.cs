using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CShakeObject : CObjectBase
{
    public enum EShakePos
    {
        All,
        X,
        XY,
        XZ,
        Y,
        YZ,
        Z
    }

    [SerializeField]
    [DisplayName("흔들리기 적용위치")]
    private EShakePos _eShakePosType = EShakePos.All;
    [SerializeField]
    [DisplayName("기본 흔드는 힘")]
    private float _fDefaultShakePow = 1f;
    [SerializeField]
    [DisplayName("흔드는 힘을 깎는 양")]
    private float _fShakeMinusDelta = 0.1f;
	//[SerializeField]
	//private bool _bMachineShaking = false;

	private Vector3 _vecOriginPos;
    private float _fRemainShakePow;

    bool _bIsShaking;

	// ========================== [ Division ] ========================== //

	public void DoSetShakeOnMobileShake(bool bEnable)
	{
		//_bMachineShaking = bEnable;
	}

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Button("Test Shake")]
#endif
    public void DoShakeObject()
    {
        DoShakeObject(_fDefaultShakePow);
    }

    public void DoShakeObject(float fShakePow)
    {
        if(_bIsShaking)
            transform.localPosition = _vecOriginPos;

        _bIsShaking = true;
        _fRemainShakePow = fShakePow;
        _vecOriginPos = transform.localPosition;
    }

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        _vecOriginPos = Vector3.one * float.MaxValue; // 더미값
    }

    private void Update()
    {
        if(_bIsShaking)
        {
            if (_fRemainShakePow > 0f)
            {
                Vector3 vecShakePos = PrimitiveHelper.RandomRange(_vecOriginPos.AddFloat(-_fRemainShakePow), _vecOriginPos.AddFloat(_fRemainShakePow));
                if (_eShakePosType != EShakePos.All)
                {
                    if (_eShakePosType == EShakePos.Y || _eShakePosType == EShakePos.YZ || _eShakePosType == EShakePos.Z)
                        vecShakePos.x = _vecOriginPos.x;

                    if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XZ || _eShakePosType == EShakePos.Z)
                        vecShakePos.y = _vecOriginPos.y;

                    if (_eShakePosType == EShakePos.X || _eShakePosType == EShakePos.XY || _eShakePosType == EShakePos.Y)
                        vecShakePos.z = _vecOriginPos.z;
                }

                transform.localPosition = vecShakePos;
                _fRemainShakePow -= _fShakeMinusDelta;
            }
            else
            {
                transform.localPosition = _vecOriginPos;
                _bIsShaking = false;
            }
        }
    }
}
