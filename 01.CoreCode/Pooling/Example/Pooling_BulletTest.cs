using UnityEngine;

public class Pooling_BulletTest : MonoBehaviour {

	public enum ProjectileModel
	{
		Model_Projectile_Blue,
		Model_Projectile_Red
	}

	public float _fSpeed = 10;

	public void DoInitBullet(Transform pTransMuzzle)
	{
		transform.position = pTransMuzzle.position;
		transform.rotation = pTransMuzzle.rotation;
	}

	void OnEnable()
	{
		Invoke("DisableObject", 1f);
	}

	// Update is called once per frame
	void Update ()
	{
		transform.Translate(0, 0, _fSpeed * Time.deltaTime, Space.Self);	
	}

	void OnDisable()
	{
		CManagerPooling<ProjectileModel, Pooling_BulletTest>.instance.DoPush(this);
	}

	private void DisableObject()
	{
		gameObject.SetActive(false);
	}
}
