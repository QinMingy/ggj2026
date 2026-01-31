using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class AssignTestMaterialsToUI : EditorWindow
{
    [MenuItem("Tools/Assign Test Materials to MaskCrafter UI")]
    public static void AssignMaterials()
    {
        MaskCrafterUI ui = FindObjectOfType<MaskCrafterUI>();
        if (ui == null)
        {
            Debug.LogError("场景中未找到 MaskCrafterUI 组件！请先使用 Tools > Build MaskCrafter UI 创建UI。");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:MaterialData", new[] { "Assets/Resources/Materials" });
        if (guids.Length == 0)
        {
            Debug.LogError("未找到任何 MaterialData 资源！请先使用 Tools > Create Test Materials 创建测试数据。");
            return;
        }

        List<MaterialData> materials = new List<MaterialData>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MaterialData material = AssetDatabase.LoadAssetAtPath<MaterialData>(path);
            if (material != null)
            {
                materials.Add(material);
            }
        }

        materials = materials.OrderBy(m => m.elementType).ThenBy(m => m.id).ToList();

        SerializedObject serializedUI = new SerializedObject(ui);
        SerializedProperty testMaterialsProp = serializedUI.FindProperty("testMaterials");
        
        testMaterialsProp.ClearArray();
        for (int i = 0; i < materials.Count; i++)
        {
            testMaterialsProp.InsertArrayElementAtIndex(i);
            testMaterialsProp.GetArrayElementAtIndex(i).objectReferenceValue = materials[i];
        }
        
        serializedUI.ApplyModifiedProperties();
        EditorUtility.SetDirty(ui);

        Debug.Log($"已成功将 {materials.Count} 个材料数据分配给 MaskCrafterUI！");
    }
}
