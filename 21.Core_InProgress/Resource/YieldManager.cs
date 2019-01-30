#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 : https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitForSecondsRealTime_ForPooling : CustomYieldInstruction
{
    private float waitTime;

    public override bool keepWaiting
    {
        get { return Time.realtimeSinceStartup < waitTime; }
    }

    public WaitForSecondsRealTime_ForPooling(float time)
    {
        waitTime = Time.realtimeSinceStartup + time;
    }
}

public static class YieldManager
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class floatComparer : EqualityComparer<float>
    {
        public override bool Equals(float x, float y)
        {
            return x == y;
        }

        public override int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    #region Field
    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */

    static private Dictionary<float, WaitForSeconds> _mapYieldSeconds = new Dictionary<float, WaitForSeconds>(new floatComparer());
    static private Dictionary<float, WaitForSecondsRealTime_ForPooling> _mapYieldSecondsRealTime = new Dictionary<float, WaitForSecondsRealTime_ForPooling>(new floatComparer());

    #endregion Field
    #region Public
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public WaitForSeconds GetWaitForSecond(float fSec)
	{
		if (_mapYieldSeconds.ContainsKey(fSec) == false)
			_mapYieldSeconds.Add(fSec, new WaitForSeconds(fSec));

		return _mapYieldSeconds[fSec];
	}

    static public WaitForSecondsRealTime_ForPooling GetWaitForSecondRealTime(float fSec)
    {
        if (_mapYieldSecondsRealTime.ContainsKey(fSec) == false)
            _mapYieldSecondsRealTime.Add(fSec, new WaitForSecondsRealTime_ForPooling(fSec));

        return _mapYieldSecondsRealTime[fSec];
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/
    #endregion Public
    // ========================================================================== //
    #region Protected
    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    #endregion Protected
    // ========================================================================== //
    #region Private
    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */
    #endregion Private
}
