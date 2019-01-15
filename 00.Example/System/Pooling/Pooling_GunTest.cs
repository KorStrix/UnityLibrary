using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling_GunTest : MonoBehaviour {

	[SerializeField]
	private Transform _pTransMuzzle = null;
	private CManagerPooling_InResources<Pooling_BulletTest.ProjectileModel, Pooling_BulletTest> _pManagerPool_Bullet;

	// Use this for initialization
	void Start()
	{
		_pManagerPool_Bullet = CManagerPooling_InResources<Pooling_BulletTest.ProjectileModel, Pooling_BulletTest>.instance;
        _pManagerPool_Bullet.DoInitPoolingObject("PoolingTest_Bullet");

        _pManagerPool_Bullet.p_EVENT_OnMakeResource += _pManagerPool_Bullet_p_EVENT_OnMakeResource;
		_pManagerPool_Bullet.p_EVENT_OnPopResource += _pManagerPool_Bullet_p_EVENT_OnPopResource;
		_pManagerPool_Bullet.DoStartPooling(new List<Pooling_BulletTest.ProjectileModel>() { Pooling_BulletTest.ProjectileModel.Blue }, 10);
	}

	private void _pManagerPool_Bullet_p_EVENT_OnPopResource(Pooling_BulletTest.ProjectileModel arg1, Pooling_BulletTest arg2)
	{
		Debug.Log(string.Format("총알을 누군가 요청 Key : {0} Object Name : {1}", arg1, arg2.name));
	}

	private void _pManagerPool_Bullet_p_EVENT_OnMakeResource(Pooling_BulletTest.ProjectileModel arg1, Pooling_BulletTest arg2)
	{
		Debug.Log(string.Format("총알 생성 Key : {0} Object Name : {1}", arg1, arg2.name));
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Pooling_BulletTest pBullet = _pManagerPool_Bullet.DoPop(Pooling_BulletTest.ProjectileModel.Blue);
			pBullet.DoInitBullet(_pTransMuzzle);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Pooling_BulletTest pBullet = _pManagerPool_Bullet.DoPop(Pooling_BulletTest.ProjectileModel.Red);
			pBullet.DoInitBullet(_pTransMuzzle);
		}
	}
}
