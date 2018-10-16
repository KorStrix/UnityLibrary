using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if NGUI
/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-02 오후 3:13:57
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(CUI2DSpriteAnimation))]
public class CUI2DSpriteAnimator : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */
    
    /* private - Variable declaration           */
    
    static private Dictionary<string, Sprite[]> g_mapSpriteAnimation = new Dictionary<string, Sprite[]>();
    static private Dictionary<string, int> g_mapSpriteAnimation_EventIndex = new Dictionary<string, int>();

    private CUI2DSpriteAnimation _pSpriteAnimation;
    private System.Action _OnFinishAnimation;

    private string _strCurrentAnimationName;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    static public void DoAddSpriteEvent<ENUM_SpriteName>(ENUM_SpriteName eSpriteName, int iAnimationKey)
        where ENUM_SpriteName : System.IFormattable, System.IConvertible, System.IComparable
    {
        string strSpriteName = eSpriteName.ToString();
        if (g_mapSpriteAnimation_EventIndex.ContainsKey(strSpriteName) == false)
            g_mapSpriteAnimation_EventIndex.Add(strSpriteName, iAnimationKey);
        else
            Debug.LogWarning("애니메이션 이벤트가 이미 들어가있다.. " + strSpriteName);
    }

    static public void DoAddSpriteAnimationInfo<ENUM_SpriteName>(ENUM_SpriteName eSpriteName, Sprite[] arrSprite)
        where ENUM_SpriteName : System.IFormattable, System.IConvertible, System.IComparable
    {
        string strSpriteName = eSpriteName.ToString();
        if (g_mapSpriteAnimation.ContainsKey(strSpriteName) == false)
            g_mapSpriteAnimation.Add(strSpriteName, arrSprite);
        else
            Debug.LogWarning("애니메이션이 이미 들어가있다.. " + strSpriteName);
    }

    static public void DoAddSpriteAnimationInfo<ENUM_SpriteName>(ENUM_SpriteName eSpriteName, List<Sprite> listSprite)
    where ENUM_SpriteName : System.IFormattable, System.IConvertible, System.IComparable
    {
        string strSpriteName = eSpriteName.ToString();
		
        if (g_mapSpriteAnimation.ContainsKey(strSpriteName) == false)
            g_mapSpriteAnimation.Add(strSpriteName, listSprite.ToArray());
        else
            Debug.LogWarning("애니메이션이 이미 들어가있다.. " + strSpriteName);
    }

    // ========================== [ Division ] ========================== //

    public void DoPlaySpriteAnimation(string strSpriteName, float fAnimationDuratinSec, bool bIsLoop, System.Action OnFinishAnimation, System.Action OnAnimationEvent = null)
    {
        if (strSpriteName.Equals(_strCurrentAnimationName))
            return;

        if (g_mapSpriteAnimation.ContainsKey(strSpriteName))
        {
            _strCurrentAnimationName = strSpriteName;
            _OnFinishAnimation = OnFinishAnimation;
            _pSpriteAnimation.loop = bIsLoop;
            _pSpriteAnimation.framesPerSecond = (int)(g_mapSpriteAnimation[strSpriteName].Length / fAnimationDuratinSec);
            _pSpriteAnimation.frames = g_mapSpriteAnimation[strSpriteName];
            _pSpriteAnimation.frameIndex = 0;

            if (OnAnimationEvent != null && g_mapSpriteAnimation_EventIndex.ContainsKey(strSpriteName))
                _pSpriteAnimation.Play(g_mapSpriteAnimation_EventIndex[strSpriteName], OnAnimationEvent);
            else
                _pSpriteAnimation.Play();
        }
        else
            Debug.LogWarning("애니메이션이 목록에 없다.." + strSpriteName);
    }

    public void DoPlaySpriteAnimation<ENUM_SpriteName>(ENUM_SpriteName eSpriteName, float fAnimationDuratinSec, bool bIsLoop, System.Action OnFinishAnimation, System.Action OnAnimationEvent = null)
        where ENUM_SpriteName : System.IFormattable, System.IConvertible, System.IComparable
    {
        DoPlaySpriteAnimation(eSpriteName.ToString(), fAnimationDuratinSec, bIsLoop, OnFinishAnimation, OnAnimationEvent);
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

        _pSpriteAnimation = GetComponent<CUI2DSpriteAnimation>();
        _pSpriteAnimation.p_EVENT_OnEndAnim += OnFinishSpriteAnimation;
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void OnFinishSpriteAnimation()
    {
        if(_OnFinishAnimation != null)
        {
            System.Action OnFinishAnimation = _OnFinishAnimation;
            _OnFinishAnimation = null;
            OnFinishAnimation();
        }

        _pSpriteAnimation.frameIndex = 0;
        _strCurrentAnimationName = null;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif