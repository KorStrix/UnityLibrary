using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 1:48:43
// Description : 
// Edit Log    : 
// ============================================ 

public class CYield_IsWaitingSoundSlot : CustomYieldInstruction
{
    static System.Func<bool> g_OnCheckIsWaiting;

    public static void SetIsWaiting(System.Func<bool> OnCheckIsWaiting)
    {
        g_OnCheckIsWaiting = OnCheckIsWaiting;
    }

    public override bool keepWaiting
    {
        get
        {
            if (g_OnCheckIsWaiting == null)
                return false;
            else
                return g_OnCheckIsWaiting();
        }
    }
}

public class CSoundSlot : CObjectBase
{
    const int const_iLimitSoundName = 30;

	// ===================================== //
	// public - Variable declaration         //
	// ===================================== //

	public delegate void OnEventSoundClip( CSoundSlot pSoundSlot );

    public List<OnEventSoundClip> p_Event_OnStartClip = new List<OnEventSoundClip>();
    public List<OnEventSoundClip> p_Event_OnFinishedClip = new List<OnEventSoundClip>();

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    [DisplayName("플레이중인 오디오", false)]
    public AudioClip _pAudioClip;
    [DisplayName("다음 플레이 예정인 오디오", false)]
    public AudioClip _pAudioClipNext;

    private AudioSource _pAudioSource;  public AudioSource p_pAudioSource {  get { return _pAudioSource; } }
    private float _fVolume;
    private float _fVolumeNext;
    private bool _bLoopSound;

	private System.Action<CSoundSlot> _OnFinishClip;

    Coroutine _pCoroutine_FadeInOut;
    Coroutine _pCoroutine_AudioPlay;

	// private bool _bIsFadeIn;

	// ========================================================================== //

	// ===================================== //
	// public - [Do] Function                //
	// 외부 객체가 요청                      //
	// ===================================== //

	public void DoPlaySound()
    {
		ProcPlaySound(_pAudioClip);
    }

    public void DoPlaySound(AudioClip pAudioClip, float fVolume)
    {
        _fVolume = fVolume;
        _bLoopSound = false;
        ProcPlaySound(pAudioClip);
    }

    public void DoPlaySoundLoop()
    {
        _bLoopSound = true;
        ProcPlaySound(_pAudioClip);
    }

    public void DoStopSound()
    {
		if (gameObject.activeSelf == false)
			return;

		for (int i = 0; i < p_Event_OnFinishedClip.Count; i++)
			p_Event_OnFinishedClip[i](this);

        if (_pCoroutine_FadeInOut != null)
            StopCoroutine(_pCoroutine_FadeInOut);
        if (_pCoroutine_AudioPlay != null)
            StopCoroutine(_pCoroutine_AudioPlay);

        gameObject.SetActive(false);

#if UNITY_EDITOR
		name = _strOriginName;
#endif
	}

	public void DoSetFinishEvent_OneShot(System.Action<CSoundSlot> OnFinishEvent )
	{
		_OnFinishClip = OnFinishEvent;
	}

    public bool CheckIsPlaying()
    {
        return _pAudioSource.isPlaying;
    }
    
	public void DoSet3DSound(Vector3 vecPlayPos, float fListenDistance_Min, float fListenDistance_Max)
	{
        transform.position = vecPlayPos;

        _pAudioSource.spatialBlend = 1f;
		_pAudioSource.minDistance = fListenDistance_Min;
		_pAudioSource.maxDistance = fListenDistance_Max;
	}

	public void DoSet2DSound()
	{
		_pAudioSource.spatialBlend = 0f;
	}


	// ===================================== //
	// public - [Event] Function             //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	public float DoGetVolume()
	{
        return _pAudioSource.volume;
	}

	public void DoSetVolume(float fVolume)
	{
        
        if(_pAudioSource != null)
    		_pAudioSource.volume = fVolume;
	}

    public void DoSetFadeIn()
    {
        if (_pCoroutine_FadeInOut != null)
            StopCoroutine(_pCoroutine_FadeInOut);
        _pCoroutine_FadeInOut = StartCoroutine(CoPlayFadeInOut(false));
    }

