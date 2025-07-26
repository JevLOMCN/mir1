using Shared;

public sealed class WarriorAI : BaseAI
{
    public WarriorAI(GameClient client) : base(client) { }

    protected override IReadOnlyList<DesiredItem> DesiredItems { get; } = new DesiredItem[]
    {
        new DesiredItem(ItemType.Potion, hpPotion: true, weightFraction: 0.40),
        new DesiredItem(ItemType.Potion, hpPotion: false, weightFraction: 0.20),
        new DesiredItem(ItemType.Scroll, shape: 1, count: 1)
    };

    protected override int GetItemScore(UserItem item, EquipmentSlot slot)
    {
        if (item.Info == null) return 0;

        bool offensive = IsOffensiveSlot(slot);

        if (offensive)
        {
            return item.Info.Stats[Stat.MinDC] + item.Info.Stats[Stat.MaxDC]
                 + item.AddedStats[Stat.MinDC] + item.AddedStats[Stat.MaxDC]
                 + item.Info.Stats[Stat.AttackSpeed] + item.AddedStats[Stat.AttackSpeed]
                 + item.Info.Stats[Stat.Accuracy] + item.AddedStats[Stat.Accuracy]
                 + item.Info.Stats[Stat.Agility] + item.AddedStats[Stat.Agility];
        }

        return item.Info.Stats[Stat.MinAC] + item.Info.Stats[Stat.MaxAC]
             + item.AddedStats[Stat.MinAC] + item.AddedStats[Stat.MaxAC]
             + item.Info.Stats[Stat.Accuracy] + item.AddedStats[Stat.Accuracy]
             + item.Info.Stats[Stat.Agility] + item.AddedStats[Stat.Agility];
    }
}
