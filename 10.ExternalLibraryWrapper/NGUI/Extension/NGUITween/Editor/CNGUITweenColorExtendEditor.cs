using UnityEngine;
using UnityEditor;
#if NGUI
[CanEditMultipleObjects]
[CustomEditor(typeof(CNGUITweenColorExtend))]
public class CNGUITweenColorExtendEditor : CNGUITweenExtendBaseEditor<CNGUITweenColorExtend.STweenColorInfo>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		CNGUITweenColorExtend pTarget = target as CNGUITweenColorExtend;
		
        for (int i = 0; i < pTarget.listTweenInfo.Count; i++)
        {
            if (NGUIEditorTools.DrawHeader("Tweener_" + i))
            {
                GUI.changed = false;
                Color pColorFrom = EditorGUILayout.ColorField("From", pTarget.listTweenInfo[i].pColorFrom);
                Color pColorTo = EditorGUILayout.ColorField("From", pTarget.listTweenInfo[i].pColorTo);

                if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].pColorFrom = pColorFrom;
                    pTarget.listTweenInfo[i].pColorTo = pColorTo;
                    NGUITools.SetDirty(pTarget);
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}
#endif