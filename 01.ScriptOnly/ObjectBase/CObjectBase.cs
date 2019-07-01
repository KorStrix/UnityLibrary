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

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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
    public class Object_Activate_Arg
    {
        public CObjectBase pScript;
        public GameObject pGameObject;
        public bool bActive;

        public Object_Activate_Arg(CObjectBase pScript, GameObject pGameObject, bool bActivate)
        {
            this.pScript = pScript;
            this.pGameObject = pGameObject;
            this.bActive = bActivate;
        }
    }


    [DisplayName("디버깅 필터")]
    public EDebugFilter p_eDebugFilter = EDebugFilter.None;

    public ObservableCollection<Object_Activate_Arg> p_Event_OnActivate { get; private set; } = new ObservableCollection<Object_Activate_Arg>();

    protected bool _bIsExcuteAwake = false;
    protected bool _bIsQuitApplciation = false;
    private Coroutine _pCoroutineOnEnable;

    new public bool enabled
    {
        get { return base.enabled; }
        set { SetEnable_Script(value); }
    }

    bool _bIsDestroy = false;

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

        if(this != null)
            base.enabled = bEnable;
    }

#if ODIN_INSPECTOR
    [ShowIf(nameof(CheckIs_DrawDebugButton))]
    [ButtonGroup("Debug_1", Order = 100)]
    [InfoBox("Debug Button")]
    [Button("Awake", ButtonSizes.Large)]
#endif
    public void EventOnAwake()
    {
        if (_bIsExcuteAwake == false)
            OnAwake();
    }

#if ODIN_INSPECTOR
    [ButtonGroup("Debug_1", Order = 100)]
    [ShowIf(nameof(CheckIs_DrawDebugButton))]
    [Button("Awake Force", ButtonSizes.Large)]
#endif
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
        CManagerUpdateObject.instance?.DoAddObject(this);

        OnEnableObject();
        if (_pCoroutineOnEnable != null)
            StopCoroutine(_pCoroutineOnEnable);

        if (gameObject.activeSelf)
            _pCoroutineOnEnable = StartCoroutine(OnEnableObjectCoroutine());

        p_Event_OnActivate.DoNotify(new Object_Activate_Arg(this, gameObject, true));
    }

    void OnDisable()
    {
        OnDisableObject(_bIsQuitApplciation);

        p_Event_OnActivate.DoNotify(new Object_Activate_Arg(this, gameObject, false));
    }

    void Start()
    {
        OnStart();
    }


    private void OnDestroy()
    {
        _bIsDestroy = true;

        CManagerUpdateObject.instance?.DoRemoveObject(this);
        OnDestroyObject(_bIsQuitApplciation);
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplciation = true;
    }

    public void IUpdateAble_GetUpdateInfo(ref bool bIsUpdate_Default_IsFalse, ref float fTimeScale_Invidiaul_Default_IsOne)
    {
        bIsUpdate_Default_IsFalse = this != null && gameObject.activeSelf;
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
                StopCoroutine(nameof(OnAwakeCoroutine));
                StartCoroutine(nameof(OnAwakeCoroutine));
            }
        }
    }

    virtual protected void OnReset() { }
    virtual protected void OnStart() { }
    virtual protected void OnEnableObject() {}

    virtual protected IEnumerator OnAwakeCoroutine() { yield break; }
    virtual protected IEnumerator OnEnableObjectCoroutine() { yield break; }
    virtual protected void OnDisableObject(bool bIsQuitApplciation) { }
    virtual protected void OnDestroyObject(bool bIsQuitApplciation) { }


    /// <summary>
    /// Unity Update와 동일한 로직입니다.
    /// </summary>
    public void OnUpdate()
    {
        OnUpdate(1f);
    }

    /// <summary>
    /// Unity Update와 동일합니다. Manager에서 컴포넌트 별 TimeScale을 관리합니다.
    /// </summary>
    /// <param name="fTimeScale_Individual">각 컴포넌트 별 TimeScale입니다.</param>
    virtual public void OnUpdate(float fTimeScale_Individual) { }

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

    protected bool CheckIs_DrawDebugButton()
    {
        return p_eDebugFilter != EDebugFilter.None;
    }

    protected bool CheckIs_DrawDebugButton_And_PlayModeOnly()
    {
        return p_eDebugFilter != EDebugFilter.None && Application.isPlaying;
    }
}