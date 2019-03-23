using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-24 오후 8:59:21
   Description : 
   Edit Log    : 
   ============================================ */

public class CManagerEffect : CSingletonDynamicMonoBase<CManagerEffect>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    CManagerPooling_InResources<string, CEffect> _pManagerPooling;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public CEffect DoPop( string strEffectName )
	{
        return _pManagerPooling.DoPop(strEffectName);
	}

    public CEffect DoPlayEffect(string strEffectName, Vector3 vecPos)
    {
        if (_bIsQuitApplication)
            return null;

        CEffect pEffect = _pManagerPooling.DoPop(strEffectName);
		pEffect.DoPlayEffect(vecPos);

		return pEffect;
    }

    public CEffect DoPlayEffect(string strEffect, Transform pTransParents, Vector3 vecPos)
	{
        if (_bIsQuitApplication)
            return null;

        CEffect pEffect = instance.DoPop( strEffect);
		pEffect.DoPlayEffect(pTransParents, vecPos);

		return pEffect;
	}

    public CEffect DoPlayEffect(string strEffect, Vector3 vecPos, Quaternion quatRot)
    {
        if (_bIsQuitApplication)
            return null;

        CEffect pEffect = instance.DoPop( strEffect);
        pEffect.transform.rotation = quatRot;
        pEffect.DoPlayEffect(vecPos);

        return pEffect;
    }


    public CEffect DoPlayEffect(string strEffect, Vector3 vecPos, Vector3 vecRot)
    {
        if (_bIsQuitApplication)
            return null;

        CEffect pEffect = instance.DoPop( strEffect);
        pEffect.transform.rotation = Quaternion.LookRotation(vecRot);
        pEffect.DoPlayEffect(vecPos);

        return pEffect;
    }

    public void DoPushAll()
    {
        _pManagerPooling.DoPushAll();
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pManagerPooling = CManagerPooling_InResources<string, CEffect>.instance;
        _pManagerPooling.DoInitPoolingObject("Effect");
        _pManagerPooling.p_EVENT_OnPopResource += _pManagerPooling_p_EVENT_OnPopResource;
    }

#if UNITY_EDITOR
    public override void OnUpdate()
    {
        base.OnUpdate();
        name = string.Format("이펙트 매니져/{0}개 재생중", _pManagerPooling.p_iPopCount);
    }
#endif

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */


    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    private void _pManagerPooling_p_EVENT_OnPopResource(string arg1, CEffect arg2)
    {
        arg2._strEffectName = arg1;
        arg2.p_Event_Effect_OnDisable += Arg2_p_Event_Effect_OnDisable;
    }

    private void Arg2_p_Event_Effect_OnDisable(CEffect obj)
    {
        _pManagerPooling.DoPush(obj);
    }
}