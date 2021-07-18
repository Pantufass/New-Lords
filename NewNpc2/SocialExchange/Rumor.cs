using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace NewNpc2
{
    public class Rumor 
    {
        private const float INTEREST_VALUE = 1f;
        private const float I = 0.5f;

        [SaveableField(1)]
        private float value;

        [SaveableField(2)]
        private SocialExchange s;

        [SaveableField(3)]
        public Information info;

        public Rumor(SocialExchange social) 
        {
            s = social;
            info = new Information();
            value = 1f;
        }
        public Rumor(Information i, SocialExchange social = null, float v = 1f) 
        {
            info = i;
            value = v;
            s = social;
        }


        public bool Equals(Rumor r)
        {
            bool res = true;
            res = res && r is Rumor;
            res = res && this is Rumor;
            if(s != null && r.s != null) res = res && s.Equals(r.s);
            res = res && info.Equals(r.info);
            return res;
        }

        public SocialExchange exchange()
        {
            return s;
        }

        //calc interest the character has in the rumor
        public float interest(Character c)
        {
            float res = INTEREST_VALUE;
            res += INTEREST_VALUE * c.getCurious() / 2;
            List<Character> involved = new List<Character>();

            if(s != null)
            {
                involved.Add(s.getInitiator());
                involved.Add(s.getReceiver());
                //involved.AddRange(s.others);

                foreach (Character inv in involved)
                {

                    if (inv == c && c.isFalse(s))
                    {
                        //lie found
                        return 0;
                    }
                    res += c.calcCharacterInterest(inv) * 0.1f;
                }
            }
            
            if (info.getType() == c.preference) res *= 1.3f;
            if (res > 5) res = 5;
            else if (res < 0) res = 0;
            return res * c.Culture.interestMod;
        }

        public float getValue()
        {
            return value;
        }

        public void raiseValue()
        {
            value *= 1.08f;
        }
        public void lowerValue()
        {
            value *= 0.9f;
        }

        public class Information
        {
            [SaveableField(4)]
            protected type infotype;
            [SaveableField(5)]
            public List<string> values;
            [SaveableField(6)]
            public bool war;

            public Information(type t = type.Gossip)
            {
                infotype = t;
            }
            public Information(List<string> list, type t = type.Economic)
            {
                infotype = t;
                values = list;
            }
            public Information(List<string> list, bool w = true, type t = type.Warfare)
            {
                infotype = t;
                values = list;
                war = w;
            }

            public type getType()
            {
                return infotype;
            }

            public enum type
            {
                Gossip,
                Warfare,
                Economic
            }
        }

    }

    
    public class Lie : Rumor
    {
        protected Character origin;

        public Lie(SocialExchange social, Character o) : base(social)
        {
            origin = o;
        }

        public Lie(Information i, Character o, float v = 0.8f, SocialExchange social = null) : base(i, social, v)
        {
            origin = o;
        }
    }

}
