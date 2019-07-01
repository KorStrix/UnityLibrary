using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class RandomTable_Test
    {
        public enum EItemGrade
        {
            Named,
            Legend,
            Unique,
            Rare,
            Common,
        }

        public class SItemData : IRandomItem
        {
            public EItemGrade eGrade;
            public string strName;
            public int iPercent;

            public SItemData(EItemGrade eGrade, string strName, int iPercent)
            {
                this.eGrade = eGrade; this.strName = strName; this.iPercent = iPercent;
            }

            public int IRandomItem_GetPercent()
            {
                return iPercent;
            }
        }

        [UnityTest]
        public IEnumerator Test_RandomItem_Peek()
        {
            HashSet<SItemData> setGetItem = new HashSet<SItemData>();
            CManagerRandomTable<SItemData> pRandomItemTable = CManagerRandomTable<SItemData>.instance;
            CManagerRandomTable<SItemData>.instance.DoClearRandomItemTable();
            pRandomItemTable.DoSetRandomMode(CManagerRandomTable<SItemData>.ERandomGetMode.Peek);

            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 40));  // Total
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 40));  // 80
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 장갑", 19));      // 95
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Named, "네임드 신발", 1));       // 100

            for (int i = 0; i < 100; i++)
                setGetItem.Add(pRandomItemTable.GetRandomItem());

            UnityEngine.Assertions.Assert.IsTrue(setGetItem.Count > 1); // 종류가 1가지 이상 뽑혔는지

            var pIterNamed = setGetItem.Where(x => x.eGrade == EItemGrade.Named);
            if (pIterNamed != null) // 한개도 없으면 null이다.
                UnityEngine.Assertions.Assert.IsTrue(pIterNamed.Count() <= 10); // 네임드 퍼센트가 1%인데 100번중 10개 이하로 뽑혔는지

            yield break;
        }

        [UnityEngine.TestTools.UnityTest]
        public IEnumerator Test_RandomItem_Delete()
        {
            HashSet<SItemData> setGetItem = new HashSet<SItemData>();
            CManagerRandomTable<SItemData> pRandomItemTable = CManagerRandomTable<SItemData>.instance;
            CManagerRandomTable<SItemData>.instance.DoClearRandomItemTable();
            pRandomItemTable.DoSetRandomMode(CManagerRandomTable<SItemData>.ERandomGetMode.Delete);

            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 40));  // Total
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 40));  // 80
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 장갑", 19));      // 95
            pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Named, "네임드 신발", 1));       // 100

            for (int i = 0; i < 4; i++)
                setGetItem.Add(pRandomItemTable.GetRandomItem());

            UnityEngine.Assertions.Assert.IsTrue(setGetItem.Count >= 4); // 딜리트 모드는 모든 아이템이 뽑혀야 한다.

            Assert.IsNull(pRandomItemTable.GetRandomItem()); // 다 뽑으면 Null이 나온다.

            // 다시 한번 반복
            setGetItem.Clear();
            pRandomItemTable.DoReset_OnDeleteMode();

            for (int i = 0; i < 4; i++)
                setGetItem.Add(pRandomItemTable.GetRandomItem());

            UnityEngine.Assertions.Assert.IsTrue(setGetItem.Count >= 4); // 딜리트 모드는 모든 아이템이 뽑혀야 한다.
            Assert.IsNull(pRandomItemTable.GetRandomItem()); // 다 뽑으면 Null이 나온다.

            yield break;
        }
    }

}
