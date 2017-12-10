using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace VayneTheRuler
{
    public static class Spells
    {
        public static Spell Q, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 850);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R);
        }
    }
}