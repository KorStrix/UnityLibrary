using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */
   
abstract public class CManagerSpawnBase<Class_Manager, Enum_SpawnName, Class_SpawnTarget> : CSingletonMonoBase<Class_Manager>
	where Class_Manager : CManagerSpawnBase<Class_Manager, Enum_SpawnName, Class_SpawnTarget>
	where Enum_SpawnName : System.IConvertible, System.IComparable
	where Class_SpawnTarget : Component
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum EState
	{
		Wait,
		Spawning,
		SpawnFinish,
	}

	/* public - Variable declaration            */

	public bool p_bLock_Spawn = false;

	[SerializeField]
	private int _iDifficulty_Default = 1;
	[SerializeField]
	private float _fSpawnDelay_Min = 1f;
	[SerializeField]
	private float _fSpawnDelay_Max = 1f;
	[SerializeField]
	private int _iIncreaseDifficulty = 1;
	[SerializeField]
	private int _iIncreaseCondition_LoopCount = 5;

	[SerializeField]
	private bool _bIsAutoIncreseDifficulty = false;

	/* protected - Variable declaration         */

	protected List<CSpawnerBase<Enum_SpawnName, Class_SpawnTarget>> _listSpawner = new List<CSpawnerBase<Enum_SpawnName, Class_SpawnTarget>>();

	/* private - Variable declaration           */

	[Header( "디버그용 모니터링" )]
	[SerializeField]
	private int _iDifficulty_OnAuto;
	[SerializeField]
	private EState _eState = EState.Wait;	public EState p_eState {  get { return _eState; } }

	private float _fDelayOffset = 1f;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoSetDelayOffset(float fDelayOffset)
	{
		_fDelayOffset = fDelayOffset;
	}

	public void DoStopSpawning()
	{
		_eState = EState.Wait;
		StopCoroutine( "CoUpdateSpawnAuto" );
	}

	public void DoPlaySpawn()
	{
		DoPlaySpawn_AutoUpdate(_iDifficulty_Default);
	}

	public void DoPlaySpawn_AutoUpdate(int iDifficulty)
	{
		_iDifficulty_OnAuto = iDifficulty;
		OnSetDifficulty(iDifficulty);

		StopCoroutine( "CoUpdateSpawnAuto" );
		StartCoroutine( "CoUpdateSpawnAuto");
	}

	public void DoPlaySpawnSomthing( int iDifficultyValue )
	{
		if (p_bLock_Spawn) return;

		_eState = EState.Spawning;

		Enum_SpawnName eSpawnName = OnSpawnSomthing(iDifficultyValue);
		for (int i = 0; i < iDifficultyValue; i++)
		{
			int iRandomIndex = Random.Range( 0, _listSpawner.Count );
			CSpawnerBase<Enum_SpawnName, Class_SpawnTarget> pSpawner = _listSpawner[iRandomIndex];
			pSpawner.p_eGenerateKey = eSpawnName;
			pSpawner.DoPlayPattern();
		}
		_eState = EState.SpawnFinish;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	abstract protected Enum_SpawnName OnSpawnSomthing(int iDifficultyRemain);
	abstract protected void OnSpawnSomthing_After( Enum_SpawnName eSpawnName, Class_SpawnTarget pSpawnTarget );
	virtual protected void OnSetDifficulty(int iDifficulty ) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		GetComponentsInChildren( true, _listSpawner );
		for(int i = 0; i < _listSpawner.Count; i++)
			_listSpawner[i].p_EVENT_OnGenerate += CManagerSpawnBase_p_EVENT_OnGenerate;
	}

	private void CManagerSpawnBase_p_EVENT_OnGenerate( Enum_SpawnName arg1, Class_SpawnTarget arg2 )
	{
		OnSpawnSomthing_After( arg1, arg2 );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoUpdateSpawnAuto()
	{
		int iLoopCount = 0;
		while (true)
		{
			DoPlaySpawnSomthing(_iDifficulty_OnAuto);
			if(_bIsAutoIncreseDifficulty && iLoopCount++ > _iIncreaseCondition_LoopCount)
			{
				iLoopCount = 0;
				_iDifficulty_OnAuto += _iIncreaseDifficulty;
			}

			float fRandomDelay = Random.Range( _fSpawnDelay_Min, _fSpawnDelay_Max ) * _fDelayOffset;
			yield return SCManagerYield.GetWaitForSecond( fRandomDelay );
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
