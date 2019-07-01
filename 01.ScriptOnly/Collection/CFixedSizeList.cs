#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-07 오후 5:53:14
 *	기능 : https://stackoverflow.com/questions/5852863/fixed-size-queue-which-automatically-dequeues-old-values-upon-new-enques
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 사이즈가 고정된 List입니다. 사이즈 오버 시 index가 0인것부터 삭제합니다.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CFixedSizeList<T> : List<T>
{
    public int iFixedSize { get; private set; }

    public CFixedSizeList(int iFixedSize)
    {
        this.iFixedSize = iFixedSize;
        this.Capacity = iFixedSize;
    }

    /// <summary>
    /// 아이템을 추가합니다. 이 때 사이즈 오버시 index가 0인것부터 삭제합니다.
    /// </summary>
    /// <param name="pItem"></param>
    public new bool Add(T pItem)
    {
        base.Add(pItem);

        bool bIsOver = Count > this.iFixedSize;
        if (bIsOver)
            base.RemoveAt(0);

        return bIsOver;
    }
}
