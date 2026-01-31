using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConfigData;

public class PurchasePopup : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI quantityText;

    public Slider quantitySlider;
    public Button confirmButton;
    public Button cancelButton;

    private Item currentItem;
    private System.Action<Item, int> onConfirm;

    private void Awake()
    {
        gameObject.SetActive(false);

        quantitySlider.onValueChanged.AddListener(OnQuantityChanged);
        cancelButton.onClick.AddListener(Hide);
        confirmButton.onClick.AddListener(Confirm);
    }

    public void Show(Item item, System.Action<Item, int> confirmCallback)
    {
        currentItem = item;
        onConfirm = confirmCallback;

        itemNameText.text = item.name;
        descText.text = item.desc;

        quantitySlider.minValue = 1;
        quantitySlider.maxValue = 99;
        quantitySlider.value = 1;

        UpdatePrice();

        gameObject.SetActive(true);
    }

    private void OnQuantityChanged(float value)
    {
        quantityText.text = ((int)value).ToString();
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        int qty = (int)quantitySlider.value;
        priceText.text = (currentItem.price * qty).ToString();
    }

    private void Confirm()
    {
        int qty = (int)quantitySlider.value;
        onConfirm?.Invoke(currentItem, qty);
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

