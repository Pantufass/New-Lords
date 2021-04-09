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
        private SocialInteraction type;

        //date of the exchange
        private int date;

        //outcome of the exchange
        private outcome o;

        //other characters involved 
        private List<Character> others;

        //TODO other type of information possible in the exchange
        private Information info;


        //TODO effects
        //study dialog manager better
        private List<Effect> effects;

        public SocialExchange(Character i, Character r, SocialInteraction set, outcome o) : base(i)
        {
            type = set;

            receiver = r;
            this.o = o;
        }


    }


    

    public class Effect
    {

    }
    public class Information
    {

    }



    public enum outcome
    {
        Neutral,
        Positive,
        Negative
    }

    
}
