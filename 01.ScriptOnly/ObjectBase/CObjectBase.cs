#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : 
 *	GetComponentAttribute를 지원합니다. - GetComponentAttribute.cs 필요
 *	Update 퍼포먼스가 향상됩니다.        - CManagerUpdateObject.cs 필요
 *	Awake, Enable 코루틴을 지원합니다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using UnityEngine.TestTools;

[System.Flags]
public enum EDebugFilter
{
    None = 0,

    Debug_Level_LowLevel = 1 << 1,
    Debug_Level_App = 1 << 2,
    Debug_Level_Core = 1 << 3,
}



public class CObjectBase :
#if ODIN_INSPECTOR
    Sirenix.OdinInspector.SerializedMonoBehaviour, IUpdateAble
#else
    MonoBehaviour, IUpdateAble
#endif

{
    [SerializeField]
    [Rename_Inspector("디버깅 필터")]
    protected EDebugFilter p_eDebugFilter = EDebugFilter.None;

    public CObserverSubject<CObjectBase, GameObject, bool> p_Event_OnActivate { get; private set; } = new CObserverSubject<CObjectBase, GameObject, bool>();

    protected bool _bIsExcuteAwake = false;
    protected bool _bIsQuitApplciation = false;
    private Coroutine _pCoroutineOnEnable;

    new public bool enabled
    {
        get { return base.enabled; }
        set { SetEnable_Script(value); }
    }

    bool _bIsDestroy =false;

    // ========================== [ Division ] ========================== //

    public void SetActive(bool bActive)
    {
        if (_bIsDestroy)
            return;

        if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " SetActive_GameObject : " + bActive, this);

        gameObject.SetActive(bActive);
    }

    public void SetEnable_Script(bool bEnable)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_LowLevel))
            Debug.Log(name + " SetEnable_Script : " + bEnable, this);

        base.enabled = bEnable;
    }

    public void EventOnAwake()
    {
        if (_bIsExcuteAwake == false)
            OnAwake();
    }

    public void EventOnAwake_Force()
    {
        _bIsExcuteAwake = false;
        OnAwake();
    }

    public void EventExcuteDelay(System.Action OnAfterDelayAction, float fDelaySec)
    {
        if (fDelaySec == 0f)
            OnAfterDelayAction();
        else
        {
            if (this != null && gameObject.activeInHierarchy)
            {
                if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction))
                    StopCoroutine(_mapCoroutinePlaying[OnAfterDelayAction]);

                Coroutine pCoroutine = StartCoroutine(CoDelayAction(OnAfterDelayAction, fDelaySec));
                if (_mapCoroutinePlaying.ContainsKey(OnAfterDelayAction) == false)
                    _mapCoroutinePlaying.Add(OnAfterDelayAction, pCoroutine);
            }
        }
    }

    public void EventStop_ExcuteDelayAll()
    {
        foreach (var pCoroutine in _mapCoroutinePlaying.Values)
            StopCoroutine(pCoroutine);

        _mapCoroutinePlaying.Clear();
    }

    // ========================== [ Division ] ========================== //

    void Reset()
    {
        if (Application.isEditor && Application.isPlaying == false)
            OnReset();
    }

    void Awake()
    {
        if(_bIsExcuteAwake == false)
            OnAwake();
    }

    void OnEnable()
    {
        CManagerUpdateObject.instance.DoAddObject(this);
        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);

        if (gameObject.activeSelf)
            _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());

        p_Event_OnActivate.DoNotify(this, gameObject, true);
    }

    private void RegistUpdateObject()
    {
        CManagerUpdateObject.instance.DoAddObject(this);
    }

    void OnDisable()
    {
        OnDisableObject();

        p_Event_OnActivate.DoNotify(this, gameObject, false);
    }

    void Start()
    {
        OnStart();
    }



    private void OnDestroy()
    {
        _bIsDestroy = true;

        CManagerUpdateObject.instance.DoRemoveObject(this);
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplciation = true;
    }

    public bool IUpdateAble_IsRequireUpdate()
    {
        return this != null && gameObject.activeSelf;
    }

    // ========================== [ Division ] ========================== //

    virtual protected void OnAwake()
    {
		if (_bIsExcuteAwake == false)
        {
            _bIsExcuteAwake = true;
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);

            if(gameObject.activeInHierarchy && Application.isPlaying)
            {
                StopCoroutine("OnAwakeCoroutine");
                StartCoroutine("OnAwakeCoroutine");
            }
        }
    }

    virtual protected void OnReset() { }
    virtual protected void OnStart() { }
    virtual protected void OnEnableObject() {}

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual protected void OnDisableObject() { }

    /// <summary>
    /// Unity Update와 동일한 로직입니다.
    /// </summary>
    virtual public void OnUpdate() { }

    // ========================== [ Division ] ========================== //

    Dictionary<System.Action, Coroutine> _mapCoroutinePlaying = new Dictionary<System.Action, Coroutine>();

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return YieldManager.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
		_mapCoroutinePlaying.Remove( OnAfterDelayAction );
    }

    public bool CheckDebugFilter(EDebugFilter eDebugFilter)
    {
        return p_eDebugFilter.ContainEnumFlag(eDebugFilter);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DoPrintLog(EDebugFilter eDebugFilter, string strLog)
    {
        if(CheckDebugFilter(eDebugFilter))
            Debug.Log(strLog);
    }
}

#region Test
[Category("StrixLibrary")]
public class CObjectBase_Test : CObjectBase
{
    [GetComponent]
    [HideInInspector]
    public CObjectBase pGetComponent;

    [GetComponentInParent]
    [HideInInspector]
    public CObjectBase pGetComponentParents;

    [UnityTest]
    public IEnumerator Test_ObjectBase_GetComponent_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        CObjectBase_Test pTarget = pObjectNew.AddComponent<CObjectBase_Test>();
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponent);
    }

    [UnityTest]
    public IEnumerator Test_ObjectBase_GetComponentInChildren_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        CObjectBase_Test pTargetParents = pObjectNew.AddComponent<CObjectBase_Test>();

        CObjectBase_Test pTarget = pObjectNew.AddComponent<CObjectBase_Test>();
        pTarget.transform.SetParent(pTargetParents.transform);
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponentParents);
    }
}
#endregion