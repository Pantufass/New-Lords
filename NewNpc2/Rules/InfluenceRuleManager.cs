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
            r.setDel((List<dynamic> d) => (d[0] as Character).getKind() * 5 + 12);
            existing.Add(n, r);

            n = "ImproveRel";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => 5 * (d[2] as intent? == intent.Positive ? 1 : -0.5f));
            existing.Add(n, r);

            n = "IsNice";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getKind() * 5);
            existing.Add(n, r);

            n = "BeingNice";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isHappy() ? (d[0] as Character).getKind() * 10 : 0);
            existing.Add(n, r);

            n = "Hurtful";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isGood() ? -2 : (d[0] as Character).getHonor() * 8 + 4);
            existing.Add(n, r);

            n = "Gloated";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isGloated() ? 10 : 0);
            existing.Add(n, r);

            n = "Bored";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isBored() ? 8 : 0);
            existing.Add(n, r);

            n = "LikesToSpeak";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => 4);
            existing.Add(n, r);

            n = "Feared";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).isFeared() ? 6 : 0);
            existing.Add(n, r);

            n = "Romantic";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => d[2] as intent? == intent.Romantic ? 10 : 0);
            existing.Add(n, r);


            n = "Charming";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getCharm() * 10 + (d[1] as Character).getKind() * 3);
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

            n = "InterestRumor";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) =>(d[0] as Character).hasRumor ? (d[0] as Character).getRumorValue() * 2.2f : -5);
            existing.Add(n, r);

            n = "LowHonor";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getHonor() < 0.3f ? 6 : -4);
            existing.Add(n, r);

            n = "NotHonor";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getHonor() * - 10);
            existing.Add(n, r);


            n = "Helpful";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getHelp() * 2);
            existing.Add(n, r);


            n = "HasStatus";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).hasStatus() ? 8 : -2);
            existing.Add(n, r);

            n = "SuckUp";
            r = new InfluenceRule(n);
            r.setDel((List<dynamic> d) => (d[0] as Character).getCalc() * 3 + (d[0] as Character).getHonor() * - 3);
            existing.Add(n, r);


            return existing;
        }
    }
}
