#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-03 오후 4:02:13
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


public class OnChangeHP_Arg
{
    public float fHPMax;
    public float fHPCurrent;
    public float fDamage;
}

public interface IHasHP
{
    ObservableCollection p_Event_OnResetHP { get; }
    /// <summary>
    /// HP Max, HP Current, Damage
    /// </summary>
    ObservableCollection<OnChangeHP_Arg> p_Event_OnDamage { get; }
    ObservableCollection<IHasHP> p_Event_OnDead { get; }
}


/// <summary>
/// 
/// </summary>
public class CHealthBarAnimation : CObjectBase, IPoolingUIObject
{
    /* const & readonly declaration             */

    private const string const_StandardSpritePath = "UI/Skin/UISprite.psd";

    /* enum & struct declaration                */

    public enum EUIElementName_ForInit
    {
        Image_HPFrame,
        Image_HPFill,
        Image_HPDamage,
    }

    /* public - Field declaration            */

    [DisplayName("체력이 변경될 때만 보일지")]
    public bool p_bIsShow_OnChangeHPOnly = true;
#if ODIN_INSPECTOR
    [ShowIf(nameof(p_bIsShow_OnChangeHPOnly))]
#endif
    [DisplayName("보이고 사라지는 시간")]
    public float p_fShowTime_OnChangeHPOnly = 1f;

    [Header("애니메이션 관련"), Space(10)]
    [DisplayName("HP 감소 애니메이션 딜레이")]
    public float p_fDecreaseAnimationDelay = 0.2f;
    [DisplayName("HP 감소 애니메이션 시간")]
    public float p_fDecreaseAnimationTime = 0.2f;
    [DisplayName("체력량에 따른 HPBar Color")]
    public Gradient p_pHPBarColor;

    [Header("필요한 UI Element"), Space(10)]
    [DisplayName("Image `" + nameof(EUIElementName_ForInit.Image_HPFrame) + "`", false)]
    public Image p_pImage_HPFrame = null;
    [DisplayName("Image `" + nameof(EUIElementName_ForInit.Image_HPFill) + "`", false)]
    public Image p_pImage_HPFill = null;
    [DisplayName("Image `" + nameof(EUIElementName_ForInit.Image_HPDamage) + "`", false)]
    public Image p_pImage_HPDamage = null;

    /* protected & private - Field declaration         */

    [GetComponentInChildren]
    List<Image> _listRenderer = new List<Image>();

    Transform _pTransform_UI_HPBar_Pos;
    Coroutine _pCoroutine_AutoHide;

    Vector2 _vecDamageImage_OriginSize;
    Vector3 _vecDamageImage_OriginPos;
    IHasHP _pDrawTarget;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInit(IHasHP pDrawTarget, Transform pTransform_UI_HPBar_Pos)
    {
        _pDrawTarget = pDrawTarget;
        _pDrawTarget.p_Event_OnResetHP.Subscribe += OnResetHP;
        _pDrawTarget.p_Event_OnDamage.Subscribe += OnChangeHP;
        _pDrawTarget.p_Event_OnDead.Subscribe += Event_OnDead;

        _pTransform_UI_HPBar_Pos = pTransform_UI_HPBar_Pos;
        if (_pTransform_UI_HPBar_Pos != null)
            transform.position = _pTransform_UI_HPBar_Pos.position;

        if (p_bIsShow_OnChangeHPOnly)
            SetEnable_Renderer(false);
    }

    public void DoSet_HealthBar(float fHPDelta_0_1)
    {
        p_pImage_HPFill.color = p_pHPBarColor.Evaluate(fHPDelta_0_1);
        p_pImage_HPFill.fillAmount = fHPDelta_0_1;
    }

#if ODIN_INSPECTOR
    [Button("HP Animation Test")]
#endif
    public void DoPlay_HealthBarAnimation(float fHPDelta_0_1_Remain, float fDamageDelta_0_1)
    {
        DoSet_HealthBar(fHPDelta_0_1_Remain);
        p_pImage_HPDamage.fillAmount = 1 - fHPDelta_0_1_Remain;

        // StartCoroutine(CoHelathBarAnimation());
    }

#if UNITY_EDITOR
#if ODIN_INSPECTOR
    [Button("Build HealthBar")]
#endif
    public void DoBuildHelathBar()
    {
        InitComponent(ref p_pImage_HPFrame, nameof(EUIElementName_ForInit.Image_HPFrame));
        InitComponent(ref p_pImage_HPFill, nameof(EUIElementName_ForInit.Image_HPFill));
        InitComponent(ref p_pImage_HPDamage, nameof(EUIElementName_ForInit.Image_HPDamage));

        Sprite pSprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>(const_StandardSpritePath);

        p_pImage_HPFrame.sprite = pSprite;
        p_pImage_HPFrame.rectTransform.sizeDelta = new Vector2(100f, 10f);

        p_pImage_HPFill.sprite = pSprite;
        p_pImage_HPFill.type = Image.Type.Filled;
        p_pImage_HPFill.fillMethod = Image.FillMethod.Horizontal;
        p_pImage_HPFill.color = Color.green;

        p_pImage_HPDamage.sprite = pSprite;
        p_pImage_HPDamage.transform.localScale = new Vector3(-1f, 1f, 1f);

        p_pImage_HPDamage.type = Image.Type.Filled;
        p_pImage_HPDamage.fillMethod = Image.FillMethod.Horizontal;
        p_pImage_HPDamage.fillAmount = 0f;
        p_pImage_HPDamage.color = Color.red;

        p_pImage_HPFill.transform.SetParent(p_pImage_HPFrame.transform);
        p_pImage_HPDamage.transform.SetParent(p_pImage_HPFrame.transform);

        p_pImage_HPFill.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        p_pImage_HPFill.rectTransform.offsetMin = new Vector2(5, 1f);
        p_pImage_HPFill.rectTransform.offsetMax = new Vector2(-5, -1);

        p_pImage_HPDamage.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        p_pImage_HPDamage.rectTransform.offsetMin = new Vector2(5, 1f);
        p_pImage_HPDamage.rectTransform.offsetMax = new Vector2(-5, -1);

        // p_pImage_HPFrame.rectTransform.

        GradientColorKey[] arrColorKey = new GradientColorKey[3];
        arrColorKey[0].color = Color.red;
        arrColorKey[0].time = 0.0f;
        arrColorKey[1].color = Color.yellow;
        arrColorKey[1].time = 0.5f;
        arrColorKey[2].color = Color.green;
        arrColorKey[2].time = 1.0f;

        GradientAlphaKey[] arrAlphaKey = new GradientAlphaKey[2];
        arrAlphaKey[0].alpha = 1f;
        arrAlphaKey[0].time = 0.0f;
        arrAlphaKey[1].alpha = 1f;
        arrAlphaKey[1].time = 1f;

        p_pHPBarColor.SetKeys(arrColorKey, arrAlphaKey);
    }
#endif

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _vecDamageImage_OriginSize = p_pImage_HPDamage.rectTransform.sizeDelta;
        _vecDamageImage_OriginPos = p_pImage_HPDamage.transform.localPosition;

