#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-17 오후 10:36:36
 *	기능 : 
 *	https://blogs.unity3d.com/kr/2015/12/23/1k-update-calls/
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Unity.Jobs;
using Unity.Collections;

public interface IUpdateAble
{
    void OnUpdate(ref bool bCheckUpdateCount);
}

public class CManagerUpdateObject : CSingletonSOBase<CManagerUpdateObject>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    /* protected & private - Field declaration         */

#if UNITY_EDITOR
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspectorAttribute]
#endif
    public List<IUpdateAble> _listObject_ForDebug = new List<IUpdateAble>();

    int _iPrevObjectCount;

#endif

    static private LinkedList<IUpdateAble> g_listObject = new LinkedList<IUpdateAble>();
    static private HashSet<IUpdateAble> g_setUpdateObject = new HashSet<IUpdateAble>();

    GameObject _pObjectManager;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAddObject(IUpdateAble pObject)
    {
        if (g_setUpdateObject.Contains(pObject))
            return;

        g_setUpdateObject.Add(pObject);
        g_listObject.AddLast(pObject);

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

    protected override void OnGenerate_SingletonGameObject()
    {
        _pObjectManager = _pObjectAttacher.gameObject;
        _pObjectAttacher.StartCoroutine(OnEnableObjectCoroutine());
        DontDestroyOnLoad(_pObjectManager);
    }

    protected IEnumerator OnEnableObjectCoroutine()
    {
        while (true)
        {
            int iUpdateObjectCount = 0;
            int iLoopCount = g_listObject.Count;
            var pNode = g_listObject.First;

            while (pNode != null)
            {
                bool bCheckUpdate = false;
                pNode.Value.OnUpdate(ref bCheckUpdate);
                if (bCheckUpdate)
                    ++iUpdateObjectCount;

                pNode = pNode.Next;
            }


#if UNITY_EDITOR
            if (iUpdateObjectCount != _iPrevObjectCount)
            {
                _iPrevObjectCount = iUpdateObjectCount;
                _pObjectManager.name = string.Format("업데이트 매니져/{0}개 업데이트중", iUpdateObjectCount);
            }
#endif

            yield return null;
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
