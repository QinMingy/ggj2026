using UnityEngine;
using UnityEditor;

public class CreateTestMaterials : EditorWindow
{
    [MenuItem("Tools/Create Test Materials")]
    public static void CreateMaterials()
    {
        string folderPath = "Assets/Resources/Materials";
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Materials");
        }

        CreateMaterial(folderPath, 1, "竹草", ElementType.Fire, 10, 3);
        CreateMaterial(folderPath, 2, "火焰石", ElementType.Fire, 15, 2);
        CreateMaterial(folderPath, 3, "福祉香", ElementType.Fire, 8, 5);
        
        CreateMaterial(folderPath, 4, "甘露", ElementType.Water, 12, 4);
        CreateMaterial(folderPath, 5, "清泉水", ElementType.Water, 10, 3);
        CreateMaterial(folderPath, 6, "冰晶", ElementType.Water, 18, 2);
        
        CreateMaterial(folderPath, 7, "风羽", ElementType.Wind, 9, 4);
        CreateMaterial(folderPath, 8, "疾风草", ElementType.Wind, 11, 3);
        
        CreateMaterial(folderPath, 9, "雷石", ElementType.Thunder, 14, 2);
        CreateMaterial(folderPath, 10, "电光花", ElementType.Thunder, 10, 3);
        
        CreateMaterial(folderPath, 11, "土灵石", ElementType.Earth, 13, 3);
        CreateMaterial(folderPath, 12, "大地之心", ElementType.Earth, 20, 1);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"已创建12个测试材料数据到 {folderPath}");
    }

    private static void CreateMaterial(string folderPath, int id, string name, ElementType type, int value, int quantity)
    {
        MaterialData material = ScriptableObject.CreateInstance<MaterialData>();
        material.id = id;
        material.materialName = name;
        material.description = $"这是{name}的描述";
        material.elementType = type;
        material.attributeValue = value;
        material.initialQuantity = quantity;

        Texture2D texture = new Texture2D(64, 64);
        Color color = GetElementColor(type);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                float alpha = distance < 30 ? 1f : 0f;
                texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
            }
        }
        texture.Apply();

        string iconPath = $"{folderPath}/{name}_Icon.png";
        System.IO.File.WriteAllBytes(iconPath, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(iconPath);
        
        TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }

        material.icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);

        string assetPath = $"{folderPath}/{name}.asset";
        AssetDatabase.CreateAsset(material, assetPath);
    }

    private static Color GetElementColor(ElementType type)
    {
        switch (type)
        {
            case ElementType.Fire: return new Color(1f, 0.3f, 0.2f);
            case ElementType.Water: return new Color(0.2f, 0.5f, 1f);
            case ElementType.Wind: return new Color(0.6f, 1f, 0.6f);
            case ElementType.Thunder: return new Color(0.9f, 0.8f, 0.2f);
            case ElementType.Earth: return new Color(0.7f, 0.5f, 0.3f);
            default: return Color.white;
        }
    }
}