        SetEnable_Renderer(!p_bIsShow_OnChangeHPOnly);
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        if (bIsQuitApplciation)
            return;

        _pTransform_UI_HPBar_Pos = null;

        if(_pDrawTarget != null)
        {
            _pDrawTarget.p_Event_OnResetHP.DoRemove_Listener(OnResetHP);
            _pDrawTarget.p_Event_OnDamage.DoRemove_Listener(OnChangeHP);
            _pDrawTarget.p_Event_OnDead.DoRemove_Listener(Event_OnDead);

            _pDrawTarget = null;
        }
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        if(_pTransform_UI_HPBar_Pos != null)
            transform.position = _pTransform_UI_HPBar_Pos.position;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void OnResetHP()
    {
        DoSet_HealthBar(1f);
    }

    private void OnChangeHP(OnChangeHP_Arg pArg)
    {
        DoPlay_HealthBarAnimation(pArg.fHPCurrent / pArg.fHPMax, pArg.fDamage / pArg.fHPMax);

        if (p_bIsShow_OnChangeHPOnly)
            SetEnable_Renderer(true);
    }

    private void Event_OnDead(IHasHP pHasHP)
    {
        gameObject.SetActive(false);
    }

    private void InitComponent<ComponentType>(ref ComponentType pComponent, string strComponentName)
        where ComponentType : Component
    {
        pComponent = SCManagerGetComponent.GetComponentInChildren_SameName(this, strComponentName, typeof(ComponentType), true) as ComponentType;
        if (pComponent == null)
        {
            GameObject pObject = new GameObject(strComponentName);
            pComponent = pObject.AddComponent<ComponentType>();
            pObject.transform.SetParent(transform);
            pObject.transform.DoResetTransform();
        }
    }

    void SetEnable_Renderer(bool bShow)
    {
        if(bShow)
        {
            if (gameObject.activeSelf == false)
                return;

            if (_pCoroutine_AutoHide != null)
                StopCoroutine(_pCoroutine_AutoHide);

            _pCoroutine_AutoHide = StartCoroutine(CoShow_AutoHide());
        }
        else
        {
            for (int i = 0; i < _listRenderer.Count; i++)
                _listRenderer[i].enabled = false;
        }
    }

    IEnumerator CoHelathBarAnimation()
    {
        yield return new WaitForSeconds(p_fDecreaseAnimationDelay);

        float fAnimationTime = 0f;
        while (fAnimationTime < p_fDecreaseAnimationTime)
        {
            CalculateDamageUI(_vecDamageImage_OriginSize, _vecDamageImage_OriginPos, fAnimationTime / p_fDecreaseAnimationTime);
            fAnimationTime += Time.deltaTime;
        }

        CalculateDamageUI(_vecDamageImage_OriginSize, _vecDamageImage_OriginPos, 1f);
    }

    private void CalculateDamageUI(Vector2 vecOriginSize, Vector3 vecOriginPos, float fProgress_0_1)
    {
        p_pImage_HPDamage.rectTransform.sizeDelta = new Vector2(vecOriginSize.x * (1f - fProgress_0_1), vecOriginSize.y);
        p_pImage_HPDamage.transform.localPosition = new Vector3(vecOriginPos.x * fProgress_0_1, vecOriginPos.y, vecOriginPos.z);
    }

    public IEnumerator CoShow_AutoHide()
    {
        for (int i = 0; i < _listRenderer.Count; i++)
            _listRenderer[i].enabled = true;

        yield return new WaitForSeconds(p_fShowTime_OnChangeHPOnly);

        for (int i = 0; i < _listRenderer.Count; i++)
            _listRenderer[i].enabled = false;
    }

    #endregion Private
}