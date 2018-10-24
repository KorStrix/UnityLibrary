using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;
using NUnit.Framework;

/* ============================================ 
   Editor      : Strix
   Description : 열거자 관련 확장 메서드용 클래스
   Version	   :
   ============================================ */

public interface IList_DataContain<TData>
{
    void IList_DataContain(TData pData);
}

public interface IDictionaryItem<TKeyType>
{
	TKeyType IDictionaryItem_GetKey();
}

public interface IDictionaryItem_ContainData<TKeyType, TDataType> : IDictionaryItem<TKeyType>
{
	TKeyType IDictionaryItem_ContainData_GetKey();
	void IDictionaryItem_ContainData_SetData( TDataType sData );
}

public interface IListItem_HasField<TFieldType>
{
	TFieldType IListItem_HasField_GetField();
}

public static class SCEnumeratorHelper
{
    static System.Text.StringBuilder pStringBuilder = new System.Text.StringBuilder();

    #region Enumerable

    /// <summary>
    /// 디버그용 출력
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pIter"></param>
    /// <returns></returns>
    static public string ToString_DataList<T>(this IEnumerable<T> pIter)
        where T : MonoBehaviour
    {
        pStringBuilder.Length = 0;
        foreach (T t in pIter)
        {
            pStringBuilder.Append(t.name);
            pStringBuilder.Append(", ");
        }

        return pStringBuilder.ToString();
    }

    public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
    {
        int iCapacity = 0;
        IEnumerator<TSource> pIter = source.GetEnumerator();
        while (pIter.MoveNext())
        {
            iCapacity++;
        }

        pIter = source.GetEnumerator();
        TSource[] arrReturn = new TSource[iCapacity];
        int iIndex = 0;
        while (pIter.MoveNext())
        {
            arrReturn[iIndex++] = pIter.Current;
        }

        return arrReturn;
    }


    public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
    {
        return new List<TSource>(source);
    }

    public static void ToList<TSource>(this IEnumerable<TSource> source, List<TSource> listOut)
    {
        listOut.AddRange(source);
    }

    public static Dictionary<TKey, TSource> ToDictionary<TKey, TSource>(this IEnumerable<TSource> source)
        where TSource : IDictionaryItem<TKey>
    {
        Dictionary<TKey, TSource> mapReturn = new Dictionary<TKey, TSource>();
        mapReturn.DoAddItem(source);

        return mapReturn;
    }

    static List<int> listRandomIndexTemp = new List<int>();

    /// 테스트 코드 링크
    /// <see cref="SCEnumeratorHelper_테스트.GetRandom_테스트"/>
    /// <summary>
    /// 예외를 제외한 랜덤을 리턴합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="arrFilter"></param>
    /// <returns></returns>
    static public T GetRandom_Filter<T>(this IEnumerable<T> source, params T[] arrFilter)
    {
        listRandomIndexTemp.Clear();
        IEnumerator<T> pIter = source.GetEnumerator();
        int iLoopIndex = 0;
        while (pIter.MoveNext())
        {
            bool bIsException = false;
            for (int i = 0; i < arrFilter.Length; i++)
            {
                if (pIter.Current.Equals(arrFilter[i]))
                {
                    bIsException = true;
                    break;
                }
            }

            if (bIsException == false)
                listRandomIndexTemp.Add(iLoopIndex);

            iLoopIndex++;
        }

        if (iLoopIndex == 0)
            return default(T);

        int iRandomIndex = listRandomIndexTemp.GetRandom() + 1;
        pIter = source.GetEnumerator();
        T pDataReturn = default(T);

        while (pIter.MoveNext() && iRandomIndex-- > 0)
        {
            pDataReturn = pIter.Current;
        }

        return pDataReturn;
    }

    #endregion

    static public void DoExtractItem<T, TFieldType>( this List<T> list, List<TFieldType> listOut )
		where T : IListItem_HasField<TFieldType>
	{
		listOut.Clear();
		for (int i = 0; i < list.Count; i++)
			listOut.Add( list[i].IListItem_HasField_GetField() );
	}

	static public void DoExtractItemList<T, TFieldType>( this List<T> list, List<TFieldType> listOut )
		where T : IListItem_HasField<List<TFieldType>>
	{
		listOut.Clear();
		for (int i = 0; i < list.Count; i++)
			listOut.AddRange( list[i].IListItem_HasField_GetField() );
	}

