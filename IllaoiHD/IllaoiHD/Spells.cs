using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace IllaoiHD
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.25f, 80, 1600, false, SkillshotType.Line);
            E.SetSkillshot(0.25f, 90, 1500, true, SkillshotType.Line);
        }
    }
}