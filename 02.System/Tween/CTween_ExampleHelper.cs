using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTween_ExampleHelper : CUGUIPanelHasInputBase<CTween_ExampleHelper.EUIInput>
{
    public enum EUIInput
    {
        Button_SetTimeScale,
        InputField_TimeScale,
    }

    [GetComponentInChildren]
    public InputField _pInput_TimeScale;

    public void DoJustPrintLog()
    {
        Debug.Log("JustPrint");
    }

    public void DoJustPrintLog(string strLog)
    {
        Debug.Log(strLog);
    }

    public override void OnButtons_Click(EUIInput eButtonName)
    {
        switch (eButtonName)
        {
            case EUIInput.Button_SetTimeScale:

                float fTimeScale;
                if (float.TryParse(_pInput_TimeScale.text, out fTimeScale))
                    Time.timeScale = fTimeScale;
                else
                    _pInput_TimeScale.text = Time.timeScale.ToString();

                break;
        }
    }
}
