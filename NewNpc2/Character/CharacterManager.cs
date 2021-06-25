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
        private static Character _main;
        public static Dictionary<Character, Agent> characters;
        public static Dictionary<CharacterObject, Character> characters2;
        public static Character MainCharacter
        {
            get
            {
                if (_main == null) _main = new Character(Agent.Main);
                if (_main.agent == null) _main.agent = Agent.Main;
                return _main;
            }
            set
            {
                _main = value;
            }
        }
        public static Dictionary<string, Culture> cultures;


        public static void startAgents()
        {
            characters = new Dictionary<Character, Agent>();
        }

        public static void createCultures()
        {
            cultures = new Dictionary<string, Culture>();

            Culture x = CultureManager.createCultureX();
            cultures.Add(x.name, x);


        }

        public static void main(Agent a)
        {
            if (characters.TryGetValue(MainCharacter, out Agent agent))
            {
                if (agent != null && agent != a) characters.Add(MainCharacter, a);
            }
            else
            {
                characters.Add(MainCharacter, a);
            }
        }

        public static Character findChar(Hero h)
        {
            foreach(KeyValuePair<Character, Agent> pair in characters)
            {
                if(pair.Value.IsHero && pair.Value.Character == h.CharacterObject)
                {
                    return pair.Key;
                }
            }
            foreach (KeyValuePair<CharacterObject, Character> pair in characters2)
            {
                if (pair.Key.IsHero && pair.Key == h.CharacterObject)
                {
                    return pair.Value;
                }
            }
            Character c = new Character(h.Culture, h.CharacterObject);
            characters.Add(c, null);
            characters2.Add(h.CharacterObject,c);
            return c;
        }

        public static Character findChar(CharacterObject c)
        {
            return new Character(null,c);
        }

        public static void addChar(Agent a)
        {
            Character c = new Character(a);
            characters.Add(c, a);
            characters2.Add((CharacterObject)a.Character, c);
        }

        private static Character getCharacter(Agent a)
        {
            Character c = new Character(a);
            characters.Add(c, a);
            characters2.Add((CharacterObject)a.Character, c);
            return c;
        }

        public static Character findChar(Agent h)
        {
            foreach(KeyValuePair<Character,Agent> pair in characters)
            {
                if (h == pair.Value) return pair.Key;
            }

            return getCharacter(h);
        }

        public static List<InfluenceRule> generalRules()
        {
            List<InfluenceRule> ir = new List<InfluenceRule>();


            InfluenceRule r = new InfluenceRule("Shyness");
            r.setDel((List<dynamic> d) => (d[0] as Character).getShy() * -3);
            ir.Add(r);

            r = new InfluenceRule("Repetition");
            r.setDel((List<dynamic> d) => (d[0] as Character).getLast().Contains(d[3] as SocialInteraction) ? (d[0] as Character).getAnoy() * 3 - 3 : 0);
            ir.Add(r);

            return ir;
        }

        public static List<Character> getCharacters(IReadOnlyList<Agent> agents)
        {
            List<Character> res = new List<Character>();
            foreach(Agent a in agents)
            {
                bool found = false;
                foreach (KeyValuePair<Character,Agent> pair in characters)
                {
                    if (pair.Value == a)
                    {
                        found = true;
                        res.Add(pair.Key);
                        break;
                    }
                }
                if (!found)
                {
                    Character c = new Character(a);
                    characters.Add(c, a);
                    characters2.Add((CharacterObject)a.Character, c);
                    res.Add(c);
                }

            }
            return res;
        }
    }
}
