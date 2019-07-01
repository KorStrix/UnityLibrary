#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-19 오후 12:33:07
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CommandExcuted
{
    public ICommandExecuter pManagerCommand;
    public CommandBase pCommandExcuted;

    public SInputValue sInputValue;
    public float fExcuteTime;

    public CommandExcuted(ICommandExecuter pManagerCommand, CommandBase pCommandExcuted, SInputValue sInputValue, float fExcuteTime)
    {
        this.pManagerCommand = pManagerCommand;
        this.pCommandExcuted = pCommandExcuted;
        this.sInputValue = sInputValue;
        this.fExcuteTime = fExcuteTime;
    }

    public void DoExcute()
    {
        pCommandExcuted.DoExcute(ref sInputValue);
    }

    public void DoExcute_Undo()
    {
        pCommandExcuted.DoExcute_Undo(ref sInputValue);
    }
}

public struct ICommandExecuteArg
{
    public CommandExcuted pCommandExecuted { get; private set; }

    public ICommandExecuteArg(CommandExcuted pCommandExecuted)
    {
        this.pCommandExecuted = pCommandExecuted;
    }
}

public interface ICommandExecuter
{
    ObservableCollection<ICommandExecuteArg> p_Event_OnExecuteCommand { get; }

    bool p_bEnableExecuter { get; set; }
    void ICommandExecuter_Update(ref List<CommandExcuted> listCommandExecute_Default_Is_Empty);
}

/// <summary>
/// 
/// </summary>
public class CManagerCommand : CSingletonDynamicMonoBase<CManagerCommand>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */


    /* public - Field declaration            */

    /* protected & private - Field declaration         */

    List<ICommandExecuter> _listCommandExecuter = new List<ICommandExecuter>();
    List<CommandExcuted> listCommandExecute = new List<CommandExcuted>();
    // List<CommandExcuted> _listCommandExcuted = new List<CommandExcuted>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAdd_CommandExecuter(ICommandExecuter pCommandExecuter)
    {
        if (_listCommandExecuter.Contains(pCommandExecuter) == false)
            _listCommandExecuter.Add(pCommandExecuter);
    }

    public void DoRemove_CommandExecuter(ICommandExecuter pCommandExecuter)
    {
        if (_listCommandExecuter.Contains(pCommandExecuter))
            _listCommandExecuter.Remove(pCommandExecuter);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        float fTime = Time.time;
        for(int i = 0; i < _listCommandExecuter.Count; i++)
        {
            ICommandExecuter pExecuter = _listCommandExecuter[i];
            if (pExecuter.p_bEnableExecuter == false)
                continue;

            listCommandExecute.Clear();
            pExecuter.ICommandExecuter_Update(ref listCommandExecute);
            //var listCommand = pExecuter.ICommandExecuter_GetLastExecute_CommandList();
            //if (listCommand.Count == 0)
            //    continue;

            //for (int j = 0; j < listCommand.Count; j++)
            //    _listCommandExcuted.Add(new CommandExcuted(pExecuter, listCommand[i], fTime));
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}