#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-10-14 오후 8:19:02
 *	개요 : 단일 함수만 필요로 하는 옵저버 패턴 래퍼
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class ObservableCollection
{
    protected HashSet<System.Action> _setListener = new HashSet<System.Action>();
    protected List<System.Action> _listListener = new List<System.Action>();

    public event System.Action Subscribe
    {
        add
        {
            DoRegist_Listener(value);
        }

        remove
        {   
            DoRemove_Listener(value);
        }
    }

    public event System.Action Subscribe_And_Listen_CurrentData
    {
        add
        {
            DoRegist_Listener(value, true);
        }

        remove
        {
            DoRemove_Listener(value);
        }
    }

    public void DoNotify()
    {
        _setListener.ToList(_listListener);
        foreach (var pAction in _listListener)
            pAction();
    }

    public void DoClear_Listener()
    {
        _setListener.Clear();
    }

    public void DoRegist_Listener(System.Action OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnNotify == null)
            return;

        if (_setListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnNotify();

            _setListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action OnNotify)
    {
        if (_setListener.Contains(OnNotify))
            _setListener.Remove(OnNotify);
    }
}

/// <summary>
/// 
/// </summary>
public class ObservableCollection<Args>
{
    [NonSerialized]
    private Args _LastArg_1; public Args GetLastArg_1() { return _LastArg_1; }

    protected HashSet<System.Action<Args>> _setListener = new HashSet<System.Action<Args>>();
    protected List<System.Action<Args>> _listListener = new List<System.Action<Args>>();

    public event System.Action<Args> Subscribe
    {
        add { DoRegist_Listener(value); }
        remove { DoRemove_Listener(value); }
    }

    public event System.Action<Args> Subscribe_And_Listen_CurrentData
    {
        add { DoRegist_Listener(value, true); }
        remove { DoRemove_Listener(value); }
    }


    public void DoNotify(Args arg)
    {
        _setListener.ToList(_listListener);
        foreach (var pAction in _listListener)
            pAction(arg);

        _LastArg_1 = arg;
    }

    public void DoClear_Listener()
    {
        _setListener.Clear();
    }

    public void DoRegist_Listener(System.Action<Args> OnSubscribe, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnSubscribe == null)
            return;

        if (_setListener.Contains(OnSubscribe) == false)
            _setListener.Add(OnSubscribe);

        if (bInstantNotify_To_ThisListener)
            OnSubscribe(_LastArg_1);
    }

    public void DoRemove_Listener(System.Action<Args> OnNotify)
    {
        if (_setListener.Contains(OnNotify))
            _setListener.Remove(OnNotify);
    }
}