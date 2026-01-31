using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConfigData;

public class ShopItemCell : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button button;

    private ShopItem data;
    private System.Action<ShopItem> onClick;

    public void Init(ShopItem item, System.Action<ShopItem> clickCallback, Sprite fallbackIcon = null)
    {
        data = item;
        onClick = clickCallback;

        if (icon != null && fallbackIcon != null)
            icon.sprite = fallbackIcon;

        nameText.text = item.name;
        priceText.text = item.price.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
