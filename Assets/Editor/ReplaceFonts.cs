using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ReplaceFonts : EditorWindow
{
    private TMP_FontAsset newFont;

    [MenuItem("Tools/Replace All Fonts")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceFonts>("Replace Fonts");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Replace Fonts in the Current Scene", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font:", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts Now!"))
        {
            if (newFont == null)
            {
                Debug.LogError("Please drag and drop a new Font Asset into the field!");
                return;
            }

            // Find all TextMeshProUGUI components in the currently open scene
            TextMeshProUGUI[] textComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            int count = 0;

            foreach (var text in textComponents)
            {
                // Exclude project assets/prefabs that are not currently instantiated in the scene hierarchy
                if (!EditorUtility.IsPersistent(text.transform.root.gameObject))
                {
                    Undo.RecordObject(text, "Replace Font");
                    text.font = newFont;
                    EditorUtility.SetDirty(text);
                    count++;
                }
            }

            Debug.Log($"Successfully updated fonts for {count} text objects!");
        }
    }
}
