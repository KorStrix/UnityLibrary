using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CSingletonMonoBase<CLASS_SingletoneTarget> : CObjectBase
    where CLASS_SingletoneTarget : CSingletonMonoBase<CLASS_SingletoneTarget>
{
    static public CLASS_SingletoneTarget instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CLASS_SingletoneTarget>();
                if (_instance == null)
                {
                    if (_bIsQuitApplication) // 더미를 생성해야 한다.
                    {
                        //try
                        //{
                        //}
                        //catch
                        //{

                        //}
                    }
                    else // 다른 씬의 오브젝트를 찾아본다.
                    {
                        for (int i = 0; i < SceneManager.sceneCount; i++)
                        {
                            Scene pScene = SceneManager.GetSceneAt(i);
                            GameObject[] arrObject = pScene.GetRootGameObjects();
                            for(int j = 0; j < arrObject.Length; j++)
                            {
                                _instance = arrObject[j].GetComponentInChildren<CLASS_SingletoneTarget>();
                                if (_instance != null)
                                    break;
                            }
                        }
                    }
                }

                if (_instance != null && _instance._bIsExcuteAwake == false)
                    _instance.OnAwake();
            }

            return _instance;
        }
    }

    static private CLASS_SingletoneTarget _instance;
    static private bool _bIsQuitApplication = false;

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        if (_bIsExcuteAwake == false)
        {
            if (_instance == null)
                _instance = FindObjectOfType<CLASS_SingletoneTarget>();
        }

        base.OnAwake();
    }

    void OnDestroy()
    {
        _instance = null;
        _bIsExcuteAwake = false;
    }

    private void OnApplicationQuit()
    {
        _bIsQuitApplication = true;
    }

    // ========================== [ Division ] ========================== //

    static public CLASS_SingletoneTarget EventMakeSingleton()
    {
        if (_bIsQuitApplication) return null;
        if (_instance != null) return instance;

        GameObject pObjectNewManager = new GameObject(typeof(CLASS_SingletoneTarget).ToString());
        return pObjectNewManager.AddComponent<CLASS_SingletoneTarget>();
    }
}
