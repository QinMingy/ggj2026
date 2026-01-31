using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    public GameObject categoryPrefab;
    public GameObject materialItemPrefab;
    public Transform contentParent;

    private Dictionary<ElementType, List<MaterialItemEntry>> materialsByType = new Dictionary<ElementType, List<MaterialItemEntry>>();
    private Dictionary<int, MaterialItem> activeItems = new Dictionary<int, MaterialItem>();
    private System.Action<MaterialData> onMaterialSelect;
    private System.Action<MaterialData> onMaterialUse;

    private class MaterialItemEntry
    {
        public MaterialData data;
        public int quantity;
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
            Destroy(child.gameObject);
        }
        activeItems.Clear();

        foreach (ElementType type in System.Enum.GetValues(typeof(ElementType)))
        {
            var materials = materialsByType[type];
            if (materials.Count == 0) continue;

            GameObject categoryObj = Instantiate(categoryPrefab, contentParent);
            TextMeshProUGUI categoryText = categoryObj.GetComponentInChildren<TextMeshProUGUI>();
            categoryText.text = $"【{GetElementName(type)}属性材料】";

            foreach (var entry in materials)
            {
                if (entry.quantity <= 0) continue;

                GameObject itemObj = Instantiate(materialItemPrefab, contentParent);
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
