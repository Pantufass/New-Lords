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
        private static MainCharacter _main;
        public static Dictionary<Character, Agent> characters;
        public static Dictionary<CharacterObject, Character> characters2;
        public static MainCharacter MainCharacter
        {
            get
            {
                if (_main == null)
                {
                    if(Agent.Main == null)
                        _main = new MainCharacter(Hero.MainHero);
                    else _main = new MainCharacter(Agent.Main);
                }
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
                if (agent != a)
                {
                    MainCharacter.setAgent(a);
                    characters[MainCharacter] = a;
                }
            }
            else
            {
                characters.Add(MainCharacter, a);
            }
        }

        public static Character findChar(Hero h)
        {
            if (h == Hero.MainHero) return MainCharacter;
            foreach(KeyValuePair<Character, Agent> pair in characters)
            {
                if(pair.Key.characterObject == h.CharacterObject)
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
            Character c = new Character(h);
            characters.Add(c, null);
            characters2.Add(h.CharacterObject,c);
            return c;
        }

        public static Character findChar(CharacterObject c)
        {
            if (c.IsHero && c == Hero.MainHero.CharacterObject) return MainCharacter;
            foreach (KeyValuePair<Character, Agent> pair in characters)
            {
                if (pair.Key.characterObject == c)
                {
                    return pair.Key;
                }
            }
            if (characters2.TryGetValue(c, out Character res)) return res;
            Character cha = new Character(c);
            characters.Add(cha, null);
            characters2.Add(c, cha);
            return cha;
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
            if (h == Agent.Main) return MainCharacter;
            foreach (KeyValuePair<Character,Agent> pair in characters)
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
            r.setDel((List<dynamic> d) => (d[0] as Character).hasLast() ? (d[0] as Character).getLast().Contains(d[3] as SocialInteraction) ? (d[0] as Character).getAnoy() * 4 - 4 : 0 : 0);
            ir.Add(r);


            r = new InfluenceRule("LikesThem", (List<dynamic> d) => 
            {
                if (d.Count < 2) return 0;
                Feeling f = (d[0] as Character).getStrongestFeeling(d[1] as Character);
                if (f == null) return 0;
                return f.getIntent() == intent.Romantic ? f.getIntensity() * 1.5f : f.getIntensity() / 2;
                }
            );
            ir.Add(r);

            r = new InfluenceRule("Proximity", (List<dynamic> d) => {
                if (d.Count < 2) return 0;
                if ((d[0] as Character).agent != null && (d[1] as Character).agent != null)
                {
                    float dist = (d[0] as Character).agent.Position.Distance((d[1] as Character).agent.Position);
                    if (dist < 3) dist = 3;
                    return 30 / (dist * dist);
                }
                return 0;
            });
            ir.Add(r);


            return ir;
        }

        public static List<Character> getCharacters(IReadOnlyList<Agent> agents)
        {
            List<Character> res = new List<Character>();
            foreach(Agent a in agents)
            {
                if (a.Character == null)
                    continue;
                if (a == Agent.Main)
                {
                    res.Add(MainCharacter);
                }
                else if (a.Character != null && characters2.TryGetValue((CharacterObject)a.Character, out Character c)) {
                    res.Add(c);
                    c.setAgent(a);
                    characters[c] = a;
                }
                else
                {
                    Character character = new Character(a);
                    characters.Add(character, a);
                    if(a.Character != null && !characters2.ContainsKey((CharacterObject)a.Character)) characters2.Add((CharacterObject)a.Character, character);
                    res.Add(character);
                }

            }
            return res;
        }

        internal static Character findChar(PartyBase partyBase)
        {
            throw new NotImplementedException();
        }
    }
}
