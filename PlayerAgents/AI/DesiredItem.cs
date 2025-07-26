using Shared;

public sealed class DesiredItem
{
    public ItemType Type { get; init; }
    public bool? HpPotion { get; init; }
    public short? Shape { get; init; }
    public int? Count { get; init; }
    public double WeightFraction { get; init; }

    public DesiredItem(ItemType type, bool? hpPotion = null, short? shape = null, int? count = null, double weightFraction = 0)
    {
        Type = type;
        HpPotion = hpPotion;
        Shape = shape;
        Count = count;
        WeightFraction = weightFraction;
    }
}
