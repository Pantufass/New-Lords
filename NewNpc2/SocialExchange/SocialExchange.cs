using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{

    public class SocialExchange : Feeling
    {

        protected Character receiver;
        //type of the social exchange
        public SocialInteraction type;

        public outcome outcome;
        //date of the exchange
        private int date;

        //outcome of the exchange
        private outcome o;

        //other characters involved 
        private List<Character> others;

        //TODO other type of information possible in the exchange
        private Information info;


        public SocialExchange(Character s, Character r, SocialInteraction set, intent i) : base(s)
        {
            type = set;

            receiver = r;
            base.intent = i;
        }

        //TODO calculate the receiver's response
        public float calculateResponse()
        {
            float res = 0;
            foreach (InfluenceRule r in type.getRespRules())
            {
                if (r.validate()) res += r.value(initiator, receiver, intent);
            }
            return res;
        }
        public void setOutcome(float v)
        {
            outcome = (v > type.upperThresh ? outcome.Positive : v > type.lowerThresh ? outcome.Neutral : outcome.Negative);
        }

        private class Information
        {

        }

        public List<TriggerRule> getTriggerRules()
        {
            return type.getTrigger();
        }

        public void setInCharacters()
        {
            //TODO add to viewers
            initiator.addExchange(this);
            receiver.addExchange(this);
        }

        public Character getReceiver()
        {
            return receiver;
        }
    }


    public enum outcome
    {
        Neutral = 0,
        Positive = 1,
        Negative = -1
    }
    

}
