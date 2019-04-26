using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0414

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 1:23:00
// Description : BGM 및 SoundEffect의 볼륨 등을 쉽게 제어하고, 변경할 수 있게 하기 위함.
//               SoundEffect는 풀링 방식을 사용.
// Edit Log    : 
// ============================================ 

[System.Serializable]
public struct SINI_Sound : IDictionaryItem<string>
{
    public SINI_Sound(string strSoundName, float fVolume)
    {
        this.strSoundName = strSoundName;
        this.fVolume = fVolume;
        iGroupNumber = 0;
    }

    public string IDictionaryItem_GetKey()
    {
        return strSoundName;
    }

    public string strSoundName;
    public float fVolume;
    public int iGroupNumber;
}

public class CManagerSound : CSingletonDynamicMonoBase<CManagerSound>
{
    private const int const_iDefault_SoundSlot_PoolingCount = 10;

    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    [Rename_Inspector("이펙트볼륨")]
    public float _fVolumeEffect = 1f;
    [Rename_Inspector("BGM볼륨")]
    public float _fVolumeBGM = 1f;

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    // ===================================== //
    // private - Variable declaration        //
    // ===================================== //

    static private System.Action _CallBackOnFinishBGM;

    private List<CSoundSlot> _listSoundSlotAll = new List<CSoundSlot>();
    private HashSet<CSoundSlot> _queueNotUseSlot = new HashSet<CSoundSlot>();
    private Dictionary<string, CSoundSlot> _mapCurrentPlayingSound = new Dictionary<string, CSoundSlot>();

    private Dictionary<string, float> _mapSoundVolume = new Dictionary<string, float>();
    private Dictionary<int, List<string>> _mapGroupSound = new Dictionary<int, List<string>>();

    private CResourceGetter<AudioClip> _pAudioGetter;

    private bool _bIsMute = false;
    private bool _bPause = false;

    private float _fVolumeBackup_Effect;
    private float _fVolumeBackup_BGM;

    private float _fVolumeBackUp_CurrentBGM;

    private CSoundSlot _pSlotBGM;

    // ========================================================================== //

    // ===================================== //
    // public - [Do] Function                //
    // 외부 객체가 요청                      //
    // ===================================== //

    public bool DoCheckIsPlayingBGM(string strSound)
    {
        bool bIsPlaying = false;
        if (_pSlotBGM.CheckIsPlaying())
            bIsPlaying = _pSlotBGM.p_pAudioSource.clip.GetInstanceID() == GetAudioClip(strSound).GetInstanceID();

        return bIsPlaying;
    }

    public void DoPlayBGM(string strSound, System.Action CallBackOnFinishBGM = null)
    {
        float fVolume = 0f;
        if (_mapSoundVolume.ContainsKey(strSound))
            fVolume = _mapSoundVolume[strSound] * _fVolumeBGM;
        else
            fVolume = _fVolumeBGM;

        //Debug.Log( eSound + "Volume : " + fVolume );

        if (_pSlotBGM.CheckIsPlaying())
            _pSlotBGM.DoSetFadeOut(GetAudioClip(strSound), fVolume);
        else
            _pSlotBGM.DoPlaySound(GetAudioClip(strSound), fVolume);

        _CallBackOnFinishBGM = CallBackOnFinishBGM;
    }

    public CSoundSlot DoPlaySoundEffect_OrNull(AudioClip pClip, float fVolume = 1f)
    {
        if (_bIsMute) return null;

        //Debug.Log("Play Sound : " + eSound);
        CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
        if (pSoundSlot == null) return null;

        if (pSoundSlot != null)
            pSoundSlot.DoPlaySound(pClip, fVolume * _fVolumeEffect);

        return pSoundSlot;
    }

    public CSoundSlot DoPlaySoundEffect_OrNull(string strSound, float fVolume = 1f)
    {
        if (_bIsMute) return null;

        CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
        if (pSoundSlot == null) return null;

        if (_mapSoundVolume.ContainsKey(strSound))
            pSoundSlot.DoPlaySound(GetAudioClip(strSound), _mapSoundVolume[strSound] * fVolume * _fVolumeEffect);
        else
            pSoundSlot.DoPlaySound(GetAudioClip(strSound), fVolume * _fVolumeEffect);


        return pSoundSlot;
    }

