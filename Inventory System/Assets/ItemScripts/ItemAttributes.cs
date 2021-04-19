using UnityEngine;

[System.Serializable]
public class ItemAttributes : IModifiers
{
    public AttributeType attributeType;
    private int value;

    [Header("RandomAttributeRange")]
    public int minAttributeRange;
    public int maxAttributeRange;
    public ItemAttributes(int _min, int _max)
    {
        minAttributeRange = _min;
        maxAttributeRange = _max;
        GenerateValue();
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = Random.Range(minAttributeRange, maxAttributeRange);
    }
}
