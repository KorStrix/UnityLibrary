#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-17 오전 10:08:46
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

[System.Flags]
public enum EBuffOverlapOption_Flag
{
    Nothing = 1 << 0,
    Plus_Duration_And_Clamp = 1 << 1,
    Plus_Power = 1 << 2,
    Reset_Duration = 1 << 3,
}

public interface IBuff<T_BUFF_NAME, CLASS_DRIVEN>
    where CLASS_DRIVEN : IBuff<T_BUFF_NAME, CLASS_DRIVEN>
{
    T_BUFF_NAME IBuff_Key { get; }
    CBuffManager<T_BUFF_NAME, CLASS_DRIVEN>.CBuff p_pBuffData { get; set; }

    void IBuff_OnInit(CObjectBase pObjectOwner, ref EBuffOverlapOption_Flag eOverlapOption_Flag_Default_Is_Nothing, ref float fTickUnit_Default_Is_OneSecond);

    void IBuff_OnStartBuff(CBuffManager<T_BUFF_NAME, CLASS_DRIVEN> pBuffManager, float fDebuffDuration, int iDebuffPower, Dictionary<T_BUFF_NAME, CLASS_DRIVEN> mapCurrentWorkingBuff);
    void IBuff_OnBuff(CBuffManager<T_BUFF_NAME, CLASS_DRIVEN> pBuffManager, int iBuffPower, Dictionary<T_BUFF_NAME, CLASS_DRIVEN> mapCurrentWorkingBuff, ref bool bIsFinish_ThisBuff_Default_IsFalse);
    void IBuff_OnFinishBuff();
}

