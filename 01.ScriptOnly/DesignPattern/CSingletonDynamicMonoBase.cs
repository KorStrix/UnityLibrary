#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;

public class CSingletonDynamicMonoBase<CLASS_SingletoneTarget> : CObjectBase
    where CLASS_SingletoneTarget : CSingletonDynamicMonoBase<CLASS_SingletoneTarget>, new()
{
    static public CLASS_SingletoneTarget instance
    {
        get
        {
            if (_instance == null)
            {
                if(_bIsQuitApplication)
                {
                    try
                    {
                        _instance = new CLASS_SingletoneTarget();
                    }
                    catch
                    {

                    }
                }
                else
                {
#if UNITY_EDITOR
                    if(Application.isPlaying == false)
                        return new CLASS_SingletoneTarget();
#endif

                    _instance = FindObjectOfType<CLASS_SingletoneTarget>();
                    if (_instance == null)
                    {
                        GameObject pObjectDynamicGenerate = new GameObject(typeof(CLASS_SingletoneTarget).Name);
                        _instance = pObjectDynamicGenerate.AddComponent<CLASS_SingletoneTarget>();
                    }

                    if (_instance._bIsExcuteAwake == false)
                        _instance.OnAwake();
                }
            }

            return _instance;
        }
    }

	public GameObject p_pObjectManager { get; protected set;  }
	static protected Transform _pTransManager;

	static private CLASS_SingletoneTarget _instance;
    static protected bool _bIsQuitApplication = false;

	// ========================== [ Division ] ========================== //

	static public void DoReleaseSingleton()
	{
		if (_instance != null)
		{
			_instance.OnReleaseSingleton();
			_instance = null;
		}
	}
	
	static public void DoSetParents_ManagerObject( Transform pTransformParents )
	{
		//CManagerPooling<ENUM_Resource_Name, Class_Resource> pManagerCurrent = instance;
		//if (_pTransManager == null)
		//{
		//	_bIsInit = false; _bIsDestroy = false;
		//	pManagerCurrent.OnMakeSingleton();
		//}

		_pTransManager.SetParent( pTransformParents );
		_pTransManager.DoResetTransform();
	}

	// ========================== [ Division ] ========================== //

	virtual protected void OnReleaseSingleton() { }

	// ========================== [ Division ] ========================== //
    
    void OnDestroy()
    {
        _instance = null;
        _bIsExcuteAwake = false;

        OnReleaseSingleton();
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplication = true;
    }

    // ========================== [ Division ] ========================== //

    static public CLASS_SingletoneTarget EventMakeSingleton(bool bIsCreateNew_Force = false)
    {
        if (_bIsQuitApplication)
            return null;

        if (bIsCreateNew_Force == false && _instance != null)
            return instance;

        GameObject pObjectNewManager = new GameObject(typeof(CLASS_SingletoneTarget).ToString());
        _instance = pObjectNewManager.AddComponent<CLASS_SingletoneTarget>();
        return _instance;
    }
}
