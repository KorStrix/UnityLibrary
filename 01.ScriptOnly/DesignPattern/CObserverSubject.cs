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

#if UNITY_EDITOR
using NUnit.Framework;
#endif

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CObserverSubject
{
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
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i]();
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnNotify == null)
            return;

        if (_listListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnNotify();

            _listListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action OnNotify)
    {
        if (_listListener.Contains(OnNotify))
            _listListener.Remove(OnNotify);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject<T1>
{
    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }

    protected List<System.Action<T1>> _listListener = new List<System.Action<T1>>();

    public event System.Action<T1> Subscribe
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

    public event System.Action<T1> Subscribe_And_Listen_CurrentData
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

    public void DoNotify(T1 arg)
    {
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i](arg);

        _LastArg_1 = arg;
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action<T1> OnSubscribe, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnSubscribe == null)
            return;

        if (_listListener.Contains(OnSubscribe) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnSubscribe(_LastArg_1);

            _listListener.Add(OnSubscribe);
        }
    }

    public void DoRemove_Listener(System.Action<T1> OnNotify)
    {
        if (_listListener.Contains(OnNotify))
            _listListener.Remove(OnNotify);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject<T1, T2>
{
    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }
    [NonSerialized]
    private T2 _LastArg_2; public T2 GetLastArg_2() { return _LastArg_2; }

    protected List<System.Action<T1, T2>> _listListener = new List<System.Action<T1, T2>>();

    public event System.Action<T1, T2> Subscribe
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

    public event System.Action<T1, T2> Subscribe_And_Listen_CurrentData
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

    public void DoNotify(T1 arg1, T2 arg2)
    {
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i](arg1, arg2);

        _LastArg_1 = arg1;
        _LastArg_2 = arg2;
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action<T1, T2> OnNotify, bool bListen_CurrentData = false)
    {
        if (OnNotify == null)
            return;

        if (_listListener.Contains(OnNotify) == false)
        {
            if (bListen_CurrentData)
                OnNotify(_LastArg_1, _LastArg_2);

            _listListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action<T1, T2> OnNotify)
    {
        if (_listListener.Contains(OnNotify))
            _listListener.Remove(OnNotify);
    }
}

/// <summary>
/// 
/// </summary>
public class CObserverSubject<T1, T2, T3>
{
    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }
    [NonSerialized]
    private T2 _LastArg_2; public T2 GetLastArg_2() { return _LastArg_2; }
    [NonSerialized]
    private T3 _LastArg_3; public T3 GetLastArg_3() { return _LastArg_3; }

    protected List<System.Action<T1, T2, T3>> _listListener = new List<System.Action<T1, T2, T3>>();

    public event System.Action<T1, T2, T3> Subscribe
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

    public event System.Action<T1, T2, T3> Subscribe_And_Listen_CurrentData
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

    public void DoNotify(T1 arg1, T2 arg2, T3 arg3)
    {
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i](arg1, arg2, arg3);

        _LastArg_1 = arg1;
        _LastArg_2 = arg2;
        _LastArg_3 = arg3;
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action<T1, T2, T3> OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnNotify == null)
            return;

        if (_listListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnNotify(_LastArg_1, _LastArg_2, _LastArg_3);

            _listListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action<T1, T2, T3> OnNotify)
    {
        if (_listListener.Contains(OnNotify))
            _listListener.Remove(OnNotify);
    }
}


/// <summary>
/// 
/// </summary>
public class CObserverSubject<T1, T2, T3, T4>
{
    [NonSerialized]
    private T1 _LastArg_1; public T1 GetLastArg_1() { return _LastArg_1; }
    [NonSerialized]
    private T2 _LastArg_2; public T2 GetLastArg_2() { return _LastArg_2; }
    [NonSerialized]
    private T3 _LastArg_3; public T3 GetLastArg_3() { return _LastArg_3; }
    [NonSerialized]
    private T4 _LastArg_4; public T4 GetLastArg_4() { return _LastArg_4; }

