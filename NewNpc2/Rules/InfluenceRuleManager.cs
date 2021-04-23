using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public static class InfluenceRuleManager
    {

        public static Dictionary<string,InfluenceRule> createRules()
        {
            Dictionary<string, InfluenceRule> existing = new Dictionary<string, InfluenceRule>();

            string n = "NeedToIntroduce";
            InfluenceRule r = new InfluenceRule(n); 
            r.setDel((Character c1, Character c2, intent it) => 100);
            existing.Add(n, r);

            n = "ImproveRel";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => 5 * (it == intent.Positive ? 1 : -1));
            existing.Add(n, r);

            n = "IsNice";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.personality.kind * 5);
            existing.Add(n, r);

            n = "BeingNice";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.isHappy() ? 5 : 0);
            existing.Add(n, r);

            n = "Hurtful";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.isGood() ? 0 : 10);
            existing.Add(n, r);

            n = "Gloated";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.isGloated() ? 10 : 0);
            existing.Add(n, r);

            n = "Bored";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.isBored() ? 10 : 0);
            existing.Add(n, r);

            n = "Entertain";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => it == intent.Entertain ? 10 : 0);
            existing.Add(n, r);

            n = "LikesToSpeak";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => 5);
            existing.Add(n, r);

            n = "Feared";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.isFeared() ? 10 : 0);
            existing.Add(n, r);

            n = "HasRumor";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => c1.hasRumor() ? 5 : -10);
            existing.Add(n, r);

            n = "Romantic";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => it == intent.Romantic ? 10 : 0);
            existing.Add(n, r);

            n = "IsLiked";
            r = new InfluenceRule(n);
            r.setDel((Character c1, Character c2, intent it) => {
                Feeling feeling;
                if (c1.beliefs.TryGetValue(c2, out feeling))
                    if (feeling.getIntent() == intent.Romantic)
                        return feeling.getIntensity();
                return 0;
            });
            existing.Add(n, r);

            return existing;
        }
    }
}
