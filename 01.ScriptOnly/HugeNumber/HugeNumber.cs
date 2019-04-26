#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NUnit.Framework;
#endif

#pragma warning disable 0661

[System.Serializable]
public struct HugeNumber
{
	public static HugeNumber Zero { get { return _Zero; } }
	private static HugeNumber _Zero = new HugeNumber();

	public enum EUnit
	{
		None,
		A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, W, X, Y, Z,
	}

	public bool p_bCheckIsEmpty { get { return _dNumber == 0.0; } }

	[SerializeField]
	private double _dNumber;	public double p_dNumber {  get { return _dNumber; } }
	[SerializeField]
	private EUnit _eUnit;		public EUnit p_eUnit {  get { return _eUnit; } }

    public HugeNumber(int iNumber, EUnit eUnit = EUnit.None)
    {
        bool bIsNegative = iNumber < 0;
        if (bIsNegative)
            iNumber *= -1;

        decimal dNumberDecimal = (decimal)iNumber;
        int iUnitOffset = 0;
        while (dNumberDecimal < 1)
        {
            if ((int)eUnit - iUnitOffset <= 0)
                break;

            dNumberDecimal = dNumberDecimal * 1000;
            iUnitOffset--;
        }

        while (dNumberDecimal >= 1000)
        {
            dNumberDecimal = dNumberDecimal / 1000;
            iUnitOffset++;
        }

        _dNumber = (double)dNumberDecimal;
        _eUnit = eUnit + iUnitOffset;

        if (bIsNegative)
            _dNumber *= -1;
    }

	public HugeNumber( double dNumber, EUnit eUnit = EUnit.None )
	{
		bool bIsNegative = dNumber < 0;
		if (bIsNegative)
			dNumber *= -1;

		decimal dNumberDecimal = (decimal)dNumber;
		int iUnitOffset = 0;
		while (dNumberDecimal < 1)
		{
			if ((int)eUnit - iUnitOffset <= 0)
				break;

			dNumberDecimal = dNumberDecimal * 1000;
			iUnitOffset--;
		}

		while (dNumberDecimal >= 1000)
		{
			dNumberDecimal = dNumberDecimal / 1000;
			iUnitOffset++;
		}

		_dNumber = (double)dNumberDecimal;
		_eUnit = eUnit + iUnitOffset;

		if (bIsNegative)
			_dNumber *= -1;
	}

	static public bool CheckIsCompareUnit( HugeNumber sNumberA, HugeNumber sNumberB, out HugeNumber sNumberHuge, out HugeNumber sNumberSmall )
	{
		bool bIsCompare = true;
		if (sNumberA._eUnit > sNumberB._eUnit)
		{
			sNumberHuge = sNumberA;
			sNumberSmall = sNumberB;
			bIsCompare = false;
		}
		else if (sNumberA._eUnit < sNumberB._eUnit)
		{
			sNumberHuge = sNumberB;
			sNumberSmall = sNumberA;
			bIsCompare = false;
		}

		else if (sNumberA._dNumber > sNumberB._dNumber)
		{
			sNumberHuge = sNumberA;
			sNumberSmall = sNumberB;
			bIsCompare = true;
		}
		else if (sNumberA._dNumber < sNumberB._dNumber)
		{
			sNumberHuge = sNumberB;
			sNumberSmall = sNumberA;
			bIsCompare = true;
		}
		else
		{
			sNumberHuge = new HugeNumber();
			sNumberSmall = new HugeNumber();
		}

		return bIsCompare;
	}

    public static explicit operator HugeNumber(int iNumber)
    {
        return new HugeNumber(iNumber);
    }

    // ================================================================================

    public override string ToString()
	{
		if (Mathf.Abs( (float)_dNumber) < 0.01)
			return "0";
		else if (_eUnit != EUnit.None)
			return _dNumber.ToString( "###.#" + _eUnit );
		else
			return _dNumber.ToString( "###.#" );
	}

	static public HugeNumber operator +( HugeNumber sNumberA, HugeNumber sNumberB )
	{
		HugeNumber sNumberHuge;
		HugeNumber sNumberSmall;
		bool bIsCompare = CheckIsCompareUnit( sNumberA, sNumberB, out sNumberHuge, out sNumberSmall );
		if (bIsCompare)
			return new HugeNumber( sNumberA._dNumber + sNumberB._dNumber, sNumberHuge._eUnit );
		else
		{
			int iUnitOffset = sNumberHuge._eUnit - sNumberSmall._eUnit;
			for (int i = 0; i < iUnitOffset; i++)
				sNumberSmall._dNumber *= 0.001;

			return new HugeNumber( sNumberHuge._dNumber + sNumberSmall._dNumber, sNumberHuge._eUnit );
		}
	}


	static public HugeNumber operator +( HugeNumber sNumberA, int iNumberB )
	{
		int iUnitOffset = sNumberA._eUnit - EUnit.None;
		double dNumberB = iNumberB;
		for (int i = 0; i < iUnitOffset; i++)
			dNumberB *= 0.001;

		return new HugeNumber( sNumberA._dNumber + dNumberB, sNumberA._eUnit );
	}

