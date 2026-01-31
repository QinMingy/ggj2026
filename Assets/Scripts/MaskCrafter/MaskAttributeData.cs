using System;
using UnityEngine;

[Serializable]
public class MaskAttributeData
{
    public int fireValue;
    public int waterValue;
    public int windValue;
    public int thunderValue;
    public int earthValue;

    public int GetValue(ElementType type)
    {
        switch (type)
        {
            case ElementType.Fire: return fireValue;
            case ElementType.Water: return waterValue;
            case ElementType.Wind: return windValue;
            case ElementType.Thunder: return thunderValue;
            case ElementType.Earth: return earthValue;
            default: return 0;
        }
    }

    public void AddValue(ElementType type, int value)
    {
        switch (type)
        {
            case ElementType.Fire: fireValue += value; break;
            case ElementType.Water: waterValue += value; break;
            case ElementType.Wind: windValue += value; break;
            case ElementType.Thunder: thunderValue += value; break;
            case ElementType.Earth: earthValue += value; break;
        }
    }
}
