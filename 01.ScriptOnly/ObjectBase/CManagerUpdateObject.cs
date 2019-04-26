#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-17 오후 10:36:36
 *	기능 : 
 *	https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/
 *	업데이트 오브젝트 카운팅
 *	모노비헤비어 외의 객체에 업데이트 기능 지원
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface IUpdateAble
{
    bool IUpdateAble_IsRequireUpdate();
    void OnUpdate();
}

public class CManagerUpdateObject : CSingletonDynamicMonoBase<CManagerUpdateObject>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public List<IUpdateAble> _listObject_ForDebug = new List<IUpdateAble>();

    int _iPrevObjectCount;

    static private List<IUpdateAble> g_listObject = new List<IUpdateAble>();
    static private HashSet<IUpdateAble> g_setUpdateObject = new HashSet<IUpdateAble>();

    List<int> _listDestroyIndex = new List<int>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAddObject(IUpdateAble pObject, bool bForceAdd = false)
    {
        if (g_setUpdateObject.Contains(pObject))
        {
            if (bForceAdd)
                g_setUpdateObject.Remove(pObject);
            else
                return;
        }

        g_setUpdateObject.Add(pObject);
        g_listObject.Add(pObject);

#if UNITY_EDITOR
        _listObject_ForDebug.Add(pObject);
#endif
    }

    public void DoRemoveObject(IUpdateAble pObject)
    {
        if (g_setUpdateObject.Contains(pObject) == false)
            return;

        g_setUpdateObject.Remove(pObject);
        g_listObject.Remove(pObject);

#if UNITY_EDITOR
        _listObject_ForDebug.Remove(pObject);
#endif
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        DontDestroyOnLoad(gameObject);
    }

    protected override IEnumerator OnEnableObjectCoroutine()
    {
        while (true)
        {
            _listDestroyIndex.Clear();
            int iUpdateObjectCount = 0;
            int iListCount = g_listObject.Count;
            for (int i = 0; i < iListCount; i++)
        {
                IUpdateAble pUpdateAble = g_listObject[i];
                if (pUpdateAble.IUpdateAble_IsRequireUpdate())
                {
                    pUpdateAble.OnUpdate();
                    ++iUpdateObjectCount;
                }
            }

            //for(int i = 0; i < _listDestroyIndex.Count; i++)
            //    g_listObject.RemoveAt(_listDestroyIndex[i]);


#if UNITY_EDITOR
            if (iUpdateObjectCount != _iPrevObjectCount)
            {
                _iPrevObjectCount = iUpdateObjectCount;
                name = string.Format("업데이트 매니져/{0}개 업데이트중", iUpdateObjectCount);
            }
#endif

            yield return null;
        }
    }

#if UNITY_EDITOR
    protected override void OnReleaseSingleton()
    {
        base.OnReleaseSingleton();

        g_setUpdateObject.Clear();
        g_listObject.Clear();
        _listObject_ForDebug.Clear();
    }
#endif


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}
