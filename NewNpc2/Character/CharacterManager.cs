using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public static class CharacterManager
    {
        public static Dictionary<Agent, Character> characters2;
        public static Dictionary<CharacterObject, Character> characters;
        public static Character MainCharacter;
        public static Dictionary<string, Culture> cultures;

        public static void start(Culture ck)
        {
            //create a character for each hero
            foreach (CharacterObject h in Campaign.Current.Characters)
            {
                characters.Add(h, new Character(ck, h));
            }
        }

        public static void startAgents()
        {
            characters2 = new Dictionary<Agent, Character>();
        }

        public static void createCultures()
        {
            cultures = new Dictionary<string, Culture>();

            Culture x = CultureManager.createCultureX();
            cultures.Add(x.name, x);


        }

        public static Character findChar(CharacterObject h)
        {
            if (characters.TryGetValue(h, out Character c))
            {
                return c;
            }
            Culture x;
            cultures.TryGetValue("x",out x);
            Character character;
                if (h.IsHero)
                {
                    character = new Character(x, h.HeroObject.GetHeroTraits(), h);
                    CharacterManager.characters.Add(h, character);
                }
                else
                {
                    character = new Character(x, h);
                    CharacterManager.characters.Add(h, character);
                }
            
            return character;
        }

        public static List<InfluenceRule> generalRules()
        {
            List<InfluenceRule> ir = new List<InfluenceRule>();


            InfluenceRule r = new InfluenceRule("Shyness");
            r.setDel((List<dynamic> d) => (d[0] as Character).getShy() * -5);
            ir.Add(r);

            return ir;
        }

        public static List<Character> getCharacters(IReadOnlyList<Agent> agents)
        {
            List<Character> res = new List<Character>();
            foreach(Agent a in agents)
            {
                Character c;
                if (characters2.TryGetValue(a, out c)) res.Add(c);
                else
                {
                    c = new Character(a);
                    if (a.Character is CharacterObject) c.setCharacterObject((CharacterObject) a.Character);

                    characters2.Add(a, c);
                    res.Add(c);
                }
            }
            return res;
        }
    }
}
