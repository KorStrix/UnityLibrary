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

public class CSOAttacher : MonoBehaviour
{
    static public bool g_bIsQuit { get; private set; }

    public ScriptableObject p_pOwnerSO;

    private void OnApplicationQuit()
    {
        g_bIsQuit = true;
    }
}
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
            {
                _pInstance = CreateInstance<T>();
                _pInstance.OnGenerate_SingletonInstance();
            }

            if (_pObjectAttacher == null && Application.isPlaying)
            {
                if (CSOAttacher.g_bIsQuit == false)
                {
                    GameObject pObjectManager = new GameObject(typeof(T).Name, typeof(CSOAttacher));
                    _pObjectAttacher = pObjectManager.GetComponent<CSOAttacher>();
                    instance.OnGenerate_SingletonGameObject();
                    _pObjectAttacher.p_pOwnerSO = instance;
                }
            }

            return _pInstance;
        }
    }

    protected virtual void OnGenerate_SingletonInstance() { }
    protected virtual void OnGenerate_SingletonGameObject() { }
}
