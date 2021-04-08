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

        protected List<InfluenceRule> initRules;
        protected List<InfluenceRule> respRules;

        public SocialExchangeType(string n, float up, float low)
        {
            name = n;
            preconditions = new List<Condition>();
            sentences = new List<Dialog>();
            upperThresh = up;
            lowerThresh = low;
            addsentence("...");
        }

        public void addInitRule(InfluenceRule rule)
        {
            initRules.Add(rule);
        }

        public void addRespRule(InfluenceRule rule)
        {
            respRules.Add(rule);
        }

        public void addsentence(string sentence)
        {
            sentences.Add(new Dialog(sentence,0,sentenceType.Normal));
        }
        public void addsentence(string sentence,float v, sentenceType t)
        {
            sentences.Add(new Dialog(sentence, v,t));
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

        //TODO calc intent
        //return a sum of the influence rules
        public float calculateVolition(Character init,Character rec, intent intent)
        {

            float res = 0;
            foreach(InfluenceRule r in initRules)
            {
                if(r.validate()) res += r.value(init,rec,intent);
            }
            foreach(InfluenceRule r in init.getRules())
            {
                if (r.validate()) res += r.value(init,rec,intent);
            }

            return res;
        }


        //TODO calculate the receiver's response
        public float calculateResponse(Character init, Character rec, intent intent)
        {
            float res = 0;
            foreach (InfluenceRule r in respRules)
            {
                if (r.validate()) res += r.value(init, rec, intent);
            }
            return res;
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
        public float getIntensity()
        {
            return intensity;
        }
        public void setIntent(intent i)
        {
            intent = i;
        }

        public intent getIntent()
        {
            return intent;
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
        
        //more value means more confidence
        public float value;

        public sentenceType t;

        public Dialog(string s, float v, sentenceType t)
        {
            sentence = s;
            value = v;
            this.t = t;
        }

        public void updateValue(float v)
        {
            value = v;
        }
    }

    public enum sentenceType
    {
        Cordial,
        Normal,
        Crude,
        pResponse,
        nResponse,
        normalResponse
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

            t.addCondition(ConditionManager.NotIntroduced());

            InfluenceRule r = new InfluenceRule("NeedtoIntroduce");
            r.setDel((Character c1, Character c2, intent it) => 100);

            t.addsentence("Hello.",1,sentenceType.Normal);

            t.addsentence("Hi back.", 1, sentenceType.pResponse);
            t.addsentence("No.", 0, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Compliment(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Compliment";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);


            InfluenceRule r = new InfluenceRule("ImproveRel"); //is positive
            r.setDel((Character c1, Character c2, intent it) => 5 * (it == intent.Positive ? 1 : -1) );
            t.addInitRule(r);

            r = new InfluenceRule("IsNice"); //-5 to 5 less to more kind
            r.setDel((Character c1, Character c2, intent it) => c1.personality.kind * 5);
            t.addInitRule(r);

            r = new InfluenceRule("BeingNice");//return 5 if happy 0 if not
            r.setDel((Character c1, Character c2, intent it) => c1.isHappy() ? 5 : 0);
            t.addInitRule(r);

            t.addsentence("You look good.");
            sc.Add(n, t);
        }

        private static void Insult(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Insult";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Hurtful");//return 5 if not good
            r.setDel((Character c1, Character c2, intent it) => c1.isGood() ? 0 : 10);
            t.addInitRule(r);


            t.addsentence("You look stupid.");
            sc.Add(n, t);
        }

        private static void Brag(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Brag";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Gloated"); 
            r.setDel((Character c1, Character c2, intent it) => c1.isGloated() ? 10 : 0);
            t.addInitRule(r);

            t.addsentence("I am the best.");
            sc.Add(n, t);
        }

        private static void Converse(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Converse";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Bored");
            r.setDel((Character c1, Character c2, intent it) => c1.isBored() ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("Entertain");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Entertain ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("LikesToSpeak");
            r.setDel((Character c1, Character c2, intent it) => 5);
            t.addInitRule(r);

            t.addsentence("I like pudding.");
            sc.Add(n, t);
        }
        private static void RunAway(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "RunAway";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);


            InfluenceRule r = new InfluenceRule("Hurtful");//return 5 if not good
            r.setDel((Character c1, Character c2, intent it) => c1.isGood() ? 0 : 10);
            t.addInitRule(r);

            r = new InfluenceRule("Feared");
            r.setDel((Character c1, Character c2, intent it) => c1.isFeared() ? 10 : 0);
            t.addInitRule(r);

            sc.Add(n, t);
        }

        //rumor
        private static void RelayInformation(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "RelayInformation";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Bored");
            r.setDel((Character c1, Character c2, intent it) => c1.isBored() ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("Entertain");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Entertain ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("LikesToSpeak");
            r.setDel((Character c1, Character c2, intent it) => 5);
            t.addInitRule(r);

            r = new InfluenceRule("HasRumor");
            r.setDel((Character c1, Character c2, intent it) => c1.hasRumor() ? 5 : -10);
            t.addInitRule(r);


            t.addsentence("I have to tell you something.");
            sc.Add(n, t);
        }

        private static void Gossip(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Gossip";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Bored");
            r.setDel((Character c1, Character c2, intent it) => c1.isBored() ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("LikesToSpeak");
            r.setDel((Character c1, Character c2, intent it) => 5);
            t.addInitRule(r);

            t.addsentence("Please tell me something.");
            sc.Add(n, t);
        }


        //romance
        private static void Flirt(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Flirt";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(ConditionManager.Adults());

            InfluenceRule r = new InfluenceRule("Romantic");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Romantic ? 10 : 0);
            t.addInitRule(r);

            t.addsentence("Do you want to hang out?");
            sc.Add(n, t);
        }

        private static void Date(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "Date";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.NoRomanceYet());

            InfluenceRule r = new InfluenceRule("Romantic");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Romantic ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("Isliked");
            r.setDel((Character c1, Character c2, intent it) => {
                Feeling feeling;
                if (c1.beliefs.TryGetValue(c2, out feeling))
                    if (feeling.getIntent() == intent.Romantic)
                        return feeling.getIntensity();
                return 0;
                });
            t.addInitRule(r);

            t.addsentence("Let's date.");
            sc.Add(n, t);
        }

        private static void DeclareLove(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "DeclareLove";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(ConditionManager.Adults());



            t.addsentence("My passion for...");
            sc.Add(n, t);
        }

        private static void BreakUp(Dictionary<string, SocialExchangeType> sc)
        {
            string n = "BreakUp";
            SocialExchangeType t = new SocialExchangeType(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.Romance());



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
            t.addCondition(ConditionManager.LordsOnly());



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
