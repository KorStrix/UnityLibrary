#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-06-07 오전 10:53:38
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IHasResource<ResourceClass>
{
    ResourceClass IHasResource_GetResource();
}

public interface IResourceBase
{
    void IResourceBase_OnAwake();
    void IResourceBase_OnEnable();
}

public class CResourceBase<Class_DERIVED> : IResourceBase
    where Class_DERIVED : CResourceBase<Class_DERIVED>
{
    public struct Buff_Arg
    {
        public int iCurrentAmount;

        public Buff_Arg(int iCurrentAmount)
        {
            this.iCurrentAmount = iCurrentAmount;
        }
    }

    public struct ResourceAdd_Arg
    {
        public Class_DERIVED pDerived;
        public int iAddAmount;
        public int iCurrentAmount;

        public ResourceAdd_Arg(Class_DERIVED pDerived, int iAddAmount, int iCurrentAmount)
        {
            this.pDerived = pDerived;
            this.iAddAmount = iAddAmount;
            this.iCurrentAmount = iCurrentAmount;
        }
}

    public ObservableCollection_ChainData<Buff_Arg, int> p_Event_OnCalculate_Add { get; private set; } = new ObservableCollection_ChainData<Buff_Arg, int>();
    public ObservableCollection_ChainData<Buff_Arg, int> p_Event_OnCalculate_Use { get; private set; } = new ObservableCollection_ChainData<Buff_Arg, int>();

    public ObservableCollection<ResourceAdd_Arg> p_Event_OnChangeAmount { get; private set; } = new ObservableCollection<ResourceAdd_Arg>();

    public int iCurrentAmount { get; private set; }

    Class_DERIVED _pDerived_This;
    bool _bIsInit = false;

    int _iDefaultValue;
    int _iMinValue;
    int _iMaxValue;

    public void IResourceBase_OnAwake()
    {
        _bIsInit = true;
        _pDerived_This = this as Class_DERIVED;

        _iMinValue = 0;
        _iMaxValue = int.MaxValue;
        OnAwake(ref _iMinValue, ref _iMaxValue, ref _iDefaultValue);
    }

    public virtual void IResourceBase_OnEnable()
    {
        iCurrentAmount = _iDefaultValue;
    }

    public Class_DERIVED DoAdd(int iAddAmount)
    {
        if (_bIsInit == false)
            IResourceBase_OnAwake();

        int iAddAmount_Calculated = p_Event_OnCalculate_Add.DoNotify(new Buff_Arg(iCurrentAmount), iAddAmount);
        iCurrentAmount = Mathf.Clamp(iCurrentAmount + iAddAmount_Calculated, _iMinValue, _iMaxValue);
        p_Event_OnChangeAmount.DoNotify(new ResourceAdd_Arg(_pDerived_This, iAddAmount_Calculated, iCurrentAmount));

        return _pDerived_This;
    }

    public Class_DERIVED DoUse(int iUseAmount)
    {
        if (_bIsInit == false)
            IResourceBase_OnAwake();

        int iUseAmount_Calculated = p_Event_OnCalculate_Use.DoNotify(new Buff_Arg(iCurrentAmount), iUseAmount);
        iCurrentAmount = Mathf.Clamp(iCurrentAmount - iUseAmount_Calculated, _iMinValue, _iMaxValue);
        p_Event_OnChangeAmount.DoNotify(new ResourceAdd_Arg(_pDerived_This, -iUseAmount_Calculated, iCurrentAmount));

        return _pDerived_This;
    }

    public bool Check_IsEnough(int iRequireAmount)
    {
        if (_bIsInit == false)
            IResourceBase_OnAwake();

        int iUseAmount_Calculated = p_Event_OnCalculate_Use.DoNotify(new Buff_Arg(iCurrentAmount), iRequireAmount);
        return iCurrentAmount >= iUseAmount_Calculated;
    }

    protected virtual void OnAwake(ref int iMinValue, ref int iMaxValue, ref int iDefaultValue)
    {
        iMinValue = 0;
        iMaxValue = int.MaxValue;
        iDefaultValue = 0;
    }
}
