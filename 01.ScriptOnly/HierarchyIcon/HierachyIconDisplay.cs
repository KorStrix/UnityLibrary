#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-24 오전 10:50:13
 *	개요 : 
 *	https://github.com/mminer/hierarchy-icons
 *	https://gist.github.com/edwardrowe/acda1ee53eb037b31d54d583afc13973
   ============================================ */
#endregion Header

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public struct DrawIconInfo
{
    public Texture2D pTexture2D { get; private set; }
    public int iOrder { get; private set; }

    public DrawIconInfo(Texture2D pTexture2D, int iOrder)
    {
        this.pTexture2D = pTexture2D;
        this.iOrder = iOrder;
    }
}

public interface IDraw_HierarchyIcon
{
    void IDraw_HierarchyIcon_GetIcon(ref List<DrawIconInfo> listDrawIconInfo_DefaultCount_IsZero);
}

[InitializeOnLoad]
public class HierarchyIcon
{
    public struct IconDrawOrderInfo
    {
        public class Comparer : IComparer<IconDrawOrderInfo>
        {
            public int Compare(IconDrawOrderInfo x, IconDrawOrderInfo y)
            {
                return x.iOrder.CompareTo(y.iOrder);
            }
        }

        static Comparer _pComarer;
        static public Comparer p_pComarer
        {
            get
            {
                if (_pComarer == null)
                    _pComarer = new Comparer();

                return _pComarer;
            }
        }

        public Texture2D pTexture { get; private set; }
        public int iOrder { get; private set; }

        public IconDrawOrderInfo(Texture2D pTexture, int iOrder)
        {
            this.pTexture = pTexture; this.iOrder = iOrder;
        }
    }

    // ========================================================================== //

    const float const_fIconOffset_X = 20f;

    static List<DrawIconInfo> listDrawIconInfo = new List<DrawIconInfo>();
    static List<IconDrawOrderInfo> listIconDrawOrderInfo = new List<IconDrawOrderInfo>();

    static HierarchyIcon()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawIconOnWindowItem;
    }

    // ========================================================================== //

    private static void DrawIconOnWindowItem(int instanceID, Rect rect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null)
            return;

        listIconDrawOrderInfo.Clear();

        int iDrawIconCount = 0;
        var pDrawIcon = gameObject.GetComponent<IDraw_HierarchyIcon>();
        if (pDrawIcon != null)
            DrawIcon_InheritDrawer(rect, pDrawIcon, ref iDrawIconCount);

        Texture2D pTexture_Tag;
        int iOrder;
        if (HierachyIconConfig.GetTexture_Per_Tag(gameObject.tag, out pTexture_Tag, out iOrder))
            listIconDrawOrderInfo.Add(new IconDrawOrderInfo(pTexture_Tag, iDrawIconCount++));

        Component[] arrComponent = gameObject.GetComponents<Component>();
        for(int i = 0; i < arrComponent.Length; i++)
        {
            Texture2D pTexture;
            if(HierachyIconConfig.GetTexture_Per_Type(arrComponent[i].GetType(), out pTexture, out iOrder))
                listIconDrawOrderInfo.Add(new IconDrawOrderInfo(pTexture, iDrawIconCount++));
        }

        listIconDrawOrderInfo.Sort(IconDrawOrderInfo.p_pComarer);
        for (int i = 0; i < listIconDrawOrderInfo.Count; i++)
            DrawIcon(rect, listIconDrawOrderInfo[i].pTexture, i);
    }

    private static void DrawIcon_InheritDrawer(Rect rect, IDraw_HierarchyIcon pDrawIcon, ref int iDrawIconCount)
    {
        listDrawIconInfo.Clear();
        pDrawIcon.IDraw_HierarchyIcon_GetIcon(ref listDrawIconInfo);

        for (int i = 0; i < listDrawIconInfo.Count; i++)
        {
            if (listDrawIconInfo[i].pTexture2D == null)
                continue;

            listIconDrawOrderInfo.Add(new IconDrawOrderInfo(listDrawIconInfo[i].pTexture2D, iDrawIconCount++));
        }
    }

    private static void DrawIcon(Rect rect, Texture2D pTexture, int iDrawIconCount)
    {
        float iconWidth = 15f;
        EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
        var padding = new Vector2(5f + (const_fIconOffset_X * iDrawIconCount), 0);
        var iconDrawRect = new Rect(
                                rect.xMax - (iconWidth + padding.x),
                                rect.yMin,
                                rect.width,
                                rect.height);
        var iconGUIContent = new GUIContent(pTexture);
        EditorGUI.LabelField(iconDrawRect, iconGUIContent);
        EditorGUIUtility.SetIconSize(Vector2.zero);
    }
}
#endif