using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-04 오후 5:54:23
   Description : 
   Edit Log    : 
   ============================================ */

public class CSingletonNotMonoBase<CLASS_SingletoneTarget>
    where CLASS_SingletoneTarget : CSingletonNotMonoBase<CLASS_SingletoneTarget>, new()
{
	static private Dictionary<int, CLASS_SingletoneTarget> _mapInstanceMultiple = new Dictionary<int, CLASS_SingletoneTarget>();
	static private CLASS_SingletoneTarget _instance;

	// ========================== [ Division ] ========================== //

	static public CLASS_SingletoneTarget instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CLASS_SingletoneTarget();
				_instance.OnMakeSingleton();
			}

			return _instance;
		}
	}

	public CLASS_SingletoneTarget this[int iID]
	{
		get
		{
			if (_mapInstanceMultiple.ContainsKey( iID ) == false)
			{
				_mapInstanceMultiple.Add( iID, new CLASS_SingletoneTarget() );
				_mapInstanceMultiple[iID].OnMakeSingleton();
			}

			return _mapInstanceMultiple[iID];
		}
	}

	static public void DoReleaseSingleton()
	{
		if(_instance != null)
		{
			_instance.OnReleaseSingleton();
			_instance = null;
		}
	}

	// ========================== [ Division ] ========================== //

	virtual protected void OnMakeSingleton() { }
	virtual protected void OnReleaseSingleton() { }

	protected void EventSetInstance(CLASS_SingletoneTarget pInstanceSet)
	{
		_instance = pInstanceSet;
	}
}