	static public HugeNumber operator -( HugeNumber sNumberA, HugeNumber sNumberB )
	{
		HugeNumber sNumberHuge;
		HugeNumber sNumberSmall;
		bool bIsCompare = CheckIsCompareUnit( sNumberA, sNumberB, out sNumberHuge, out sNumberSmall );
		if (bIsCompare)
			return new HugeNumber( sNumberA._dNumber - sNumberB._dNumber, sNumberHuge._eUnit );
		else
		{
			bool bIsNegative = sNumberA.p_eUnit < sNumberB.p_eUnit;
			int iUnitOffset = sNumberHuge._eUnit - sNumberSmall._eUnit;
			for (int i = 0; i < iUnitOffset; i++)
				sNumberSmall._dNumber *= 0.001;

			HugeNumber sReturnNumber = new HugeNumber( sNumberHuge._dNumber - sNumberSmall._dNumber, sNumberHuge._eUnit );
			if (bIsNegative)
				sReturnNumber._dNumber *= -1;

			return sReturnNumber;
		}
	}

	static public HugeNumber operator -( HugeNumber sNumberA, int iNumberB )
	{
		int iUnitOffset = sNumberA._eUnit - EUnit.None;
		double dNumberB = iNumberB;
		for (int i = 0; i < iUnitOffset; i++)
			dNumberB *= 0.001;

		return new HugeNumber( sNumberA._dNumber - dNumberB, sNumberA._eUnit );
	}

	static public HugeNumber operator *( HugeNumber sNumberA, HugeNumber sNumberB )
	{
		HugeNumber sNumberHuge;
		HugeNumber sNumberSmall;
		bool bIsCompare = CheckIsCompareUnit( sNumberA, sNumberB, out sNumberHuge, out sNumberSmall );
		if (bIsCompare)
			return new HugeNumber( sNumberA._dNumber + sNumberB._dNumber, sNumberHuge._eUnit );
		else
		{
			int iUnitOffset = sNumberHuge._eUnit - sNumberSmall._eUnit;
			for (int i = 0; i < iUnitOffset; i++)
				sNumberSmall._dNumber *= 0.001;

			return new HugeNumber( sNumberHuge._dNumber * sNumberSmall._dNumber, sNumberHuge._eUnit );
		}
	}

	static public HugeNumber operator *( HugeNumber sNumber, int iNumber )
	{
		return new HugeNumber( sNumber._dNumber * iNumber, sNumber._eUnit );
	}

	static public HugeNumber operator *(HugeNumber sNumber, float fNumber)
	{
		return new HugeNumber(sNumber._dNumber * fNumber, sNumber._eUnit);
	}

	static public float operator /( HugeNumber sNumberA, HugeNumber sNumberB )
	{
		HugeNumber sNumberHuge;
		HugeNumber sNumberSmall;
		bool bIsCompare = CheckIsCompareUnit( sNumberA, sNumberB, out sNumberHuge, out sNumberSmall );
		if (bIsCompare)
			return (float)(sNumberA._dNumber / sNumberB._dNumber);
		else
		{
			int iUnitOffset = sNumberHuge._eUnit - sNumberSmall._eUnit;
			for (int i = 0; i < iUnitOffset; i++)
				sNumberSmall._dNumber *= 0.001;

			return (float)(sNumberSmall._dNumber / sNumberHuge._dNumber);
		}
	}

	static public bool operator <( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_dNumber < 0) return true;
		if (sNumberA.p_eUnit >= EUnit.C) return false;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return sNumberA._dNumber < iNumber;
	}

	static public bool operator >( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_dNumber > 0 && sNumberA.p_eUnit >= EUnit.C) return true;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return sNumberA._dNumber > iNumber;
	}

	static public bool operator <=( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_dNumber < 0) return true;
		if (sNumberA.p_eUnit >= EUnit.C) return false;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return sNumberA._dNumber <= iNumber;
	}

	static public bool operator >=( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_dNumber > 0 && sNumberA.p_eUnit >= EUnit.C) return true;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return sNumberA._dNumber >= iNumber;
	}

	static public bool operator ==( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_eUnit >= EUnit.C) return false;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return Mathf.FloorToInt( (float)sNumberA._dNumber ) == iNumber;
	}

	static public bool operator !=( HugeNumber sNumberA, int iNumber )
	{
		if (sNumberA.p_eUnit >= EUnit.C) return true;

		for (int i = 0; i < (int)sNumberA.p_eUnit; i++)
			sNumberA._dNumber *= 1000;

		return Mathf.FloorToInt( (float)sNumberA._dNumber ) != iNumber;
	}

	public override bool Equals( object obj )
	{
		return base.Equals( obj );
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

#region Test
#if UNITY_EDITOR

public class SHugeNumber_Test
{
    [Test]
    public void HugeNumber_ToStringTest()
    {
        int iValue = 1000;
        string striValueString = "1A";

        HugeNumber sHugeNumber = new HugeNumber(iValue);
        Assert.IsTrue(sHugeNumber.ToString() == striValueString);
    }
}

#endif
#endregion