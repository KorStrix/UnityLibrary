#if NGUI

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
/* ============================================ 
   Editor      : parkjonghwa                             
   Date        : 2017-02-09 오후 7:02:48
   Description : 
   Edit Log    : 
   ============================================ */

[CanEditMultipleObjects]
[CustomEditor(typeof(CUIButtonMultiToggle), true)]
public class CUIButtonMultiToggleEditor : Editor
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private CUIButtonMultiToggle _pTarget;
    private UIAtlas _pAtlas;

    private int _iOriginStateCount;
    private int _iCurrentEditItemIndex;
    private CUIButtonMultiToggle.EButtonToggleOption _eButtonToggleOption;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        _pTarget = target as CUIButtonMultiToggle;
        _pAtlas = _pTarget.GetComponent<UISprite>().atlas;

		if (_pTarget.listEvent == null)
			_pTarget.listEvent = new List<CUIButtonMultiToggle.SButtonToggle>();

		_iOriginStateCount = _pTarget.listEvent.Count;
        _iOriginStateCount = EditorGUILayout.IntField("상태 개수", _iOriginStateCount);

        if (_pTarget.listEvent == null)
            _pTarget.listEvent = new List<CUIButtonMultiToggle.SButtonToggle>();

        if (_pAtlas == null)
        {
            Debug.LogWarning("UI Sprite의 Atlas가 세팅되지 않았습니다..", _pTarget);
            return;
        }

        if (_iOriginStateCount != _pTarget.listEvent.Count)
        {
            if(_iOriginStateCount < _pTarget.listEvent.Count)
            {
                int iLoopCount = _pTarget.listEvent.Count - _iOriginStateCount;
                try
                {
                    for (int i = 0; i < iLoopCount; i++)
                        _pTarget.listEvent.Remove(_pTarget.listEvent[i]);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            else
            {
                int iLoopCount = _iOriginStateCount - _pTarget.listEvent.Count;
                for (int i = 0; i < iLoopCount; i++)
                    _pTarget.listEvent.Add(new CUIButtonMultiToggle.SButtonToggle());
            }
        }

        if(_iOriginStateCount != 0)
            _pTarget.iDefaultState = EditorGUILayout.IntField(string.Format("기본 상태 1 ~ {0}", _pTarget.listEvent.Count), _pTarget.iDefaultState);
        if (_pTarget.iDefaultState > _pTarget.listEvent.Count|| _pTarget.iDefaultState <= 0)
            Debug.LogError("상태개수 범위안의 숫자를 선택해 주십시오.");

        _pTarget._eButtonToggleOption = (CUIButtonMultiToggle.EButtonToggleOption)EditorGUILayout.EnumPopup("옵션선택", _pTarget._eButtonToggleOption);

        for (int i = 0; i < _pTarget.listEvent.Count; i++)
        {
            CUIButtonMultiToggle.SButtonToggle sToggle = _pTarget.listEvent[i];
            //NGUIEditorTools.DrawEvents("On StateChange", _pTarget, sToggle.listEvent, false);

            if (NGUIEditorTools.DrawHeader(string.Format("OnChangeState_{0}",i+1)) == false) continue;

            if (_pTarget.listEvent[i].listEvent == null)
                _pTarget.listEvent[i].listEvent = new List<EventDelegate>();

            EventDelegateEditor.Field(_pTarget, _pTarget.listEvent[i].listEvent, false);
            // 여기서 필요한 로직만 가져옴
            //NGUIEditorTools.DrawAdvancedSpriteField(_pAtlas, sToggle.strSpriteName, SelectSprite, false);
            GUILayout.BeginHorizontal();
            {
                if (NGUIEditorTools.DrawPrefixButton("Sprite"))
                {
                    NGUISettings.atlas = _pAtlas;
                    NGUISettings.selectedSprite = sToggle.strSpriteName;
                    SpriteSelector.Show(SelectSprite);
                    _iCurrentEditItemIndex = i;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(sToggle.strSpriteName, "HelpBox", GUILayout.Height(18f));
                NGUIEditorTools.DrawPadding();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    void SelectSprite(string spriteName)
    {
        serializedObject.Update();
        _pTarget.listEvent[_iCurrentEditItemIndex].strSpriteName = spriteName;
        NGUITools.SetDirty(_pTarget);
        NGUISettings.selectedSprite = spriteName;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif