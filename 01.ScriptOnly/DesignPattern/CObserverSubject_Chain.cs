#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-14 오후 8:19:02
 *	개요 : 옵저버 패턴 래퍼
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NUnit.Framework;
#endif

public class Data_HasOrder<T>
{
    public T TData { get; private set; }
    public int iSortOrder { get; private set; }

    public Data_HasOrder(T Data, int iSortOrder)
    {
        this.TData = Data;
        this.iSortOrder = iSortOrder;
    }

    static public int Compare_Data_HasOrder(Data_HasOrder<T> x, Data_HasOrder<T> y)
    {
        return x.iSortOrder.CompareTo(y.iSortOrder);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject_ChainData<TChainData>
{
    public delegate void OnChainData(TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public TChainData DoNotify(TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(TChainData arg)
    {
        TChainData pOrigin = arg;
        for (int i = 0; i < _listListener.Count; i++)
        {
            Data_HasOrder<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(pOrigin, ref arg);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + " Origin : " + pOrigin + " Current : " + arg);
        }

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = arg;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = new Data_HasOrder<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(Data_HasOrder<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject_ChainData<T1, TChainData>
{
    public delegate void OnChainData(T1 arg1, TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private T1 _LastArg; public T1 GetLastArg() { return _LastArg; }

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public TChainData DoNotify(T1 arg, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(arg, pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(T1 arg, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
        {
            Data_HasOrder<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(arg, pOrigin, ref pValue);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + "arg : " + arg + " Origin : " + pOrigin + " Current : " + pValue);
        }

        _LastArg = arg;
        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = new Data_HasOrder<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(Data_HasOrder<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject_ChainData<T1, T2, TChainData>
{
    public delegate void OnChainData(T1 arg1, T2 arg2, TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }
    [NonSerialized]
    private T2 _LastArg_2; public T2 GetLastArg_2() { return _LastArg_2; }

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public TChainData DoNotify(T1 arg1, T2 arg2, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(arg1, arg2, pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(T1 arg1, T2 arg2, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
        {
            Data_HasOrder<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(arg1, arg2, pOrigin, ref pValue);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + " arg1 : " + arg1 + " arg2 : " + arg2 + " Origin : " + pOrigin + " Current : " + pValue);
        }

        _LastArg_1 = arg1;
        _LastArg_2 = arg2;
        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = new Data_HasOrder<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(Data_HasOrder<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}


/// <summary>
/// 
/// </summary>
public class CObserverSubject_ChainData<T1, T2, T3, TChainData>
{
    public delegate void OnChainData(T1 arg1, T2 arg2, T3 arg3, TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }
    [NonSerialized]
    private T2 _LastArg_2; public T2 GetLastArg_2() { return _LastArg_2; }
    [NonSerialized]
    private T3 _LastArg_3; public T3 GetLastArg_3() { return _LastArg_3; }

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public TChainData DoNotify(T1 arg1, T2 arg2, T3 arg3, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(arg1, arg2, arg3, pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(T1 arg1, T2 arg2, T3 arg3, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
        {
            Data_HasOrder<OnChainData> data_HasOrder = _listListener[i];
            data_HasOrder.TData(arg1, arg2, arg3, pOrigin, ref pValue);
            Debug.Log(data_HasOrder.TData.Method.Name + "Call Order : " + i + " arg1 : " + arg1 + " arg2 : " + arg2 + " arg3 : " + arg3 + " Origin : " + pOrigin + " Current : " + pValue);
        }

        _LastArg_1 = arg1;
        _LastArg_2 = arg2;
        _LastArg_3 = arg3;
        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;
    }

    public void DoClear_Listener()
    {
        _mapDelegate_And_HasOrderInstance.Clear();
        _listListener.Clear();
    }

    public void DoRegist_Listener(OnChainData OnNotify, int iOrder)
    {
        if (OnNotify == null || _mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify))
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = new Data_HasOrder<OnChainData>(OnNotify, iOrder);
        _mapDelegate_And_HasOrderInstance.Add(OnNotify, pDeleagateInstance);

        _listListener.Add(pDeleagateInstance);
        _listListener.Sort(Data_HasOrder<OnChainData>.Compare_Data_HasOrder);
    }

    public void DoRemove_Listener(OnChainData OnNotify)
    {
        if (_mapDelegate_And_HasOrderInstance.ContainsKey(OnNotify) == false)
            return;

        Data_HasOrder<OnChainData> pDeleagateInstance = _mapDelegate_And_HasOrderInstance[OnNotify];

        _mapDelegate_And_HasOrderInstance.Remove(OnNotify);
        _listListener.Remove(pDeleagateInstance);
    }
}


#region Test
#if UNITY_EDITOR
public class CObserverSubject_ChainTest
{
    int _iTestValue;

    [Test]
    public void IsNotOverlap_Observer()
    {
        CObserverSubject_ChainData<int> pObserverSubject = new CObserverSubject_ChainData<int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 0);
        pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 1); // Not Regist

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify(123);
        Assert.IsTrue(_iTestValue == 123);
    }

    private void Plus_To_TestValue(int pValue_Origin, ref int pValue_Current)
    {
        _iTestValue += pValue_Origin;
    }

    // ===========================================================

    [Test]
    public void Regist_And_Remove()
    {
        CObserverSubject_ChainData<int> pObserverSubject = new CObserverSubject_ChainData<int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 0);
        pObserverSubject.DoRegist_Listener(Minus_To_TestValue, 1);

        pObserverSubject.DoRemove_Listener(Plus_To_TestValue); // Remove Plus & Minus Only

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify(10); // Minus Only
        Assert.IsTrue(_iTestValue == -10);
    }

    private void Minus_To_TestValue(int pValue_Origin, ref int pValue_Current)
    {
        _iTestValue -= pValue_Origin;
    }

    // ===========================================================

    [Test]
    public void HasMultipleObserver_And_MustCall_In_Order()
    {
        CObserverSubject_ChainData<int> pObserverSubject = new CObserverSubject_ChainData<int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(Plus_Current_To_TestValue, 2);
        pObserverSubject.DoRegist_Listener(Plus_10_Current, 0);
        pObserverSubject.DoRegist_Listener(Minus_20_Current, 1);

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify_ForDebug(100); // Result : 0 + (100 + 10 - 20)
        Assert.IsTrue(_iTestValue == (100 + 10 - 20));
    }

    private void Plus_10_Current(int pValue_Origin, ref int pValue_Current)
    {
        pValue_Current += 10;
    }

    private void Minus_20_Current(int pValue_Origin, ref int pValue_Current)
    {
        pValue_Current -= 20;
    }

    private void Plus_Current_To_TestValue(int pValue_Origin, ref int pValue_Current)
    {
        _iTestValue += pValue_Current;
    }

    // ===========================================================

    [Test]
    public void HasMultipleObserver_And_GenericParams()
    {
        CObserverSubject_ChainData<string, string, int> pObserverSubject = new CObserverSubject_ChainData<string, string, int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_HasParam, 1);
        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify("테스트_A", "테스트_1", 5);
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoRegist_Listener(PrintLog_HasParam, 2);
        pObserverSubject.DoNotify("테스트_B", "테스트_2", 8);
        Assert.IsTrue(_iTestValue == 5 + 8);

        pObserverSubject.DoRegist_Listener(Decrease_20_CurrentValue_HasParam, 0);

        pObserverSubject.DoNotify("테스트_C", "테스트_3", 10);
        Assert.IsTrue(_iTestValue == 5 + 8 + (10 - 20));
    }

    private void AddField_HasParam(string arg1, string arg2, int pValue_Origin, ref int pValue_Current)
    {
        _iTestValue += pValue_Current;
    }

    private void Decrease_20_CurrentValue_HasParam(string arg1, string arg2, int pValue_Origin, ref int pValue_Current)
    {
        pValue_Current -= 20;
    }

    private void PrintLog_HasParam(string arg1, string arg2, int pValue_Origin, ref int pValue_Current)
    {
        Debug.Log("PrintLog_HasParam - arg1 : " + arg1 + " arg2 : " + arg2 + " pValue_Origin : " + pValue_Origin + " pValue_Current : " + pValue_Current);
    }
}
#endif
#endregion
