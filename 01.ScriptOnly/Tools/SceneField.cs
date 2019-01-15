using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if ODIN_INSPECTOR
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
#endif

// https://answers.unity.com/questions/242794/inspector-field-for-scene-asset.html

[System.Serializable]
public class SceneField
{
    [SerializeField]
    private Object _SceneAsset;         public Object SceneAsset { get { return _SceneAsset; } }
    [SerializeField]
    private string _SceneName = "";     public string SceneName { get { return _SceneName; } }
    [SerializeField]
    private string _ScenePath = ""; public string ScenePath { get { return _ScenePath; } }

    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.SceneName;
    }

    public SceneField(Object pSceneAsset, string strSceneName, string strScenePath)
    {
        _SceneAsset = pSceneAsset;
        _SceneName = strSceneName;
    }
}

#if UNITY_EDITOR

#if ODIN_INSPECTOR
public class CustomStructDrawer : OdinValueDrawer<SceneField>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        SceneField value = this.ValueEntry.SmartValue;

        var position = EditorGUILayout.GetControlRect();

        Object sceneAsset = value.SceneAsset;
        var sceneName = value.SceneName;
        // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.BeginChangeCheck();
        sceneAsset = EditorGUI.ObjectField(position, value.SceneAsset, typeof(SceneAsset), false);
        if (EditorGUI.EndChangeCheck())
        {
            if (sceneAsset != null)
            {
                var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                string strScenePath = scenePath;
                var assetsIndex = scenePath.IndexOf("Assets", System.StringComparison.Ordinal) + 7;
                var extensionIndex = scenePath.LastIndexOf(".unity", System.StringComparison.Ordinal);
                scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);

                this.ValueEntry.SmartValue = new SceneField(sceneAsset, scenePath, strScenePath);
            }
        }
    }
}

#else

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        var sceneAsset = property.FindPropertyRelative("sceneAsset");
        var sceneName = property.FindPropertyRelative("sceneName");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        if (sceneAsset != null)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                sceneAsset.objectReferenceValue = value;
                if (sceneAsset.objectReferenceValue != null)
                {
                    var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                    string strScenePath = scenePath;
                    var assetsIndex = scenePath.IndexOf("Assets", System.StringComparison.Ordinal) + 7;
                    var extensionIndex = scenePath.LastIndexOf(".unity", System.StringComparison.Ordinal);
                    scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                    sceneName.stringValue = scenePath;
                }
            }
        }
        EditorGUI.EndProperty();
    }
}

#endif
#endif