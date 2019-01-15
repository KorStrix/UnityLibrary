using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling_RocketLauncherTest : MonoBehaviour {

	[SerializeField]
	private Transform _pTransMuzzle = null;

    void Start()
    {
        CManagerPooling_InResources<Pooling_RocketTest.ProjectileModel, Pooling_RocketTest>.instance.DoInitPoolingObject("PoolingTest_Rocket");
    }

    // Update is called once per frame
    void Update()
	{

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Pooling_RocketTest pRocket = CManagerPooling_InResources<Pooling_RocketTest.ProjectileModel, Pooling_RocketTest>.instance.DoPop(Pooling_RocketTest.ProjectileModel.Blue);
			pRocket.DoInitRocket(_pTransMuzzle, 10);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Pooling_RocketTest pRocket = CManagerPooling_InResources<Pooling_RocketTest.ProjectileModel, Pooling_RocketTest>.instance.DoPop(Pooling_RocketTest.ProjectileModel.Red);
			pRocket.DoInitRocket(_pTransMuzzle, 20);
		}
	}
}
