using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace ZiggsTheRuler
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 1400);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 5300);

            Q.SetSkillshot(0.25f, 180, 1700, false, SkillshotType.Circle);
            W.SetSkillshot(0.25f, 325, 1750, false, SkillshotType.Circle);
            E.SetSkillshot(1, 325, 1750, false, SkillshotType.Circle);
            R.SetSkillshot(1, 350, 1500, false, SkillshotType.Circle);
        }
    }
}