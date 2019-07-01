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
    [DisplayName("Enter List")]
    private List<T> _listEnter;
    [SerializeField]
    [DisplayName("Stay List")]
    private List<T> _listStay;
    [SerializeField]
    [DisplayName("Exit List")]
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

        // _setStay.UnionWith(_listEnter); 퍼포먼스가 안좋아서 밑의 함수로 대체
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