    public void DoSetFadeOut(AudioClip pAudioClipNext, float fNextVolume)
    {
        _pAudioClipNext = pAudioClipNext;
        _fVolumeNext = fNextVolume;
        if (_pCoroutine_FadeInOut != null)
            StopCoroutine(_pCoroutine_FadeInOut);
        _pCoroutine_FadeInOut = StartCoroutine(CoPlayFadeInOut(true));
    }

	public void EventInitSoundSlot(MonoBehaviour pManager, OnEventSoundClip OnMethodOnStart, OnEventSoundClip OnMethodOnFinish )
    {
		p_Event_OnStartClip.Add( OnMethodOnStart );
		p_Event_OnFinishedClip.Add( OnMethodOnFinish );

        _pAudioSource = gameObject.AddComponent<AudioSource>();
        _pAudioSource.playOnAwake = false;
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // protected - [Event] Function          //
    // 프랜드 객체가 요청                    //
    // ===================================== //

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //

#if UNITY_EDITOR
    protected override void OnAwake()
    {
        base.OnAwake();
        
        _strOriginName = name;
    }
#endif

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    private void ProcPlaySound(AudioClip pAudioClip)
    {
		for (int i = 0; i < p_Event_OnStartClip.Count; i++)
			p_Event_OnStartClip[i](this);

        _pAudioClip = pAudioClip;
		DoSet2DSound();

		if (gameObject != null)
			gameObject.SetActive(true);

        if (_pCoroutine_AudioPlay != null)
            StopCoroutine(_pCoroutine_AudioPlay);
        _pCoroutine_AudioPlay = StartCoroutine(CoPlaySoundEffect());
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private IEnumerator CoPlayFadeInOut(bool bFadeOut)
    {
        float fDestVolume = bFadeOut ? 0f : _fVolume;
        _pAudioSource.volume = bFadeOut ? _fVolume : 0f;
		
		float fFadeProgress = 0f;
        while (fFadeProgress < 1f)
        {
            _pAudioSource.volume = Mathf.Lerp(_pAudioSource.volume, fDestVolume, fFadeProgress);
            fFadeProgress += 0.1f;

            yield return YieldManager.GetWaitForSecond(0.1f);
        }

        if(bFadeOut)
        {
            DoPlaySound(_pAudioClipNext, _fVolumeNext);
			DoSetFadeIn();
		}
	}

#if UNITY_EDITOR
	private string _strOriginName;
#endif

	private IEnumerator CoPlaySoundEffect()
    {
        _pAudioSource.clip = _pAudioClip;
        _pAudioSource.volume = _fVolume;

		// 에디터상으로는 사운드 시간을 표시하기 위해 로직을 살짝 변경
#if UNITY_EDITOR
        
		float fPlayingSec = 0f;
		do
		{
			string strAudioName = _pAudioClip.name;
			if (strAudioName.Length > const_iLimitSoundName)
				strAudioName = strAudioName.Substring( 0, const_iLimitSoundName) + "...";

			int iAudioLength = (int)_pAudioClip.length;
			_pAudioSource.Play();

            while (fPlayingSec < _pAudioClip.length)
			{
				name = string.Format("{0} Time[ {1}s / {2}s ] Volume[ {3} ] {4}", _strOriginName, fPlayingSec, iAudioLength, _pAudioSource.volume, strAudioName);
                yield return new WaitForSecondsRealtime( 1f );
                yield return new CYield_IsWaitingSoundSlot();
                fPlayingSec += 1f;
			}

		} while (_bLoopSound);
		name = _strOriginName;

#else

		do
		{
            _pAudioSource.Play();
            yield return new CYield_IsWaitingSoundSlot();
            yield return new WaitForSecondsRealtime(_pAudioClip.length);
        } while (_bLoopSound);

#endif

        gameObject.SetActive(false);
		for (int i = 0; i < p_Event_OnFinishedClip.Count; i++)
			p_Event_OnFinishedClip[i]( this );

		if (_OnFinishClip != null)
		{
            var OnFinisishClip = _OnFinishClip;
			_OnFinishClip = null;
            OnFinisishClip(this);
        }
	}
}
