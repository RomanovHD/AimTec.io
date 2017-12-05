using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace FrostAshe
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 1200);
            R = new Spell(SpellSlot.R, float.MaxValue);

            W.SetSkillshot(0.25f, 57.5f, 2000, true, SkillshotType.Cone);
            R.SetSkillshot(0.25f, 90, 1600, false, SkillshotType.Line);
        }
    }
}