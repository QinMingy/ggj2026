using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MaterialItem : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI attributeText;

    private MaterialData data;
    private int currentQuantity;
    private System.Action<MaterialData> onSelect;
    private System.Action<MaterialData> onUse;

    public void Init(MaterialData material, int quantity, System.Action<MaterialData> selectCallback, System.Action<MaterialData> useCallback)
    {
        data = material;
        currentQuantity = quantity;
        onSelect = selectCallback;
        onUse = useCallback;

        icon.sprite = material.icon;
        nameText.text = material.materialName;
        quantityText.text = $"x{currentQuantity}";
        attributeText.text = $"+{material.attributeValue} {GetElementName(material.elementType)}";
    }

    public void UpdateQuantity(int quantity)
    {
        currentQuantity = quantity;
        quantityText.text = $"x{currentQuantity}";
    }

    public int GetQuantity() => currentQuantity;

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelect?.Invoke(data);
    }

    public void UseMaterial()
    {
        onUse?.Invoke(data);
    }

    private string GetElementName(ElementType type)
    {
        switch (type)
        {
            case ElementType.Fire: return "火";
            case ElementType.Water: return "水";
            case ElementType.Wind: return "风";
            case ElementType.Thunder: return "雷";
            case ElementType.Earth: return "土";
            default: return "";
        }
    }
}
