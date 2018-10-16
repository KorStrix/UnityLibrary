using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLog_Test : MonoBehaviour
{
    public class DebugCustom : Strix.Debug<ETestLogWriter, ETestLogLevelCustom>
    {

    }

    public enum ETestLogWriter
    {
        Programmer_Senior = 1,
        Programmer_Junior = 2,
        Programmer_Newbie = 4,
    }

    public enum ETestLogLevelGroup
    {
        InGameGroup,
        UIGroup,
    }

    public enum ETestLogLevelCustom
    {
        UI = 1,
        UI_ForDebug = 2,

        InGame = 4,
        InGame_ForDebug = 8,

        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Warning)]
        Warning,
        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Error)]
        Error,

        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Log, (int)ETestLogLevelGroup.InGameGroup)]
        InGame_Work_1,
        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Warning, (int)ETestLogLevelGroup.InGameGroup)]
        InGame_Work_2,
        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Error)]
        InGame_Work_3,
        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Log)]
        UI_Work_1,
        [Strix.CustomLogLevel(Strix.ELogLevel.CustomLine_Warning, (int)ETestLogLevelGroup.UIGroup)]
        UI_Work_2,

    }

    void Start()
    {
        DebugCustom.ClearCustomLogSetting();
        TestCase_1();
        TestCase_2();
        TestCase_3();
        TestCase_4();
    }

    void TestCase_1()
    {
        DebugCustom.SetFileExportType(DebugCustom.EFlagFileExportType.TXT, DebugCustom.EFlagFileExportType.CSV);
        DebugCustom.SetFileNameType(DebugCustom.EFileNameType.OnMinute);

        DebugCustom.Log(ETestLogWriter.Programmer_Senior, ETestLogLevelCustom.InGame_Work_1, "Senior Work", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Junior, ETestLogLevelCustom.InGame_Work_1, "Junior Work", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_Work_1, "Newbie Work", this);

        // 특정 작성자 로그만 보고싶다 하시면 이것을 통해 필터링합니다.
        Debug.LogWarning("SetIgnoreLogWriter - Programmer_Senior, Programmer_Junior", this);
        DebugCustom.SetIgnoreLogWriter(ETestLogWriter.Programmer_Senior, ETestLogWriter.Programmer_Junior);

        // 그다음 다시 로그를 출력하면 뉴비 로그만 출력
        DebugCustom.Log(ETestLogWriter.Programmer_Senior, ETestLogLevelCustom.InGame_Work_1, "Senior Work 2", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Junior, ETestLogLevelCustom.InGame_Work_1, "Junior Work 2", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_Work_1, "Newbie Work 2", this);

        // 커스텀 로그 레벨도 가능합니다.
        // 만약 UI 로그만 보고싶다면
        // 그 외 로그는 다 무시
        Debug.LogWarning("SetIgnoreLogLevel_Custom - InGame, InGame_ForDebug", this);
        DebugCustom.SetIgnoreLogLevel(ETestLogLevelCustom.InGame, ETestLogLevelCustom.InGame_ForDebug);

        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame, "Work Ingame", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_ForDebug, "Work Ingame Debuging", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI, "Work UI", this);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI_ForDebug, "Work UI Debuging", this);
    }

    void TestCase_2()
    {
        // 일반 유니티 로그도 메세지만 등록하면 똑같이 사용 가능합니다.
        Application.logMessageReceived += DebugCustom.OnUnityDebugLogCallBack;

        Debug.Log("Unity Log!!", this);
        Debug.LogWarning("Unity Warning!!", this);
        Debug.LogError("Unity Error!!", this);
    }

    void TestCase_3()
    {
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.Warning, "Warnning!!");
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.Error, "Error!!");
    }

    void TestCase_4()
    {
        DebugCustom.SetIgnoreLogLevelGroup(ETestLogLevelGroup.InGameGroup);
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_Work_1, ETestLogLevelCustom.InGame_Work_1.ToString());
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.InGame_Work_2, ETestLogLevelCustom.InGame_Work_2.ToString());

        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI_Work_1, ETestLogLevelCustom.UI_Work_1.ToString());
        DebugCustom.Log(ETestLogWriter.Programmer_Newbie, ETestLogLevelCustom.UI_Work_2, ETestLogLevelCustom.UI_Work_2.ToString());

    }

}