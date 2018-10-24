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

#if UNITY_EDITOR
using NUnit.Framework;
#endif

/// <summary>
/// 
/// </summary>
public class CObserverSubject
{
    List<System.Action> _listListener = new List<System.Action>();

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
public class CObserverSubject<T>
{
    List<System.Action<T>> _listListener = new List<System.Action<T>>();
    T _LastArg;

    public void DoNotify(T arg)
    {
        for (int i = 0; i < _listListener.Count; i++)
            _listListener[i](arg);

        _LastArg = arg;
    }

    public void DoClear_Listener()
    {
        _listListener.Clear();
    }

    public void DoRegist_Listener(System.Action<T> OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (_listListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
                OnNotify(_LastArg);

            _listListener.Add(OnNotify);
        }
    }

    public void DoRemove_Listener(System.Action<T> OnNotify)
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
    List<System.Action<T1, T2>> _listListener = new List<System.Action<T1, T2>>();
    T1 _LastArg_1;
    T2 _LastArg_2;

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

    public void DoRegist_Listener(System.Action<T1, T2> OnNotify, bool bInstantNotify_To_ThisListener = false)
    {
        if (_listListener.Contains(OnNotify) == false)
        {
            if (bInstantNotify_To_ThisListener)
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
    List<System.Action<T1, T2, T3>> _listListener = new List<System.Action<T1, T2, T3>>();
    T1 _LastArg_1;
    T2 _LastArg_2;
    T3 _LastArg_3;

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

#region Test
#if UNITY_EDITOR
public class CObserverSubjectTest
{
    int _iTestValue;

    [Test]
    public void CObserverSubject_IsNotOverlap_Observer()
    {
        CObserverSubject pObserverSubject = new CObserverSubject();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_1);
        pObserverSubject.DoRegist_Listener(AddField_1); // Not Regist

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify();
        Assert.IsTrue(_iTestValue == 1);
    }

    [Test]
    public void CObserverSubject_HasMultipleObserver()
    {
        CObserverSubject pObserverSubject = new CObserverSubject();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_1);
        pObserverSubject.DoRegist_Listener(AddField_2);

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify();
        Assert.IsTrue(_iTestValue == 3);
    }

    [Test]
    public void CObserverSubject_HasGenericParams()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        pObserverSubject.DoRegist_Listener(AddField_HasParam); // Not Regist

        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 5);
    }

    [Test]
    public void CObserverSubject_HasMultipleObserver_And_GenericParams()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoRegist_Listener(MinusField_HasParam);
        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoRemove_Listener(AddField_HasParam);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 0);
    }

    [Test]
    public void CObserverSubject_Regist_And_InstantNotify()
    {
        CObserverSubject<int> pObserverSubject = new CObserverSubject<int>();

        _iTestValue = 0;
        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoRegist_Listener(AddField_HasParam, true); // 뒤늦게 요청했을 때, 최신값을 받을 수 있다.
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoRemove_Listener(AddField_HasParam);
        pObserverSubject.DoNotify(5);
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoRegist_Listener(AddField_HasParam);
        Assert.IsTrue(_iTestValue == 5);

        pObserverSubject.DoNotify(-5);
        Assert.IsTrue(_iTestValue == 0);
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
    public void CObserverSubject_DefineDelegateParameter()
    {
        ObserverSubject_DefineParameter pObserverSubject = new ObserverSubject_DefineParameter();
        _iTestValue = 0;

        pObserverSubject.DoRegist_Listener_Define(AddField_HasParam);
        Assert.IsTrue(_iTestValue == 0);

        pObserverSubject.DoNotify_Define(5);
        Assert.IsTrue(_iTestValue == 5);
    }

    private void AddField_1()
    {
        _iTestValue += 1;
    }

    private void AddField_2()
    {
        _iTestValue += 2;
    }


    private void AddField_HasParam(int iAddValue)
    {
        _iTestValue += iAddValue;
    }

    private void MinusField_HasParam(int iMinusValue)
    {
        _iTestValue -= iMinusValue;
    }
}
#endif
#endregion
