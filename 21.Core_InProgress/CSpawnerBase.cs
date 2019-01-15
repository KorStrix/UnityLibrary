using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 패턴 담당 클래스 - 패턴 기본 로직은 여기서..
   Version	   :
   ============================================ */

public partial class CSpawnerBase<Enum_Key, Class_Resource> : CObjectBase
	where Enum_Key : System.IComparable, System.IConvertible
	where Class_Resource : Component
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public event System.Action<Enum_Key, Class_Resource> p_EVENT_OnGenerate;

	public Enum_Key p_eGenerateKey;

	[Header( "디버그용 모니터링" )]
	[SerializeField]
	private int _iGenerateRemainCount;
	[SerializeField]
	private bool _bIsGenerating = true;
	[SerializeField]
	private int _iPlayCountCurrent;

	[Header( "패턴 옵션" )]
	[SerializeField]
	public float _fDelaySec_Pattern = 0f;
	[Rename_InspectorAttribute("최소 랜덤 딜레이")]
	public float _fDelaySec_GenerateMin = 0.2f;
	[Rename_InspectorAttribute( "최대 랜덤 딜레이" )]
	public float _fDelaySec_Generate_Max = 0.2f;
	[SerializeField]
	private EMissionPatternName _ePattern = EMissionPatternName.Line;
	[SerializeField]
	private int _iPlayCount_Pattern = 1;
	[SerializeField]
	private bool _bPlayOnEnable = false;
	[SerializeField]
	private bool _bIsLoop = false;
	[SerializeField]
	private float _fAngleMuzzle = 15f;
	[SerializeField]
	private Vector2 _vecPosGap = Vector2.zero;
	[SerializeField]
	private int _iMuzzleCount = 1;
	[SerializeField]
	private int _iGenerateCount_Max = 1; public int p_iGenerateCount_Max { get { return _iGenerateCount_Max; } }
	[SerializeField]
	private int _iGenerateCount_Min = 1;	public int p_iGenerateCount_Min {  get { return _iGenerateCount_Min; } }
	[SerializeField]
	private bool _bIsAimShot = false;
	[SerializeField]
	private float _fAngleRandom = 0f;
	[SerializeField]
	private int _PlayCount_OnCircle = 1;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private CManagerPooling_InResources<Enum_Key, Class_Resource> _pManagerPool;
	private System.Func<IEnumerator> _pPattern;
	private System.Action _OnFinishPattern;

	// private float _fAdjust_GenerateDelay = 1f;
	private Vector2 _vecAdjust_GenerateScale = Vector2.one;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetAdjust_BulletScale( Vector2 vecAdjust_GenreateScale )
	{
		_vecAdjust_GenerateScale = vecAdjust_GenreateScale;
	}

	public void DoSetAdjust_GenerateDelay(float fGenerateDelayAdjust )
	{
		// _fAdjust_GenerateDelay = fGenerateDelayAdjust;
	}

	public void DoSetGenrating( bool bGenerating )
	{
		_bIsGenerating = bGenerating;
	}

	public void DoStopPattern(bool bGameObjectActiveOff = false)
	{
		_bIsGenerating = true;

		// StopAllCoroutines();
		if (bGameObjectActiveOff)
			gameObject.SetActive(false);
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual protected void OnPatternPlay() { }
	virtual protected void OnPatternStop()
	{
		if (_OnFinishPattern != null)
		{
			_OnFinishPattern();
			_OnFinishPattern = null;
		}
	}

	virtual protected void OnGenerateSomthing( Enum_Key eKey, Class_Resource pResource) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pManagerPool = CManagerPooling_InResources<Enum_Key, Class_Resource>.instance;
	}
	
	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		if (_bPlayOnEnable)
			Invoke( "DoPlayPattern", 0.5f );
			//DoPlayPattern();
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if(Application.isEditor)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube( transform.position, Vector3.one * 0.5f );
			Gizmos.DrawLine( transform.position, transform.position + transform.up * 1f );
			UnityEditor.Handles.Label( transform.position, name );
		}
	}
