// Class OriginName : cnguitweenextendbase
// Date 			: 2016.3.10
// Author			: Goolligo_Strix13
// Code Shcema		: 여러개의 Tween 정보를 저장 및 플레이 하기위한 기본 객체.
//
//				      - 기존의 Tween과 다르게 Play만 해도 자동으로 Factor가 리셋 됩니다.
//					  -
//                    -
#if NGUI

using UnityEngine;
using System.Collections.Generic;

public static class CGameObjectExtends
{
	public static void DoEnable(this GameObject pObj)
	{
		pObj.SetActive(true);
	}

	public static void DoDisable(this GameObject pObj)
	{
		pObj.SetActive(false);
	}
}

[System.Serializable]
abstract public class STweenInfoBase
{
	public bool bAutoDisableThis = false;
	public float fDuration = 1f;
	public float fStartDelay_Min;
	public float fStartDelay_Max;
	public UITweener.Style eStyle;
	public AnimationCurve pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public List<EventDelegate> listOnFinished = new List<EventDelegate>();

	protected float _fGap = -1f;

	public abstract float GetFromToGap(bool bCalculate = false);
}

abstract public class CNGUITweenExtendBase<TEMPLATE> : UITweener
    where TEMPLATE : STweenInfoBase, new()
{
    public List<TEMPLATE> listTweenInfo = new List<TEMPLATE>(2);
    protected TEMPLATE m_pCurrentTweenInfo;
	
	[HideInInspector]
	public float p_fTweenSpeed
	{
		get
		{
			return _fTweenSpeed;
		}

		set
		{
			_fTweenSpeed = value;
			if (Application.isPlaying && m_pCurrentTweenInfo != null)
			{
				m_pCurrentTweenInfo.fDuration = m_pCurrentTweenInfo.GetFromToGap() / _fTweenSpeed;
				duration = m_pCurrentTweenInfo.fDuration;
			}
		}
	}

	public bool _bCheckTweenAmount = true;
	public bool _bPlayOnEnable = true;
	public int _iDefaultPlayIndex = 0;

	[HideInInspector]
	public float p_fTweenAmount;

	private float _fTweenSpeed;
	private int _iLastPlayIndex = 0;
	private float _fLastFactor = 0f;
	private bool _bFactorIsIncrease = false;

	//=============================== [1. Start Public] ===============================//
	#region Public

	protected override void Start()
	{
		if(_bPlayOnEnable)
		{
			CCompoEventTrigger pEventTrigger = gameObject.AddComponent<CCompoEventTrigger>();
			pEventTrigger.DoAddEvent_Main( DoPlayTween_Forward_0 );
			pEventTrigger.p_eInputType_Main = CCompoEventTrigger.EInputType.OnEnable;
		}

		ResetToBeginning();
		_iLastPlayIndex = _iDefaultPlayIndex;
	}

	private void EventOnEnable()
	{
		ResetToBeginning();
		enabled = true;
	}

    public void SetTweenInfoSize(int iTweenInfoSize)
    {
        ProcEditListCount(iTweenInfoSize);
    }

    public void DoObjectActiveTrue() { gameObject.SetActive(true); }
    public void DoObjectActiveFalse() { gameObject.SetActive(false); }

	public void DoPlayTween_CurrentGroup() { DoPlayTween_Forward( _iLastPlayIndex ); }
    public void DoPlayTween_Forward_0() { DoPlayTween_Forward(0); }
    public void DoPlayTween_Forward_1() { DoPlayTween_Forward(1); }
    public void DoPlayTween_Forward_2() { DoPlayTween_Forward(2); }
    public void DoPlayTween_Forward_3() { DoPlayTween_Forward(3); }

    public void DoPlayTween_Reverse_0() { DoPlayTween_Reverse(0); }
    public void DoPlayTween_Reverse_1() { DoPlayTween_Reverse(1); }
    public void DoPlayTween_Reverse_2() { DoPlayTween_Reverse(2); }

    public void DoPlayTween_Forward(int iGroupNumber)
    {
		ProcSettingBeforePlay(iGroupNumber);
		ResetToBeginning();
		Play(true);
    }

	public void DoPlayTween_Reverse(int iGroupNumber)
	{
		ProcSettingBeforePlay(iGroupNumber);
		Play(false);
	}

	public void DoPlayOrStop_CheckTweenAmount(bool bPlay)
	{
		_bCheckTweenAmount = bPlay;
	}

    #endregion

    //=============================== [2. Start Overriding] =================================//
    #region Overriding

	abstract protected void UpdateTweenValue(float fFactor, bool bIsFinished);

	private void Reset()
	{
		ProcEditListCount( 1 );
	}

	protected override void OnUpdate(float factor, bool isFinished)
    {
		if (m_pCurrentTweenInfo == null)
        {
            ProcSettingBeforePlay(0);
        }

        else
        {
            UpdateTweenValue(factor, isFinished);

			if (factor == 0f) return;
			if (ignoreTimeScale == false && Time.timeScale == 0)
				return;

			// 스타일이 원스일 경우 기존의 Tween이 OnFinish를 호출하기 때문에,
			// 루프일 경우 팩터가 1이면 OnFinish를 호출하도록 기능 확장
			if (style == Style.Loop)
			{
				// 마지막 Factor가 현재 Factor보다 크면 갱신
				if (_fLastFactor <= factor)
					_fLastFactor = factor;
				else
				{
					// 마지막 Factor가 현재 Factor보다 작다는것은 이미 한바퀴 돌았다는 것으로 그때 호출
					_fLastFactor = 0f;
					EventDelegate.Execute( onFinished );
					ProcSettingEventDelegate();
				}
			}
			else if(style == Style.PingPong)
			{
				bool bIsExcute = false;
				//Debug.Log( "_bFactorIsIncrease : " + _bFactorIsIncrease + " factor : " + factor + " _fLastFactor : " + _fLastFactor );
				if (_bFactorIsIncrease)
				{
					if (_fLastFactor >= factor || _fLastFactor == 0f)
						_fLastFactor = factor;
					else
					{
						bIsExcute = true;
						_fLastFactor = 0f;
					}
				}
				else
				{
					if (_fLastFactor < factor || _fLastFactor == 1f)
						_fLastFactor = factor;
					else
					{
						bIsExcute = true;
						_fLastFactor = 1f;
					}
				}

				if(bIsExcute)
				{
					_bFactorIsIncrease = !_bFactorIsIncrease;
					EventDelegate.Execute( onFinished );
					ProcSettingEventDelegate();
				}
			}
		}
    }

    #endregion

    //=============================== [3. Start CoreLogic] ===============================//
    #region CoreLogic

    protected void ProcSettingBeforePlay(int iGroupID)
    {
        if (iGroupID >= 0 && iGroupID < listTweenInfo.Count)
        {
			m_pCurrentTweenInfo = listTweenInfo[iGroupID];
            duration = m_pCurrentTweenInfo.fDuration;

			_fTweenSpeed = m_pCurrentTweenInfo.GetFromToGap(true) / duration;

			delay = Random.Range(m_pCurrentTweenInfo.fStartDelay_Min, m_pCurrentTweenInfo.fStartDelay_Max);
			animationCurve = m_pCurrentTweenInfo.pAnimationCurve;
			style = m_pCurrentTweenInfo.eStyle;

			_iLastPlayIndex = iGroupID;
			_fLastFactor = 0f;
			ProcSettingEventDelegate();
		}
		else
        {
            Debug.Log("Error! Not Found Group ID : " + iGroupID, this);
        }
    }

    protected void ProcEditListCount(int iListSize)
    {
        if (listTweenInfo.Count < iListSize)
        {
            int iAddCount = iListSize - listTweenInfo.Count;
            for (int i = 0; i < iAddCount; i++)
            {
                listTweenInfo.Add(new TEMPLATE());
            }
        }
        else if (listTweenInfo.Count > iListSize)
        {
            int iDeleteCount = listTweenInfo.Count - iListSize;
            for (int i = 0; i < iDeleteCount; i++)
            {
                listTweenInfo.RemoveAt(i);
            }
        }
    }

	private void ProcSettingEventDelegate()
	{
		for (int i = 0; i < m_pCurrentTweenInfo.listOnFinished.Count; i++)
		{
			EventDelegate.Add( onFinished, m_pCurrentTweenInfo.listOnFinished[i], true );
			if (m_pCurrentTweenInfo.listOnFinished[i].oneShot)
				EventDelegate.Remove( m_pCurrentTweenInfo.listOnFinished, m_pCurrentTweenInfo.listOnFinished[i] );
		}
	}

	#endregion
}
#endif