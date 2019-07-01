#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
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

public class CSingletonDynamicMonoBase<CLASS_SingletoneTarget> :
#if STRIX_LIBRARY
    CObjectBase
#else
    MonoBehaviour, IUpdateAble
#endif

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
                    return null;
                }
                else
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                        return null;
                        // return new CLASS_SingletoneTarget(); // Exception 방지를 위한 코드, 어차피 Editor에서 PlayMode -> EditMode로 돌아가는 과정이라 App 성능에 영향가지 않는다.
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
		_pTransManager.SetParent( pTransformParents );
        _pTransManager.transform.localScale = Vector3.one;
        _pTransManager.transform.localRotation = Quaternion.identity;
        _pTransManager.transform.position = Vector3.zero;
    }

    // ========================== [ Division ] ========================== //

    virtual protected void OnReleaseSingleton() { }

    // ========================== [ Division ] ========================== //

#region NotSupport_StrixLibrary
#if !STRIX_LIBRARY

    protected bool _bIsExcuteAwake = false;
    protected bool _bIsQuitApplciation = false;
    private Coroutine _pCoroutineOnEnable;

    void Awake()
    {
        if (_bIsExcuteAwake == false)
            OnAwake();
    }

    void OnEnable()
    {
        CManagerUpdateObject.instance?.DoAddObject(this);

        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);

        if (gameObject.activeSelf)
            _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());
    }

    protected virtual void OnAwake()
    {
        _bIsExcuteAwake = true;
    }

    virtual protected void OnEnableObject() { }
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual public void OnUpdate(float fTimeScale_Individual) { }

    public void IUpdateAble_GetUpdateInfo(ref bool bIsUpdate_Default_IsFalse, ref float fTimeScale_Invidiaul_Default_IsOne)
    {
        bIsUpdate_Default_IsFalse = this != null && gameObject.activeSelf;
    }

#endif
    #endregion

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
