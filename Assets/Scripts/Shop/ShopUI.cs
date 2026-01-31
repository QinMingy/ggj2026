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
    private static readonly int[] ItemIds = { 10001, 10002, 10003 };

    [Header("Close")]
    public UnityEngine.UI.Button closeButton; // 右上角关闭按钮（可选）
    public bool closeWithEsc = true;          // 是否允许ESC关闭

    public List<Item> allItems = new List<Item>();

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
        for (int i = 0; i < ItemIds.Length; i++)
        {
            var item = ItemManager.GetConfig(ItemIds[i]);
            if (item != null)
                allItems.Add(item);
        }
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

    private void OnItemClicked(Item item)
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

