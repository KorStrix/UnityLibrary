#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-06-17 오후 5:35:59
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.Utilities;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

// 인스펙터 노출을 위한 오브젝트
public class CSOAttacher : MonoBehaviour
{
    static public bool g_bIsQuit { get; private set; }

    public ScriptableObject p_pOwnerSO;

    private void OnApplicationQuit()
    {
        g_bIsQuit = true;
    }
}


#if ODIN_INSPECTOR
abstract public class CSingletonSOBase<T> : GlobalConfig<T>
    where T : CSingletonSOBase<T>, new()
{
    static protected CSOAttacher _pObjectAttacher;

    public static T instance
    {
        get
        {
            if (_pObjectAttacher == null && Application.isPlaying)
            {
                if (CSOAttacher.g_bIsQuit == false)
                {
                    GameObject pObjectManager = new GameObject(typeof(T).Name, typeof(CSOAttacher));
                    _pObjectAttacher = pObjectManager.GetComponent<CSOAttacher>();
                    _pObjectAttacher.p_pOwnerSO = GlobalConfig<T>.Instance;
                }

                GlobalConfig<T>.Instance.OnAwake(true);
            }

            return GlobalConfig<T>.Instance;
        }
    }

    public static new T Instance
    {
        get
        {
            return instance;
        }
    }

#else
abstract public class CSingletonSOBase<T> : ScriptableObject
    where T : CSingletonSOBase<T>
{
    static T _pInstance = null;
    static protected CSOAttacher _pObjectAttacher;
    
    public static T instance
    {
        get
        {
            if (_pInstance == null)
                _pInstance = CreateInstance<T>();

            if (_pObjectAttacher == null && Application.isPlaying)
            {
                if (CSOAttacher.g_bIsQuit == false)
                {
                    GameObject pObjectManager = new GameObject(typeof(T).Name, typeof(CSOAttacher));
                    _pObjectAttacher = pObjectManager.GetComponent<CSOAttacher>();
                    _pObjectAttacher.p_pOwnerSO = instance;
                }
            }
            _pInstance.OnAwake(Application.isPlaying);

            return _pInstance;
        }
    }

    void OnDestroy()
    {
        OnDestroy_Singleton();
        _pInstance = null;
    }

#endif

    public bool p_bExecute_Awake_OnPlay { get; private set; } = false;

    public void Event_OnAwake()
    {
        if(Application.isPlaying == false || 
          (Application.isPlaying && p_bExecute_Awake_OnPlay == false))
            OnAwake(Application.isPlaying);
    }

    protected virtual void OnAwake(bool bAppIsPlaying)
    {
        if(bAppIsPlaying)
            p_bExecute_Awake_OnPlay = true;

        // Debug.Log(GetType().GetFriendlyName() + nameof(OnAwake) + " bAppIsPlaying : " + bAppIsPlaying + " p_bExecute_Awake_OnPlay : " + p_bExecute_Awake_OnPlay);
    }

    /// <summary>
    /// OnEnable은 이상하게 Editor에서 Play를 누른 직후에 호출된다.
    /// Applciation.isPlaying의 경우 false로 들어온다.
    /// </summary>
    private void OnEnable()
    {
        p_bExecute_Awake_OnPlay = false;
        OnAwake(false);
    }

    protected virtual void OnDestroy_Singleton() { }
}