#endif
	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	virtual protected void ProcShotGenerate( Vector3 vecBulletPos, Quaternion rotBulletAngle )
	{
		Class_Resource pResource = _pManagerPool.DoPop( p_eGenerateKey );
		pResource.transform.position = vecBulletPos;
		pResource.transform.rotation = rotBulletAngle;
		pResource.transform.localScale = _vecAdjust_GenerateScale;

		if (_fAngleRandom != 0f)
		{
			float fRandomAngleZ = Random.Range( -_fAngleRandom, _fAngleRandom );
			pResource.transform.Rotate( 0f, 0f, fRandomAngleZ );
		}

		if (p_EVENT_OnGenerate != null)
			p_EVENT_OnGenerate( p_eGenerateKey, pResource );
		OnGenerateSomthing( p_eGenerateKey, pResource );
	}

	private IEnumerator CoPlayPattern( bool bIsLoop )
	{
		if (bIsLoop)
		{
			while (true)
			{
				if(gameObject.activeSelf)
				{
					OnPatternPlay();
					StartCoroutine( CoLookAtTarget() );
					yield return StartCoroutine( _pPattern() );
					yield return SCManagerYield.GetWaitForSecond( _fDelaySec_Pattern );
					OnPatternStop();
				}
			}
		}
		else
		{
			_iPlayCountCurrent = _iPlayCount_Pattern;
			while (_iPlayCountCurrent-- > 0)
			{
				if (gameObject.activeSelf)
				{
					OnPatternPlay();
					StartCoroutine( CoLookAtTarget() );
					yield return StartCoroutine( _pPattern() );
					yield return SCManagerYield.GetWaitForSecond( _fDelaySec_Pattern );
					OnPatternStop();
				}
			}
		}
	}

	// ========================================================================== //

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	private void ProcPlayPattern()
	{
		_iGenerateRemainCount = Random.Range( _iGenerateCount_Min, _iGenerateCount_Max );
		StopAllCoroutines();
		if(gameObject.activeInHierarchy)
			StartCoroutine( CoPlayPattern( _bIsLoop ) );
	}

	private IEnumerator CoGenerateSomthing()
	{
		while (_bIsLoop || _iGenerateRemainCount > 0)
		{
			if (_bIsGenerating)
			{
				_iGenerateRemainCount--;
				ProcShotGenerate(transform.position, transform.rotation);
			}

			float fDelaySecRandom = Random.Range( _fDelaySec_GenerateMin, _fDelaySec_Generate_Max );
			yield return SCManagerYield.GetWaitForSecond( fDelaySecRandom );
		}
	}

	private IEnumerator CoGenerateSomthing( Vector3 vecPosGap, Vector3 vecAngleGap)
	{
		while (_bIsLoop || _iGenerateRemainCount > 0)
		{
			if (_bIsGenerating)
			{
				_iGenerateRemainCount--;
				ProcShotGenerate(transform.position + vecPosGap, Quaternion.Euler(transform.rotation.eulerAngles + vecAngleGap));
			}

			float fDelaySecRandom = Random.Range( _fDelaySec_GenerateMin, _fDelaySec_Generate_Max );
			yield return SCManagerYield.GetWaitForSecond( fDelaySecRandom );
		}
	}

	private void ProcSetPatternIsFinish()
	{
		//Debug.Log("ProcSetPatternIsFinish");
		StopCoroutine(_pPattern());
	}

#if NGUI
	private CNGUITweenRotationSpin GetTweenRotate()
	{
		CNGUITweenRotationSpin pTweenRotate = GetComponent<CNGUITweenRotationSpin>();
		if (pTweenRotate == null)
			pTweenRotate = gameObject.AddComponent<CNGUITweenRotationSpin>();

		pTweenRotate.enabled = true;
		pTweenRotate.ResetToBeginning();

		return pTweenRotate;
	}
#endif

	private IEnumerator CoLookAtTarget()
	{
		if (_bIsAimShot == false)
			yield break;

		//PCMission_Player pPlayer = PCManagerInMission.instance.p_pPlayer;
		//if(pPlayer != null)
		//{
		//	Transform pTransformTarget = pPlayer.transform;
		//	while (true)
		//	{
		//		_pTransformCached.up = pTransformTarget.position - _pTransformCached.position;
		//		yield return null;
		//	}
		//}
	}
}
