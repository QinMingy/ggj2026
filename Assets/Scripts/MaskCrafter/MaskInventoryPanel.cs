using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskInventoryPanel : MonoBehaviour
{
    public Transform contentParent;
    public TextMeshProUGUI countText;

    private GameObject maskItemTemplate;
    private List<MaskInventoryItem> activeItems = new List<MaskInventoryItem>();

    private void Awake()
    {
        maskItemTemplate = contentParent.Find("MaskInventoryItem_Template")?.gameObject;

        if (maskItemTemplate == null)
        {
            Debug.LogError("MaskInventoryPanel: 未找到模板对象！请确保Content下有 MaskInventoryItem_Template");
        }
    }

    public void UpdateInventory(List<MaskData> masks)
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        activeItems.Clear();

        if (maskItemTemplate == null)
        {
            Debug.LogError("MaskInventoryPanel: 模板对象为空，无法更新背包");
            return;
        }

        foreach (var mask in masks)
        {
            GameObject itemObj = Instantiate(maskItemTemplate, contentParent);
            itemObj.SetActive(true);
            itemObj.name = $"MaskInventoryItem_{mask.id}";
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
