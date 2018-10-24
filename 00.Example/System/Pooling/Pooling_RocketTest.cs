using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pooling_RocketTest : MonoBehaviour {

	public enum ProjectileModel
	{
		Blue,
		Red
	}

	private float _fRocketPower;
	private Rigidbody _pRigidbody;

	public void DoInitRocket(Transform pTransMuzzle, float fRocketPower)
	{
		transform.position = pTransMuzzle.position;
		transform.rotation = pTransMuzzle.rotation;

		_fRocketPower = fRocketPower;
	}

	void Awake()
	{
		_pRigidbody = gameObject.GetComponent<Rigidbody>();
	}

	void OnEnable()
	{
		_pRigidbody.AddForce(new Vector3(0, _fRocketPower, 0), ForceMode.VelocityChange);
		Invoke("DisableObject", 1f);
	}

	void OnDisable()
	{
		CManagerPooling<ProjectileModel, Pooling_RocketTest>.instance.DoPush(this);
	}

	private void DisableObject()
	{
		gameObject.SetActive(false);
	}
}
