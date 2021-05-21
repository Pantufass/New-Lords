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
            r.setDel((List<dynamic> d) => 100);
            existing.Add(n, r);

            n = "ImproveRel";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => 5 * (d[2] as intent? == intent.Positive ? 1 : -1));
            existing.Add(n, r);

            n = "IsNice";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getKind() * 5);
            existing.Add(n, r);

            n = "BeingNice";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isHappy() ? 5 : 0);
            existing.Add(n, r);

            n = "Hurtful";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isGood() ? 0 : 10);
            existing.Add(n, r);

            n = "Gloated";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isGloated() ? 10 : 0);
            existing.Add(n, r);

            n = "Bored";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isBored() ? 10 : 0);
            existing.Add(n, r);

            n = "LikesToSpeak";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => 5);
            existing.Add(n, r);

            n = "Feared";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isFeared() ? 10 : 0);
            existing.Add(n, r);

            n = "HasRumor";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).hasRumor() ? 5 : -15);
            existing.Add(n, r);

            n = "Romantic";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => d[2] as intent? == intent.Romantic ? 10 : 0);
            existing.Add(n, r);

            n = "IsLiked";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => {
                List<Feeling> feelings = (d[0] as Character).getBeliefs(d[1] as Character);
                if(feelings != null)
                    foreach(Feeling f in feelings)
                    {
                        if (f.getIntent() == intent.Romantic)
                            return f.getIntensity() * 5;
                    }
                    
                return 0;
            });
            existing.Add(n, r);

            return existing;
        }
    }
}
