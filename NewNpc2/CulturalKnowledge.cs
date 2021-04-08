using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class CulturalKnowledge
    {
        protected string name;
        protected List<InfluenceRule> culturalRules;

        public CulturalKnowledge(string n)
        {
            name = n;
        }

        public List<InfluenceRule> getCulturealRules()
        {
            return culturalRules;
        }
    }
}
