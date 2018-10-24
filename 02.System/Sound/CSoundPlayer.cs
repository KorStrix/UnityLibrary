using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-18 오후 5:32:20
   Description : 
   Edit Log    : 
   ============================================ */

public class CSoundPlayer : CCompoEventTrigger
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class SSoundPlayInfo_EventWrapper : IDictionaryItem<string>
    {
        [Rename_Inspector("사운드 이벤트")]
        public string p_strSoundEvent;
        public SSoundPlayInfo p_pSoundPlayInfo;

        public string IDictionaryItem_GetKey()
        {
            return p_strSoundEvent;
        }
    }


    [System.Serializable]
    public class SSoundPlayInfo
    {
        [Rename_Inspector("플레이 할 오디오 키 - 클립이 있으면 클립이 우선")]
        public string p_strAudioKey;
        [Rename_Inspector("플레이 할 오디오 클립")]
        public AudioClip p_pAudioClip;
        [Rename_Inspector("로컬 볼륨")]
        [Range(0f, 1f)]
        public float p_fLocalVolume = 1f;
    }

	/* public - Field declaration            */

	[Rename_Inspector("현재 사용중인 사운드 슬롯", false)]
	public List<CSoundSlot> _listSlotCurrentPlaying = new List<CSoundSlot>();

	[Header("사운드 끝날때 이벤트 - 루프시에도 적용")]
	public UnityEngine.Events.UnityEvent p_listEvent_FinishSound = new UnityEngine.Events.UnityEvent();

    [Rename_Inspector("Disable 시 사운드 Off 유무")]
    public bool _bPlayOff_OnDisable = false;
    [Header("플레이할 사운드 목록")]
    public List<SSoundPlayInfo> _listSoundPlayInfo;
    [Header("플레이할 사운드 목록 - 이벤트")]
    public List<SSoundPlayInfo_EventWrapper> _listSoundPlayInfo_ByEvent;

    [Rename_Inspector("사운드 볼륨")]
    [Range( 0f, 1f )]
	public float _fSoundVolume = 1f;
	[Rename_Inspector( "반복 횟수" )]
	public int _iLoopCount = 0;
	[Rename_Inspector( "반복시 딜레이시간" )]
	public float _fLoopDelay = 0f;
	[Rename_Inspector("루프유무")]
	public bool _bIsLoop = false;
	[Rename_Inspector( "3D사운드 유무" )]
	public bool _bIs3DSound = false;
	[Rename_Inspector( "3D사운드시 최소들리는거리" )]
	public float _fMinDistance_On3DSound = 1f;
	[Rename_Inspector( "3D사운드시 최대들리는거리" )]
	public float _fMaxDistance_On3DSound = 500f;

    [Rename_Inspector("사운드 플레이시 기존 슬롯 끌지 유무")]
    public bool _bStop_OnPlaySound = false;

    [GetComponent]
    [Rename_Inspector("설정값을 복사할 오디오소스")]
    public AudioSource _pAudioSource;

	/* protected - Field declaration         */

	/* private - Field declaration           */

#if UNITY_EDITOR
	private string _strOriginName;
#endif

    Dictionary<string, List<SSoundPlayInfo_EventWrapper>> _mapSoundPlayEventWrapper = new Dictionary<string, List<SSoundPlayInfo_EventWrapper>>();

	private int _iLoopCountCurrent;
	private bool _bIsPlaying = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoPlaySound()
    {
        PlaySound();
    }

    public void DoStopSound()
    {
        for (int i = 0; i < _listSlotCurrentPlaying.Count; i++)
            _listSlotCurrentPlaying[i].DoStopSound();

        _listSlotCurrentPlaying.Clear();
    }

    public CSoundSlot DoPlaySound(string strSoundEvent)
    {
        if (_mapSoundPlayEventWrapper.ContainsKey(strSoundEvent) == false)
            return null;

        return PlaySound(strSoundEvent);
    }

    public bool CheckIsPlaySound()
    {
        return _listSlotCurrentPlaying.Count != 0;
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

        _mapSoundPlayEventWrapper.DoAddItem(_listSoundPlayInfo_ByEvent);

#if UNITY_EDITOR
        _strOriginName = name;
#endif
	}

#if UNITY_EDITOR
    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        name = _strOriginName;

        if (_listSlotCurrentPlaying.Count != 0 && _bPlayOff_OnDisable)
            DoStopSound();
    }
#endif

#if UNITY_EDITOR
    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        if (_bIsPlaying)
		{
			if(_iLoopCount != 0)
				name = string.Format("{0} 재생중.. Repeat : {1}", _strOriginName, _iLoopCountCurrent);
			else
				name = string.Format("{0} 재생중..", _strOriginName);
		}
		else
			name = _strOriginName;
	}
