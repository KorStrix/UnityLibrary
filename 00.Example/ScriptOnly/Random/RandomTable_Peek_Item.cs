using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTable_Peek_Item : MonoBehaviour
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


	private CManagerRandomTable<SItemData> _pRandomItemTable;

	void Awake()
	{
		_pRandomItemTable = CManagerRandomTable<SItemData>.instance;
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 20));	// Total
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 20));	// 40
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 장갑", 15));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 신발", 15));		// 70
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Unique, "특별한 장갑", 10)); 
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Unique, "특별한 신발", 10));	// 90
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Legend, "전설의 장갑", 4));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Legend, "전설의 신발", 4));	// 98
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Named, "유일무이한 신발", 2));  // 100

	}

	void OnEnable()
	{
		for(int i = 0; i < 100; i++)
		{
			SItemData sItem = _pRandomItemTable.GetRandomItem();
			Debug.Log(string.Format("당첨!! 확률{0}%로 {1}당첨!", sItem.iPercent, sItem.strName));
		}
	}
}
