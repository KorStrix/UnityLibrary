using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTable_Peek_Monster : MonoBehaviour
{
	public enum EMonsterRank
	{
		S = 500,
		A = 300,
		B = 200,
		C = 100,
		D = 50,
	}

	public class SMonster : IRandomItem
	{
		public EMonsterRank eGrade;
		public string strName;

		public SMonster(EMonsterRank eGrade, string strName)
		{
			this.eGrade = eGrade; this.strName = strName;
		}

		public int IRandomItem_GetPercent()
		{
			return (int)eGrade;
		}
	}
	

	void Awake()
	{
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.D, "고블린"));	
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.D, "코볼트"));	

		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.C, "오크"));
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.C, "트롤"));
		
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.B, "오우거")); 
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.B, "와이번"));

		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.A, "레드 드래곤"));
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.A, "블루 드래곤"));
		CManagerRandomTable<SMonster>.instance.DoAddRandomItem(new SMonster(EMonsterRank.S, "전설의 드래곤")); 	 
	}

	void OnEnable()
	{
		CManagerRandomTable<SMonster> pRandomTable = CManagerRandomTable<SMonster>.instance;
		Debug.Log("초보자 몬스터 존 시작");
		for (int i = 0; i < 5; i++)
		{
			SMonster sMonster = pRandomTable.GetRandomItem((int)EMonsterRank.C);
			Debug.Log(string.Format("{0}랭크 {1}등장!", sMonster.eGrade, sMonster.strName));

		}

		Debug.Log("좀 쎈 몬스터 존 시작");
		for (int i = 0; i < 5; i++)
		{
			SMonster sMonster = pRandomTable.GetRandomItem((int)EMonsterRank.B);
			Debug.Log(string.Format("{0}랭크 {1}등장!", sMonster.eGrade, sMonster.strName));
		}


		Debug.Log("드래곤 둥지 시작!!");
		for (int i = 0; i < 5; i++)
		{
			SMonster sMonster = pRandomTable.GetRandomItem((int)EMonsterRank.S);
			Debug.Log(string.Format("{0}랭크 {1}등장!", sMonster.eGrade, sMonster.strName));
		}
	}
}
