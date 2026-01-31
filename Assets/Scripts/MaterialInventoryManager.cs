using System.Collections.Generic;
using UnityEngine;
using ConfigData;

public class MaterialInventoryManager : MonoSingleton<MaterialInventoryManager>
{
    private Dictionary<int, int> materials = new Dictionary<int, int>();
    public System.Action OnInventoryChanged;

    private static readonly int[] InitialItemIds = { 10001, 20001, 30001, 40001, 50001 };
    private const int InitialItemQuantity = 1;

    protected override void Awake()
    {
        base.Awake();
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        materials.Clear();
        
        foreach (int itemId in InitialItemIds)
        {
            materials[itemId] = InitialItemQuantity;
        }

        Debug.Log($"MaterialInventoryManager: 初始化背包，添加了 {InitialItemIds.Length} 种材料");
        OnInventoryChanged?.Invoke();
    }

    public void AddMaterial(int itemId, int quantity)
    {
        if (quantity <= 0) return;

        if (materials.ContainsKey(itemId))
        {
            materials[itemId] += quantity;
        }
        else
        {
            materials[itemId] = quantity;
        }

        Debug.Log($"MaterialInventoryManager: 添加材料 {itemId} x{quantity}，当前数量: {materials[itemId]}");
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveMaterial(int itemId, int quantity)
    {
        if (quantity <= 0) return false;

        if (!materials.ContainsKey(itemId) || materials[itemId] < quantity)
        {
            Debug.LogWarning($"MaterialInventoryManager: 材料 {itemId} 数量不足");
            return false;
        }

        materials[itemId] -= quantity;
        
        if (materials[itemId] <= 0)
        {
            materials.Remove(itemId);
        }

        Debug.Log($"MaterialInventoryManager: 移除材料 {itemId} x{quantity}");
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetMaterialQuantity(int itemId)
    {
        return materials.TryGetValue(itemId, out int quantity) ? quantity : 0;
    }

    public Dictionary<int, int> GetAllMaterials()
    {
        return new Dictionary<int, int>(materials);
    }

    public bool HasMaterial(int itemId, int quantity = 1)
    {
        return materials.ContainsKey(itemId) && materials[itemId] >= quantity;
    }

    public List<MaterialData> GetMaterialDataList()
    {
        List<MaterialData> materialDataList = new List<MaterialData>();

        foreach (var kvp in materials)
        {
            Item itemConfig = ItemManager.GetConfig(kvp.Key);
            if (itemConfig == null)
            {
                Debug.LogWarning($"MaterialInventoryManager: 未找到物品配置 ID={kvp.Key}");
                continue;
            }

            MaterialData materialData = ScriptableObject.CreateInstance<MaterialData>();
            materialData.id = itemConfig.idValue;
            materialData.materialName = itemConfig.name;
            materialData.description = itemConfig.desc;
            materialData.elementType = GetElementTypeFromAttribute(itemConfig.attribute);
            materialData.attributeValue = 10;
            materialData.initialQuantity = kvp.Value;
            
            if (!string.IsNullOrEmpty(itemConfig.icon))
            {
                materialData.icon = ResourceManager.Instance.LoadSprite(itemConfig.icon);
            }

            materialDataList.Add(materialData);
        }

        return materialDataList;
    }

    private ElementType GetElementTypeFromAttribute(int attribute)
    {
        switch (attribute)
        {
            case 1: return ElementType.Fire;
            case 2: return ElementType.Wind;
            case 3: return ElementType.Water;
            case 4: return ElementType.Thunder;
            case 5: return ElementType.Earth;
            default: return ElementType.Fire;
        }
    }

    public void ClearInventory()
    {
        materials.Clear();
        Debug.Log("MaterialInventoryManager: 清空背包");
        OnInventoryChanged?.Invoke();
    }

    public void ResetToInitial()
    {
        InitializeInventory();
    }
}
