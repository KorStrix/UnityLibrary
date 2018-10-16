// ============================================ 
// Editor      : Strix                               
// Date        :
// Description : 
// Edit Log    : 
// ============================================ 

using UnityEngine;
using System.Collections.Generic;

public interface IResourceGetter<Resource>
{
    Resource GetResource(string strResourceName);
}

public class CResourceGetter<RESOURCE> : IResourceGetter<RESOURCE>
    where RESOURCE : UnityEngine.Object
{
    string _strResourceLocalPath = null;
    Dictionary<string, RESOURCE> _mapResourceOrigin = new Dictionary<string, RESOURCE>();

    // ========================== [ Division ] ========================== //

    public CResourceGetter(string strResourceLocalPath)
    {
        _strResourceLocalPath = strResourceLocalPath;
        RESOURCE[] arrResources = Resources.LoadAll<RESOURCE>(_strResourceLocalPath + "/");
        for (int i = 0; i < arrResources.Length; i++)
            _mapResourceOrigin.Add(arrResources[i].name, arrResources[i]);
    }

    public RESOURCE GetResource(string eResourceName)
    {
        RESOURCE pFindResource;
        if (_mapResourceOrigin.TryGetValue(eResourceName, out pFindResource) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eResourceName));

        return pFindResource;
    }
}