	static public bool Contains_PrintOnError<T>( this List<T> list, T CheckData, bool bContains = false )
	{
		bool bIsContain = list.Contains( CheckData );
		if (bIsContain == bContains)
			Debug.Log( "List.Contains 에 실패했습니다 - " + CheckData, null );

		return bIsContain;
	}

	static public bool TryGetValue<T>( this List<T> list, int iIndex, out T outData )
		where T : new()
	{
		bool bIsContain = iIndex < list.Count;
		if (bIsContain == false)
		{
			outData = new T();
			Debug.LogWarning( "List.TryGetValue 에 실패했습니다 - Index :  " + iIndex, null );
		}
		else
			outData = list[iIndex];

		return bIsContain;
	}

	static public T GetRandom<T>( this List<T> list )
	{
		if (list.Count == 0)
			return default( T );

		int iRandomIndex = Random.Range( 0, list.Count );
		return list[iRandomIndex];
	}

    static public T GetRandom<T>(this T[] arr)
    {
        if (arr.Length == 0)
            return default(T);

        int iRandomIndex = Random.Range(0, arr.Length);
        return arr[iRandomIndex];
    }

    static public List<DataContainer> Create_ContainerList<DataContainer, Data>(this IEnumerable<Data> source )
        where DataContainer : IList_DataContain<Data>, new()
    {
        List<DataContainer> listContainer = new List<DataContainer>();
        IEnumerator<Data> pEnumerator = source.GetEnumerator();
        while(pEnumerator.MoveNext())
        {
            DataContainer pNewContainer = new DataContainer();
            pNewContainer.IList_DataContain(pEnumerator.Current);
        }

        return listContainer;
    }

    static public string ToStringList<T>(this List<T> list)
    {
        string strString = "Count [";
        strString += list.Count + "] - ";
        for (int i = 0; i < list.Count; i++)
        {
            strString += list[i];
            if (i != list.Count - 1)
                strString += ",";
        }

        return strString;
    }

    // =====================================================================================

    static public void AddRange_First<T>( this LinkedList<T> source, IEnumerable<T> arrItem )
	{
		IEnumerator<T> pEnumerator = arrItem.GetEnumerator();
		while (pEnumerator.MoveNext())
		{
			source.AddFirst( pEnumerator.Current );
		}
	}

	static public void AddRange_Last<T>( this LinkedList<T> source, IEnumerable<T> arrItem )
	{
		IEnumerator<T> pEnumerator = arrItem.GetEnumerator();
		while (pEnumerator.MoveNext())
		{
			source.AddLast( pEnumerator.Current );
		}
	}

    #region Dictionary

