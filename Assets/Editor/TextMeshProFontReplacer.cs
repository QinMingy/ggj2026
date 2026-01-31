using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class TextMeshProFontReplacer : EditorWindow
{
    private string prefabFolderPath = "Assets/";
    private TMP_FontAsset targetFont;
    private Vector2 scrollPosition;
    private List<string> foundPrefabs = new List<string>();
    private int totalTextMeshProComponents = 0;
    private bool showResults = false;

    [MenuItem("Tools/TextMeshPro Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<TextMeshProFontReplacer>("TMP Font Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("TextMeshPro Font Replacer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefab Folder Path:", EditorStyles.label);
        EditorGUILayout.BeginHorizontal();
        prefabFolderPath = EditorGUILayout.TextField(prefabFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Prefab Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    prefabFolderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please select a folder within the Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Target Font:", targetFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();

        GUI.enabled = targetFont != null && !string.IsNullOrEmpty(prefabFolderPath);
        if (GUILayout.Button("Replace Fonts in All Prefabs", GUILayout.Height(30)))
        {
            ReplaceFontsInPrefabs();
        }
        GUI.enabled = true;

        if (showResults)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Found {foundPrefabs.Count} prefabs with {totalTextMeshProComponents} TextMeshPro components", EditorStyles.helpBox);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Processed Prefabs:", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            foreach (string prefabPath in foundPrefabs)
            {
                EditorGUILayout.LabelField(prefabPath);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void ReplaceFontsInPrefabs()
    {
        if (targetFont == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a target font.", "OK");
            return;
        }

        if (!Directory.Exists(prefabFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "The specified folder path does not exist.", "OK");
            return;
        }

        foundPrefabs.Clear();
        totalTextMeshProComponents = 0;

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });
        
        if (prefabGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Prefabs Found", "No prefabs were found in the specified folder.", "OK");
            showResults = false;
            return;
        }

        int processedCount = 0;
        int modifiedCount = 0;

        try
        {
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                
                if (EditorUtility.DisplayCancelableProgressBar(
                    "Replacing Fonts",
                    $"Processing: {prefabPath} ({i + 1}/{prefabGuids.Length})",
                    (float)i / prefabGuids.Length))
                {
                    break;
                }

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null) continue;

                bool prefabModified = false;
                int componentCount = 0;

                TextMeshProUGUI[] tmpUGUIComponents = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI tmp in tmpUGUIComponents)
                {
                    if (tmp.font != targetFont)
                    {
                        tmp.font = targetFont;
                        prefabModified = true;
                        componentCount++;
                    }
                }

                TextMeshPro[] tmpComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);
                foreach (TextMeshPro tmp in tmpComponents)
                {
                    if (tmp.font != targetFont)
                    {
                        tmp.font = targetFont;
                        prefabModified = true;
                        componentCount++;
                    }
                }

                if (prefabModified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    foundPrefabs.Add(prefabPath);
                    totalTextMeshProComponents += componentCount;
                    modifiedCount++;
                }

                processedCount++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        showResults = true;
        
        EditorUtility.DisplayDialog(
            "Font Replacement Complete",
            $"Processed {processedCount} prefabs.\n" +
            $"Modified {modifiedCount} prefabs.\n" +
            $"Replaced fonts in {totalTextMeshProComponents} TextMeshPro components.",
            "OK");
    }
}
