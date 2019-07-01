using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    //public class CommandTest_Print_1 : CommandBase
    //{
    //    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    //    {
    //        CManagerCommand_Test.g_strBuffer += "1";
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //    public override void DoExcute_Undo(ref SInputValue sValue)
    //    {
    //        CManagerCommand_Test.g_strBuffer = CManagerCommand_Test.g_strBuffer.Remove(CManagerCommand_Test.g_strBuffer.Length - 1);
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //}
    //public class CommandTest_Print_2 : CommandBase
    //{
    //    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    //    {
    //        CManagerCommand_Test.g_strBuffer += "2";
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //    public override void DoExcute_Undo(ref SInputValue sValue)
    //    {
    //        CManagerCommand_Test.g_strBuffer = CManagerCommand_Test.g_strBuffer.Remove(CManagerCommand_Test.g_strBuffer.Length - 1);
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //}
    //public class CommandTest_Print_3 : CommandBase
    //{
    //    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    //    {
    //        CManagerCommand_Test.g_strBuffer += "3";
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //    public override void DoExcute_Undo(ref SInputValue sValue)
    //    {
    //        CManagerCommand_Test.g_strBuffer = CManagerCommand_Test.g_strBuffer.Remove(CManagerCommand_Test.g_strBuffer.Length - 1);
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //}
    //public class CommandTest_Print_1_And_2 : CommandBase
    //{
    //    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    //    {
    //        CManagerCommand_Test.g_strBuffer += "12";
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //    public override void DoExcute_Undo(ref SInputValue sValue)
    //    {
    //        CManagerCommand_Test.g_strBuffer = CManagerCommand_Test.g_strBuffer.Remove(CManagerCommand_Test.g_strBuffer.Length - 2, 2);
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //}
    //public class CommandTest_Print_2_And_3 : CommandBase
    //{
    //    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    //    {
    //        CManagerCommand_Test.g_strBuffer += "23";
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //    public override void DoExcute_Undo(ref SInputValue sValue)
    //    {
    //        CManagerCommand_Test.g_strBuffer = CManagerCommand_Test.g_strBuffer.Remove(CManagerCommand_Test.g_strBuffer.Length - 2, 2);
    //        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    //    }
    //}


    //public class CManagerCommand_Test : CManagerCommand
    //{
    //    public enum EInputElementName
    //    {
    //        Virtual_KeyboardInput_1,
    //        Virtual_KeyboardInput_2,
    //        Virtual_KeyboardInput_3,

    //        KeyboardInput_Max,
    //    }

    //    public enum EInputEventName
    //    {
    //        InputEvent_1,
    //        InputEvent_2,
    //        InputEvent_3,
    //    }

    //    static public string g_strBuffer;

    //    /// <summary>
    //    /// Input Element를 등록합니다.
    //    /// Input Element란 GetButtonDown, GetButton, GetAxis 등의 구체적인 Input Event입니다.
    //    /// </summary>
    //    /// <param name="listInput_RegistHere"></param>
    //    /// 

    //    protected override void OnAwake()
    //    {
    //        base.OnAwake();

    //        _mapInputElement.Clear();
    //        _mapInputEvent.Clear();
    //        _mapCommandWrapper.Clear();
    //        _mapCommandWrapper_Category.Clear();
    //        _listCommandWrapper.Clear();

    //        Step1_OnRegist_InputElement();
    //        Step2_OnRegist_Command();

    //        _mapCommandWrapper.Values.ToList(_listCommandWrapper);
    //        _listCommandWrapper.Sort(CommandWrapper.Comparer);
    //    }

    //    protected void Step1_OnRegist_InputElement()
    //    {
    //        // 테스트를 위한 가상 키보드 입력
    //        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_1);
    //        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_2);
    //        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_3);

    //        // 원래 프로그램에 작성해야 될 인풋 코드
    //        // listInput_RegistHere.Add(Create_InputElement_Keyboard(EInputElementName.Virtual_KeyboardInput_1, "1", EInputType.ButtonDown));
    //    }

    //    /// <summary>
    //    /// Command란 게임에 실질적으로 들어갈 로직입니다.
    //    /// </summary>
    //    /// <param name="listCommand_RegistHere"></param>

    //    protected void Step2_OnRegist_Command()
    //    {
    //        DoCreate_CommandWrapper(new CommandTest_Print_1(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_1.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_1))));
    //        DoCreate_CommandWrapper(new CommandTest_Print_2(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_2.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_2))));
    //        DoCreate_CommandWrapper(new CommandTest_Print_3(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_3.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_3))));
    //    }

    //    // ========================================================
    //    // Test 시작

    //    Dictionary<EInputElementName, VirtualInputElement> mapVirtualInput_ForTest;

    //    [Test]
    //    public void Simple_Test()
    //    {
    //        CManagerCommand_Test pManagerTest = InitCommandManager();
    //        g_strBuffer = "";
    //        Assert.AreEqual(g_strBuffer, "");

    //        // 키입력을 하려면 true를 한다음 update 이후 false를 해야 합니다.
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate(1f);
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
    //        Assert.AreEqual(g_strBuffer, "1");

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate(1f);
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;
    //        Assert.AreEqual(g_strBuffer, "12");

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = true;
    //        pManagerTest.OnUpdate(1f);
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = false;
    //        Assert.AreEqual(g_strBuffer, "123");
    //    }

    //    [Test]
    //    public void Undo_Redo_Test()
    //    {
    //        CManagerCommand_Test pManagerTest = InitCommandManager();
    //        g_strBuffer = "";
    //        Assert.AreEqual(g_strBuffer, "");

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
    //        Assert.AreEqual(g_strBuffer, "1");

    //        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
    //        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1");
    //        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1"); // 두번 Redo를 해도 변함이 없습니다.
    //        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
    //        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1");
    //        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
    //        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, ""); // 두번 Undo를 해도 변함이 없습니다.

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate(1f);
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate(1f);
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

    //        Assert.AreEqual(g_strBuffer, "12");
    //        pManagerTest.DoUndo_Command();
    //        pManagerTest.DoUndo_Command();

    //        Assert.AreEqual(g_strBuffer, "");
    //    }

    //    [Test]
    //    public void Replay_Test()
    //    {
    //        CManagerCommand_Test pManagerTest = InitCommandManager();
    //        g_strBuffer = "";
    //        Assert.AreEqual(g_strBuffer, "");

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

    //        g_strBuffer += "_";
    //        Assert.AreEqual(g_strBuffer, "1212_");

    //        pManagerTest.DoReplay_Command(); // 이전에 1212를 입력한 채로 Replay를 요청하면 1212를 또 입력합니다.
    //        Assert.AreEqual(g_strBuffer, "1212_1212");
    //    }

    //    [UnityTest]
    //    public IEnumerator Replay_Test_WithTimeScale() // 로그로 실행 시간을 출력했습니다. 
    //    {
    //        CManagerCommand_Test pManagerTest = InitCommandManager();
    //        g_strBuffer = "";
    //        Assert.AreEqual(g_strBuffer, "");

    //        List<float> listWaitTime = new List<float>();

    //        Time.timeScale = 5f;

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

    //        float fRandomWait_1 = CreateWaitSecond_And_PrintLog(listWaitTime);
    //        yield return new WaitForSeconds(fRandomWait_1);

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

    //        float fRandomWait_2 = CreateWaitSecond_And_PrintLog(listWaitTime);
    //        yield return new WaitForSeconds(fRandomWait_2);

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = false;

    //        float fRandomWait_3 = CreateWaitSecond_And_PrintLog(listWaitTime);
    //        yield return new WaitForSeconds(fRandomWait_3);

    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
    //        pManagerTest.OnUpdate();
    //        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

    //        g_strBuffer += "_";
    //        Assert.AreEqual(g_strBuffer, "2132_");

    //        float fReplayTimeAgo = fRandomWait_2 + fRandomWait_3 + 0.3f;  // 0.3을 더한 이유는 WaitForSecound 특성상 딱맞게 기다리지 않기 때문입니다.
    //        float fReplayStartTime = Time.time;
    //        Debug.Log("");
    //        Debug.Log("Replay Start : " + fReplayStartTime.ToString("F2") + " Time Ago : " + fReplayTimeAgo.ToString("F2"));
    //        Debug.Log("Start Time : " + (fReplayStartTime - fReplayTimeAgo).ToString("F2"));
    //        Debug.Log("");

    //        int iLoopCount = 0;
    //        var pCoroutine = pManagerTest.CoReplay_Command(fReplayTimeAgo);
    //        while (pCoroutine.MoveNext())
    //        {
    //            yield return pCoroutine.Current;

    //            float fCurrentTime = Time.time;
    //            float fWaitSecond = fCurrentTime - fReplayStartTime;
    //            Debug.Log("Wait Second : " + fWaitSecond.ToString("F2"));
    //            fReplayStartTime = fCurrentTime;

    //            // 실행시 기다리는 시간과 리플레이시 기다리는 시간의 값의 차이가 0.2초이하면 True / 0.2초의 경우 위와 마찬가지로 WaitForSecond 특성상 정확하지 않다.
    //            // 로그 확인
    //            if (iLoopCount != 0)
    //                Assert.IsTrue(fWaitSecond.IsSimilar(listWaitTime[iLoopCount], 0.3f));

    //            iLoopCount++;
    //        }

    //        Assert.AreEqual(g_strBuffer, "2132_132");
    //        Time.timeScale = 1f;

    //        yield break;
    //    }

    //    // ========================================================

    //    /// <summary>
    //    /// 커맨드 매니져 초기화 코드
    //    /// </summary>
    //    /// <returns></returns>
    //    private CManagerCommand_Test InitCommandManager()
    //    {
    //        GameObject pObjectCommand = new GameObject();
    //        CManagerCommand_Test pManagerTest = pObjectCommand.AddComponent<CManagerCommand_Test>();
    //        pManagerTest.EventOnAwake_Force();
    //        pManagerTest.DoInit_CommandAll();

    //        mapVirtualInput_ForTest = new Dictionary<EInputElementName, VirtualInputElement>();
    //        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_1, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_1)) as VirtualInputElement);
    //        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_2, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_2)) as VirtualInputElement);
    //        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_3, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_3)) as VirtualInputElement);

    //        return pManagerTest;
    //    }

    //    private static float CreateWaitSecond_And_PrintLog(List<float> listWaitTime)
    //    {
    //        float fRandomWait = Random.Range(0.8f, 1.5f);
    //        Debug.Log("Wait Second : " + fRandomWait.ToString("F2"));
    //        listWaitTime.Add(fRandomWait);
    //        return fRandomWait;
    //    }
    //}
}
