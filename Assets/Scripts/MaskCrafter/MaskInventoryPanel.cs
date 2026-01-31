using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskInventoryPanel : MonoBehaviour
{
    public GameObject maskItemPrefab;
    public Transform contentParent;
    public TextMeshProUGUI countText;

    private List<MaskInventoryItem> activeItems = new List<MaskInventoryItem>();

    public void UpdateInventory(List<MaskData> masks)
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        activeItems.Clear();

        foreach (var mask in masks)
        {
            GameObject itemObj = Instantiate(maskItemPrefab, contentParent);
            MaskInventoryItem item = itemObj.GetComponent<MaskInventoryItem>();
            item.Init(mask);
            activeItems.Add(item);
        }

        if (countText != null)
        {
            countText.text = $"已创建面具: {masks.Count}";
        }
    }
}
