using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_To_Json : CObjectBase {

	public enum EPropTest
	{
		갈색나무통, 남색도자기, 노란나무통, 노란상자, 빨간상자, 빨간항아리, 주황도자기, 파란항아리,
	}

	[System.Serializable]
	public class SDataProp : IDictionaryItem<EPropTest>, IRandomItem
	{
		public string str물건이름;
		public int i등장확률;

		public int i최소드랍골드;
		public int i최대드랍골드;

		public EPropTest eProp { get { return str물건이름.ConvertEnum<EPropTest>(); } }

		public EPropTest IDictionaryItem_GetKey()
		{
			return eProp;
		}

		public int IRandomItem_GetPercent()
		{
			return i등장확률;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();

		Dictionary<EPropTest, SDataProp> _mapData_Prop = new Dictionary<EPropTest, SDataProp>();
		SCManagerParserJson pParser = SCManagerParserJson.DoMakeInstance( this, SCManagerParserJson.const_strFolderName, EResourcePath.Resources );
		pParser.DoReadJson_And_InitEnumerator( "인게임오브젝트_테스트", ref _mapData_Prop );

		var listTest = _mapData_Prop.ToList();
		for (int i = 0; i < listTest.Count; i++)
			Debug.Log( string.Format( "Key : {0} Value ( i등장확률 : {1} i최대드랍골드 : {2} )", listTest[i].Key, listTest[i].Value.i등장확률, listTest[i].Value.i최대드랍골드 ) );
	}
}
