using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ConfigData;

public class ShopUI : UIPage
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
    public UnityEngine.UI.Button closeButton;
    public bool closeWithEsc = true;
    public bool useGameManagerClose = true;

    public List<Item> allItems = new List<Item>();

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
        popup.Show(item);
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

}

