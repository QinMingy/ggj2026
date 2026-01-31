using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskDisplay : MonoBehaviour
{
    public Image maskImage;
    public TextMeshProUGUI fireValueText;
    public TextMeshProUGUI waterValueText;
    public TextMeshProUGUI windValueText;
    public TextMeshProUGUI thunderValueText;
    public TextMeshProUGUI earthValueText;

    private MaskAttributeData attributes = new MaskAttributeData();

    public void ApplyMaterial(MaterialData material)
    {
        attributes.AddValue(material.elementType, material.attributeValue);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        fireValueText.text = attributes.fireValue.ToString();
        waterValueText.text = attributes.waterValue.ToString();
        windValueText.text = attributes.windValue.ToString();
        thunderValueText.text = attributes.thunderValue.ToString();
        earthValueText.text = attributes.earthValue.ToString();
    }

    public MaskAttributeData GetAttributes()
    {
        return attributes;
    }

    public void ResetAttributes()
    {
        attributes = new MaskAttributeData();
        UpdateDisplay();
    }
}
