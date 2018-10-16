using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if NGUI
public static class NGUIExtensionHelper
{
	public static void UpdateInputLabel( this UIInput pInput, UIInput.InputType eInputType )
	{
		pInput.inputType = eInputType;
		pInput.UpdateLabel();
	}
}
#endif