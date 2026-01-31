using System;
using UnityEngine;

[Serializable]
public class MaskData
{
    public int id;
    public string maskName;
    public DateTime createTime;
    public MaskAttributeData attributes;
    public Sprite icon;

    public MaskData(int id, MaskAttributeData attributes)
    {
        this.id = id;
        this.maskName = $"面具_{id}";
        this.createTime = DateTime.Now;
        this.attributes = new MaskAttributeData
        {
            fireValue = attributes.fireValue,
            waterValue = attributes.waterValue,
            windValue = attributes.windValue,
            thunderValue = attributes.thunderValue,
            earthValue = attributes.earthValue
        };
    }

    public string GetAttributesSummary()
    {
        return $"火:{attributes.fireValue} 水:{attributes.waterValue} 风:{attributes.windValue} 雷:{attributes.thunderValue} 土:{attributes.earthValue}";
    }
}
