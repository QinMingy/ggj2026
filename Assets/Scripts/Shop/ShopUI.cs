using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ConfigData;

public class ShopUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI goldText;
    public Transform contentRoot;
    public ShopItemCell itemCellPrefab;

    [Header("Popup")]
    public PurchasePopup popup;

    [Header("Config")]
    public Sprite defaultIcon;

    [Header("Close")]
    public UnityEngine.UI.Button closeButton; // 右上角关闭按钮（可选）
    public bool closeWithEsc = true;          // 是否允许ESC关闭

    public List<ShopItem> allItems = new List<ShopItem>();

    private void Start()
    {
        LoadItemsFromConfig();
        Refresh();
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
    }

    private void Update()
    {
        if (closeWithEsc && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
            return;
        }
        if (goldText != null)
        {
            var player = PlayerEntity.Instance;
            if (player != null)
                goldText.text = player.GetGold().ToString();
        }
    }

    private void LoadItemsFromConfig()
    {
        allItems.Clear();
        var dict = ShopItemManager.GetAllConfigs();
        if (dict != null)
        {
            foreach (var kv in dict)
            {
                allItems.Add(kv.Value);
            }
        }

        allItems.Sort((a, b) => a.ID.CompareTo(b.ID));
    }

    public void Refresh()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        if (allItems.Count == 0)
        {
            return;
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            var cell = Instantiate(itemCellPrefab, contentRoot);
            cell.Init(allItems[i], OnItemClicked, defaultIcon);
        }

    }

    private void OnItemClicked(ShopItem item)
    {
        popup.Show(item);
    }

    public void CloseShop()
    {
        // 关闭购买弹窗（防止下次打开还残留）
        if (popup != null)
            popup.Hide();

        // 关闭整个商店面板（ShopUI挂在ShopPanel上）
        gameObject.SetActive(false);
    }

}

