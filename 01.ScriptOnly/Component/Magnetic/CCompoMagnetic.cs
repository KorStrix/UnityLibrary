#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : KJH
 *	작성자 : KJH
 *	
 *	기능 : 자석 효과를 담당한다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IMagneticListener
{
	void IOnAttracted();
}

[RequireComponent(typeof(SphereCollider))]
public class CCompoMagnetic : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	private enum EStateMagnetic
	{
		None,
		Reaction,
		Attracted,
		Repelled
	}

	private enum EMagneticType
	{
		Attract = -1,
		Repel = 1
	}

	private class SInfoMagnetic
	{
		public Transform pTransCached;
		public IMagneticListener IMagneticListener;
		public EStateMagnetic eStateMagnetic;
		public float fLastDistance;
	}

	#region Field

	/* public - Field declaration            */

	[Header("탐색 반경 (잃는 반경)")]
	[SerializeField] private float p_fFindRadius = 10f;

	[Header("자석 반응 타입 (Attract - 당기기, Repel - 밀기)")]
	[SerializeField] private EMagneticType p_eMagnetReactType = EMagneticType.Attract;

	[Header("자석 반응 반경")]
	[SerializeField] private float p_fMagnetReactRadius = 5f;

	[Header("자석 반응 속도배율")]
	[SerializeField] private float p_fMagnetReactMultiplier = 1f;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	private Dictionary<int, SInfoMagnetic> _mapMagneticPool = new Dictionary<int, SInfoMagnetic>();

	private SphereCollider _pSphereCollider;
	private int _iMagneticTypeDir;

	#endregion Field

	#region Public

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	#endregion Public

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, p_fMagnetReactRadius);

		if (_pSphereCollider == null)
            this.GetComponent(out _pSphereCollider);

		_pSphereCollider.radius = p_fFindRadius;
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		Transform pTrans = pCollider.transform;

		int iHashCode = pCollider.GetHashCode();
		float fDistance = (pTrans.position - transform.position).sqrMagnitude;
		if (_mapMagneticPool.ContainsKey(iHashCode) == false)
		{
			IMagneticListener IMagneticListener = pTrans.GetComponent<IMagneticListener>();

			SInfoMagnetic sInfoMagnetic = new SInfoMagnetic();
			sInfoMagnetic.pTransCached = pTrans;
			sInfoMagnetic.IMagneticListener = IMagneticListener;
			sInfoMagnetic.eStateMagnetic = EStateMagnetic.None;
			sInfoMagnetic.fLastDistance = fDistance;

			_mapMagneticPool.Add(iHashCode, sInfoMagnetic);
		}
		else
		{
			SInfoMagnetic sInfoMagnetic = _mapMagneticPool[iHashCode];
			sInfoMagnetic.eStateMagnetic = EStateMagnetic.None;
			sInfoMagnetic.fLastDistance = fDistance;
		}
	}

	private void OnTriggerStay(Collider pCollider)
	{
		int iHashCode = pCollider.GetHashCode();
		SInfoMagnetic sInfoMagnetic = _mapMagneticPool[iHashCode];
		if (sInfoMagnetic.eStateMagnetic == EStateMagnetic.Attracted) return;

		Transform pTrans = sInfoMagnetic.pTransCached;

		float fDistanceSqrt = (pTrans.position - transform.position).sqrMagnitude;
		float fDistAtrractSqrt = (p_fMagnetReactRadius * p_fMagnetReactRadius);

		if (sInfoMagnetic.eStateMagnetic == EStateMagnetic.Reaction || fDistanceSqrt < fDistAtrractSqrt)
		{
			if (sInfoMagnetic.eStateMagnetic == EStateMagnetic.None)
				sInfoMagnetic.eStateMagnetic = EStateMagnetic.Reaction;

			Vector3 v3Direction = (pTrans.position - transform.position);
			//float fLastDistance = sInfoMagnetic.fLastDistance;
			float fRealDistance = v3Direction.sqrMagnitude;
			float fCurrentDistance = PrimitiveHelper.GetCalcReverseFloat(sInfoMagnetic.fLastDistance, fRealDistance) * p_fMagnetReactMultiplier;

			fCurrentDistance = Mathf.Clamp(fCurrentDistance, 0f, 50f);

			pTrans.position += fCurrentDistance * (v3Direction * _iMagneticTypeDir) * Time.deltaTime;

			if (fRealDistance < float.Epsilon && sInfoMagnetic.eStateMagnetic != EStateMagnetic.Attracted)
			{
				sInfoMagnetic.eStateMagnetic = EStateMagnetic.Attracted;

				IMagneticListener IMagneticListener = sInfoMagnetic.IMagneticListener;

				if (IMagneticListener != null)
					IMagneticListener.IOnAttracted();
			}
		}
	}

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

        this.GetComponent(out _pSphereCollider);

		_iMagneticTypeDir = (int)p_eMagnetReactType;

	}
	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
