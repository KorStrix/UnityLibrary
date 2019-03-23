using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-13 오후 1:00:07
   Description : 
   Edit Log    : https://ejonghyuck.github.io/blog/2016-12-12/dictionary-struct-enum-key/
                 Generic 변수에는 Enum 타입을 알수 없으므로 이걸로 대체

    Dictionary Indexor 100만번 프로파일링

    [dicInt]         TotalTime : [00:00:00.0575604]
    [dicEnum_normal] TotalTime : [00:00:00.2926733]
    [dicEnum_fast]   TotalTime : [00:00:00.0518696]  - 링크에 있는 결과, 위 사항으로 인해 못함
    [dicCustom]      TotalTime : [00:00:00.1463990]  - this case

	단점으로 Key의 반복문을 돌리기가 힘들다..

   ============================================ */

public class CDictionary_ForEnumKey<TKey, TValue> : Dictionary<int, TValue>
	where TKey : System.IConvertible, System.IComparable
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	// 카피용
	//private class CComparerEnum : IEqualityComparer<enum>
	//{
	//	public bool Equals(enum x, enum y)
	//	{
	//		return (int)x == (int) y;
	//	}

	//	public int GetHashCode(enum obj)
	//	{
	//		return (int) obj;
	//	}
	//}

	private class CComparerInt : IEqualityComparer<int>
	{
		public bool Equals(int x, int y)
		{
			return x == y;
		}

		public int GetHashCode(int obj)
		{
			return obj;
		}
	}

	/* public - Variable declaration            */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public CDictionary_ForEnumKey() : base(new CComparerInt()) { }
	
	public TValue this[TKey key]
    {
        get
        {
            int iHashCode = key.GetHashCode();
            if (ContainsKey(iHashCode) == false)
            {
                Debug.LogWarning(string.Format("{0}의 {1}은딕셔너리에 없습니다.", typeof(TKey), key));
                return default(TValue);
            }

            return base[iHashCode];
        }
        set
        {
            base[key.GetHashCode()] = value;
        }
    }

    public void Add(TKey key, TValue value)
    {
        int iHashCode = key.GetHashCode();
        if (ContainsKey(iHashCode))
        {
            Debug.Log ( string.Format("이미 {0}의 {1}은 Dictionary KeyList에 들어가 있습니다.", typeof(TKey), key));
            return;
        }

        base.Add(iHashCode, value);
    }

	public TKey ConvertHashCodeToEnum(int iHashCode)
	{
		if(ContainsKey( iHashCode ) == false)
		{
			Debug.LogWarning( "Error" );
			return default( TKey );
		}

		return (TKey)(object)iHashCode;
	}

	public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(key.GetHashCode(), out value);
    }

    public bool Remove(TKey key)
    {
        return Remove(key.GetHashCode());
    }

    public bool ContainsKey(TKey key)
    {
        return ContainsKey(key.GetHashCode());
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */
	   
    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
