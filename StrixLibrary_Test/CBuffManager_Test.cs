using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CBuffManager_Test : CObjectBase
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
            CBuffManager_Test pObjectTester = new GameObject(nameof(Working_TickUnit)).AddComponent<CBuffManager_Test>();
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
                pBuffManager.OnUpdate(1f);

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
                    if (_pHPContainer.iHP == _pHPContainer.iHP_MAX)
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
            CBuffManager_Test pObjectTester = new GameObject(nameof(HP_Buff_And_Debuff)).AddComponent<CBuffManager_Test>();
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
                pBuffManager.OnUpdate(1f);

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
                pBuffManager.OnUpdate(1f);

                fElapseTime += Time.deltaTime;
                yield return null;
            }

            Assert.AreEqual(pHPContainer.iHP, pHPContainer.iHP_MAX);
            Assert.IsFalse(pBuffManager.p_mapBuffWorking.ContainsKey(ETestBuffType.Buff_AddHP));

            Time.timeScale = 1f;
            yield break;
        }
    }

}
