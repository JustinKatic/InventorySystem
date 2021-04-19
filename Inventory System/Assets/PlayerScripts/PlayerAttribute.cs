[System.Serializable]
public class PlayerAttribute
{
    [System.NonSerialized]
    public Player parent;
    public AttributeType type;
    public ModifableInt Value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        Value = new ModifableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
