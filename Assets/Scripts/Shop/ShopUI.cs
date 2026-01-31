using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI pageText;
    public Transform contentRoot;
    public ShopItemCell itemCellPrefab;

    [Header("Popup")]
    public PurchasePopup popup;

    [Header("Config")]
    public int itemsPerPage = 16;

    [Header("Close")]
    public UnityEngine.UI.Button closeButton; // 右上角关闭按钮（可选）
    public bool closeWithEsc = true;          // 是否允许ESC关闭

    public List<ShopItemData> allItems;

    private int currentPage = 0;

    private void Start()
    {
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
        goldText.text = PlayerEntity.Instance.Gold.ToString();
    }

    public void Refresh()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        int start = currentPage * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, allItems.Count);

        for (int i = start; i < end; i++)
        {
            var cell = Instantiate(itemCellPrefab, contentRoot);
            cell.Init(allItems[i], OnItemClicked);
        }

        pageText.text = $"{currentPage + 1} / {Mathf.CeilToInt((float)allItems.Count / itemsPerPage)}";
    }

    public void NextPage()
    {
        if ((currentPage + 1) * itemsPerPage >= allItems.Count) return;
        currentPage++;
        Refresh();
    }

    public void PrevPage()
    {
        if (currentPage <= 0) return;
        currentPage--;
        Refresh();
    }

    private void OnItemClicked(ShopItemData item)
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
