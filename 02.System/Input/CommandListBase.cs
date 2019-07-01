#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-16 오후 6:11:01
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>

public abstract class CommandListBase : ScriptableObject
{
    protected abstract System.Type _pGetInheritedClassType { get; }

    static private Dictionary<System.Type, CommandBase> _mapCommandInstance = new Dictionary<System.Type, CommandBase>();
    private Dictionary<string, CommandBase> _mapCommandInstance_ByDisplayName = new Dictionary<string, CommandBase>();

    public ValueDropdownList<System.Type> GetCommandList()
    {
        ValueDropdownList<System.Type> listCommand = new ValueDropdownList<System.Type>();
        System.Type[] arrInnerClassType = _pGetInheritedClassType.GetNestedTypes(BindingFlags.Public);
        for (int i = 0; i < arrInnerClassType.Length; i++)
        {
            System.Type pCurrentType = arrInnerClassType[i];
            if (pCurrentType.IsAbstract == false)
            {
                if (_mapCommandInstance.ContainsKey(pCurrentType) == false || _mapCommandInstance[pCurrentType] == null)
                {
                    object pObjectCommandInstance = System.Activator.CreateInstance(pCurrentType);
                    if(pObjectCommandInstance != null && (pObjectCommandInstance is CommandBase) == false)
                    {
                        Debug.LogError(pCurrentType.Name + " is not inherit CommandBase");
                        continue;
                    }

                    _mapCommandInstance[pCurrentType] = pObjectCommandInstance as CommandBase;
                }

                listCommand.Add(_mapCommandInstance[pCurrentType].GetDisplayCommandName(), pCurrentType);
            }
        }

        return listCommand;
    }

    //public void DoInit()
    //{
    //    _mapCommandInstance_ByDisplayName.Clear();
    //    System.Type[] arrInnerClassType = _pGetDERIVEDType.GetNestedTypes(BindingFlags.Public);
    //    for (int i = 0; i < arrInnerClassType.Length; i++)
    //    {
    //        System.Type pCurrentType = arrInnerClassType[i];
    //        if (pCurrentType.IsAbstract == false)
    //        {
    //            if (_mapCommandInstance.ContainsKey(pCurrentType) == false)
    //                _mapCommandInstance.Add(pCurrentType, System.Activator.CreateInstance(pCurrentType) as CCommandBase_V2);

    //            _mapCommandInstance_ByDisplayName.Add(_mapCommandInstance[pCurrentType].GetDisplayCommandName(), _mapCommandInstance[pCurrentType]);
    //        }
    //    }
    //}

    public CommandBase GetCommand(string strDisplayCommandName)
    {
        CommandBase pCommand = null;
        _mapCommandInstance_ByDisplayName.TryGetValue(strDisplayCommandName, out pCommand);

        return pCommand;
    }

}