#endif

	protected override void OnPlayEvent()
    {
        base.OnPlayEvent();

#if UNITY_EDITOR
        if (Application.isPlaying == false) return;
#endif
        _iLoopCountCurrent = _iLoopCount;
        PlaySound();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */
       
    private void ProcFinishSound(CSoundSlot pSlot)
	{
		if (_iLoopCount != 0 && _iLoopCountCurrent-- > 0) // 반복 횟수가 0이 아니고 반복 횟수가 아직 0이 아니라면..
            EventExcuteDelay( PlaySound, _fLoopDelay );
		else
		{
            // 반복 횟수가 0이거나 반복 횟수가 다 끝났다면..
			p_listEvent_FinishSound.Invoke();
			_bIsPlaying = false;
            
			if (_bIsLoop)
            {
                if (_fLoopDelay != 0f)
                    EventExcuteDelay(DoPlayEventTrigger, _fLoopDelay);
                else
                    DoPlayEventTrigger();
            }
			else
            {
                if (_listSlotCurrentPlaying.Contains(pSlot))
                {
                    CManagerSound.instance.EventOnSlotFinishClip(pSlot);
                    _listSlotCurrentPlaying.Remove(pSlot);
                }

#if UNITY_EDITOR
                if (this != null)
					name = _strOriginName;
#endif
			}
		}
	}

	private void PlaySound()
	{
        CSoundSlot pSlot = ProcPlaySound_GetSlot(null);
        if (pSlot == null)
            return;
        if(_listSlotCurrentPlaying.Contains(pSlot) == false)
            _listSlotCurrentPlaying.Add(pSlot);

        pSlot.DoSetFinishEvent_OneShot(ProcFinishSound);
        if (_bIs3DSound)
            pSlot.DoSet3DSound(transform.position, _fMinDistance_On3DSound, _fMaxDistance_On3DSound);
    }

    private CSoundSlot PlaySound(string strSoundEvent)
    {
        CSoundSlot pSlot = ProcPlaySound_GetSlot(strSoundEvent);
        if (pSlot == null)
            return null;
        if (_listSlotCurrentPlaying.Contains(pSlot) == false)
            _listSlotCurrentPlaying.Add(pSlot);

        pSlot.DoSetFinishEvent_OneShot(ProcFinishSound);
        if (_bIs3DSound)
            pSlot.DoSet3DSound(transform.position, _fMinDistance_On3DSound, _fMaxDistance_On3DSound);

        return pSlot;
    }

    private CSoundSlot ProcPlaySound_GetSlot(string strSoundEvent)
    {
        if (_bStop_OnPlaySound && _listSlotCurrentPlaying.Count != 0)
        {
            for(int i = 0; i < _listSlotCurrentPlaying.Count; i++)
                CManagerSound.instance.EventOnSlotFinishClip(_listSlotCurrentPlaying[i]);

            _listSlotCurrentPlaying.Clear();
        }

        CSoundSlot pSlot = null;
        if (_listSoundPlayInfo != null)
        {
            SSoundPlayInfo pPlayInfo = string.IsNullOrEmpty(strSoundEvent) == false && _mapSoundPlayEventWrapper.ContainsKey(strSoundEvent) ?
                _mapSoundPlayEventWrapper[strSoundEvent].GetRandom().p_pSoundPlayInfo : _listSoundPlayInfo.GetRandom();

            if(pPlayInfo == null)
            {
                Debug.LogError(name + "pPlayInfo == null strSoundEvent : " + strSoundEvent, this);
            }
            else
            {
                if (pPlayInfo.p_pAudioClip != null)
                    pSlot = CManagerSound.instance.DoPlaySoundEffect_OrNull(pPlayInfo.p_pAudioClip, _fSoundVolume * pPlayInfo.p_fLocalVolume);
                else
                {
                    if (string.IsNullOrEmpty(pPlayInfo.p_strAudioKey) == false)
                        pSlot = CManagerSound.instance.DoPlaySoundEffect_OrNull(pPlayInfo.p_strAudioKey, _fSoundVolume * pPlayInfo.p_fLocalVolume);
                    else
                        Debug.LogError(name + "SoundPlayInfo 가 잘못되었습니다", this);
                }
            }
        }

        if (pSlot != null && _pAudioSource != null)
        {
            AudioSource pSlotSource = pSlot.p_pAudioSource;
            pSlotSource.rolloffMode = _pAudioSource.rolloffMode;
            for (int i = 0; i < 3; i++)
            {
                AnimationCurve pCurve = _pAudioSource.GetCustomCurve((AudioSourceCurveType)i);
                pSlotSource.SetCustomCurve((AudioSourceCurveType)i, pCurve);
            }
        }

        _bIsPlaying = pSlot != null;

        return pSlot;
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
