using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeNum_Test : MonoBehaviour {

	private void Awake()
	{
		TestCase_InitNumber();
		TestCase_Operator_1();
		TestCase_Operator_2();
	}

	private void TestCase_InitNumber()
	{
		Debug.Log( "Start TestCase_InitNumber" );

		HugeNumber sHugeNum = new HugeNumber( 100, HugeNumber.EUnit.None );
		Debug.Log( sHugeNum );

		sHugeNum = new HugeNumber( 1234100.1234, HugeNumber.EUnit.None );
		Debug.Log( sHugeNum );

		sHugeNum = new HugeNumber( 0.1234567, HugeNumber.EUnit.A );
		Debug.Log( sHugeNum );

		sHugeNum = new HugeNumber( 1234100.1234, HugeNumber.EUnit.G );
		Debug.Log( sHugeNum );
	}

	private void TestCase_Operator_1()
	{
		Debug.Log( "Start TestCase_Operator_1" );

		HugeNumber sHugeNum1 = new HugeNumber( 123, HugeNumber.EUnit.None );
		HugeNumber sHugeNum2 = new HugeNumber( 456, HugeNumber.EUnit.None );
		Debug.Log( sHugeNum1 + sHugeNum2 );
		Debug.Log( sHugeNum1 + sHugeNum2 - sHugeNum1);

		sHugeNum1 = new HugeNumber( 100, HugeNumber.EUnit.A );
		sHugeNum2 = new HugeNumber( 100, HugeNumber.EUnit.B );
		Debug.Log( sHugeNum1 + sHugeNum2 );
		Debug.Log( sHugeNum1 + sHugeNum2 - sHugeNum1 );

		//CManagerProfiler.instance.DoStartTestCase( "Test" );
		sHugeNum1 = new HugeNumber( 1, HugeNumber.EUnit.A );
		sHugeNum2 = new HugeNumber( 1, HugeNumber.EUnit.C );
		HugeNumber sHugeNum3 = sHugeNum2;
		for (int i = 0; i < 1000; i++)
			sHugeNum3 += sHugeNum1;

		sHugeNum3 += new HugeNumber( 999, HugeNumber.EUnit.B );
		//CManagerProfiler.instance.DoFinishTestCase( "Test" );
		//CManagerProfiler.instance.DoPrintResult(false);

		Debug.Log( sHugeNum3 );
	}

	private void TestCase_Operator_2()
	{
		Debug.Log( "Start TestCase_Operator_2" );

		SCManagerProfiler.DoStartTestCase( "Test" );
		for(int i = 0; i < 1000; i++)
		{
			HugeNumber sHugeNum1 = new HugeNumber( 1, HugeNumber.EUnit.None );
			HugeNumber sHugeNum2 = sHugeNum1 * 100000000;
			HugeNumber sHugeNum3 = new HugeNumber( 1, HugeNumber.EUnit.G );

			sHugeNum3 += sHugeNum2;
			sHugeNum3 -= sHugeNum2;
		}
		SCManagerProfiler.DoFinishTestCase( "Test" );
		SCManagerProfiler.DoPrintResult( false );
	}
}
