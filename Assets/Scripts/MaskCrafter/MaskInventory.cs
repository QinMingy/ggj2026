using System.Collections.Generic;
using UnityEngine;

public class MaskInventory : MonoBehaviour
{
    private List<MaskData> masks = new List<MaskData>();
    private int nextMaskId = 1;

    public System.Action<MaskData> OnMaskAdded;
    public System.Action OnInventoryChanged;

    public void AddMask(MaskData mask)
    {
        masks.Add(mask);
        OnMaskAdded?.Invoke(mask);
        OnInventoryChanged?.Invoke();
        Debug.Log($"面具已添加到背包: {mask.maskName} - {mask.GetAttributesSummary()}");
    }

    public MaskData CreateMask(MaskAttributeData attributes)
    {
        MaskData newMask = new MaskData(nextMaskId++, attributes);
        AddMask(newMask);
        return newMask;
    }

    public List<MaskData> GetAllMasks()
    {
        return new List<MaskData>(masks);
    }

    public void RemoveMask(int maskId)
    {
        masks.RemoveAll(m => m.id == maskId);
        OnInventoryChanged?.Invoke();
    }

    public int GetMaskCount()
    {
        return masks.Count;
    }
}
