using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public static class CharacterManager
    {
        public static Dictionary<CharacterObject, Character> characters;
        public static Character MainCharacter;

        public static void start(CulturalKnowledge ck)
        {
            //create a character for each hero
            foreach (CharacterObject h in Campaign.Current.Characters)
            {
                characters.Add(h, new Character(ck, h));
            }
        }

        public static Character findChar(CharacterObject h)
        {
            if (characters.TryGetValue(h, out Character c))
            {
                return c;
            }
            return null;
        }

        public static List<InfluenceRule> generalRules()
        {
            List<InfluenceRule> ir = new List<InfluenceRule>();


            InfluenceRule r = new InfluenceRule("Shyness", 1);
            r.setDel((Character c1, Character c2, intent it) => c1.personality.shy * -5);
            ir.Add(r);

            return ir;
        }
    }
}
