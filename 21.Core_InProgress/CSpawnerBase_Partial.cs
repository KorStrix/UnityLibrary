using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 패턴 담당 클래스 - 패턴 추가 삭제시 여기서..
   Version	   :
   ============================================ */

public enum EMissionPatternName
{
	Line,
	Rotate_Circle,
	Circle,
	Sprial,

	Rotate_Model,
	Rotate,
}

public partial class CSpawnerBase<Enum_Key, Class_Resource> : CObjectBase
	where Enum_Key : System.IComparable, System.IConvertible
	where Class_Resource : Component
{
	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/
	 
	public void DoPlayPattern(System.Action OnFinishPattern)
	{
		_OnFinishPattern = OnFinishPattern;
		DoPlayPattern();
	}

	public void DoPlayPattern()
	{
		_iPlayCountCurrent = _iPlayCount_Pattern;
		switch (_ePattern)
		{
			case EMissionPatternName.Line: _pPattern = CoPattern_Line; break;
			case EMissionPatternName.Rotate_Circle: _pPattern = CoPattern_RotateCircle; break;
			case EMissionPatternName.Circle: _pPattern = CoPattern_Circle; break;
			case EMissionPatternName.Sprial: _pPattern = CoPattern_Sprial; break;
			case EMissionPatternName.Rotate: _pPattern = CoPattern_Rotate; break;

			case EMissionPatternName.Rotate_Model: _pPattern = CoPattern_RotateModel; break;
		}

		gameObject.SetActive(true);
		//if (gameObject.activeSelf == false)
		//	EventDelayExcuteCallBack( ProcPlayPattern, 0f );
		//else
			ProcPlayPattern();
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoPattern_Line()
	{
		_iGenerateRemainCount = Random.Range(_iGenerateCount_Min, _iGenerateCount_Max);
		if (_iMuzzleCount != 1)
		{
			while (_bIsGenerating && _iGenerateRemainCount-- > 0)
			{
				// 좌측부터 기울어진 상태에서 시작
				int iStartGap = (_iMuzzleCount / 2);
				Vector3 vecStartPos = -_vecPosGap;
				float fStartAngle = _fAngleMuzzle * -iStartGap;
				for (int i = 0; i < _iMuzzleCount; i++)
				{
					Vector3 vecPosGap = transform.position + vecStartPos;
					Vector3 vecAngleGap = transform.rotation.eulerAngles + new Vector3( 0f, 0f, fStartAngle );

					ProcShotGenerate( vecPosGap, Quaternion.Euler( vecAngleGap ) );
					fStartAngle += _fAngleMuzzle;
					vecStartPos += (Vector3)_vecPosGap;
				}
				float fDelaySecRandom = Random.Range( _fDelaySec_GenerateMin, _fDelaySec_Generate_Max );
				yield return YieldManager.GetWaitForSecond( fDelaySecRandom );
			}
		}
		else
			yield return StartCoroutine(CoGenerateSomthing());
	}

	private IEnumerator CoPattern_RotateCircle()
	{
#if NGUI
		CNGUITweenRotationSpin pTweenRotate = GetTweenRotate();

		Vector3 vecCurrentRotation = transform.rotation.eulerAngles;
		pTweenRotate.from = vecCurrentRotation;
		pTweenRotate.to = new Vector3(0f, 0f, vecCurrentRotation.z + 360f);
		//pTweenRotate.duration = _fTweenDuration;

		EventDelegate pOnFinishPattern = new EventDelegate(ProcSetPatternIsFinish);
		pOnFinishPattern.oneShot = true;
		pTweenRotate.AddOnFinished(pOnFinishPattern);
		pTweenRotate.PlayForward();
#endif
		yield return StartCoroutine(CoGenerateSomthing());
	}

	private IEnumerator CoPattern_Circle()
	{
		int iPlayCountOnCircle = _PlayCount_OnCircle;
		while (_bIsGenerating && iPlayCountOnCircle-- > 0)
		{
			Vector3 vecCurrentPos = transform.position;
			float fAngleGap = 360 / _iGenerateRemainCount;
			float fAngle = 0;
			for (int i = 0; i < _iGenerateRemainCount; i++)
			{
				ProcShotGenerate( vecCurrentPos, Quaternion.Euler( new Vector3( 0f, 0f, fAngle ) ) );
				fAngle += fAngleGap;
			}

			float fDelaySecRandom = Random.Range( _fDelaySec_GenerateMin, _fDelaySec_Generate_Max );
			yield return YieldManager.GetWaitForSecond( fDelaySecRandom );
		}
	}

	private IEnumerator CoPattern_Sprial()
	{
#if NGUI
		CNGUITweenRotationSpin pTweenRotate = GetTweenRotate();

		Vector3 vecCurrentRotation = transform.rotation.eulerAngles;
		pTweenRotate.from = vecCurrentRotation;
		pTweenRotate.to = new Vector3(0f, 0f, vecCurrentRotation.z + 360f);
		//pTweenRotate.duration = _fTweenDuration;

		//if (_fAngleRotate > 0f)
		//	pTweenRotate.PlayForward();
		//else
		//{
		//	pTweenRotate.ResetToFactor(1f);
		//	pTweenRotate.PlayReverse();
		//}

		float fAngleGap = 360 / _iMuzzleCount;
		float fAngle = 0;

		while (_bIsGenerating && _iGenerateRemainCount-- > 0)
		{
			for (int i = 0; i < _iMuzzleCount; i++)
			{
				ProcShotGenerate(transform.position, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f, fAngle)));
				fAngle += fAngleGap;
			}

			float fDelaySecRandom = Random.Range( _fDelaySec_GenerateMin, _fDelaySec_Generate_Max );
			yield return SCManagerYield.GetWaitForSecond( fDelaySecRandom );
		}
#endif
		yield return null;
	}


	private IEnumerator CoPattern_RotateModel()
	{
		//CNGUITweenRotationSpin pTweenRotate = GetTweenRotate_EnemyModel();

		//Vector3 vecCurrentRotation = pTweenRotate.transform.rotation.eulerAngles;
		//pTweenRotate.from = vecCurrentRotation;
		//pTweenRotate.to = new Vector3(0f, 0f, vecCurrentRotation.z + _fAngleRotate );
		//pTweenRotate.duration = _fTweenDuration;

		//EventDelegate pOnFinishPattern = new EventDelegate(ProcSetPatternIsFinish);
		//pOnFinishPattern.oneShot = true;
		//pTweenRotate.AddOnFinished(pOnFinishPattern);
		//pTweenRotate.PlayForward();

		//if (_iMuzzleCount != 1)
		//{
		//	while (_bIsGenerating && _iGenerateRemainCount-- > 0)
		//	{
		//		// 좌측부터 기울어진 상태에서 시작
		//		int iStartGap = (_iMuzzleCount / 2);
		//		Vector3 vecStartPos = -_vecPosGap;
		//		float fStartAngle = _fAngleMuzzle * -iStartGap;
		//		for (int i = 0; i < _iMuzzleCount; i++)
		//		{
		//			Vector3 vecPosGap = _pTransformCached.position + vecStartPos;
		//			Vector3 vecAngleGap = _pTransformCached.rotation.eulerAngles + new Vector3( 0f, 0f, fStartAngle );

		//			ProcShotGenerate( vecPosGap, Quaternion.Euler( vecAngleGap ) );
		//			fStartAngle += _fAngleMuzzle;
		//			vecStartPos += (Vector3)_vecPosGap;
		//		}

		//		yield return SCManagerYield.GetWaitForSecond( _fDelaySec_Generate );
		//	}
		//}
		//else
			yield return StartCoroutine(CoGenerateSomthing());
	}

	private IEnumerator CoPattern_Rotate()
	{
		//CNGUITweenRotationSpin pTweenRotate = GetTweenRotate();

		//Vector3 vecCurrentRotation = pTweenRotate.transform.rotation.eulerAngles;
		//pTweenRotate.from = vecCurrentRotation;
		//pTweenRotate.to = new Vector3(0f, 0f, vecCurrentRotation.z + _fAngleMuzzle);
		//pTweenRotate.duration = _fTweenDuration;

		//EventDelegate pOnFinishPattern = new EventDelegate(ProcSetPatternIsFinish);
		//pOnFinishPattern.oneShot = true;
		//pTweenRotate.AddOnFinished(pOnFinishPattern);
		//pTweenRotate.PlayForward();

		yield return StartCoroutine(CoGenerateSomthing());
	}
	
	/* private - Other[Find, Calculate] Func 
찾기, 계산등 단순 로직(Simpe logic)         */

}
