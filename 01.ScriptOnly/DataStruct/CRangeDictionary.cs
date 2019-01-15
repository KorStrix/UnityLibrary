#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-18 오후 5:25:13
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

/// <summary>
/// Key는 Min ~ Max Range로 이루어져 있으며,
/// 특정 값을 넣으면 Range에 해당하는 Value 혹은 Null을 리턴합니다.
/// </summary>
public class CRangeDictionary<TKey, TValue> : IEnumerable //IDictionary<CRangeDictionary<TKey, TValue>.Range, TValue>
    where TKey : IComparable<TKey>
{
    [System.Serializable]
    public struct Range
    {
        public TKey TKeyMin { get; private set; }
        public TKey TKeyMax { get; private set; }

        public Range(TKey TKeyMin, TKey TKeyMax)
        {
            this.TKeyMin = TKeyMin;
            this.TKeyMax = TKeyMax;
        }
    }

    public int Count
    {
        get { return _InDictionary.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public ICollection<Range> Keys
    {
        get { return _InDictionary.Keys; }
    }

    public ICollection<TValue> Values
    {
        get { return _InDictionary.Values; }
    }

    public TValue this[TKey key]
    {
        get
        {
            return GetValue(key);
        }
    }

    Dictionary<Range, TValue> _InDictionary = new Dictionary<Range, TValue>();
    Dictionary<TKey, Range> _mapAlreadyExist = new Dictionary<TKey, Range>();

    public bool Add(TKey Range_Min, TKey Range_Max, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(Range_Min) || _mapAlreadyExist.ContainsKey(Range_Max))
            return false;

        Range sRange = new Range(Range_Min, Range_Max);
        _mapAlreadyExist.Add(Range_Min, sRange);
        _mapAlreadyExist.Add(Range_Max, sRange);
        _InDictionary.Add(sRange, pValue);

        return true;
    }

    public void Add(Range sRange, TValue pValue)
    {
        if (_mapAlreadyExist.ContainsKey(sRange.TKeyMin) || _mapAlreadyExist.ContainsKey(sRange.TKeyMax))
            return;

        _mapAlreadyExist.Add(sRange.TKeyMin, sRange);
        _mapAlreadyExist.Add(sRange.TKeyMax, sRange);
        _InDictionary.Add(sRange, pValue);
    }

    public bool Remove(TKey Range_Min, TKey Range_Max)
    {
        Range sRange = new Range(Range_Min, Range_Max);
        if(_InDictionary.ContainsKey(sRange) == false)
            return false;

        _mapAlreadyExist.Remove(Range_Min);
        _mapAlreadyExist.Remove(Range_Max);
        _InDictionary.Remove(sRange);

        return true;
    }

    public TValue GetValue(TKey tKey)
    {
        foreach(TKey tKeyCurrent in _mapAlreadyExist.Keys)
        {
            Range sRange = _mapAlreadyExist[tKeyCurrent];
            int iCompareCurrent = tKeyCurrent.CompareTo(tKey);
            if (iCompareCurrent == -1) // 비교값이 KeyCurrent보다 크다면
            {
                // Min은 KeyCurrent여야 하고, 비교값은 Max보다 작아야 한다.
                if(sRange.TKeyMin.CompareTo(tKeyCurrent) == 0 && sRange.TKeyMax.CompareTo(tKey) == 1)
                    return _InDictionary[sRange];
            }
            else if(iCompareCurrent == 1) // 비교값이 KeyCurrent보다 작다면
            {
                // Max은 KeyCurrent여야 하고, 비교값은 Max보다 작아야 한다.
                if (sRange.TKeyMin.CompareTo(tKey) == -1 && sRange.TKeyMax.CompareTo(tKeyCurrent) == 0)
                    return _InDictionary[sRange];
            }
            else // 비교값이 KeyCurrent와 같다면 위 조건을 둘다 실행해야 한다.
            {
                if (sRange.TKeyMin.CompareTo(tKeyCurrent) == 0 && sRange.TKeyMax.CompareTo(tKey) == 1)
                    return _InDictionary[sRange];
                if (sRange.TKeyMin.CompareTo(tKey) == -1 && sRange.TKeyMax.CompareTo(tKeyCurrent) == 0)
                    return _InDictionary[sRange];
            }
        }

        return default(TValue);
    }

    public void Clear()
    {
        _mapAlreadyExist.Clear();
        _InDictionary.Clear();
    }

    public bool ContainsKey(Range key)
    {
        return _InDictionary.ContainsKey(key);
    }

    public bool Remove(Range key)
    {
        return _InDictionary.Remove(key);
    }

    public bool TryGetValue(Range key, out TValue value)
    {
        return _InDictionary.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<Range, TValue> item)
    {
        _InDictionary.Add(item.Key, item.Value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _InDictionary.GetEnumerator();
    }
}

#region Test
#if UNITY_EDITOR

public class CRangeDictionary_Test
{
    [Test]
    public void CRangeDictionary_Dont_Add_OverlapRange()
    {
        CRangeDictionary<int, string> rangeDictionary = new CRangeDictionary<int, string>();

        Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
        Assert.IsFalse(rangeDictionary.Add(5, 10, "5~10")); // Fail
        Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));

        Assert.IsTrue(rangeDictionary.Remove(1, 10));
        Assert.IsTrue(rangeDictionary.Add(5, 10, "5~10")); // 위에선 실패했으나 이제 성공
    }

    [Test]
    public void CRangeDictionary_Is_Working()
    {
        CRangeDictionary<int, string> rangeDictionary = new CRangeDictionary<int, string>();

        Assert.IsTrue(rangeDictionary.Add(-10, 0, "-10~0"));
        Assert.IsTrue(rangeDictionary.Add(1, 10, "1~10"));
        Assert.IsTrue(rangeDictionary.Add(11, 20, "11~20"));

        // True Case
        for (int i = -10; i <= 0; i++)
            Assert.IsTrue(rangeDictionary.GetValue(i) == "-10~0");

        for (int i = 1; i <= 10; i++)
            Assert.IsTrue(rangeDictionary.GetValue(i) == "1~10");

        for (int i = 11; i <= 20; i++)
            Assert.IsTrue(rangeDictionary.GetValue(i) == "11~20");


        // False Case
        for (int i = 0; i < 10; i++)
            Assert.IsTrue(rangeDictionary.GetValue(UnityEngine.Random.Range(11, 100000)) != "1~10");

        // Null Case
        for (int i = 0; i < 10; i++)
            Assert.IsTrue(rangeDictionary.GetValue(UnityEngine.Random.Range(21, 100000)) == null);
    }
}

#endif
#endregion Test