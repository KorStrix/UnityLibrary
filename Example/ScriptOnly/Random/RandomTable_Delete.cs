using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTable_Delete : MonoBehaviour {

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
		_pRandomItemTable.DoSetRandomMode(CManagerRandomTable<SItemData>.ERandomGetMode.Delete);

		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 20));  

		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 장갑", 20));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Common, "평범한 신발", 20));  


		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 장갑", 15));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 신발", 15));     
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 장갑", 15));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Rare, "드문 신발", 15));     

		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Unique, "특별한 장갑", 10));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Unique, "특별한 신발", 10));  
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Legend, "전설의 장갑", 4));
		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Legend, "전설의 신발", 4));   

		_pRandomItemTable.DoAddRandomItem(new SItemData(EItemGrade.Named, "유일무이한 신발", 2)); 		 

	}

	void OnEnable()
	{
		Debug.LogWarning( "아이템은 17개 넣었는데, 뽑기 요청은 20개를 하여 for문 한번 돌때마다 3번씩 아이템이 없다 뜰것입니다" );

		for (int i = 0; i < 20; i++)
		{
			SItemData sItem = _pRandomItemTable.GetRandomItem();

			if (sItem == null)
				Debug.Log("아이템이 없다..");
			else
				Debug.Log(string.Format("당첨!! 확률{0}%로 {1}획득!", sItem.iPercent, sItem.strName));
		}


		_pRandomItemTable.DoReset_OnDeleteMode();
		Debug.LogWarning( "리셋!!");

		for (int i = 0; i < 20; i++)
		{
			SItemData sItem = _pRandomItemTable.GetRandomItem();

			if (sItem == null)
				Debug.Log("아이템이 없다..");
			else
				Debug.Log(string.Format("당첨!! 확률{0}%로 {1}획득!", sItem.iPercent, sItem.strName));
		}
	}
}
