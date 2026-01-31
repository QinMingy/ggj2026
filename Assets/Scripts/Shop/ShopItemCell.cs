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

    private Item data;
    private System.Action<Item> onClick;

    public void Init(Item item, System.Action<Item> clickCallback, Sprite fallbackIcon = null)
    {
        data = item;
        onClick = clickCallback;

        if (icon != null)
        {
            Sprite resolved = null;
            if (!string.IsNullOrWhiteSpace(item.icon))
                resolved = Resources.Load<Sprite>(item.icon);

            icon.sprite = resolved != null ? resolved : fallbackIcon;
        }

        nameText.text = item.name;
        priceText.text = item.price.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(data));
    }
}
