using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CResourceBase_Test
    {
        public class Gold : CResourceBase<Gold>
        {
            public const int const_Default = 0;
            public const int const_Max = 1000;
            public const int const_Min = -100;

            protected override void OnAwake(ref int iMinValue, ref int iMaxValue, ref int iDefaultValue)
            {
                iMinValue = const_Min;
                iMaxValue = const_Max; 
                iDefaultValue = const_Default;
            }
        }

        [Test]
        public void Resource_Add_And_Use()
        {
            Gold pGold = new Gold();
            pGold.IResourceBase_OnAwake();
            pGold.IResourceBase_OnEnable();
            Assert.AreEqual(pGold.iCurrentAmount, Gold.const_Default);

            pGold.DoAdd(100);
            Assert.AreEqual(pGold.iCurrentAmount, Gold.const_Default + 100);

            int iCurrentAmount = pGold.iCurrentAmount;
            iCurrentAmount = iCurrentAmount + 100      - 50      + 100      - 50;
            pGold.                      DoAdd(100).DoUse(50).DoAdd(100).DoUse(50);
            Assert.AreEqual(pGold.iCurrentAmount, iCurrentAmount);

            pGold.DoAdd(Gold.const_Max);
            Assert.AreEqual(pGold.iCurrentAmount, Gold.const_Max);

            pGold.DoUse(Gold.const_Max * 2);
            Assert.AreEqual(pGold.iCurrentAmount, Gold.const_Min);
        }


        public class Health : CResourceBase<Health>
        {
            public const int const_Default = 1;
            public const int const_Max = 1000;
            public const int const_Min = 0;

            protected override void OnAwake(ref int iMinValue, ref int iMaxValue, ref int iDefaultValue)
            {
                iMinValue = const_Min;
                iMaxValue = const_Max;
                iDefaultValue = const_Default;
            }
        }

        [Test]
        public void Resource_Add_And_Use_With_Buff()
        {
            Health pHealth = new Health();
            pHealth.p_Event_OnCalculate_Add.Subscribe += OnAdd;

            pHealth.IResourceBase_OnAwake();
            pHealth.IResourceBase_OnEnable();
            Assert.AreEqual(pHealth.iCurrentAmount, Health.const_Default);

            int iValue = Mathf.FloorToInt((pHealth.iCurrentAmount + 100) * 1.1f);
            pHealth.DoAdd(100);
            Assert.AreEqual(pHealth.iCurrentAmount, iValue);

            Assert.IsTrue(pHealth.Check_IsEnough(pHealth.iCurrentAmount));
            Assert.IsFalse(pHealth.Check_IsEnough(pHealth.iCurrentAmount + 100));
        }

        private void OnAdd(CResourceBase<Health>.Buff_Arg arg1, int pValue_Origin, ref int pValue_Current)
        {
            pValue_Current = Mathf.FloorToInt(pValue_Origin * 1.1f);
        }
    }
}