using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskInventoryItem : MonoBehaviour
{
    public Image maskIcon;
    public TextMeshProUGUI maskNameText;
    public TextMeshProUGUI attributesText;
    public TextMeshProUGUI createTimeText;

    private MaskData maskData;

    public void Init(MaskData data)
    {
        maskData = data;
        
        maskNameText.text = data.maskName;
        attributesText.text = data.GetAttributesSummary();
        createTimeText.text = data.createTime.ToString("MM/dd HH:mm");

        if (data.icon != null)
        {
            maskIcon.sprite = data.icon;
        }
        else
        {
            maskIcon.color = new Color(0.8f, 0.8f, 0.9f, 1f);
        }
    }

    public MaskData GetMaskData()
    {
        return maskData;
    }
}