    protected List<System.Action<T1, T2, T3, T4>> _listListener = new List<System.Action<T1, T2, T3, T4>>();

    public event System.Action<T1, T2, T3, T4> Subscribe
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

    public event System.Action<T1, T2, T3, T4> Subscribe_And_Listen_CurrentData
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

    public void DoNotify(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i](arg1, arg2, arg3, arg4);

        _LastArg_1 = arg1;
        _LastArg_2 = arg2;
        _LastArg_3 = arg3;
        _LastArg_4 = arg4;
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action<T1, T2, T3, T4> OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (OnNotify == null)
            return;

        if (_listListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnNotify(_LastArg_1, _LastArg_2, _LastArg_3, _LastArg_4);

            _listListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action<T1, T2, T3, T4> OnNotify)
    {
        if (_listListener.Contains(OnNotify))
            _listListener.Remove(OnNotify);
    }
}


#region Test
#if UNITY_EDITOR
public class CObserverSubjectTest
{
    int _TestValue;

    [Test]
    public void IsNotOverlap_Observer()
    {
        CObserverSubject pObserverSubject = new CObserverSubject();
        _TestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_1);
        pObserverSubject.DoRegist_Listener(AddField_1); // Not Regist

        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify();
        Assert.IsTrue(_TestValue == 1);
    }

    [Test]
    public void HasMultipleObserver()
    {
        CObserverSubject pObserverSubject = new CObserverSubject();
        _TestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_1);
        pObserverSubject.Subscribe += AddField_2;

        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify();
        Assert.IsTrue(_TestValue == 3);
    }

    [Test]
    public void HasGenericParams()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();
        _TestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        pObserverSubject.DoRegist_Listener(AddField_HasParam); // Not Regist

        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 5);
    }

    [Test]
    public void HasMultipleObserver_And_GenericParams()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();
        _TestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 5);

        pObserverSubject.DoRegist_Listener(MinusField_HasParam);
        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 5);

        pObserverSubject.DoRemove_Listener(AddField_HasParam);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 0);
    }

    [Test]
    public void Regist_And_InstantNotify()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();

        _TestValue = 0;
        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoRegist_Listener(AddField_HasParam, true); // 뒤늦게 요청했을 때, 최신값을 받을 수 있다.
        Assert.IsTrue(_TestValue == 5);

        pObserverSubject.DoRemove_Listener(AddField_HasParam);
        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_TestValue == 5);

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        Assert.IsTrue(_TestValue == 5);

        pObserverSubject.DoNotify(-5);
        Assert.IsTrue(_TestValue == 0);
    }

    /// <summary>
    /// 파라미터 정의를 위한 상속 클래스
    /// </summary>
    public class ObserverSubject_DefineParameter : CObserverSubject<int>
    {
        public delegate void OnTest(int iValueDefine);

        public void DoRegist_Listener_Define(OnTest OnTest)
        {
            DoRegist_Listener(new System.Action<int>(OnTest));
        }

        public void DoNotify_Define(int iValueDefine)
        {
            DoNotify(iValueDefine);
        }
    }

    [Test]
    public void DefineDelegateParameter()
    {
        ObserverSubject_DefineParameter pObserverSubject = new ObserverSubject_DefineParameter();
        _TestValue = 0;

        pObserverSubject.DoRegist_Listener_Define(AddField_HasParam);
        Assert.IsTrue(_TestValue == 0);

        pObserverSubject.DoNotify_Define(5);
        Assert.IsTrue(_TestValue == 5);

    }

    private void AddField_1()
    {
        _TestValue += 1;
    }

    private void AddField_2()
    {
        _TestValue += 2;
    }


    private void AddField_HasParam(int iAddValue)
    {
        _TestValue += iAddValue;
    }

    private void MinusField_HasParam(int iMinusValue)
    {
        _TestValue -= iMinusValue;
    }
}
#endif
#endregion
