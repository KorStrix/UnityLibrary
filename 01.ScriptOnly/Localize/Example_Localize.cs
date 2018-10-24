using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example_Localize : MonoBehaviour {

    public TextAsset _pTextLocale_Kor;
    public TextAsset _pTextLocale_Eng;

    private void Awake()
    {
        CManagerUILocalize.instance.EventSet_LocalizeParsing(SystemLanguage.Korean, _pTextLocale_Kor.text, _pTextLocale_Kor.bytes, false);
        CManagerUILocalize.instance.EventSet_LocalizeParsing(SystemLanguage.English, _pTextLocale_Eng.text, _pTextLocale_Eng.bytes, true);

        DoSetLocalize_Kor();
    }

    public void DoSetLocalize_Kor()
    {
        CManagerUILocalize.instance.DoSet_Localize(SystemLanguage.Korean);
    }

    public void DoSetLocalize_Eng()
    {
        CManagerUILocalize.instance.DoSet_Localize(SystemLanguage.English);
    }
}
