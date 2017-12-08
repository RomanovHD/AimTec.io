using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace KaynTheRuler
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, ObjectManager.GetLocalPlayer().SpellBook.GetSpell(SpellSlot.W).SpellData.CastRange);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, W.Range - 150);

            Q.SetSkillshot(0.25f, 175, 500, false, SkillshotType.Circle);
            W.SetSkillshot(0.25f, 175, 500, false, SkillshotType.Line);
        }
    }
}