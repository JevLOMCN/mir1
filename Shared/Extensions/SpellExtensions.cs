namespace Shared.Extensions
{
    public static class SpellExtensions
    {
        public static readonly Spell[] SlayingSpells;

        static SpellExtensions()
        {
            int start = (int)Spell.Slaying;
            int end = (int)Spell.Slaying14;
            int count = end - start + 1;

            SlayingSpells = new Spell[count];
            for (int i = 0; i < count; i++)
            {
                SlayingSpells[i] = (Spell)(start + i);
            }
        }

        public static bool IsSlaying(this Spell spell)
        {
            return spell >= Spell.Slaying && spell <= Spell.Slaying14;
        }
    }
}
