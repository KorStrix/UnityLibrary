using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CCompoEventSystemChecker : CObjectBase
{
	protected override void OnAwake()
	{
		base.OnAwake();

		if (!FindObjectOfType<EventSystem>())
		{
			GameObject obj = new GameObject( "EventSystem_Dynamic" );
			obj.AddComponent<EventSystem>();
			obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
		}
	}
}
