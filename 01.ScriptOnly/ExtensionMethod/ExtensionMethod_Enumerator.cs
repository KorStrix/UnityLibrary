using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionMethod_Enumerator : MonoBehaviour
{
	class SDataWeapon : IDictionaryItem<int>
	{
		public int iWeaponID;
		public string strWeaponName;

		public SDataWeapon(int iWeaponID, string strWeaponName)
		{
			this.iWeaponID = iWeaponID;
			this.strWeaponName = strWeaponName;
		}

		public int IDictionaryItem_GetKey()
		{
			return iWeaponID;
		}
	}


	Dictionary<int, SDataWeapon> _mapWeapon = new Dictionary<int, SDataWeapon>();
	Dictionary<int, string> _mapTest = new Dictionary<int, string>();

	private void Start()
	{
		TestCase_DictionaryItem();
		TestCase_ToList();
		TestCase_ToDictionary();
	}

	void TestCase_DictionaryItem()
	{
		Debug.Log("Start TestCase_Dictionary");
		SDataWeapon[] arrWeapon = new SDataWeapon[2] {
			new SDataWeapon(0, "Gun"), new SDataWeapon(1, "Granade") };

		_mapWeapon.DoAddItem(arrWeapon);
		Debug.Log("0 : " + _mapWeapon[0].strWeaponName);
		Debug.Log("1 : " + _mapWeapon[1].strWeaponName);
	}

	void TestCase_ToList()
	{
		Debug.Log("Start TestCase_ToList");
		_mapTest.Add(1, "One");
		_mapTest.Add(2, "Two");
		_mapTest.Add(3, "Three");

		List<int> listKey = _mapTest.Keys.ToList();
		for (int i = 0; i < listKey.Count; i++)
			Debug.Log("Key : " + i.ToString() + " " + listKey[i]);

		List<string> listValue = _mapTest.Values.ToList();
		for (int i = 0; i < listValue.Count; i++)
			Debug.Log("Value : " + i.ToString() + " " + listValue[i]);
	}

	void TestCase_ToDictionary()
	{
		Debug.Log("Start TestCase_ToDictionary");
		List<SDataWeapon> listDataWeapon = new List<SDataWeapon>();
		listDataWeapon.Add(new SDataWeapon(0, "Gun"));
		listDataWeapon.Add(new SDataWeapon(1, "Gun2"));
		listDataWeapon.Add(new SDataWeapon(2, "Gun3"));

		Dictionary<int, SDataWeapon> mapWeapon = listDataWeapon.ToDictionary<int, SDataWeapon>();
		List<KeyValuePair<int, SDataWeapon>> listDataWeapon_KeyValuePair = mapWeapon.ToList();
		for (int i = 0; i < listDataWeapon_KeyValuePair.Count; i++)
			Debug.Log(string.Format("Key : {0} // Value {1}", listDataWeapon_KeyValuePair[i].Key, listDataWeapon_KeyValuePair[i].Value.strWeaponName));
	}
}
