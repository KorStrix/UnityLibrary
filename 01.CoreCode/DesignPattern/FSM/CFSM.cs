using UnityEngine;
using System.Collections;

public class CFSM<ENUM_STATE>
    where ENUM_STATE : System.IConvertible, System.IComparable
{
    public event System.Action<ENUM_STATE> p_EVENT_OnChangeState;
        
    private CDictionary_ForEnumKey<ENUM_STATE, System.Action> _mapCallBackOnStart = new CDictionary_ForEnumKey<ENUM_STATE, System.Action>();
    //private CDictionary_ForEnumKey<ENUM_STATE, EventDelegate.Callback> _mapCallBackOnUpdate = new CDictionary_ForEnumKey<ENUM_STATE, EventDelegate.Callback>();

    //private MonoBehaviour _pOwnerClass;

    private ENUM_STATE _eStateCurrent;      public ENUM_STATE p_eCurrentState {  get { return _eStateCurrent; } }
	private ENUM_STATE _eStatePrev;			public ENUM_STATE p_eStatePrev {  get { return _eStatePrev; } }

	//private bool _bUpdateCallExist;

    // ========================== [ Division ] ========================== //

    public void DoInitFSM(MonoBehaviour pOwnerClass, ENUM_STATE eCurrentState)
    {
        //_pOwnerClass = pOwnerClass;
        _eStateCurrent = eCurrentState;

        DoChangeState(eCurrentState);
	}

    public void DoAddState(ENUM_STATE eState, System.Action OnStartState)
    {
		_mapCallBackOnStart.Add(eState, OnStartState);
    }

 //   public void DoAddState(ENUM_STATE eState, EventDelegate.Callback OnStartState, EventDelegate.Callback OnUpdateState)
 //   {
	//	_mapCallBackOnStart.Add(eState, OnStartState);
	//	_mapCallBackOnUpdate.Add(eState, OnUpdateState);
	//}

	public void DoChangeState(ENUM_STATE eState)
    {
		_eStatePrev = _eStateCurrent;
		_eStateCurrent = eState;

		if (p_EVENT_OnChangeState != null)
            p_EVENT_OnChangeState(eState);

		if (_mapCallBackOnStart.ContainsKey(eState))
			_mapCallBackOnStart[eState]();

		//if (_mapCallBackOnUpdate.ContainsKey(eState))
		//	_mapCallBackOnUpdate[eState]();

		//_bUpdateCallExist = _mapCallBackOnUpdate.ContainsKey(eState);
	}

	// ========================== [ Division ] ========================== //

    //private IEnumerator CoFSMUpdate()
    //{
    //    while(true)
    //    {
    //        if (_bUpdateCallExist)
    //            _mapCallBackOnUpdate[_eStateCurrent]();
            
    //        yield return null;
    //    }
    //}
}
