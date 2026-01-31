using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    public Transform contentParent;

    private GameObject categoryTemplate;
    private GameObject materialItemTemplate;
    private Dictionary<ElementType, List<MaterialItemEntry>> materialsByType = new Dictionary<ElementType, List<MaterialItemEntry>>();
    private Dictionary<int, MaterialItem> activeItems = new Dictionary<int, MaterialItem>();
    private System.Action<MaterialData> onMaterialSelect;
    private System.Action<MaterialData> onMaterialUse;

    private class MaterialItemEntry
    {
        public MaterialData data;
        public int quantity;
    }

    private void Awake()
    {
        categoryTemplate = contentParent.Find("CategoryHeader_Template")?.gameObject;
        materialItemTemplate = contentParent.Find("MaterialItem_Template")?.gameObject;

        if (categoryTemplate == null || materialItemTemplate == null)
        {
            Debug.LogError("InventoryPanel: 未找到模板对象！请确保Content下有 CategoryHeader_Template 和 MaterialItem_Template");
        }
    }

    public void Initialize(List<MaterialData> materials, System.Action<MaterialData> selectCallback, System.Action<MaterialData> useCallback)
    {
        onMaterialSelect = selectCallback;
        onMaterialUse = useCallback;

        foreach (ElementType type in System.Enum.GetValues(typeof(ElementType)))
        {
            materialsByType[type] = new List<MaterialItemEntry>();
        }

        foreach (var material in materials)
        {
            materialsByType[material.elementType].Add(new MaterialItemEntry
            {
                data = material,
                quantity = material.initialQuantity
            });
        }

        BuildUI();
    }

    private void BuildUI()
    {
        foreach (Transform child in contentParent)
        {
            if (child.name.Contains("_Template")) continue;
            Destroy(child.gameObject);
        }
        activeItems.Clear();

        if (categoryTemplate == null || materialItemTemplate == null)
        {
            Debug.LogError("InventoryPanel: 模板对象为空，无法构建UI");
            return;
        }

        foreach (ElementType type in System.Enum.GetValues(typeof(ElementType)))
        {
            var materials = materialsByType[type];
            if (materials.Count == 0) continue;

            GameObject categoryObj = Instantiate(categoryTemplate, contentParent);
            categoryObj.SetActive(true);
            categoryObj.name = $"CategoryHeader_{type}";
            TextMeshProUGUI categoryText = categoryObj.GetComponentInChildren<TextMeshProUGUI>();
            categoryText.text = $"【{GetElementName(type)}属性材料】";

            foreach (var entry in materials)
            {
                if (entry.quantity <= 0) continue;

                GameObject itemObj = Instantiate(materialItemTemplate, contentParent);
                itemObj.SetActive(true);
                itemObj.name = $"MaterialItem_{entry.data.id}";
                MaterialItem item = itemObj.GetComponent<MaterialItem>();
                item.Init(entry.data, entry.quantity, onMaterialSelect, HandleMaterialUse);
                activeItems[entry.data.id] = item;
            }
        }
    }

    private void HandleMaterialUse(MaterialData material)
    {
        var entry = materialsByType[material.elementType].FirstOrDefault(e => e.data.id == material.id);
        if (entry != null && entry.quantity > 0)
        {
            entry.quantity--;
            MaterialInventoryManager.Instance.RemoveMaterial(material.id, 1);
            
            if (activeItems.TryGetValue(material.id, out MaterialItem item))
            {
                if (entry.quantity > 0)
                {
                    item.UpdateQuantity(entry.quantity);
                }
                else
                {
                    activeItems.Remove(material.id);
                    Destroy(item.gameObject);
                }
            }

            onMaterialUse?.Invoke(material);
        }
    }

    private string GetElementName(ElementType type)
    {
        switch (type)
        {
            case ElementType.Fire: return "火";
            case ElementType.Water: return "水";
            case ElementType.Wind: return "风";
            case ElementType.Thunder: return "雷";
            case ElementType.Earth: return "土";
            default: return "";
        }
    }
}
