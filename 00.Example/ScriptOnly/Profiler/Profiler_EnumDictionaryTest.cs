using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profiler_EnumDictionaryTest : MonoBehaviour
{
	public enum ETestEnum
	{
		Test,
	}

	private class CComparerEnum : IEqualityComparer<ETestEnum>
	{
		public bool Equals(ETestEnum x, ETestEnum y)
		{
			return (int)x == (int)y;
		}

		public int GetHashCode(ETestEnum obj)
		{
			return (int)obj;
		}
	}

	public int iTestCount = 1000000;

	Dictionary<int, int> _mapInt = new Dictionary<int, int>();
	Dictionary<ETestEnum, int> _mapEnumNormal = new Dictionary<ETestEnum, int>();
	Dictionary<ETestEnum, int> _mapEnumComparer = new Dictionary<ETestEnum, int>(new CComparerEnum());
	CDictionary_ForEnumKey<ETestEnum, int> _mapEnumCustom = new CDictionary_ForEnumKey<ETestEnum, int>();

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			print("Test Setter Start");
			SCManagerProfiler.DoStartTestCase("mapInt_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapInt[0] = i;
			SCManagerProfiler.DoFinishTestCase("mapInt_Setter");

			SCManagerProfiler.DoStartTestCase("mapEnumNormal_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumNormal[ETestEnum.Test] = i;
			SCManagerProfiler.DoFinishTestCase("mapEnumNormal_Setter");

			SCManagerProfiler.DoStartTestCase("mapEnumComparer_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumComparer[ETestEnum.Test] = i;
			SCManagerProfiler.DoFinishTestCase("mapEnumComparer_Setter");

			SCManagerProfiler.DoStartTestCase("mapEnumCustom_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumCustom[ETestEnum.Test] = i;
			SCManagerProfiler.DoFinishTestCase("mapEnumCustom_Setter");

			SCManagerProfiler.DoPrintResult(false);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			print("Test Getter Start");
			int iTestNum = 0;
			SCManagerProfiler.DoStartTestCase("mapInt_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapInt[0];
			SCManagerProfiler.DoFinishTestCase("mapInt_Getter");

			SCManagerProfiler.DoStartTestCase("mapEnumNormal_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumNormal[ETestEnum.Test];
			SCManagerProfiler.DoFinishTestCase("mapEnumNormal_Getter");

			SCManagerProfiler.DoStartTestCase("mapEnumComparer_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumComparer[ETestEnum.Test];
			SCManagerProfiler.DoFinishTestCase("mapEnumComparer_Getter");

			SCManagerProfiler.DoStartTestCase("mapEnumCustom_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumCustom[ETestEnum.Test];
			SCManagerProfiler.DoFinishTestCase("mapEnumCustom_Getter");

			SCManagerProfiler.DoPrintResult(false);
		}

	}
}