/// <summary>
/// 
/// </summary>
public class CBuffManager<T_BUFF_NAME, CLASS_BUFF> : IUpdateAble
    where CLASS_BUFF : IBuff<T_BUFF_NAME, CLASS_BUFF>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class CBuff : IDictionaryItem<T_BUFF_NAME>
    {
        public CLASS_BUFF p_pBuff { get; private set; }
        public float p_fRemainTime_0_1 { get { return p_fDuration_Remain / p_fDuration_Original; } }
        public float p_fDuration_Original { get; private set; }
        public float p_fDuration_Remain { get; private set; }

        public int p_iCurrentBuffPower;
        protected EBuffOverlapOption_Flag _eDebuffOverlapOption;
        protected float _fTickUnit;

        CBuffManager<T_BUFF_NAME, CLASS_BUFF> _pBuffManager;
        CObjectBase _pObjectOwner;
        int _iTickUnit_Last;

        // ========================================================================== //

        public CBuff(CObjectBase pObjectOwner, CLASS_BUFF pBuffData, CBuffManager<T_BUFF_NAME, CLASS_BUFF> pBuffManager)
        {
            _pObjectOwner = pObjectOwner;
            _eDebuffOverlapOption = EBuffOverlapOption_Flag.Nothing;
            _fTickUnit = 1f;
            _pBuffManager = pBuffManager;

            p_pBuff = pBuffData;
            p_pBuff.p_pBuffData = this;
            p_pBuff.IBuff_OnInit(pObjectOwner, ref _eDebuffOverlapOption, ref _fTickUnit);
        }

        public void DoFinishBuff()
        {
            p_fDuration_Remain = 0f;
        }

        public void DoSet_TickUnit(float fTickUnit)
        {
            _fTickUnit = fTickUnit;
        }

        public void Event_OnFinishBuff()
        {
            p_pBuff.IBuff_OnFinishBuff();
        }

        // ========================================================================== //

        public T_BUFF_NAME IDictionaryItem_GetKey()
        {
            return p_pBuff.IBuff_Key;
        }

        public void OnUpdate(float fDeltaTime, ref bool bIsFinishBuff_Defalt_Is_False, Dictionary<T_BUFF_NAME, CLASS_BUFF> mapCurrentWorkingBuff)
        {
            if (p_fDuration_Remain <= 0f)
            {
                bIsFinishBuff_Defalt_Is_False = true;
                return;
            }
            p_fDuration_Remain -= fDeltaTime;

            int iTickUnitCurrent = (int)(p_fDuration_Remain / _fTickUnit);
            if (iTickUnitCurrent == _iTickUnit_Last)
                return;
            _iTickUnit_Last = iTickUnitCurrent;

            p_pBuff.IBuff_OnBuff(_pBuffManager, p_iCurrentBuffPower, mapCurrentWorkingBuff, ref bIsFinishBuff_Defalt_Is_False);
        }

        public void DoStartBuff(float fBuffDuration, int iBuffPower, Dictionary<T_BUFF_NAME, CLASS_BUFF> mapCurrentWorkingBuff)
        {
            if (p_fDuration_Remain > 0f)
            {
                if (_eDebuffOverlapOption.ContainEnumFlag(EBuffOverlapOption_Flag.Plus_Duration_And_Clamp))
                {
                    p_fDuration_Remain += fBuffDuration;
                    if (p_fDuration_Remain > p_fDuration_Original)
                        p_fDuration_Remain = p_fDuration_Original;
                }

                if (_eDebuffOverlapOption.ContainEnumFlag(EBuffOverlapOption_Flag.Reset_Duration))
                    p_fDuration_Remain = fBuffDuration;

                if (_eDebuffOverlapOption.ContainEnumFlag(EBuffOverlapOption_Flag.Plus_Power))
                    p_iCurrentBuffPower += iBuffPower;
            }
            else
            {
                p_fDuration_Remain = fBuffDuration;
                p_fDuration_Original = fBuffDuration;

                p_iCurrentBuffPower = iBuffPower;

                _iTickUnit_Last = -1;
            }

            p_pBuff.IBuff_OnStartBuff(_pBuffManager, fBuffDuration, iBuffPower, mapCurrentWorkingBuff);
        }
    }

    public class Comparer_RemainTime_Minist : CSingletonNotMonoBase<Comparer_RemainTime_Minist>, IComparer<CLASS_BUFF>
    {
        public int Compare(CLASS_BUFF x, CLASS_BUFF y)
        {
            return x.p_pBuffData.p_fDuration_Remain.CompareTo(y.p_pBuffData.p_fDuration_Remain);
        }
    }

    /* public - Field declaration            */



    /// <summary>
    /// Currnet Buff Working, Changed Buff, Is Add
    /// </summary>
    public CObserverSubject<Dictionary<T_BUFF_NAME, CLASS_BUFF>, T_BUFF_NAME, bool> p_Event_OnChangeBuffList { get; private set; } = new CObserverSubject<Dictionary<T_BUFF_NAME, CLASS_BUFF>, T_BUFF_NAME, bool>();

    public Dictionary<T_BUFF_NAME, CLASS_BUFF> p_mapBuffWorking { get; private set; } = new Dictionary<T_BUFF_NAME, CLASS_BUFF>();
    public List<CLASS_BUFF> p_listBuffWorking { get; private set; } = new List<CLASS_BUFF>();

    /* protected & private - Field declaration         */

    Dictionary<T_BUFF_NAME, CBuff> _mapBuffInstance = new Dictionary<T_BUFF_NAME, CBuff>();
    CObjectBase _pObjectOwner;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAwake(CObjectBase pObjectOwner, params CLASS_BUFF[] arrBuffInstance)
    {
        _pObjectOwner = pObjectOwner;
        pObjectOwner.p_Event_OnActivate.Subscribe += OnActivate;

        _mapBuffInstance.Clear();
        for (int i = 0; i < arrBuffInstance.Length; i++)
            _mapBuffInstance.Add(new CBuff(pObjectOwner, arrBuffInstance[i], this));

        DoEnable();
    }

    public void DoEnable()
    {
        CManagerUpdateObject.instance.DoAddObject(this);
        DoClearAllBuff();
    }

    public void DoClearAllBuff()
    {
        foreach (var pBuff in p_mapBuffWorking.Values)
            pBuff.IBuff_OnFinishBuff();
        p_mapBuffWorking.Clear();
        p_listBuffWorking.Clear();
    }

    public void DoFinishBuff(T_BUFF_NAME eBuffType)
    {
        if (_mapBuffInstance.ContainsKey(eBuffType))
            FinishBuff(eBuffType, _mapBuffInstance[eBuffType]);
    }

    public void DoAddBuff(T_BUFF_NAME strBuffType, float fBuffDuration, int iBuffPower)
    {
        if (_mapBuffInstance.ContainsKey(strBuffType) == false)
        {
            Debug.LogError(_pObjectOwner.name + "Error - eDebuffType : " + strBuffType + " Not Contain", _pObjectOwner);
            return;
        }

        _mapBuffInstance[strBuffType].DoStartBuff(fBuffDuration, iBuffPower, p_mapBuffWorking);
        if (p_mapBuffWorking.ContainsKey(strBuffType) == false)
        {
            CLASS_BUFF pBuff = _mapBuffInstance[strBuffType].p_pBuff;
            p_mapBuffWorking.Add(strBuffType, pBuff);
            p_listBuffWorking.Add(pBuff);
            p_listBuffWorking.Sort(Comparer_RemainTime_Minist.instance);

            p_Event_OnChangeBuffList.DoNotify(p_mapBuffWorking, strBuffType, true);
        }
    }

    public void Event_OnDisable()
    {
        DoClearAllBuff();
        _pObjectOwner.p_Event_OnActivate.DoRemove_Listener(OnActivate);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    public bool IUpdateAble_IsRequireUpdate()
    {
        if (_pObjectOwner == null)
            return false;

        return _pObjectOwner.gameObject.activeSelf;
    }

    public void OnUpdate()
    {
        float fDeltaTime = Time.deltaTime;
        foreach(var pBuff in _mapBuffInstance)
        {
            if (p_mapBuffWorking.ContainsKey(pBuff.Key) == false)
                continue;

            bool bIsFinish = false;
            pBuff.Value.OnUpdate(fDeltaTime, ref bIsFinish, p_mapBuffWorking);
            if(bIsFinish)
            {
                FinishBuff(pBuff.Key, pBuff.Value);
                p_mapBuffWorking.Remove(pBuff.Key);
                p_listBuffWorking.Remove(pBuff.Value.p_pBuff);
                p_listBuffWorking.Sort(Comparer_RemainTime_Minist.instance);
            }
        }
    }

    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    private void FinishBuff(T_BUFF_NAME eBuffKey, CBuff pBuff)
    {
        pBuff.Event_OnFinishBuff();
        p_Event_OnChangeBuffList.DoNotify(p_mapBuffWorking, eBuffKey, false);
    }

    private void OnActivate(CObjectBase pObjectOwner, GameObject pObject, bool bActive)
    {
        if (bActive == false)
            Event_OnDisable();
    }

    #endregion Private
}

