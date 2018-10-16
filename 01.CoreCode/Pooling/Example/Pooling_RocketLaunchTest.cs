using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling_RocketLaunchTest : MonoBehaviour {

	[SerializeField]
	private Transform _pTransMuzzle = null;

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Pooling_RocketTest pRocket = CManagerPooling<Pooling_RocketTest.ProjectileModel, Pooling_RocketTest>.instance.DoPop(Pooling_RocketTest.ProjectileModel.Model_Projectile_Blue);
			pRocket.DoInitRocket(_pTransMuzzle, 10);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Pooling_RocketTest pRocket = CManagerPooling<Pooling_RocketTest.ProjectileModel, Pooling_RocketTest>.instance.DoPop(Pooling_RocketTest.ProjectileModel.Model_Projectile_Red);
			pRocket.DoInitRocket(_pTransMuzzle, 20);
		}
	}
}