    public CSoundSlot DoPlaySoundEffect_OnlySinglePlay_OrNull(string strSound, float fVolume = 1f)
    {
        if (_bIsMute) return null;

        string strSoundName = strSound;
        if (_mapCurrentPlayingSound.ContainsKey(strSoundName))
        {
            CSoundSlot pSlotCurrentPlaying = _mapCurrentPlayingSound[strSoundName];
            pSlotCurrentPlaying.DoPlaySound();

            return pSlotCurrentPlaying;
        }

        CSoundSlot pSoundSlot = FindDisableSlot_OrMakeSlot();
        if (pSoundSlot == null) return null;

        if (_mapSoundVolume.ContainsKey(strSound))
            pSoundSlot.DoPlaySound(GetAudioClip(strSound), _mapSoundVolume[strSound] * fVolume * _fVolumeEffect);
        else
            pSoundSlot.DoPlaySound(GetAudioClip(strSound), fVolume * _fVolumeEffect);

        _mapCurrentPlayingSound.Add(strSoundName, pSoundSlot);

        return pSoundSlot;
    }

    public CSoundSlot DoPlaySoundEffect_Loop(string eSound, float fVolume)
    {
        CSoundSlot pSoundSlot = DoPlaySoundEffect_OrNull(eSound, fVolume);
        if (pSoundSlot != null)
            pSoundSlot.DoPlaySoundLoop();

        return pSoundSlot;
    }

    public CSoundSlot DoPlaySoundEffect_RandomGroup<ENUM_SOUNDGROUP_NAME>(ENUM_SOUNDGROUP_NAME eGroup)
    {
        int iHashCode = eGroup.GetHashCode();
        if (_mapGroupSound[iHashCode] == null)
        {
            Debug.LogWarning("그룹이 없습니다. 그룹에 사운드를 등록해주세요 " + eGroup.ToString());
            return null;
        }
        else
        {
            int iRandomCount = _mapGroupSound[iHashCode].Count;
            int iRandomIndex = Random.Range(0, iRandomCount);

            return DoPlaySoundEffect_OrNull(_mapGroupSound[iHashCode][iRandomIndex]);
        }
    }


    public void DoSetMute(bool bMute, bool bAudioSourceControl = false)
    {
        if (_bIsMute == bMute) return;
        _bIsMute = bMute;

        if (bAudioSourceControl)
        {
            AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].mute = bMute;
        }

