using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerExample : CSpawnerBase
{
    protected override void OnInit(ref string strSpawnObject_ContainFolderPath_Default_Is_ResourcesPath)
    {
        string strFilePath = GetCurrentClassFilePath();
        int iIndex = strFilePath.LastIndexOf("\\");

        strFilePath = strFilePath.Substring(0, iIndex);
        strFilePath += "\\Resources\\";

        strSpawnObject_ContainFolderPath_Default_Is_ResourcesPath = strFilePath;
    }

    static string GetCurrentClassFilePath([System.Runtime.CompilerServices.CallerFilePath]string fileName = null)
    {
        return fileName;
    }
}
