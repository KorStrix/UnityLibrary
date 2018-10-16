#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-23 오후 3:41:56
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

#if VRStandardAssets
using VRStandardAssets.Utils;


[RequireComponent(typeof(VRInteractiveItem))]
public class CSelectionSlider : CObjectBase
{
    static private CSelectionSlider g_pSelectionSlider_Current;

    public event Action OnBarFilled;                                    // This event is triggered when the bar finishes filling.


    [SerializeField] private float m_Duration = 2f;                     // The length of time it takes for the bar to fill.
    [SerializeField] private AudioSource m_Audio;                       // Reference to the audio source that will play effects when the user looks at it and when it fills.
    [SerializeField] private AudioClip m_OnOverClip;                    // The clip to play when the user looks at the bar.
    [SerializeField] private AudioClip m_OnFilledClip;                  // The clip to play when the bar finishes filling.
    [SerializeField] private Slider m_Slider;                           // Optional reference to the UI slider (unnecessary if using a standard Renderer).
    [SerializeField] private GameObject m_BarCanvas;                    // Optional reference to the GameObject that holds the slider (only necessary if DisappearOnBarFill is true).
    [SerializeField] private Renderer m_Renderer;                       // Optional reference to a renderer (unnecessary if using a UI slider).
    [SerializeField] private SelectionRadial m_SelectionRadial;         // Optional reference to the SelectionRadial, if non-null the duration of the SelectionRadial will be used instead.
    [SerializeField] private UIFader m_UIFader;                         // Optional reference to a UIFader, used if the SelectionSlider needs to fade out.
    [SerializeField] private Collider m_Collider;                       // Optional reference to the Collider used to detect the user's gaze, turned off when the UIFader is not visible.
    [SerializeField] private bool m_DisableOnBarFill;                   // Whether the bar should stop reacting once it's been filled (for single use bars).
    [SerializeField] private bool m_DisappearOnBarFill;                 // Whether the bar should disappear instantly once it's been filled.

    [Rename_Inspector("슬라이더 표시유무")]
    public bool _bIsDefaultShow_Slider = true;
    [Rename_Inspector("가득 찼을때 다시 채울건지 유무")]
    public bool _bIsReFill = false;

    [GetComponent]
    private VRInteractiveItem _pInteractiveItem = null;                        // Reference to the VRInteractiveItem to determine when to fill the bar.
    private VRInput _pVRInput;                                          // Reference to the VRInput to detect button presses.

    private bool _bBarFilled;                                           // Whether the bar is currently filled.
    private bool _bGazeOver;                                            // Whether the user is currently looking at the bar.
    private float _fTimer;                                              // Used to determine how much of the bar should be filled.
    private Coroutine _CoFillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.


    private const string k_SliderMaterialPropertyName = "_SliderValue"; // The name of the property on the SlidingUV shader that needs to be changed in order for it to fill.

    private Button _pButtonUGUI;


    protected override void OnAwake()
    {
        base.OnAwake();

        _pVRInput = CManagerVR.instance.p_pVRInput;

        if (m_Slider)
            m_Slider.gameObject.SetActive(_bIsDefaultShow_Slider);

        _pButtonUGUI = GetComponent<Button>();
        if (_pButtonUGUI)
            _pButtonUGUI.enabled = false;
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _pVRInput.OnDown += HandleDown;
        _pVRInput.OnUp += HandleUp;

        _pInteractiveItem.OnOver += HandleOver;
        _pInteractiveItem.OnOut += HandleOut;
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        _pVRInput.OnDown -= HandleDown;
        _pVRInput.OnUp -= HandleUp;

        _pInteractiveItem.OnOver -= HandleOver;
        _pInteractiveItem.OnOut -= HandleOut;
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (!m_UIFader)
            return;

        // If this bar is using a UIFader turn off the collider when it's invisible.
        m_Collider.enabled = m_UIFader.Visible;
    }

    public IEnumerator WaitForBarToFill()
    {
        // If the bar should disappear when it's filled, it needs to be visible now.
        if (m_BarCanvas && m_DisappearOnBarFill)
            m_BarCanvas.SetActive(true);

        // Currently the bar is unfilled.
        _bBarFilled = false;

        // Reset the timer and set the slider value as such.
        ResetSlider();

        // Keep coming back each frame until the bar is filled.
        while (!_bBarFilled)
        {
            yield return null;
        }

        // If the bar should disappear once it's filled, turn it off.
        if (m_BarCanvas && m_DisappearOnBarFill)
            m_BarCanvas.SetActive(false);
    }


