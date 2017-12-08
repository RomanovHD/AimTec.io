using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace TristanaTheRuler
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static void Initialize()
        {
            var Player = ObjectManager.GetLocalPlayer();
            Q = new Spell(SpellSlot.Q, Player.AttackRange);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, Player.AttackRange);
            R = new Spell(SpellSlot.R, Player.AttackRange);
            
            W.SetSkillshot(0.25f, 125, 500, false, SkillshotType.Circle);
        }
    }
}