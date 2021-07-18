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
        public float energyMod;
        public float interestMod;

        public Culture(string n)
        {
            name = n;
            culturalExchanges = new List<SocialInteraction>();
            culturalRules = new List<InfluenceRule>();
            newData = new List<InteractionData>();
            energyMod = 1;
            interestMod = 1;
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

        public bool Equals(Culture c)
        {
            return name.Equals(c.name);
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

            khu.energyMod = 0.5f;
            khu.interestMod = 0.5f;


            return khu;
        }

        public static Culture createSturgians()
        {
            Culture c = new Culture("Sturgians");





            return c;
        }


        public static Culture createAserai()
        {
            Culture c = new Culture("Aserai");


            c.addExchange(SocialInteractionManager.Complain());
            c.addExchange(SocialInteractionManager.Embelish());



            return c;
        }



        public static Culture createBattania()
        {
            Culture c = new Culture("Battania");


            c.energyMod = 1.05f;
            c.interestMod = 0.8f;

            return c;
        }



        public static Culture createVlandia()
        {
            Culture c = new Culture("Vlandia");


            return c;
        }

    }
}
