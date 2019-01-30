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
		SCManagerProfiler.DoStartTestCase("EventTestCall");
		for(int i = 0; i < iTestCount; i++)
			_EventTestCall();
		SCManagerProfiler.DoFinishTestCase("EventTestCall");

		SCManagerProfiler.DoStartTestCase("CallBack");
		for (int i = 0; i < iTestCount; i++)
			_CallBack();
		SCManagerProfiler.DoFinishTestCase("CallBack");

		SCManagerProfiler.DoStartTestCase("SendMessage");
		for (int i = 0; i < iTestCount; i++)
			SendMessage("TestCall");
		SCManagerProfiler.DoFinishTestCase("SendMessage");

		SCManagerProfiler.DoPrintResult(false);
	}

	void TestCall()
	{
	}
}
