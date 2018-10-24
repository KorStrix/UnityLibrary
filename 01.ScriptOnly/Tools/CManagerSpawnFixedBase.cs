using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public interface ISpawner
{
	int ILevelSpawner_GetDifficulty();
	void ILevelSpawner_PlaySpawn();
}

public class CManagerSpawnFixedBase<Class_Manager, Enum_SpawnName, Class_Spawner, Data_Spawn> : CSingletonMonoBase<Class_Manager>
	where Class_Manager : CManagerSpawnFixedBase<Class_Manager, Enum_SpawnName, Class_Spawner, Data_Spawn>
	where Enum_SpawnName : System.IConvertible, System.IComparable
	where Class_Spawner : MonoBehaviour, IDictionaryItem<Enum_SpawnName>, ISpawner
	where Data_Spawn : class, IRandomItem, IDictionaryItem<Enum_SpawnName>
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
	private float _fSpawnDelay = 1f;
	[SerializeField]
	private int _iIncreaseDifficulty = 1;
	[SerializeField]
	private int _iIncreaseCondition_LoopCount = 5;

	/* protected - Variable declaration         */

	protected Dictionary<Enum_SpawnName, List<Class_Spawner>> _mapSpawner = new Dictionary<Enum_SpawnName, List<Class_Spawner>>();

	/* private - Variable declaration           */

	private CManagerRandomTable<Data_Spawn> _pManagerRandomTable;

	[Header( "디버그용 모니터링" )]
	[SerializeField]
	private int _iDifficulty_OnAuto;
	[SerializeField]
	private int _iDifficultyRemain;
	[SerializeField]
	private EState _eState = EState.Wait;	public EState p_eState {  get { return _eState; } }

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public void DoPlaySpawn()
	{
		DoPlaySpawn_AutoUpdate(_iDifficulty_Default);
	}

	public void DoPlaySpawn_AutoUpdate(int iDifficulty)
	{
		_iDifficulty_OnAuto = iDifficulty;
		OnSetDifficulty(iDifficulty);
		StartCoroutine(CoUpdateSpawnAuto());
	}

	public void DoSet_SpawnList( List<Data_Spawn> listDataSpawn)
	{
		_pManagerRandomTable.DoClearRandomItemTable();
		_pManagerRandomTable.DoAddRandomItem_Range( listDataSpawn );
	}

	public void DoPlaySpawnSomthing( int iDifficultyValue )
	{
		if (p_bLock_Spawn) return;

		_eState = EState.Spawning;
		int iLoopCount_Empty = 0;
		int iLoopCount_ZeroDifficulty = 0;
		_iDifficultyRemain = iDifficultyValue;
		while (_iDifficultyRemain > 0)
		{
			Data_Spawn pDataSpawn = _pManagerRandomTable.GetRandomItem( _iDifficultyRemain );
			if (pDataSpawn == null)
			{
				if (iLoopCount_Empty++ > 10)
					break;
				else
					continue;
			}
			
			Enum_SpawnName eSpawnName = pDataSpawn.IDictionaryItem_GetKey();
			if (_mapSpawner.ContainsKey( eSpawnName ) == false)
			{
				Debug.Log( "Manager Spawner " + eSpawnName + " Spawn 정보가 없습니다" );
				if (iLoopCount_Empty++ > 10)
					break;
				else
					continue;
			}

			int iSpawnDifficulty = 0;
			for (int i = 0; i < _mapSpawner[eSpawnName].Count; i++)
			{
				Class_Spawner pSpawner = _mapSpawner[eSpawnName][i];
				iSpawnDifficulty += pSpawner.ILevelSpawner_GetDifficulty();
				pSpawner.ILevelSpawner_PlaySpawn();
			}

			_iDifficultyRemain -= iSpawnDifficulty;
			if(iSpawnDifficulty == 0 && iLoopCount_ZeroDifficulty++ > 10)
			{
				Debug.LogWarning( "Manager Spawner " + eSpawnName + " Spawn Difficulty가 0인데 계속 생성합니다." );
				break;
			}
			//Debug.Log( eEnemy + "Play iGeneateCount : " + iGeneateCount + " Pattern Reain : " + _iDifficultyRemain );
		}
		_eState = EState.SpawnFinish;
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	virtual protected void OnSetDifficulty(int iDifficulty ) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		Class_Spawner[] arrSpanwer = GetComponentsInChildren<Class_Spawner>( true );
		_mapSpawner.DoAddItem( arrSpanwer );

		_pManagerRandomTable = CManagerRandomTable<Data_Spawn>.instance;
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
			if(iLoopCount++ > _iIncreaseCondition_LoopCount)
			{
				iLoopCount = 0;
				_iDifficulty_OnAuto += _iIncreaseDifficulty;
			}
			yield return SCManagerYield.GetWaitForSecond(_fSpawnDelay);
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
