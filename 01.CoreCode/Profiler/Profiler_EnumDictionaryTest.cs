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
			CManagerProfiler.instance.DoStartTestCase("mapInt_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapInt[0] = i;
			CManagerProfiler.instance.DoFinishTestCase("mapInt_Setter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumNormal_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumNormal[ETestEnum.Test] = i;
			CManagerProfiler.instance.DoFinishTestCase("mapEnumNormal_Setter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumComparer_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumComparer[ETestEnum.Test] = i;
			CManagerProfiler.instance.DoFinishTestCase("mapEnumComparer_Setter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumCustom_Setter");
			for (int i = 0; i < iTestCount; i++)
				_mapEnumCustom[ETestEnum.Test] = i;
			CManagerProfiler.instance.DoFinishTestCase("mapEnumCustom_Setter");

			CManagerProfiler.instance.DoPrintResult(false);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			print("Test Getter Start");
			int iTestNum = 0;
			CManagerProfiler.instance.DoStartTestCase("mapInt_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapInt[0];
			CManagerProfiler.instance.DoFinishTestCase("mapInt_Getter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumNormal_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumNormal[ETestEnum.Test];
			CManagerProfiler.instance.DoFinishTestCase("mapEnumNormal_Getter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumComparer_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumComparer[ETestEnum.Test];
			CManagerProfiler.instance.DoFinishTestCase("mapEnumComparer_Getter");

			CManagerProfiler.instance.DoStartTestCase("mapEnumCustom_Getter");
			for (int i = 0; i < iTestCount; i++)
				iTestNum = _mapEnumCustom[ETestEnum.Test];
			CManagerProfiler.instance.DoFinishTestCase("mapEnumCustom_Getter");

			CManagerProfiler.instance.DoPrintResult(false);
		}

	}
}
