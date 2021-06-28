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
        public List<Character> others;

        //TODO other type of information possible in the exchange
        //private Information info;


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
            List<dynamic> l = new List<dynamic>();
            l.Add(initiator);
            l.Add(receiver);
            l.Add(intent);

            foreach (InfluenceRule r in type.getRespRules())
            {
                if (r.validate(l)) res += r.value(l);
            }
            return res;
        }

        public void chooseResponse(float result)
        {
            if (result > type.upperThresh)
                type.chooseDialog(sentenceType.pResponse, result);
            else if (result > type.lowerThresh)
                type.chooseDialog(sentenceType.normalResponse, result);
            else type.chooseDialog(sentenceType.nResponse, result);

        }

        public Dialog getResponse(float result)
        {

            if (result > type.upperThresh)
                return type.getDialog(sentenceType.pResponse, result);
            else if (result > type.lowerThresh)
                return type.getDialog(sentenceType.normalResponse, result);
            else return type.getDialog(sentenceType.nResponse, result);
        }

        public void setOutcome(float v)
        {
            outcome = (v > type.upperThresh ? outcome.Positive : v > type.lowerThresh ? outcome.Neutral : outcome.Negative);
        }


        public List<InstRule> getInstRules()
        {
            return type.getInstRules();
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

        internal void spendEnergy()
        {
            initiator.spendEnergy();
        }

        internal void finish()
        {
            spendEnergy();
            initiator.FinishedExchange();
            receiver.FinishedExchange();
        }
    }


    public enum outcome
    {
        Neutral = 0,
        Positive = 1,
        Negative = -1
    }
    

}
