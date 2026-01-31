using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ConfigData;

public class ShopUI : UIPage
{
    [Header("UI")]
    public TextMeshProUGUI goldText;
    public Transform contentRoot;
    public Transform cartRoot;
    public ShopItemCell itemCellPrefab;
    public GameObject cartLinePrefab;

    [Header("Popup")]
    public PurchasePopup popup;

    [Header("Config")]
    public Sprite defaultIcon;
    private static readonly int[] ItemIds = { 10001, 10002, 10003 };

    [Header("Close")]
    public UnityEngine.UI.Button closeButton;
    public bool closeWithEsc = true;
    public bool useGameManagerClose = true;

    public List<Item> allItems = new List<Item>();
    private readonly Dictionary<int, CartEntry> cartEntries = new Dictionary<int, CartEntry>();

    private class CartEntry
    {
        public Item item;
        public int qty;
        public Transform viewRoot;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI qtyText;
        public TextMeshProUGUI totalText;
    }

    private void Start()
    {
        LoadItemsFromConfig();
        Refresh();
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
    }

    public override void OnPageOpen()
    {
        base.OnPageOpen();
        Refresh();
        Debug.Log("商店页面已打开");
    }

    public override void OnPageClose()
    {
        base.OnPageClose();
        if (popup != null)
            popup.Hide();
        Debug.Log("商店页面已关闭");
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
            var item = ConfigManager.Instance.GetConfig<Item>(ItemIds[i]);
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
        popup.Show(item, AddToCart);
    }

    public void CloseShop()
    {
        if (popup != null)
            popup.Hide();

        if (useGameManagerClose)
        {
            GameManager.Instance.GoBack();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void AddToCart(Item item, int qty)
    {
        if (item == null || qty <= 0) return;

        if (!cartEntries.TryGetValue(item.ID, out var entry))
        {
            if (cartRoot == null || cartLinePrefab == null)
                return;

            var line = Instantiate(cartLinePrefab, cartRoot).transform;
            entry = new CartEntry
            {
                item = item,
                qty = 0,
                viewRoot = line
            };
            ResolveCartTexts(entry);
            cartEntries[item.ID] = entry;
        }

        entry.qty += qty;
        UpdateCartLine(entry);
    }

    private void ResolveCartTexts(CartEntry entry)
    {
        if (entry == null || entry.viewRoot == null) return;

        var view = entry.viewRoot.GetComponent<CartLineView>();
        if (view != null)
        {
            entry.nameText = view.itemNameText;
            entry.qtyText = view.quantityText;
            entry.totalText = view.totalPriceText;
            return;
        }

        var tmps = entry.viewRoot.GetComponentsInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI nameText = null;
        TextMeshProUGUI qtyText = null;
        TextMeshProUGUI totalText = null;

        foreach (var tmp in tmps)
        {
            var n = tmp.name.ToLowerInvariant();
            if (nameText == null && (n.Contains("name") || n.Contains("item")))
            {
                nameText = tmp;
                continue;
            }
            if (qtyText == null && (n.Contains("qty") || n.Contains("count") || n.Contains("num")))
            {
                qtyText = tmp;
                continue;
            }
            if (totalText == null && (n.Contains("total") || n.Contains("price")))
            {
                totalText = tmp;
            }
        }

        if (tmps.Length >= 3)
        {
            if (nameText == null) nameText = tmps[0];
            if (qtyText == null) qtyText = tmps[1];
            if (totalText == null) totalText = tmps[2];
        }
        else if (tmps.Length == 2)
        {
            if (nameText == null) nameText = tmps[0];
            if (totalText == null) totalText = tmps[1];
        }

        entry.nameText = nameText;
        entry.qtyText = qtyText;
        entry.totalText = totalText;
    }

    private void UpdateCartLine(CartEntry entry)
    {
        if (entry == null || entry.item == null) return;

        if (entry.nameText != null)
            entry.nameText.text = entry.item.name;

        if (entry.qtyText != null)
            entry.qtyText.text = entry.qty.ToString();

        if (entry.totalText != null)
            entry.totalText.text = (entry.item.price * entry.qty).ToString();
    }

}

