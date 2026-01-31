using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreviewPanel : MonoBehaviour
{
    public Image materialIcon;
    public TextMeshProUGUI materialNameText;
    public TextMeshProUGUI attributeInfoText;
    public GameObject panel;

    private void Start()
    {
        HidePreview();
    }

    public void ShowPreview(MaterialData material)
    {
        if (material == null)
        {
            HidePreview();
            return;
        }

        panel.SetActive(true);
        materialIcon.sprite = material.icon;
        materialNameText.text = material.materialName;
        
        string elementName = GetElementName(material.elementType);
        attributeInfoText.text = $"使用后增加:\n{elementName}属性 +{material.attributeValue}";
    }

    public void HidePreview()
    {
        panel.SetActive(false);
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
