using UnityEngine;

[CreateAssetMenu(menuName = "MaskCrafter/Material")]
public class MaterialData : ScriptableObject
{
    public int id;
    public string materialName;
    [TextArea]
    public string description;
    public Sprite icon;
    public ElementType elementType;
    public int attributeValue;
    public int initialQuantity = 1;
}
