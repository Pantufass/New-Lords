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
        protected List<InteractionData> newData;
        public float energyMod = 0;
        public float interestMod = 0;

        public Culture(string n)
        {
            name = n;
            culturalExchanges = new List<SocialInteraction>();
            culturalRules = new List<InfluenceRule>();
            newData = new List<InteractionData>();
        }

        public void addRule(InfluenceRule ir)
        {
            culturalRules.Add(ir);
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

        public List<InteractionData> getIntData()
        {
            return newData;
        }
    }

    public class InteractionData
    {
        public string name;
        public List<InfluenceRule> rules;
        public List<Dialog> dialog;
        public List<InstRule> inst;


        public InteractionData(string n)
        {
            name = n;
            rules = new List<InfluenceRule>();
            dialog = new List<Dialog>();
            inst = new List<InstRule>();
        }

        public bool compare(string s)
        {
            return s.Equals(name);
        }
    }

    class CultureManager
    {
        public static Culture createCultureX()
        {
            Culture x = new Culture("x");


            x.addExchange(SocialInteractionManager.Complain());

            
            return x;

        }

        public static Culture createEmpire()
        {
            Culture emp = new Culture("Empire");


            emp.addExchange(SocialInteractionManager.Complain());
            emp.addExchange(SocialInteractionManager.BadMouth());

            InteractionData id = new InteractionData("Compliment");


            return emp;
        }

        public static Culture createKhuzaits()
        {
            Culture khu = new Culture("Khuzaits");


            khu.addExchange(SocialInteractionManager.Complain());
            khu.addExchange(SocialInteractionManager.Embelish());

            khu.energyMod = -0.5f;
            khu.interestMod = -0.4f;


            return khu;
        }

    }
}
