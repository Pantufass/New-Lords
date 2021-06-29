using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public static class TriggerRuleManager
    {
        private static TriggerRule RomanticExchange()
        {
            TriggerRule t = new TriggerRule("RomanticExchange", (List<dynamic> d) =>
            {
                if (d.Count < 4) return;

                Character c1 = d[0] as Character;
                Character c2 = d[1] as Character;
                intent? i = d[2] as intent?;
                outcome? o = d[3] as outcome?;

                if (o == outcome.Positive)
                {
                    c1.raiseRomantic(c2);
                    c2.raiseRomantic(c1);
                }
                else if (o == outcome.Negative)
                {
                    c1.lowerRomantic(c2);
                    c2.lowerRomantic(c1,0.5f);
                }
                else
                {
                    c1.lowerRomantic(c2, 0.5f);
                }

            });
            Condition c;
            SubModule.existingConditions.TryGetValue("IsRomanceExchange", out c);
            t.addCondition(c);
            return t;
        }

        private static TriggerRule FriendlyExchange()
        {
            TriggerRule t = new TriggerRule("FriendlyExchange", (List<dynamic> d) =>
            {
                if (d.Count < 4) return;

                Character c1 = d[0] as Character;
                Character c2 = d[1] as Character;
                intent? i = d[2] as intent?;
                outcome? o = d[3] as outcome?;

                if(i == intent.Positive)
                {
                    if (o == outcome.Positive)
                    {
                        c1.raiseFriendly(c2);
                        c2.raiseFriendly(c1);
                    }
                    else if (o == outcome.Negative)
                    {
                        c1.lowerFriendly(c2);
                        c2.lowerFriendly(c1, 0.5f);
                    }
                    
                }
                if(i == intent.Negative)
                {
                    if (o == outcome.Negative)
                    {
                        c2.lowerFriendly(c1);
                    }
                    else c1.lowerFriendly(c2, 0.5f);
                }

               

            });
            return t;
        }

        private static TriggerRule UnBored()
        {
            TriggerRule t = new TriggerRule("UnBored", (List<dynamic> d) =>
            {
                if (d.Count < 2) return;

                Character c1 = d[0] as Character;
                Character c2 = d[1] as Character;

                //TODO check for entertain exchanges
                c1.notBored();
                c2.notBored();
            });

            Condition c;
            SubModule.existingConditions.TryGetValue("IsBored", out c);
            t.addCondition(c);

            return t;
        }

        private static TriggerRule NoIntroduction()
        {
            TriggerRule t = new TriggerRule("NoIntroduction", (List<dynamic> d) =>
            {
                if (d.Count < 4) return;

                Character c1 = d[0] as Character;
                Character c2 = d[1] as Character;

                if (Introduction.Introduced(c1.characterObject, c2.characterObject) < 2)
                {
                    Introduction.Introduce(c1.characterObject, c2.characterObject); //introduced
                    Introduction.Introduce(c1.characterObject, c2.characterObject); //not introduced

                    float value = -2 + (-3 * c2.getSensitive());
                    c2.raiseFriendly(c1, value);
                }
                   
            });

            Condition c;
            SubModule.existingConditions.TryGetValue("NotIntroduced", out c);
            t.addCondition(c);

            return t;
        }

        public static List<TriggerRule> createRules()
        {
            List<TriggerRule> list = new List<TriggerRule>();
            
            list.Add(RomanticExchange());
            list.Add(FriendlyExchange());

            list.Add(NoIntroduction());

            list.Add(UnBored());

            return list;
        }
    }
}
