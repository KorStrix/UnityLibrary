#if NGUI

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CNGUITweenScaleExtend))]
public class CNGUITweenExtendScaleEditor : CNGUITweenExtendBaseEditor<CNGUITweenScaleExtend.STweenScaleInfo>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		CNGUITweenScaleExtend pTarget = target as CNGUITweenScaleExtend;
		
        for (int i = 0; i < pTarget.listTweenInfo.Count; i++)
        {
            if (NGUIEditorTools.DrawHeader("Tweener_" + i))
            {
                GUI.changed = false;
                Vector3 from = EditorGUILayout.Vector3Field("From", pTarget.listTweenInfo[i].vecFrom);
                Vector3 to = EditorGUILayout.Vector3Field("To", pTarget.listTweenInfo[i].vecTo);

                if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].vecFrom = from;
                    pTarget.listTweenInfo[i].vecTo = to;
                    NGUITools.SetDirty(pTarget);
                    GUI.changed = false;
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}

#endif