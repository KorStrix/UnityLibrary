// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplySelectedPrefabs.cs" company="Supyrb">
//   Copyright (c) 2017 Supyrb. All rights reserved.
// </copyright>
// <author>
//   baptisteLar
//   http://baptistelargaiolli.com/
// </author>
// <author>
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

/// <summary>
/// Apply or revert multiple prefabs at the same time
/// Source: https://forum.unity3d.com/threads/little-script-apply-and-revert-several-prefab-at-once.295311/
/// </summary>
public class ApplySelectedPrefabs
{
    private delegate void ChangePrefab(GameObject go);
    private const int SelectionThresholdForProgressBar = 20;
    private static bool showProgressBar;
    private static int changedObjectsCount;

    [MenuItem("GameObject/Apply Changes To Selected Prefabs %j", false, 100)]
    private static void ApplyPrefabs()
    {
        SearchPrefabConnections(ApplyToSelectedPrefabs);
    }

    [MenuItem("GameObject/Revert Changes Of Selected Prefabs", false, 100)]
    private static void ResetPrefabs()
    {
        SearchPrefabConnections(RevertToSelectedPrefabs);
    }

    [MenuItem("GameObject/Apply Changes To Selected Prefabs", true)]
    [MenuItem("GameObject/Revert Changes Of Selected Prefabs", true)]
    private static bool IsSceneObjectSelected()
    {
        return Selection.activeTransform != null;
    }

    //Look for connections
    private static void SearchPrefabConnections(ChangePrefab changePrefabAction)
    {
        GameObject[] selectedTransforms = Selection.gameObjects;
        int numberOfTransforms = selectedTransforms.Length;
        showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
        changedObjectsCount = 0;
        //Iterate through all the selected gameobjects
        try
        {
            for (int i = 0; i < numberOfTransforms; i++)
            {
                var go = selectedTransforms[i];
                if (showProgressBar)
                {
                    EditorUtility.DisplayProgressBar("Update prefabs", "Updating prefab " + go.name + " (" + i + "/" + numberOfTransforms + ")",
                        (float)i / (float)numberOfTransforms);
                }
                IterateThroughObjectTree(changePrefabAction, go);
            }
        }
        finally
        {
            if (showProgressBar)
            {
                EditorUtility.ClearProgressBar();
            }
            Debug.LogFormat("{0} Prefab(s) updated", changedObjectsCount);
        }
    }

    private static void IterateThroughObjectTree(ChangePrefab changePrefabAction, GameObject go)
    {
        var prefabType = PrefabUtility.GetPrefabType(go);
        //Is the selected gameobject a prefab?
        if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
        {
            var prefabRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
            if (prefabRoot != null)
            {
                changePrefabAction(prefabRoot);
                changedObjectsCount++;
                return;
            }
        }
        // If not a prefab, go through all children
        var transform = go.transform;
        var children = transform.childCount;
        for (int i = 0; i < children; i++)
        {
            var childGo = transform.GetChild(i).gameObject;
            IterateThroughObjectTree(changePrefabAction, childGo);
        }
    }

    //Apply    
    private static void ApplyToSelectedPrefabs(GameObject go)
    {
        var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);
        if (prefabAsset == null)
        {
            return;
        }
        PrefabUtility.ReplacePrefab(go, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
    }

    //Revert
    private static void RevertToSelectedPrefabs(GameObject go)
    {
        PrefabUtility.ReconnectToLastPrefab(go);
        PrefabUtility.RevertPrefabInstance(go);
    }
}