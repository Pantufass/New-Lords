using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Culture
    {
        public string name;
        protected List<InfluenceRule> culturalRules;
        protected List<SocialInteraction> culturalExchanges;

        public Culture(string n)
        {
            name = n;
            culturalExchanges = new List<SocialInteraction>();
            culturalRules = new List<InfluenceRule>();
        }

        public List<InfluenceRule> getCulturealRules()
        {
            return culturalRules;
        }

        public void addExchange(SocialInteraction si)
        {
            culturalExchanges.Add(si);
        }

        public List<SocialInteraction> CulturalExchanges()
        {
            return culturalExchanges;
        }
    }

    class CultureManager
    {
        public static Culture createCultureX()
        {
            Culture x = new Culture("x");


            x.addExchange(SocialInteractionManager.);

            
            return x;

        }

        
    }
}
