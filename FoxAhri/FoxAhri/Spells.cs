using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace FoxAhri
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 880);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 975);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.25f, 80, 1700, false, SkillshotType.Line);
            E.SetSkillshot(0.25f, 90, 1600, true, SkillshotType.Line);
        }
    }
}