#region Test

public class CBuffTester : CObjectBase
{
    public enum ETestBuffType
    {
        AddTickUnitCount,

        Buff_AddHP,
        Debuff_DecreaseHP,
    }

    public abstract class TestBuffBase : IBuff<ETestBuffType, TestBuffBase>
    {
        public CBuffManager<ETestBuffType, TestBuffBase>.CBuff p_pBuffData { get; set; }

        ETestBuffType IBuff<ETestBuffType, TestBuffBase>.IBuff_Key
        {
            get
            {
                return GetType().GetFriendlyName().ConvertEnum<ETestBuffType>();
            }
        }

        abstract public void IBuff_OnBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, int iBuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff, ref bool bIsFinish_ThisBuff_Default_IsFalse);

        virtual public void IBuff_OnInit(CObjectBase pObjectOwner, ref EBuffOverlapOption_Flag eOverlapOption_Flag_Default_Is_Nothing, ref float fTickUnit_Default_Is_OneSecond) { }
        virtual public void IBuff_OnStartBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, float fDebuffDuration, int iDebuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff) { }
        virtual public void IBuff_OnFinishBuff() { }
    }

    public class AddTickUnitCount : TestBuffBase
    {
        static public int ExecuteCount;
        float _fTickUnit;

        public AddTickUnitCount(float fTickUnit)
        {
            _fTickUnit = fTickUnit;
        }

        public override void IBuff_OnInit(CObjectBase pObjectOwner, ref EBuffOverlapOption_Flag eOverlapOption_Flag_Default_Is_Nothing, ref float fTickUnit_Default_Is_OneSecond)
        {
            base.IBuff_OnInit(pObjectOwner, ref eOverlapOption_Flag_Default_Is_Nothing, ref fTickUnit_Default_Is_OneSecond);

            fTickUnit_Default_Is_OneSecond = _fTickUnit;
        }

        public override void IBuff_OnBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, int iBuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff, ref bool bIsFinish_ThisBuff_Default_IsFalse)
        {
            ExecuteCount += iBuffPower;
        }
    }


    [UnityTest]
    public IEnumerator Working_TickUnit([Random(0.05f, 0.2f, 2)]float fTickUnit, [Random(1f, 3f, 2)]float fTotalDuration)
    {
        CBuffTester pObjectTester = new GameObject(nameof(Working_TickUnit)).AddComponent<CBuffTester>();
        CBuffManager<ETestBuffType, TestBuffBase> pBuffManager = new CBuffManager<ETestBuffType, TestBuffBase>();

        AddTickUnitCount pAddUnitCount = new AddTickUnitCount(fTickUnit);
        pBuffManager.DoAwake(pObjectTester, pAddUnitCount);
        Time.timeScale = 3f;

        pBuffManager.DoAddBuff(ETestBuffType.AddTickUnitCount, fTotalDuration, 1);
        AddTickUnitCount.ExecuteCount = 0;
        Assert.AreEqual(AddTickUnitCount.ExecuteCount, 0);

        float fElapseTime = 0f;
        while (fElapseTime < fTotalDuration)
        {
            pBuffManager.OnUpdate();

            fElapseTime += Time.deltaTime;
            yield return null;
        }

        int iCalculatedExecuteCount = Mathf.FloorToInt(fTotalDuration / fTickUnit);
        Debug.Log("ExecuteCount : " + AddTickUnitCount.ExecuteCount + " iCalculatedExecuteCount : " + iCalculatedExecuteCount);

        Assert.IsTrue(AddTickUnitCount.ExecuteCount == iCalculatedExecuteCount || AddTickUnitCount.ExecuteCount == iCalculatedExecuteCount + 1);
        Time.timeScale = 1f;
        yield break;
    }




    // ============================================================================================================================

    public class HPContainer
    {
        public int iHP;
        public int iHP_MAX { get; private set; }
        public bool p_bIsAlive { get { return iHP > 0; } }

        public HPContainer(int iHP)
        {
            iHP_MAX = iHP;
            DoReset();
        }

        public void DoReset()
        {
            iHP = iHP_MAX;
        }

        public void DoDamageHP(int iDamage)
        {
            iHP -= iDamage;
            if (iHP < 0)
                iHP = 0;
        }

        public void DoRecoveryHP(int iRecovery)
        {
            iHP += iRecovery;
            if (iHP > iHP_MAX)
                iHP = iHP_MAX;
        }
    }

    public class Buff_AddHP : TestBuffBase
    {
        HPContainer _pHPContainer;

        public Buff_AddHP(HPContainer pHPContainer)
        {
            _pHPContainer = pHPContainer;
        }

        public override void IBuff_OnInit(CObjectBase pObjectOwner, ref EBuffOverlapOption_Flag eOverlapOption_Flag_Default_Is_Nothing, ref float fTickUnit_Default_Is_OneSecond)
        {
            base.IBuff_OnInit(pObjectOwner, ref eOverlapOption_Flag_Default_Is_Nothing, ref fTickUnit_Default_Is_OneSecond);

            eOverlapOption_Flag_Default_Is_Nothing = EBuffOverlapOption_Flag.Plus_Duration_And_Clamp;
            fTickUnit_Default_Is_OneSecond = const_fTickUnit;
        }

        public override void IBuff_OnBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, int iBuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff, ref bool bIsFinish_ThisBuff_Default_IsFalse)
        {
            if (_pHPContainer != null)
            {
                _pHPContainer.DoRecoveryHP(iBuffPower);
                if(_pHPContainer.iHP == _pHPContainer.iHP_MAX)
                    bIsFinish_ThisBuff_Default_IsFalse = true;
            }
            else
                bIsFinish_ThisBuff_Default_IsFalse = true;
        }
    }

    public class Debuff_DecreaseHP : TestBuffBase
    {
        HPContainer _pHPContainer;

        public Debuff_DecreaseHP(HPContainer pHPContainer)
        {
            _pHPContainer = pHPContainer;
        }

        public override void IBuff_OnInit(CObjectBase pObjectOwner, ref EBuffOverlapOption_Flag eOverlapOption_Flag_Default_Is_Nothing, ref float fTickUnit_Default_Is_OneSecond)
        {
            base.IBuff_OnInit(pObjectOwner, ref eOverlapOption_Flag_Default_Is_Nothing, ref fTickUnit_Default_Is_OneSecond);

            eOverlapOption_Flag_Default_Is_Nothing = EBuffOverlapOption_Flag.Plus_Power;
            fTickUnit_Default_Is_OneSecond = const_fTickUnit;
        }

        public override void IBuff_OnStartBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, float fDebuffDuration, int iDebuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff)
        {
            if (mapCurrentWorkingBuff.ContainsKey(ETestBuffType.Buff_AddHP))
                pBuffManager.DoFinishBuff(ETestBuffType.Buff_AddHP);
        }

        public override void IBuff_OnBuff(CBuffManager<ETestBuffType, TestBuffBase> pBuffManager, int iBuffPower, Dictionary<ETestBuffType, TestBuffBase> mapCurrentWorkingBuff, ref bool bIsFinish_ThisBuff_Default_IsFalse)
        {
            if (_pHPContainer != null && _pHPContainer.p_bIsAlive)
            {
                _pHPContainer.DoDamageHP(iBuffPower);
                if (_pHPContainer.p_bIsAlive == false)
                    bIsFinish_ThisBuff_Default_IsFalse = true;
            }
            else
                bIsFinish_ThisBuff_Default_IsFalse = true;
        }
    }

    const float const_fTickUnit = 0.1f;
    const int const_iInitHP = 10;


    [UnityTest]
    public IEnumerator HP_Buff_And_Debuff()
    {
        CBuffTester pObjectTester = new GameObject(nameof(HP_Buff_And_Debuff)).AddComponent<CBuffTester>();
        CBuffManager<ETestBuffType, TestBuffBase> pBuffManager = new CBuffManager<ETestBuffType, TestBuffBase>();

        HPContainer pHPContainer = new HPContainer(const_iInitHP);
        pBuffManager.DoAwake(pObjectTester, new Buff_AddHP(pHPContainer), new Debuff_DecreaseHP(pHPContainer));
        Time.timeScale = 3f;

        int iDecreasePower = Random.Range(1, 5);
        pBuffManager.DoAddBuff(ETestBuffType.Debuff_DecreaseHP, const_iInitHP, iDecreasePower);

        Assert.AreEqual(pHPContainer.iHP, const_iInitHP);

        float fElapseTime = 0f;
        while (pHPContainer.p_bIsAlive)
        {
            pBuffManager.OnUpdate();

            fElapseTime += Time.deltaTime;
            yield return null;
        }

        Assert.AreEqual(pHPContainer.iHP, 0);
        Assert.IsFalse(pBuffManager.p_mapBuffWorking.ContainsKey(ETestBuffType.Debuff_DecreaseHP));

        fElapseTime = 0f;
        int iRecoveryPower = Random.Range(1, 5);
        pBuffManager.DoAddBuff(ETestBuffType.Buff_AddHP, const_iInitHP, iRecoveryPower);
        while (pHPContainer.iHP != pHPContainer.iHP_MAX)
        {
            pBuffManager.OnUpdate();

            fElapseTime += Time.deltaTime;
            yield return null;
        }

        Assert.AreEqual(pHPContainer.iHP, pHPContainer.iHP_MAX);
        Assert.IsFalse(pBuffManager.p_mapBuffWorking.ContainsKey(ETestBuffType.Buff_AddHP));

        Time.timeScale = 1f;
        yield break;
    }
}

#endregion Test