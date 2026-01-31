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

    private void Awake()
    {
        gameObject.SetActive(false);

        quantitySlider.onValueChanged.AddListener(OnQuantityChanged);
        cancelButton.onClick.AddListener(Hide);
        confirmButton.onClick.AddListener(Confirm);
    }

    public void Show(Item item)
    {
        currentItem = item;

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
        int cost = currentItem.price * qty;

        if (PlayerEntity.Instance == null)
        {
            Debug.LogError("PlayerEntity.Instance 为空！");
            return;
        }

        if (MaterialInventoryManager.Instance == null)
        {
            Debug.LogError("MaterialInventoryManager.Instance 为空！");
            return;
        }

        if (PlayerEntity.Instance.SpendGold(cost))
        {
            MaterialInventoryManager.Instance.AddMaterial(currentItem.idValue, qty);
            Debug.Log($"购买 {currentItem.name} x{qty}，已添加到背包");
            Hide();
        }
        else
        {
            Debug.Log("金币不足");
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

