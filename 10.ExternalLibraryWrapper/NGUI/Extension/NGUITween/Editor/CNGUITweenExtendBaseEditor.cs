#if NGUI

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
public class CNGUITweenExtendBaseEditor<T> : UITweenerEditor
	where T : STweenInfoBase, new()
{
    public override void OnInspectorGUI()
    {
		GUILayout.Space( 6f );
		NGUIEditorTools.SetLabelWidth( 120f );

		CNGUITweenExtendBase<T> pTarget = target as CNGUITweenExtendBase<T>;

		GUI.changed = false;
		GUILayout.BeginHorizontal();
		int iGroupSizeNew = EditorGUILayout.IntField( "TweenInfoCount", pTarget.listTweenInfo.Count, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool bIgnoreTimeScale = EditorGUILayout.Toggle( "IgnoreTimeScale", pTarget.ignoreTimeScale, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool bPlayOnEnable = EditorGUILayout.Toggle( "PlayOnEnable", pTarget._bPlayOnEnable, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float fTweenSpeed = EditorGUILayout.FloatField( "Speed", pTarget.p_fTweenSpeed, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.FloatField( "Tween Amount", pTarget.p_fTweenAmount, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			pTarget._bPlayOnEnable = bPlayOnEnable;
			pTarget.p_fTweenSpeed = fTweenSpeed;
			pTarget.ignoreTimeScale = bIgnoreTimeScale;

			pTarget.SetTweenInfoSize( iGroupSizeNew );
			NGUITools.SetDirty( pTarget );
			GUI.changed = false;
		}
	}

	protected void EventDrawCommonProperties(Object pTarget, STweenInfoBase sTweenInfoBase)
    {
        NGUIEditorTools.BeginContents();
        NGUIEditorTools.SetLabelWidth(110f);

        GUI.changed = false;

        UITweener.Style eStyle = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", sTweenInfoBase.eStyle);
        AnimationCurve pAnimationCurve = EditorGUILayout.CurveField("Animation Curve", sTweenInfoBase.pAnimationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
		
		GUILayout.BeginHorizontal();
        float fDuration = EditorGUILayout.FloatField("Duration", sTweenInfoBase.fDuration, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        float fStartDelay_Min = EditorGUILayout.FloatField("Start Delay_Min", sTweenInfoBase.fStartDelay_Min, GUILayout.Width(170f));
		float fStartDelay_Max = EditorGUILayout.FloatField( "Start Delay_Max", sTweenInfoBase.fStartDelay_Max, GUILayout.Width( 170f ) );
		GUILayout.Label("seconds");
        GUILayout.EndHorizontal();
        
        if (GUI.changed)
        {
            NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
			sTweenInfoBase.pAnimationCurve = pAnimationCurve;
            sTweenInfoBase.eStyle = eStyle;
            sTweenInfoBase.fDuration = fDuration;
            sTweenInfoBase.fStartDelay_Min = fStartDelay_Min;
			sTweenInfoBase.fStartDelay_Max = fStartDelay_Max;
			NGUITools.SetDirty(pTarget);
            GUI.changed = false;
        }
        NGUIEditorTools.EndContents();

        NGUIEditorTools.DrawEvents("On Finished", pTarget, sTweenInfoBase.listOnFinished);
    }
}
#endif