        if (bMute)
        {
            _fVolumeBackup_BGM = _fVolumeBGM;
            _fVolumeBackup_Effect = _fVolumeEffect;

            _fVolumeBGM = 0f;
            _fVolumeEffect = 0f;

            _fVolumeBackUp_CurrentBGM = _pSlotBGM.DoGetVolume();
        }
        else
        {
            _fVolumeBGM = _fVolumeBackup_BGM;
            _fVolumeEffect = _fVolumeBackup_Effect;

            _pSlotBGM.DoSetVolume(_fVolumeBackUp_CurrentBGM);
        }
    }

    public void DoSetPause(bool bPause)
    {
        _bPause = bPause;

        AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
        if (_bPause)
        {
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Pause();
        }
        else
        {
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Play();
        }
    }

    public void DoSetVolumeEffect(float fVolumeEffect)
    {
        _fVolumeEffect = fVolumeEffect;
    }

    public void DoSetVolumeBGM(float fVolumeBGM)
    {
        float fVolume = fVolumeBGM;

        if (_pSlotBGM.p_pAudioSource.clip != null)
        {
            string strSoundName = _pSlotBGM.p_pAudioSource.clip.name;
            if (_mapSoundVolume.ContainsKey(strSoundName))
                fVolume = _mapSoundVolume[strSoundName] * fVolumeBGM;
        }

        _fVolumeBGM = fVolumeBGM;
        _pSlotBGM.p_pAudioSource.volume = fVolume;
    }

    public void DoEnableSoundBGM(bool bEnable)
    {
        DoSetVolumeBGM(bEnable ? 0.5f : 0);
    }

    public void DoEnableSoundEffect(bool bEnable)
    {
        DoSetVolumeEffect(bEnable ? 0.5f : 0);
    }

    public void DoStopAllSound(bool bIsBGMSoundOff = true, bool bAudioSourceControl = false)
    {
        if (bAudioSourceControl)
        {
            AudioSource[] arrAudioSource = GameObject.FindObjectsOfType<AudioSource>();
            for (int i = 0; i < arrAudioSource.Length; i++)
                arrAudioSource[i].Stop();
        }

        if (bIsBGMSoundOff)
            _pSlotBGM.DoStopSound();

        for (int i = 0; i < _listSoundSlotAll.Count; i++)
            _listSoundSlotAll[i].DoStopSound();
    }

    public AudioClip GetAudioClip(string strSound)
    {
        return _pAudioGetter.GetResource(strSound);
    }

    // ===================================== //
    // public - [Event] Function             //
    // 프랜드 객체가 요청                    //
    // ===================================== //

    public void EventRegist_Group<ENUM_SOUNDGROUP_NAME>(string eSound, ENUM_SOUNDGROUP_NAME eGroup)
        where ENUM_SOUNDGROUP_NAME : System.IFormattable, System.IConvertible, System.IComparable
    {
        int iHashCode = eGroup.GetHashCode();
        if (_mapGroupSound.ContainsKey(iHashCode) == false)
            _mapGroupSound.Add(iHashCode, new List<string>());

        _mapGroupSound[iHashCode].Add(eSound);
    }

    public void EventSetSoundOption(SINI_Sound[] arrSoundINI, float fMainVolume)
    {
        if (arrSoundINI == null)
        {
            Debug.LogWarning("에러, INI 세팅 바람");
            return;
        }

        _fVolumeEffect = fMainVolume;
        for (int i = 0; i < arrSoundINI.Length; i++)
        {
            SINI_Sound sSoundInfo = arrSoundINI[i];
            if (sSoundInfo.strSoundName == "")
                continue;

            try
            {
                _mapSoundVolume[sSoundInfo.strSoundName] = sSoundInfo.fVolume;
                if (_mapGroupSound.ContainsKey(sSoundInfo.iGroupNumber) == false)
                    _mapGroupSound.Add(sSoundInfo.iGroupNumber, new List<string>());

                _mapGroupSound[sSoundInfo.iGroupNumber].Add(sSoundInfo.strSoundName);
            }
            catch
            {
                Debug.LogWarning(string.Format("Sound INI에 없는 파일이 존재합니다. {0}", sSoundInfo.strSoundName));
            }
        }
    }

    public void EventOnSlotPlayClip(CSoundSlot pSlot)
    {
        _queueNotUseSlot.Remove(pSlot);
    }

    public void EventOnSlotFinishClip(CSoundSlot pSlot)
    {
        if (pSlot == _pSlotBGM)
        {
            if (_CallBackOnFinishBGM != null)
            {
                var OnFinishBackup = _CallBackOnFinishBGM;
                _CallBackOnFinishBGM = null;
                OnFinishBackup();
            }
        }
        else
        {
            _queueNotUseSlot.Add(pSlot);
            if (pSlot.p_pAudioSource.clip != null)
            {
                string strClipName = pSlot.p_pAudioSource.clip.name;
                if (_mapCurrentPlayingSound.ContainsKey(strClipName))
                    _mapCurrentPlayingSound.Remove(strClipName);
            }
        }
    }

    // ========================================================================== //

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        _pAudioGetter = new CResourceGetter<AudioClip>("Sound");
        for (int i = 0; i < const_iDefault_SoundSlot_PoolingCount; i++)
            MakeSoundSlot();

        _pSlotBGM = FindDisableSlot_OrMakeSlot();
        EventOnSlotPlayClip(_pSlotBGM);
    }

    int _iPrevCount;

#if UNITY_EDITOR

    public override void OnUpdate()
    {
        base.OnUpdate();

        int iPlaySoundSlotCount = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
                iPlaySoundSlotCount++;
        }

        if(_iPrevCount != iPlaySoundSlotCount)
        {
            _iPrevCount = iPlaySoundSlotCount;
            name = string.Format("사운드 매니져/{0}개 재생중/Effect{1}/BGM{2}", iPlaySoundSlotCount, _fVolumeEffect, _fVolumeBGM);
        }
    }
#endif

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    private CSoundSlot MakeSoundSlot()
    {
        GameObject pObject = new GameObject(string.Format("SoundSlot_{0}", _listSoundSlotAll.Count));
        Transform pTrans = pObject.transform;

        pTrans.SetParent(transform);
        pTrans.localRotation = Quaternion.identity;
        pTrans.gameObject.SetActive(false);

        CSoundSlot pSlot = pObject.AddComponent<CSoundSlot>();
        pSlot.EventInitSoundSlot(this, EventOnSlotPlayClip, EventOnSlotFinishClip);
        _listSoundSlotAll.Add(pSlot);
        _queueNotUseSlot.Add(pSlot);

        return pSlot;
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //

    private CSoundSlot FindDisableSlot_OrMakeSlot()
    {
        CSoundSlot pFindSlot = null;
        IEnumerator<CSoundSlot> pEnum = _queueNotUseSlot.GetEnumerator();
        if (pEnum.MoveNext())
        {
            pFindSlot = pEnum.Current;
            _queueNotUseSlot.Remove(pFindSlot);
        }

        if (pFindSlot == null)
            pFindSlot = MakeSoundSlot();

        return pFindSlot;
    }
}
