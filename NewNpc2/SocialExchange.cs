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

        public List<Dialog> sentences;

        public SocialExchangeType(string n, float up, float low)
        {
            name = n;
            preconditions = new List<Condition>();
            sentences = new List<Dialog>();
            upperThresh = up;
            lowerThresh = low;
        }

        public void addsentence(string sentence)
        {
            sentences.Add(new Dialog(sentence,0));
        }

        public void addCondition(Condition c)
        {
            preconditions.Add(c);
        }

        //return a sum of the preconditions
        public bool preConditions()
        {
            bool b = true;
            foreach(Condition c in preconditions)
            {
                b = b && c.validate();
            }
            return b;
        }
    }

    public class SocialExchange : Feeling
    {

        protected Character receiver;
        //type of the social exchange
        private SocialExchangeType type;

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



        public SocialExchange(Character i, Character r, SocialExchangeType set) : base(i)
        {
            type = set;

            receiver = r;

        }



        //TODO calculate the initiator's volition
        public float calculateVolition()
        {
            //initiator.influencerules calc intensity
            return 0;
        }

        //TODO calculate the receiver's response
        public float calculateResponse()
        {
            //receiver.influencerules calc outcome
            return 0;
        }

        
    }

    public class Belief : Feeling
    {

        protected Character receiver;
        public Belief(Character init, Character rec) : base(init)
        {
            receiver = rec;
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


    public class Dialog
    {
        protected string sentence;
        public float value;

        public Dialog(string s, float v)
        {
            sentence = s;
            value = v;
        }

        public void updateValue(float v)
        {
            value = v;
        }
    }

    public enum intent
    {
        Neutral,
        Positive,
        Negative,
        Romantic,
        SpreadNews,
        AdquireKnowledge,
        Power,
        Entertain,
        Like,
        Dislike
    }

    public enum outcome
    {
        Neutral,
        Positive,
        Negative
    }

    public static class SocialExchangeManager
    {
        private static void Introduce(Dictionary<string,SocialExchangeType> sc)
        {
            string n = "Introduce";
            SocialExchangeType t = new SocialExchangeType(n,1,1);
            t.addCondition(new Condition(conditions.NotIntroduced,"NotIntroduced"));
            t.addsentence("Hello.");
            sc.Add(n, t);
        }

        private static void Compliment(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Compliment";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("You look good.");
            sc.Add(n, t);
        }

        private static void Insult(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Insult";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("You look stupid.");
            sc.Add(n, t);
        }

        private static void Brag(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Brag";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("I am the best.");
            sc.Add(n, t);
        }

        private static void Converse(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Converse";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("I like pudding.");
            sc.Add(n, t);
        }
        private static void RunAway(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "RunAway";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            sc.Add(n, t);
        }

        //rumor
        private static void RelayInformation(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "RelayInformation";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("I have to tell you something.");
            sc.Add(n, t);
        }

        private static void Gossip(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Gossip";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("Please tell me something.");
            sc.Add(n, t);
        }


        //romance
        private static void Flirt(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Flirt";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(new Condition(conditions.Adults,"Adults"));
            t.addsentence("Do you want to hang out?");
            sc.Add(n, t);
        }

        private static void Date(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Date";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(new Condition(conditions.Adults, "Adults"));
            t.addCondition(new Condition(conditions.NoRomance, "NoRomanceYet"));
            t.addsentence("Let's date.");
            sc.Add(n, t);
        }

        private static void DeclareLove(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "DeclareLove";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(new Condition(conditions.Adults, "Adults"));
            t.addsentence("My passion for...");
            sc.Add(n, t);
        }

        private static void BreakUp(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "BreakUp";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(new Condition(conditions.Adults, "Adults"));
            t.addCondition(new Condition(conditions.Romance, "Romance"));
            t.addsentence("I don't like you anymore.");
            sc.Add(n, t);
        }

        //lords only
        private static void Kick(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Kick";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addsentence("Get out right now.");
            sc.Add(n, t);
        }


        private static void Exile(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Exile";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(new Condition(conditions.LordsOnly, "LordsOnly"));
            t.addsentence("You are not allowed here anymore.");
            sc.Add(n, t);
        }


        public static Dictionary<string,SocialExchangeType> createSocialExchanges()
        {
            Dictionary<string, SocialExchangeType> existingExchanges = new Dictionary<string, SocialExchangeType>();

            Introduce(existingExchanges);
            Compliment(existingExchanges);
            Insult(existingExchanges);
            Brag(existingExchanges);
            Converse(existingExchanges);
            RunAway(existingExchanges);

            RelayInformation(existingExchanges);
            Gossip(existingExchanges);

            Flirt(existingExchanges);
            Date(existingExchanges);
            DeclareLove(existingExchanges);
            BreakUp(existingExchanges);

            Kick(existingExchanges);
            Exile(existingExchanges);

            return existingExchanges;
        }

        
    }
}
