using UnityEngine;

public class Profiler_CallbackTest : MonoBehaviour {
	
	public int iTestCount = 1000000;

	public event System.Action _EventTestCall;
	public System.Action _CallBack;

	private void Awake()
	{
		_EventTestCall += TestCall;
		_CallBack = TestCall;
	}
	
	private void OnEnable()
	{
		CManagerProfiler.instance.DoStartTestCase("EventTestCall");
		for(int i = 0; i < iTestCount; i++)
			_EventTestCall();
		CManagerProfiler.instance.DoFinishTestCase("EventTestCall");

		CManagerProfiler.instance.DoStartTestCase("CallBack");
		for (int i = 0; i < iTestCount; i++)
			_CallBack();
		CManagerProfiler.instance.DoFinishTestCase("CallBack");

		CManagerProfiler.instance.DoStartTestCase("SendMessage");
		for (int i = 0; i < iTestCount; i++)
			SendMessage("TestCall");
		CManagerProfiler.instance.DoFinishTestCase("SendMessage");

		CManagerProfiler.instance.DoPrintResult(false);
	}

	void TestCall()
	{
	}
}
