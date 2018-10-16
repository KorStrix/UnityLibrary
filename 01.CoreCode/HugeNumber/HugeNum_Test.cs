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

		SHugeNumber sHugeNum = new SHugeNumber( 100, SHugeNumber.EUnit.None );
		Debug.Log( sHugeNum );

		sHugeNum = new SHugeNumber( 1234100.1234, SHugeNumber.EUnit.None );
		Debug.Log( sHugeNum );

		sHugeNum = new SHugeNumber( 0.1234567, SHugeNumber.EUnit.A );
		Debug.Log( sHugeNum );

		sHugeNum = new SHugeNumber( 1234100.1234, SHugeNumber.EUnit.G );
		Debug.Log( sHugeNum );
	}

	private void TestCase_Operator_1()
	{
		Debug.Log( "Start TestCase_Operator_1" );

		SHugeNumber sHugeNum1 = new SHugeNumber( 123, SHugeNumber.EUnit.None );
		SHugeNumber sHugeNum2 = new SHugeNumber( 456, SHugeNumber.EUnit.None );
		Debug.Log( sHugeNum1 + sHugeNum2 );
		Debug.Log( sHugeNum1 + sHugeNum2 - sHugeNum1);

		sHugeNum1 = new SHugeNumber( 100, SHugeNumber.EUnit.A );
		sHugeNum2 = new SHugeNumber( 100, SHugeNumber.EUnit.B );
		Debug.Log( sHugeNum1 + sHugeNum2 );
		Debug.Log( sHugeNum1 + sHugeNum2 - sHugeNum1 );

		//CManagerProfiler.instance.DoStartTestCase( "Test" );
		sHugeNum1 = new SHugeNumber( 1, SHugeNumber.EUnit.A );
		sHugeNum2 = new SHugeNumber( 1, SHugeNumber.EUnit.C );
		SHugeNumber sHugeNum3 = sHugeNum2;
		for (int i = 0; i < 1000; i++)
			sHugeNum3 += sHugeNum1;

		sHugeNum3 += new SHugeNumber( 999, SHugeNumber.EUnit.B );
		//CManagerProfiler.instance.DoFinishTestCase( "Test" );
		//CManagerProfiler.instance.DoPrintResult(false);

		Debug.Log( sHugeNum3 );
	}

	private void TestCase_Operator_2()
	{
		Debug.Log( "Start TestCase_Operator_2" );

		CManagerProfiler.instance.DoStartTestCase( "Test" );
		for(int i = 0; i < 1000; i++)
		{
			SHugeNumber sHugeNum1 = new SHugeNumber( 1, SHugeNumber.EUnit.None );
			SHugeNumber sHugeNum2 = sHugeNum1 * 100000000;
			SHugeNumber sHugeNum3 = new SHugeNumber( 1, SHugeNumber.EUnit.G );

			sHugeNum3 += sHugeNum2;
			sHugeNum3 -= sHugeNum2;
		}
		CManagerProfiler.instance.DoFinishTestCase( "Test" );
		CManagerProfiler.instance.DoPrintResult( false );
	}
}
