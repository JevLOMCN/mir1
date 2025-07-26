using Shared;

public sealed class WizardAI : BaseAI
{
    public WizardAI(GameClient client) : base(client) { }

    protected override IReadOnlyList<DesiredItem> DesiredItems { get; } = new DesiredItem[]
    {
        new DesiredItem(ItemType.Potion, hpPotion: true, weightFraction: 0.10),
        new DesiredItem(ItemType.Potion, hpPotion: false, weightFraction: 0.50),
        new DesiredItem(ItemType.Scroll, shape: 1, count: 1)
    };

    protected override int GetItemScore(UserItem item, EquipmentSlot slot)
    {
        if (item.Info == null) return 0;

        bool offensive = IsOffensiveSlot(slot);

        if (offensive)
        {
            return item.Info.Stats[Stat.MinMC] + item.Info.Stats[Stat.MaxMC]
                 + item.AddedStats[Stat.MinMC] + item.AddedStats[Stat.MaxMC];
        }

        return item.Info.Stats[Stat.MinMAC] + item.Info.Stats[Stat.MaxMAC]
             + item.AddedStats[Stat.MinMAC] + item.AddedStats[Stat.MaxMAC];
    }
}
