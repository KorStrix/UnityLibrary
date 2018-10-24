#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-06 오후 4:17:35
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

#if Spine

using Spine;
using Spine.Unity;
using System.Text;

public class CEditorProjectView_Spine_GetAnimation : Editor
{
    [MenuItem("Assets/StrixTool/Get Spine Animation", false, 0)]
    static public void GetSpineAnimation()
    {
        if (Selection.objects == null)
            return;
        else
        {
            SkeletonDataAsset pSpineDataAsset = Selection.objects[0] as SkeletonDataAsset;
            if (pSpineDataAsset == null)
                return;

            SkeletonData pData = pSpineDataAsset.GetSkeletonData(false);

            StringBuilder pBuilder = new StringBuilder();
            foreach (var pAnimation in pData.Animations)
            {
                pBuilder.Append(pAnimation.Name);
                pBuilder.Append(", ");
            }

            pBuilder.Remove(pBuilder.Length - 2, 2);
            Debug.Log(pBuilder.ToString());
        }
    }
}
 

#region Test
#if UNITY_EDITOR

#endif
#endregion Test

#endif
