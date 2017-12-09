using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace SyndraTheRuler
{
    public static class Spells
    {
        public static Spell Q, W, E, EQ, R;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 790);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 700);
            EQ = new Spell(SpellSlot.Q, Q.Range + 500);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.Circle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.Circle);
            E.SetSkillshot(0.25f, 100, 2500f, false, SkillshotType.Line);
            EQ.SetSkillshot(0.6f, 100f, 2500f, false, SkillshotType.Line);
        }
    }
}