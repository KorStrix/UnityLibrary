using UnityEngine;
using UnityEditor;
#if NGUI
[CanEditMultipleObjects]
[CustomEditor(typeof(CNGUITweenAlphaExtend))]
public class CNGUITweenAlphaExtendEditor : CNGUITweenExtendBaseEditor<CNGUITweenAlphaExtend.STweenAlphaInfo>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		CNGUITweenAlphaExtend pTarget = target as CNGUITweenAlphaExtend;

        for (int i = 0; i < pTarget.listTweenInfo.Count; i++)
        {
            if (NGUIEditorTools.DrawHeader("Tweener_" + i))
            {
                GUI.changed = false;
                float fFrom = EditorGUILayout.Slider("From", pTarget.listTweenInfo[i].fFrom, 0f, 1f);
                float fTo = EditorGUILayout.Slider("To", pTarget.listTweenInfo[i].fTo, 0f, 1f);

                if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].fFrom = fFrom;
                    pTarget.listTweenInfo[i].fTo = fTo;
                    NGUITools.SetDirty(pTarget);
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}
#endif