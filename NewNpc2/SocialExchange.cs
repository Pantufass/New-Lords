using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{

    public class SocialExchangeType
    {
        public string name;
        //list of preconditions
        private List<Condition> preconditions;

        //threshold for a positive response
        public float upperThresh;
        //treshhold for a neutral response
        public float lowerThresh;

        public SocialExchangeType(string n)
        {
            name = n;
            preconditions = new List<Condition>();
        }

        public void addCondition(Condition c)
        {
            preconditions.Add(c);
        }

        //return a sum of the preconditions
        public bool preConditions()
        {
            return false;
        }
    }

    public class SocialExchange : Feeling
    {
        //type of the social exchange
        private SocialExchangeType type;

        protected Character receiver;

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

        //threshold for a positive response
        private float upperThresh;
        //treshhold for a neutral response
        private float lowerThresh;


        public SocialExchange(Character i, Character r, SocialExchangeType set) : base(i)
        {
            type = set;

            receiver = r;

            //TODO? threshold modifiers relating to the specified exchange
            upperThresh = set.upperThresh;
            lowerThresh = set.lowerThresh;
        }



        //calculate the initiator's volition
        public void calculateVolition()
        {
            //initiator.influencerules calc intensity
        }

        //calculate the receiver's response
        public void calculateResponse()
        {
            //receiver.influencerules calc outcome
        }
    }

    public class Belief : Feeling
    {
        protected Character receiver;

        private beliefType type;

        public Belief(Character init, Character rec, beliefType t) : base(init)
        {
            receiver = rec;
            type = t;
        }


    }

    public class Feeling
    {
        protected Character initiator;

        protected intent intent;

        protected float intensity;

        public Feeling(Character init)
        {
            initiator = init;
        }
        public void setIntensity(float i)
        {
            intensity = i;
        }
        public float getInt()
        {
            return intensity;
        }
        public void setIntent(intent i)
        {
            intent = i;
        }
    }

    public class Effect
    {

    }
    public class Information
    {

    }

    public enum beliefType
    {
        FriendshipRelation,
        RomanticRelation,
        PowerRelation
    }

    public enum intent
    {
        Neutral,
        Positive,
        Negative,
        Romantic
    }

    public enum outcome
    {
        Neutral,
        Accept,
        Reject
    }
}