    static public bool ContainsKey_PrintOnError<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey CheckKey, Object pObjectForDebug = null)
    {
        bool bIsContain = CheckKey != null;
        if (bIsContain)
            bIsContain = map.ContainsKey(CheckKey);

        if (bIsContain == false)
        {
            string strKeyName = typeof(TKey).Name;
            string strValueName = typeof(TValue).Name;
            Debug.LogWarning(string.Format("Dictionary<{0}, {1}>.ContainsKey 에 실패했습니다 - ({2})", strKeyName, strValueName, CheckKey), pObjectForDebug);
        }

        return bIsContain;
    }

    static public void DoAddItem<TKey, TSource>(this Dictionary<TKey, TSource> mapDataTable, IEnumerable<TSource> source, bool bIsClear = true)
    where TSource : IDictionaryItem<TKey>
    {
        if (bIsClear)
            mapDataTable.Clear();

        IEnumerator<TSource> pIter = source.GetEnumerator();
        while (pIter.MoveNext())
        {
            TSource pCurrent = pIter.Current;
            TKey hDataID = pCurrent.IDictionaryItem_GetKey();
            if (mapDataTable.ContainsKey(hDataID))
                Debug.LogWarning("에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof(TSource) + " : " + hDataID);
            else
                mapDataTable.Add(hDataID, pCurrent);
        }
    }
    
    static public bool Remove<TKey, TSource>(this Dictionary<TKey, TSource> mapDataTable, TSource pValue)
    where TSource : IDictionaryItem<TKey>
    {
        return mapDataTable.Remove(pValue.IDictionaryItem_GetKey());
    }

    static public void Add<TKey, TSource>(this Dictionary<TKey, TSource> mapDataTable, TSource pAddSource)
        where TSource : IDictionaryItem<TKey>
    {
        TKey hDataID = pAddSource.IDictionaryItem_GetKey();
        if (mapDataTable.ContainsKey(hDataID))
        {
            mapDataTable.Remove(hDataID);
        }

        mapDataTable.Add(hDataID, pAddSource);
    }

    static public void Add<TKey, TSource>(this Dictionary<TKey, List<TSource>> mapDataTable, TSource pAddSource)
        where TSource : IDictionaryItem<TKey>
    {
        TKey hDataID = pAddSource.IDictionaryItem_GetKey();
        if (mapDataTable.ContainsKey(hDataID) == false)
            mapDataTable.Add(hDataID, new List<TSource>());

        mapDataTable[hDataID].Add(pAddSource);
    }


    static public void DoAddItem<TSource, TKey>(this Dictionary<TKey, TSource> mapDataTable, TSource[] arrDataTable, UnityEngine.Object pObjectForDebug = null)
        where TSource : IDictionaryItem<TKey>
    {
        mapDataTable.Clear();
        if (arrDataTable == null) return;

        for (int i = 0; i < arrDataTable.Length; i++)
        {
            TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();
            if (mapDataTable.ContainsKey(hDataID))
                Debug.LogWarning("에러, 데이터 테이블에 공통된 키값을 가진 데이터가 존재!!" + typeof(TSource) + " : " + hDataID, pObjectForDebug);
            else
                mapDataTable.Add(hDataID, arrDataTable[i]);
        }
    }

    static public void DoAddItem<TSource, TKey>(this Dictionary<TKey, List<TSource>> mapDataTable, IEnumerable<TSource> source, bool bIsClear = true)
        where TSource : IDictionaryItem<TKey>
    {
        if (bIsClear)
            mapDataTable.Clear();

        IEnumerator<TSource> pIter = source.GetEnumerator();
        while (pIter.MoveNext())
        {
            TSource pCurrent = pIter.Current;
            TKey hDataID = pCurrent.IDictionaryItem_GetKey();
            if (mapDataTable.ContainsKey(hDataID) == false)
                mapDataTable.Add(hDataID, new List<TSource>());

            mapDataTable[hDataID].Add(pCurrent);
        }
    }

    static public void DoAddItem<TSource, TKey>(this Dictionary<TKey, List<TSource>> mapDataTable, TSource[] arrDataTable)
        where TSource : IDictionaryItem<TKey>
    {
        mapDataTable.Clear();
        if (arrDataTable == null) return;

        for (int i = 0; i < arrDataTable.Length; i++)
        {
            TKey hDataID = arrDataTable[i].IDictionaryItem_GetKey();

            if (mapDataTable.ContainsKey(hDataID) == false)
                mapDataTable.Add(hDataID, new List<TSource>());

            mapDataTable[hDataID].Add(arrDataTable[i]);
        }
    }

    static public void DoLinkOwnerToData<TOwner, TKeyData, TData>(this Dictionary<TKeyData, TData> mapData, List<TOwner> listDataOwner)
        where TOwner : class, IDictionaryItem_ContainData<TKeyData, TData>
        where TData : IDictionaryItem<TKeyData>
    {
        for (int i = 0; i < listDataOwner.Count; i++)
        {
            TOwner pOwner = listDataOwner[i];
            TKeyData pKey = pOwner.IDictionaryItem_GetKey();
            if (mapData.ContainsKey(pKey))
                pOwner.IDictionaryItem_ContainData_SetData(mapData[pKey]);
            else
                Debug.LogWarning(string.Format("{0}이 {1}에 없습니다)", pKey.ToString(), mapData.ToString()));
        }
    }

    static public bool ContainsKey<TKeyData, TData>(this Dictionary<TKeyData, TData> mapData, IDictionaryItem<TKeyData> pItem)
    {
        return mapData.ContainsKey(pItem.IDictionaryItem_GetKey());
    }

    #endregion
}

#region Test

public class SCEnumeratorHelper_테스트
{
    [Test]
    public void GetRandom_테스트()
    {
        List<int> arrTest = new List<int>() { 1, 3, 5 };
        int iRandomValue = arrTest.GetRandom_Filter(1, 2, 3, 4);
        Assert.AreEqual(iRandomValue, 5);
    }
}

#endregion