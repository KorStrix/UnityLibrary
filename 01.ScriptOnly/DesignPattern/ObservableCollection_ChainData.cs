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
public class ObservableCollection_ChainData<TChainData>
{
    public delegate void OnChainData(TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public event OnChainData Subscribe
    {
        add
        {
            DoRegist_Listener(value, 0);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

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
public class ObservableCollection_ChainData<Args, TChainData>
{
    public delegate void OnChainData(Args arg1, TChainData pValue_Origin, ref TChainData pValue_Current);

    [NonSerialized]
    private Args _LastArg; public Args GetLastArg() { return _LastArg; }

    [NonSerialized]
    private TChainData _LastChainData_Origin; public TChainData GetLastChainData_Origin() { return _LastChainData_Origin; }
    [NonSerialized]
    private TChainData _LastChainData_Current; public TChainData GetLasChainData_Current() { return _LastChainData_Current; }

    protected Dictionary<OnChainData, Data_HasOrder<OnChainData>> _mapDelegate_And_HasOrderInstance = new Dictionary<OnChainData, Data_HasOrder<OnChainData>>();
    protected List<Data_HasOrder<OnChainData>> _listListener = new List<Data_HasOrder<OnChainData>>();

    public event OnChainData Subscribe
    {
        add
        {
            DoRegist_Listener(value, 0);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

    public TChainData DoNotify(Args arg, TChainData pValue)
    {
        TChainData pOrigin = pValue;
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i].TData(arg, pOrigin, ref pValue);

        _LastChainData_Origin = pOrigin;
        _LastChainData_Current = pValue;

        return _LastChainData_Current;
    }

    public void DoNotify_ForDebug(Args arg, TChainData pValue)
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