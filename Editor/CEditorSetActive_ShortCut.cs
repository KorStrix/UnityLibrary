#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-12 오전 11:16:26
 *	기능 : 
 *	
 *	밑에 두개 참고
 *	https://bitbucket.org/sirgru/simple-editor-shortcuts-tools-collection/src/0d7066d1c0720d082f482f98a164ed935b2ae2b0?at=default
 *	https://github.com/baba-s/unity-shortcut-key-plus
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class CEditor_ShortCut
{
    [MenuItem("Edit/Active Toggle &a", false, -101)]
    public static void Active_Toggle()
    {
        Undo.RecordObjects(Selection.gameObjects, nameof(Active_Toggle));
        foreach (var selectedObject in Selection.gameObjects)
            selectedObject.SetActive(!selectedObject.activeSelf);
    }




    private static readonly int _iLengthOfAddedString = "(Clone)".Length;
    private static List<int> _listSelection = new List<int>();

    [MenuItem("Edit/Duplicate Without Auto-Name &d", false, -101)]
    public static void Duplicate()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            var pNewGameObject = GameObject.Instantiate(selectedObject, selectedObject.transform.parent);
            pNewGameObject.name = pNewGameObject.name.Substring(0, pNewGameObject.name.Length - _iLengthOfAddedString);

            int iUnderbarIndex = pNewGameObject.name.LastIndexOf('_');
            if (iUnderbarIndex !=- 1)
            {
                string strUnderbarNextText = pNewGameObject.name.Substring(iUnderbarIndex + 1);

                int iNumber;
                if(int.TryParse(strUnderbarNextText, out iNumber))
                {
                    pNewGameObject.name = pNewGameObject.name.Substring(0, pNewGameObject.name.Length - iNumber.ToString().Length);
                    pNewGameObject.name += (++iNumber).ToString();
                }
            }

            Undo.RegisterCreatedObjectUndo(pNewGameObject, nameof(Duplicate));
            _listSelection.Add(pNewGameObject.GetInstanceID());
        }

        Selection.instanceIDs = _listSelection.ToArray();
        _listSelection.Clear();
    }
}
