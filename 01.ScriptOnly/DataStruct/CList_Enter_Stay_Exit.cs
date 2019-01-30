#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-21 오후 12:14:04
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif

#endif

[System.Serializable]
public class CList_Enter_Stay_Exit<T>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class HashSetComparer : EqualityComparer<T>
    {
        public override bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    /* public - Field declaration            */

    public List<T> p_list_Enter { get { return _listEnter; } }
    public List<T> p_list_Stay { get { return _listStay; } }
    public List<T> p_list_Exit { get { return _listExit; } }

    /* protected & private - Field declaration         */

    private HashSet<T> _setStay = new HashSet<T>(new HashSetComparer());

    [SerializeField]
    private List<T> _listEnter;
    [SerializeField]
    private List<T> _listStay;
    [SerializeField]
    private List<T> _listExit;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public CList_Enter_Stay_Exit()
    {
        _listEnter = new List<T>();
        _listStay = new List<T>();
        _listExit = new List<T>();

        _setStay = new HashSet<T>(new HashSetComparer());
    }

    public void ClearAll()
    {
        _listEnter.Clear();
        _listExit.Clear();
        _listStay.Clear();

        _setStay.Clear();
    }

    public void AddEnter(IEnumerable<T> listEnter)
    {
        _listEnter.Clear();
        _listExit.Clear();
        _listEnter.AddRange(listEnter);

        foreach (var pValue in _setStay)
        {
            if (_listEnter.Contains(pValue))
                _listEnter.Remove(pValue);
            else
                _listExit.Add(pValue);
        }

        foreach(var pValue in _listExit)
            _setStay.Remove(pValue);

        // _setStay.UnionWith(_listEnter);
        foreach (var pValue in _listEnter)
            _setStay.Add(pValue);

        _setStay.ToList(_listStay);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

public class CList_Enter_Stay_Exit
{
    [Test]
    public void Working_Test()
    {
        SCManagerProfiler.DoStartTestCase(nameof(Working_Test));

        CList_Enter_Stay_Exit<int> list = new CList_Enter_Stay_Exit<int>();

        int[] arrEmpty = new int[] { };
        int[] arrValue = new int[] { 0, 1, 3, 5};
        list.AddEnter(arrValue);

        Assert.AreEqual(list.p_list_Enter.ToArray(), arrValue);
        Assert.AreEqual(list.p_list_Stay.ToArray(), arrValue);
        Assert.AreEqual(list.p_list_Exit.ToArray(), arrEmpty);

        arrValue = new int[] { 0 };
        int[] arrExit = new int[] { 1, 3, 5 };
        list.AddEnter(arrValue);

        Assert.AreEqual(list.p_list_Enter.ToArray(), arrEmpty);
        Assert.AreEqual(list.p_list_Stay.ToArray(), arrValue);
        Assert.AreEqual(list.p_list_Exit.ToArray(), arrExit);

        SCManagerProfiler.DoFinishTestCase(nameof(Working_Test));
        SCManagerProfiler.DoPrintResult(true);
    }
}

#endif
#endregion Test

#if ODIN_INSPECTOR
#if UNITY_EDITOR

//public class CList_Enter_Stay_Exit_Drawer<T> : OdinValueDrawer<CList_Enter_Stay_Exit<T>>
//    where T : class
//{
//    protected override void DrawPropertyLayout(GUIContent label)
//    {
//        SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(), Color.red);
//        this.CallNextDrawer(label);
//    }
//}

#endif
#endif