#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
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

    Log_Level_1 = 1 << 1,
    Log_Level_2 = 1 << 2,
    Debug_Level_1 = 1 << 5,
    Debug_Level_2 = 1 << 6,
}


public class CObjectBase : MonoBehaviour, IUpdateAble
{
    [SerializeField]
    [Rename_Inspector("디버깅 필터")]
    protected EDebugFilter p_eDebugFilter = EDebugFilter.None;

    protected bool _bIsExcuteAwake = false;
    protected bool _bIsQuitApplciation = false;
    private Coroutine _pCoroutineOnEnable;

    // ========================== [ Division ] ========================== //

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
        //Invoke("RegistUpdateObject", 1f);
        CManagerUpdateObject.instance.DoAddObject(this);
        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);

        if (gameObject.activeSelf)
            _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());
    }

    private void RegistUpdateObject()
    {
        CManagerUpdateObject.instance.DoAddObject(this);
    }

    void OnDisable()
    {
        CManagerUpdateObject.instance.DoRemoveObject(this);
        OnDisableObject();
    }

    void Start() { OnStart(); }

    private void OnApplicationQuit()
    {
        _bIsQuitApplciation = true;
    }

    // ========================== [ Division ] ========================== //

    virtual protected void OnAwake()
    {
		if (_bIsExcuteAwake == false)
        {
            _bIsExcuteAwake = true;
            SCManagerGetComponent.DoUpdateGetComponentAttribute(this);

            if(gameObject.activeSelf && Application.isPlaying)
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

    public void OnUpdate() { bool bIsCurrentUpdating = false; OnUpdate(ref bIsCurrentUpdating); }
    /// <summary>
    /// Unity Update와 동일한 로직입니다.
    /// </summary>
    virtual public void OnUpdate(ref bool bIsCurrentUpdating) { }

    // ========================== [ Division ] ========================== //

    Dictionary<System.Action, Coroutine> _mapCoroutinePlaying = new Dictionary<System.Action, Coroutine>();

	protected IEnumerator CoDelayAction( System.Action OnAfterDelayAction, float fDelaySec )
	{
		yield return SCManagerYield.GetWaitForSecond( fDelaySec );

		OnAfterDelayAction();
		_mapCoroutinePlaying.Remove( OnAfterDelayAction );
	}    
}

#region Test
[Category("StrixLibrary")]
public class CObjectBase_테스트 : CObjectBase
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
        CObjectBase_테스트 pTarget = pObjectNew.AddComponent<CObjectBase_테스트>();
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponent);
    }

    [UnityTest]
    public IEnumerator Test_ObjectBase_GetComponentInChildren_Attribute()
    {
        GameObject pObjectNew = new GameObject();
        CObjectBase_테스트 pTargetParents = pObjectNew.AddComponent<CObjectBase_테스트>();

        CObjectBase_테스트 pTarget = pObjectNew.AddComponent<CObjectBase_테스트>();
        pTarget.transform.SetParent(pTargetParents.transform);
        pTarget.EventOnAwake();

        yield return null;

        Assert.IsNotNull(pTarget.pGetComponentParents);
    }
}
#endregion