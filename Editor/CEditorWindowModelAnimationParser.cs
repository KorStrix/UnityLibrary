using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-03-29 오전 12:30:30
   Description : 
   Edit Log    : 
   ============================================ */

public class CEditorWindowModelAnimationParser : EditorWindow
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    private enum EState
    {
        Setting,
        AlreadyParse,
        Done
    }

    private struct SAnimationKey
    {
        public string strAnimationName;
        public int iFrameStart;
        public int iFrameLast;
    }

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    private List<SAnimationKey> _listAnimationKey = new List<SAnimationKey>();

    private ModelImporter _pModelImporter;
    private TextAsset _pAnimationFrameText;
    private EState _eState;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    [MenuItem("Tools/Strix_Tools/Model_Animation Parser Keyframe")]
    public static void DoOpen_Model_AnimationParser()
    {
        GetWindow<CEditorWindowModelAnimationParser>(true, "Model_Animation Parser Keyframe", true);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    private void OnDisable()
    {
        _eState = EState.Setting;
        _listAnimationKey.Clear();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal(); GUILayout.Label("설명 : 하단은 예제 양식"); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("Move 1 ~ 13"); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("Attack 15 ~ 25"); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("애니메이션 키값 / 시작 프레임 / 끝 프레임"); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("왠만하면 애니메이션 키값은 영어로 작성바람"); GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("애니메이션을 추출할 모델");
        GameObject pObject = EditorGUILayout.ObjectField(_pModelImporter, typeof(GameObject), false) as GameObject;
        GUILayout.EndHorizontal();

        if (_pModelImporter == null && pObject != null)
        {
            string strPath = AssetDatabase.GetAssetPath(pObject);
            _pModelImporter = ModelImporter.GetAtPath(strPath) as ModelImporter;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("애니메이션 프레임이 들어있는 Text파일");
        _pAnimationFrameText = EditorGUILayout.ObjectField(_pAnimationFrameText, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        if (_pModelImporter != null && _pAnimationFrameText != null)
        {
            switch (_eState)
            {
                case EState.Setting:
                    if (GUILayout.Button("텍스트 파일 파싱하기", GUILayout.MaxWidth(300)))
                    {
                        bool bSuccess = false;

                        _listAnimationKey.Clear();

                        string strText = _pAnimationFrameText.text;
                        strText = strText.Replace('\r', ' ');

                        while(true)
                        {
                            SAnimationKey sAnimationKey = new SAnimationKey();
                            strText = strText.CutString(' ', out sAnimationKey.strAnimationName);
                            if (sAnimationKey.strAnimationName == null)
                                break;

                            strText = strText.CutString('~', out sAnimationKey.iFrameStart);
                            strText = strText.CutString('\n', out sAnimationKey.iFrameLast);
                            if(sAnimationKey.iFrameLast == -1)
                            {
                                int.TryParse(strText, out sAnimationKey.iFrameLast);
                                bSuccess = true;
                            }

                            _listAnimationKey.Add(sAnimationKey);

                            if (bSuccess)
                                break;
                        }

                        if (bSuccess)
                            _eState = EState.AlreadyParse;  
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("파싱 실패..");
                            GUILayout.EndHorizontal();
                        }
                    }
                    break;

                case EState.AlreadyParse:
                    if (GUILayout.Button("애니메이션 파싱!!", GUILayout.MaxWidth(300)))
                    {
                        ModelImporterClipAnimation[] arrModelAnimationClip = new ModelImporterClipAnimation[_listAnimationKey.Count];
                        for (int i = 0; i < _listAnimationKey.Count; i++)
                        {
                            ModelImporterClipAnimation pAnimation = new ModelImporterClipAnimation();
                            pAnimation.firstFrame = _listAnimationKey[i].iFrameStart;
                            pAnimation.lastFrame = _listAnimationKey[i].iFrameLast;
                            pAnimation.name = _listAnimationKey[i].strAnimationName;

                            arrModelAnimationClip[i] = pAnimation;
                        }

                        _pModelImporter.clipAnimations = arrModelAnimationClip;
                        ProcReset();
                    }

                    for (int i = 0; i < _listAnimationKey.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(string.Format("Key[{0}] Name [{1}] StartFrame [{2}] EndFrame [{3}]",
                            i, _listAnimationKey[i].strAnimationName, _listAnimationKey[i].iFrameStart, _listAnimationKey[i].iFrameLast));
                        GUILayout.EndHorizontal();
                    }

                    break;
            }
        }

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("리셋하기", GUILayout.MaxWidth(300)))
        //{
        //    ProcReset();
        //}
        //GUILayout.EndHorizontal();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void ProcReset()
    {
        _pModelImporter = null;
        _pAnimationFrameText = null;
        _listAnimationKey.Clear();
        _eState = EState.Setting;
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