    private IEnumerator FillBar()
    {
        // When the bar starts to fill, reset the timer.
        _fTimer = 0f;

        // The amount of time it takes to fill is either the duration set in the inspector, or the duration of the radial.
        float fillTime = m_SelectionRadial != null ? m_SelectionRadial.SelectionDuration : m_Duration;

        // Until the timer is greater than the fill time...
        while (_fTimer < fillTime)
        {
            // ... add to the timer the difference between frames.
            _fTimer += Time.deltaTime;

            // Set the value of the slider or the UV based on the normalised time.
            SetSliderValue(_fTimer / fillTime);

            // Wait until next frame.
            yield return null;

            // If the user is still looking at the bar, go on to the next iteration of the loop.
            if (_bGazeOver)
                continue;

            // If the user is no longer looking at the bar, reset the timer and bar and leave the function.
            ResetSlider();
            yield break;
        }

        // If the loop has finished the bar is now full.
        _bBarFilled = true;

        // If anything has subscribed to OnBarFilled call it now.
        if (OnBarFilled != null)
            OnBarFilled();

        // Play the clip for when the bar is filled.

        OnFillBar();
    }


    private void SetSliderValue(float sliderValue)
    {
        // If there is a slider component set it's value to the given slider value.
        if (m_Slider)
            m_Slider.value = sliderValue;

        // If there is a renderer set the shader's property to the given slider value.
        if (m_Renderer)
            m_Renderer.sharedMaterial.SetFloat(k_SliderMaterialPropertyName, sliderValue);
    }


    /// <summary>
    /// 시선을 본 상태에서 스위치를 눌렀을 때
    /// </summary>
    private void HandleDown()
    {
        // If the user is looking at the bar start the FillBar coroutine and store a reference to it.
        if (_bGazeOver)
            _CoFillBarRoutine = StartCoroutine(FillBar());
    }

    /// <summary>
    /// 시선을 본 상태에서 스위치를 땠을 때
    /// </summary>
    private void HandleUp()
    {
        // If the coroutine has been started (and thus we have a reference to it) stop it.
        if (_CoFillBarRoutine != null)
        {
            StopCoroutine(_CoFillBarRoutine);
            _CoFillBarRoutine = null;
        }

        ResetSlider();
    }
    
    /// <summary>
    /// 시선이 컬라이더 내에 들어올 때
    /// </summary>
    private void HandleOver()
    {
        if (g_pSelectionSlider_Current != null && g_pSelectionSlider_Current != this)
            g_pSelectionSlider_Current.HandleOut();

        g_pSelectionSlider_Current = this;

        ResetSlider();

        if (m_Slider)
            m_Slider.gameObject.SetActive(true);

        // The user is now looking at the bar.
        _bGazeOver = true;

        // Play the clip appropriate for when the user starts looking at the bar.

        if (m_Audio != null)
        {
            m_Audio.clip = m_OnOverClip;
            m_Audio.Play();
        }
    }


    /// <summary>
    /// 시선이 컬라이더 밖으로 나갈 때
    /// </summary>
    private void HandleOut()
    {
        if (m_Slider)
            m_Slider.gameObject.SetActive(_bIsDefaultShow_Slider);

        // The user is no longer looking at the bar.
        _bGazeOver = false;

        // If the coroutine has been started (and thus we have a reference to it) stop it.
        if (_CoFillBarRoutine != null)
        {
            StopCoroutine(_CoFillBarRoutine);
            _CoFillBarRoutine = null;
        }

        ResetSlider();
    }

    private void ResetSlider()
    {
        _fTimer = 0f;
        SetSliderValue(0f);
    }

    private void OnFillBar()
    {
        if (m_Audio != null)
        {
            m_Audio.clip = m_OnFilledClip;
            m_Audio.Play();
        }

        // If the bar should be disabled once it is filled, do so now.
        if (m_DisableOnBarFill)
            enabled = false;

        if(_pButtonUGUI != null)
        {
            _pButtonUGUI.enabled = true;
            _pButtonUGUI.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
            _pButtonUGUI.enabled = false;
        }

        if (_bIsReFill)
        {
            _fTimer = 0f;
            SetSliderValue(0f);
            HandleDown();
        }
    }
}
#endif