using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-04 오후 5:54:23
   Description : 
   Edit Log    : 
   ============================================ */

public class CSingletonNotMonoBase<CLASS_SingletoneTarget> : UnityEngine.Object
    where CLASS_SingletoneTarget : CSingletonNotMonoBase<CLASS_SingletoneTarget>, new()
{
	static private CLASS_SingletoneTarget _instance;

    public class CCompo_OnDisable : MonoBehaviour
    {
        public event System.Action<GameObject> p_Event_OnDisable;

        private void OnDisable()
        {
            if (p_Event_OnDisable != null)
                p_Event_OnDisable(gameObject);
        }
    }


    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

	// ========================== [ Division ] ========================== //

	static public CLASS_SingletoneTarget instance
	{
		get
		{
			if (UnityEngine.Object.Equals(_instance, null))
			{
				_instance = new CLASS_SingletoneTarget();

                bool bIsGenearteGameObject;
                _instance.OnMakeSingleton(out bIsGenearteGameObject);
                if(bIsGenearteGameObject)
                {
                    System.Type pTypeDriven = typeof(CLASS_SingletoneTarget);
                    _instance.gameObject = new GameObject(pTypeDriven.GetFriendlyName());
                    _instance.transform = _instance.gameObject.transform;

                    CCompoEventTrigger_OnDisable pOnDisable = _instance.gameObject.AddComponent<CCompoEventTrigger_OnDisable>();
                    pOnDisable.p_Event_OnDisable += _instance.OnDisable_p_Event_OnDisable;
                    SceneManager.sceneUnloaded += _instance.OnSceneUnloaded;

                    _instance.OnMakeGameObject(_instance.gameObject);
                }
			}

			return _instance;
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

	virtual protected void OnMakeSingleton(out bool bIsGenearteGameObject) { bIsGenearteGameObject = false; }
    virtual protected void OnReleaseSingleton() { }

    virtual protected void OnMakeGameObject(GameObject pObject) { }
    virtual protected void OnDestroyGameObject(GameObject pObject) { }

    virtual protected void OnSceneUnloaded(Scene pScene) { }

    // ========================== [ Division ] ========================== //

    protected void EventSetInstance(CLASS_SingletoneTarget pInstanceSet)
	{
		_instance = pInstanceSet;
	}

    private void OnDisable_p_Event_OnDisable(GameObject pObject)
    {
        OnDestroyGameObject(pObject);
        DoReleaseSingleton();
    }
}

// https://stackoverflow.com/questions/4185521/c-sharp-get-generic-type-name/26429045
public static class TypeNameExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        string friendlyName = type.Name;
        if (type.IsGenericType)
        {
            int iBacktick = friendlyName.IndexOf('`');
            if (iBacktick > 0)
            {
                friendlyName = friendlyName.Remove(iBacktick);
            }
            friendlyName += "<";
            Type[] typeParameters = type.GetGenericArguments();
            for (int i = 0; i < typeParameters.Length; ++i)
            {
                string typeParamName = GetFriendlyName(typeParameters[i]);
                friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
            }
            friendlyName += ">";
        }

        return friendlyName;
    }
}