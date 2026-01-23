namespace Shared.Extensions
{
    public static class SpellExtensions
    {
        public static readonly Spell[] SlayingSpells;
        public static readonly Spell[] HalfMoonSpells;

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

            HalfMoonSpells = new[]
            {
                Spell.HalfMoon,
                Spell.HalfMoon2,
                Spell.HalfMoon3,
                Spell.HalfMoon4,
                Spell.HalfMoon5,
                Spell.HalfMoon6,
                Spell.HalfMoon7,
                Spell.HalfMoon8,
                Spell.HalfMoon9,
                Spell.HalfMoon10,
                Spell.HalfMoon11,
                Spell.HalfMoon12,
                Spell.HalfMoon13,
                Spell.HalfMoon14
            };
        }

        public static bool IsSlaying(this Spell spell)
        {
            return spell >= Spell.Slaying && spell <= Spell.Slaying14;
        }

        public static bool IsHalfMoon(this Spell spell)
        {
            switch (spell)
            {
                case Spell.HalfMoon:
                case Spell.HalfMoon2:
                case Spell.HalfMoon3:
                case Spell.HalfMoon4:
                case Spell.HalfMoon5:
                case Spell.HalfMoon6:
                case Spell.HalfMoon7:
                case Spell.HalfMoon8:
                case Spell.HalfMoon9:
                case Spell.HalfMoon10:
                case Spell.HalfMoon11:
                case Spell.HalfMoon12:
                case Spell.HalfMoon13:
                case Spell.HalfMoon14:
                    return true;
                default:
                    return false;
            }
        }
    }
}
