using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace NewNpc2
{
    public class RumorHolder
    {
        protected float INTEREST_VALUE = 0.7f;

        [SaveableField(1)]
        protected List<Rumor> holding;
        [SaveableField(2)]
        protected int max;

        public RumorHolder(int m = 6)
        {
            holding = new List<Rumor>();
            max = m;
        }

        private bool foundRumor(Rumor r)
        {
            foreach(Rumor rumor in holding)
            {
                if (rumor.Equals(r)) return true;
            }
            return false;
        }

        public virtual void setRumors(List<Rumor> r)
        {
            foreach (Rumor rumor in holding)
            {
                rumor.lowerValue();
            }
            foreach(Rumor rumor in r)
            {
                if (!foundRumor(rumor))
                    holding.Add(rumor);
                else
                {
                    holding.Find((Rumor rum) => rum.Equals(rumor)).raiseValue();
                }
            }
            holding.OrderByDescending(rumor => rumor.getValue());
            for (int i = 0; i < (holding.Count - max); i++)
            {
                holding.Remove(holding.Last());
            }
        }

        public virtual Tuple<Rumor, float> getRumor(Character c)
        {
            if (holding.Count < 1) return null;
            Tuple<Rumor, float> res = new Tuple<Rumor, float>(holding[0], holding[0].interest(c));
            foreach(Rumor r in holding)
            {
                float v = r.interest(c);
                if ( v > res.Item2) res = new Tuple<Rumor, float>(r, v);
            }
            return res;
        }

        internal List<Rumor> getRumors()
        {
            return holding;
        }

        internal float interest()
        {
            float res = 0;
            foreach (Rumor r in holding)
            {
                res += r.getValue() * INTEREST_VALUE;
            }
            return res;
        }

        internal void addRumor(Rumor r)
        {
            holding.Add(r);
            holding.OrderByDescending(rumor => rumor.getValue());
            if(holding.Count > max) holding.Remove(holding.Last());
        }
    }

    public class RumorParty : RumorHolder
    {
        protected Rumor.Information.type preference;
        protected MobileParty party;

        public RumorParty(MobileParty mp, int m = 3) : base(m)
        {
            party = mp;
            //Character c = CharacterManager.findChar(h);
            //calc preference
            preference = Rumor.Information.type.Warfare;
        }


        public override void setRumors(List<Rumor> r)
        {
            if (r.Count >= max)
            {
                r.OrderBy(value => (value.info.getType() == preference) ? value.getValue() * 1.1f : value.getValue());
                holding.Clear();
                for (int i = 0; i < max; i++)
                {
                    holding.Add(r[i]);
                }
            }
        }
    }
}
