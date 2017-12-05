using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace ChemtechUrgot
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 490);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1600);

            Q.SetSkillshot(0.25f, 150, 1700, false, SkillshotType.Circle);
            E.SetSkillshot(0.25f, 160, 1000, false, SkillshotType.Line);
            R.SetSkillshot(0.25f, 90, 3200, false, SkillshotType.Line);
        }
    }
}