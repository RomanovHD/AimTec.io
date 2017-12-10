using Aimtec;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.Extensions;
using Spell = Aimtec.SDK.Spell;

namespace VayneTheRuler
{
    public static class Spells
    {
        public static Spell Q, E, R, Flash;
        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 850);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R);
            
            if (ObjectManager.GetLocalPlayer().SpellBook.GetSpell(SpellSlot.Summoner1).SpellData.Name == "SummonerFlash")
                Flash = new Spell(SpellSlot.Summoner1, 425);
            if (ObjectManager.GetLocalPlayer().SpellBook.GetSpell(SpellSlot.Summoner2).SpellData.Name == "SummonerFlash")
                Flash = new Spell(SpellSlot.Summoner2, 425);
        }
    }
}