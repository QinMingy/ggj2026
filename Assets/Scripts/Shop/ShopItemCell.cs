using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCell : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button button;

    private ShopItemData data;
    private System.Action<ShopItemData> onClick;

    public void Init(ShopItemData item, System.Action<ShopItemData> clickCallback)
    {
        data = item;
        onClick = clickCallback;

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
