using UnityEngine;
using UnityEditor;
#if NGUI

//[CanEditMultipleObjects]
[CustomEditor(typeof(CNGUITweenPositionExtend))]
public class CNGUITweenPositionExtendEditor : CNGUITweenExtendBaseEditor<CNGUITweenPositionExtend.STweenPositionInfo>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		CNGUITweenPositionExtend pTarget = target as CNGUITweenPositionExtend;

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