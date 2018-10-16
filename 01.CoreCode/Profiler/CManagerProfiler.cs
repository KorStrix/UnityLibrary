using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using TimeSpan = System.TimeSpan;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-09 오후 10:59:57
   Description : 
   Edit Log    : 

    사용법 : 
    CManagerScriptProfiler.instance.DoStartTestCase("Case1");
    Test 할 Funcion Case 1
    CManagerScriptProfiler.instance.DoFinishTestCase("Case1");

    출력하고 싶을땐
    CManagerScriptProfiler.instance.DoPrintResult();

   ============================================ */

public class CManagerProfiler : CSingletonNotMonoBase<CManagerProfiler>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    private class STestCase
    {
        public string strTestCaseName;
        public Stopwatch pStopWatch = new Stopwatch();
        public int iExcuteCount;

        public STestCase(string strTestCaseName)
        {
            this.strTestCaseName = strTestCaseName;
            iExcuteCount = 0;
        }

        public void DoStartTestCase()
        {
            pStopWatch.Start();
            iExcuteCount++;
        }

        public void DoFinishTestCase()
        {
            pStopWatch.Stop();
        }

        public void DoReset()
        {
			iExcuteCount = 0;
			pStopWatch.Stop();
            pStopWatch.Reset();
        }
    }

    /* public - Field declaration            */

    /* private - Field declaration           */

    private Dictionary<string, STestCase> _mapTestCase = new Dictionary<string, STestCase>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoStartTestCase(string strTestCaseName)
    {
        if (_mapTestCase.ContainsKey(strTestCaseName) == false)
            _mapTestCase.Add(strTestCaseName, new STestCase(strTestCaseName));

        _mapTestCase[strTestCaseName].DoStartTestCase();
    }
	
    public void DoFinishTestCase(string strTestCaseName)
    {
        _mapTestCase[strTestCaseName].DoFinishTestCase();
    }
	
    public void DoResetTestCase()
    {
        List<STestCase> listTestCase = _mapTestCase.Values.ToList();
        for (int i = 0; i < listTestCase.Count; i++)
            listTestCase[i].DoReset();
    }

    public void DoPrintResult(bool bReset)
    {
        List<STestCase> listTestCase = _mapTestCase.Values.ToList();
        for(int i = 0; i < listTestCase.Count; i++)
        {
            STestCase pTest = listTestCase[i];
            Debug.Log(string.Format("Profile Name : [{0}] TotalTime : [{1}] TestCount : [{2}] AverageMilliSec [{3}]",
                pTest.strTestCaseName, pTest.pStopWatch.Elapsed, pTest.iExcuteCount, new TimeSpan(pTest.pStopWatch.Elapsed.Ticks / pTest.iExcuteCount)));
        }

		if (bReset)
			DoResetTestCase();
    }
}
