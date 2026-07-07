#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public static class PlayFromFirstScene
{
    static PlayFromFirstScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    EditorBuildSettings.scenes[0].path
                );
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Reset về null để không ảnh hưởng khi build
            EditorSceneManager.playModeStartScene = null;
        }
    }
}
#endif