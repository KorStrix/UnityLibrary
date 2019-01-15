#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CCompoEffectPlayer : CObjectBase, IResourceEventListener
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class SEffectPlayInfo : IDictionaryItem<string>
    {
        public string strEffectEvent;

        public List<CEffect> listEffectPlay;

        public CEffect GetRandomEffect()
        {
            return listEffectPlay.GetRandom();
        }

        public string IDictionaryItem_GetKey()
        {
            return strEffectEvent;
        }
    }

    /* public - Field declaration            */

    [Rename_Inspector("Enable시 Effect Play할지")]
    public bool p_bEffectPlay_OnEnable = false;
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowIf("p_bEffectPlay_OnEnable")]
#endif
    [Rename_Inspector("Enable시 Play할 Effect이름")]
    public string p_strEffectName_OnEnablePlay;

    [Rename_Inspector("Effect가 끝나면 Object Disable할지")]
    public bool p_bDisableObject_OnFinishEffect = false;

    [Header("이벤트 이펙트 리스트")]
    public List<SEffectPlayInfo> p_listEffectPlayInfo = new List<SEffectPlayInfo>();

    [Header("이펙트 끝날때 이벤트")]
    public UnityEngine.Events.UnityEvent p_listEvent_FinishEffect = new UnityEngine.Events.UnityEvent();

    [Rename_Inspector("플레이중인 이펙트가 활성중이면 끕니다")]
    public bool _bIsDisableEffectPlayed_When_EffectPlaying = true;

    [Rename_Inspector("이펙트 플레이 포지션 오프셋")]
    public Vector3 _vecEffectPos_Offset;

    /* protected - Field declaration         */

    /* private - Field declaration           */

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
    [Sirenix.OdinInspector.HideInEditorMode]
#endif
    Dictionary<string, SEffectPlayInfo> _mapEffectPlayInfo = new Dictionary<string, SEffectPlayInfo>();
    CEffect _pEffectPlaying;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public CEffect DoPlayEffect(string strEffectEvent = "")
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Debug.Log(ConsoleProWrapper.ConvertLog_ToCore(" DoPlayEffect - " + strEffectEvent + " Is Contain : " + _mapEffectPlayInfo.ContainsKey(strEffectEvent)), this);
            return null;
        }

        if (_mapEffectPlayInfo.ContainsKey(strEffectEvent) == false)
            return null;

        SEffectPlayInfo pEffectPlayInfo = _mapEffectPlayInfo[strEffectEvent];
        return PlayEffect(pEffectPlayInfo.GetRandomEffect());
    }

    public CEffect DoPlayEffect(string strEffectEvent, Transform pTransform)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Debug.Log(ConsoleProWrapper.ConvertLog_ToCore(" DoPlayEffect - " + strEffectEvent + " Is Contain : " + _mapEffectPlayInfo.ContainsKey(strEffectEvent)), this);
            return null;
        }

        if (_mapEffectPlayInfo.ContainsKey(strEffectEvent) == false)
            return null;

        SEffectPlayInfo pEffectPlayInfo = _mapEffectPlayInfo[strEffectEvent];
        return PlayEffect(pEffectPlayInfo.GetRandomEffect(), pTransform);
    }

    public CEffect DoPlayEffect(string strEffectEvent, Vector3 vecPos)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Debug.Log(ConsoleProWrapper.ConvertLog_ToCore(" DoPlayEffect - " + strEffectEvent + " Is Contain : " + _mapEffectPlayInfo.ContainsKey(strEffectEvent)), this);
            return null;
        }

        CEffect pEffect = DoPlayEffect(strEffectEvent);
        if (pEffect != null)
            pEffect.transform.position = vecPos;

        return pEffect;
    }

    public CEffect DoPlayEffect(string strEffectEvent, Vector3 vecPos, Quaternion rotRotation)
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Debug.Log(ConsoleProWrapper.ConvertLog_ToCore(" DoPlayEffect - " + strEffectEvent + " Is Contain : " + _mapEffectPlayInfo.ContainsKey(strEffectEvent)), this);
            return null;
        }

        CEffect pEffect = DoPlayEffect(strEffectEvent);
        if(pEffect != null)
        {
            pEffect.transform.position = vecPos;
            pEffect.transform.rotation = rotRotation;
        }

        return pEffect;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _mapEffectPlayInfo.DoAddItem(p_listEffectPlayInfo);
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if(p_bEffectPlay_OnEnable)
            DoPlayEffect(p_strEffectName_OnEnablePlay);
    }

    public void IResourceEventListener_Excute(string strEventName)
    {
        DoPlayEffect(strEventName, transform.position, transform.rotation);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core) == false)
            return;

        GUIStyle pStyle = new GUIStyle();
        pStyle.normal.textColor = Color.green;
        Handles.Label(transform.position + _vecEffectPos_Offset + (Vector3.right * 3f), "Effect Pos", pStyle);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _vecEffectPos_Offset, 1f);
    }
#endif

    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    private CEffect PlayEffect(CEffect pEffectPlay)
    {
        if (pEffectPlay == null)
            return null;

        _pEffectPlaying = CManagerEffect.instance.DoPlayEffect(pEffectPlay.name, transform.position + _vecEffectPos_Offset);
        if (_pEffectPlaying != null)
            _pEffectPlaying.p_Event_Effect_OnDisable += PEffectPlaying_p_Event_Effect_OnDisable;

        return _pEffectPlaying;
    }

    private CEffect PlayEffect(CEffect pEffectPlay, Transform pTransform)
    {
        if (pEffectPlay == null)
            return null;

        _pEffectPlaying = CManagerEffect.instance.DoPlayEffect(pEffectPlay.name, pTransform.position, pTransform.rotation);
        _pEffectPlaying.p_Event_Effect_OnDisable += PEffectPlaying_p_Event_Effect_OnDisable;
        _pEffectPlaying.transform.SetParent(pTransform);

        return _pEffectPlaying;
    }

    private void PEffectPlaying_p_Event_Effect_OnDisable(CEffect obj)
    {
        p_listEvent_FinishEffect.Invoke();
        _pEffectPlaying = null;

        if (p_bDisableObject_OnFinishEffect)
            gameObject.SetActive(false);
    }
    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
