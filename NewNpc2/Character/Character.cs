using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.TwoDimension;

namespace NewNpc2
{
    public class Character
    {
        //class with the personality traits
        private Traits personality;

        //List of possible statuses
        private List<Status> status;

        //likes?
        public CharacterObject characterObject;

        //social network
        //3 unidirectional relations between this and other characters
        private List<Feeling> friendlyFeelings;
        private List<Feeling> romanticFeelings;
        private List<Feeling> admiration;

        //belief network
        private Dictionary<Character,List<Feeling>> beliefs;

        private intent currIntent;

        //list of social exchanges known
        //each exchange is paired with the respective believability 
        private Dictionary<SocialExchange, float> socialKnowledge;

        //cultural knowledge TODO probably 
        private CulturalKnowledge culture;

        private List<SocialInteraction> intendedSocialExchange;

        //TODO rumor
        private Rumor rumor;

        //TODO influence rules
        private List<InfluenceRule> influenceRules;

        private int intendedMaxNumber = 3;

        public Character(CulturalKnowledge culture,CharacterObject co)
        {
            personality = new Traits();
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new Dictionary<Character, List<Feeling>>();
            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<InfluenceRule>();
            intendedSocialExchange = new List<SocialInteraction>();
            rumor = null;
            this.culture = culture;
            characterObject = co;
            currIntent = intent.Neutral;
            initialState();
        }


        public Character(CulturalKnowledge culture, CharacterTraits characterTraits,CharacterObject co)
        {
            personality = new Traits(characterTraits);
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new Dictionary<Character, List<Feeling>>();
            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<InfluenceRule>();
            intendedSocialExchange = new List<SocialInteraction>();
            rumor = null;
            this.culture = culture;
            characterObject = co;
            initialState();
        }

        internal List<Feeling> getBeliefs(Character character)
        {
            List<Feeling> l;
            beliefs.TryGetValue(character, out l);
            return l;
        }

        public bool hasRumor()
        {
            return rumor != null;
        }

        private void initialState()
        {

            var rand = new Random();
            if (rand.NextDouble() > 0.5)
                status.Add(Status.Happy);
            else
                status.Add(Status.Angry);
            status.Add(Status.Bored);
        }

        public void clearIntented()
        {
            intendedSocialExchange.Clear();
        }

        public void setIntendedRule(SocialInteraction si)
        {
            intendedSocialExchange.Add(si);
        }

        public List<SocialInteraction> getIntented()
        {
            return intendedSocialExchange;
        }
        
        public void addInfluenceRule(InfluenceRule r)
        {
            influenceRules.Add(r);
        }

        public intent calcVolitions(Character r)
        {
            intent intent = calcIntent(r);
            Random rand = new Random();
            float chance = 2;

            Dictionary<float, SocialInteraction> d = new Dictionary<float, SocialInteraction>();
            foreach(SocialInteraction si in SubModule.existingExchanges.Values){
                if (si.validate(this.characterObject, r.characterObject))
                {
                    float res = (float)(si.calculateVolition(this, r, intent) + (2*chance*rand.NextDouble()-chance));
                    //TDODO fix
                    while (d.ContainsKey(res))
                        res += (float)rand.NextDouble()* 0.2f -0.1f;
                    d.Add(res,si);
                }
            }
            List<float> l = d.Keys.ToList();
            l.Sort();
            l.Reverse();

            for(int i = 0; i < intendedMaxNumber; i++)
            {
                intendedSocialExchange.Add(d[l[i]]);
            }

            return intent;
        }

        internal sentenceType calcDialogType()
        {
            Random r = new Random();
            float v = Mathf.Lerp(0,1,(personality.careful + personality.honor + personality.shy) * ((float)r.NextDouble() * 0.3f - 0.15f));
            if (v < 0.1) return sentenceType.Crude;
            else if (v > 0.85) return sentenceType.Cordial;
            else return sentenceType.Normal;
        }
        
        private intent calcIntent(Character r)
        {
            intent i = intent.Neutral;

            Feeling friendly = getFeeling(friendlyFeelings, r);
            Feeling romantic = getFeeling(romanticFeelings, r);
            Feeling admire = getFeeling(admiration, r);

            if(friendly != null) 
                i = friendly.getIntent();

            if (romantic != null && romantic.getIntensity() > (friendly != null ? friendly.getIntensity(): 0))
                i = romantic.getIntent();

            if (admire != null)
                if (admire.getIntensity() > (friendly != null ? friendly.getIntensity() : 0) &&
                    admire.getIntensity() > (romantic != null ? romantic.getIntensity() : 0))
                    i = admire.getIntent();
                    
            return i;
        }

        public void raiseRomantic(Character c, float v = 1)
        {
            float value = v + ((v / 5) * personality.stubborn);
            Feeling f = null;
            for(int i = 0; i < romanticFeelings.Count; i++)
            {
                if (romanticFeelings[i].getInitiator() == c)
                {
                    romanticFeelings[i].addIntensity(value);
                    f = romanticFeelings[i];
                }
            }
            if (f == null) romanticFeelings.Add(new Feeling(c,v,intent.Romantic));
        }

        public void lowerRomantic(Character c, float v = 1)
        {
            raiseRomantic(c, -v);

        }

        internal void overpowered(Character c, float v = 1)
        {
            Feeling f = null;
            for (int i = 0; i < admiration.Count; i++)
            {
                if (admiration[i].getInitiator() == c)
                {
                    admiration[i].addIntensity(v);
                    f = admiration[i];
                    if (getFeeling(friendlyFeelings, c).getIntent() == intent.Positive) admiration[i].setIntent(intent.Embellish);
                }
            }
            if (f == null) admiration.Add(new Feeling(c, v, intent.Neutral));

        }

        internal void raiseFriendly(Character c, float v = 1)
        {
            Random r = new Random();
            float value = v - ((v / 4) * personality.stubborn *(float) r.NextDouble());
            Feeling f = null;
            for (int i = 0; i < friendlyFeelings.Count; i++)
            {
                if (friendlyFeelings[i].getInitiator() == c)
                {
                    f = friendlyFeelings[i];
                    friendlyFeelings[i].addIntensity(value);
                    if(friendlyFeelings[i].getIntensity() > 10 && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Positive);
                    else if(friendlyFeelings[i].getIntensity() > 0 && friendlyFeelings[i].getIntent() == intent.Negative)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < 10 && friendlyFeelings[i].getIntent() == intent.Positive)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < 0 && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Negative);
                }
            }
            if (f == null) friendlyFeelings.Add(new Feeling(c, v, intent.Positive));
        }

        internal void lowerFriendly(Character c, float v = 1)
        {
            raiseFriendly(c, -v);
        }
         
        private Feeling getFeeling(List<Feeling> list, Character c)
        {
            Feeling f = null;
            foreach(Feeling f2 in list)
            {
                if (f2.getInitiator() == c) f = f2;
            }
            return f;
        }

        public void setIntent(intent i)
        {
            currIntent = i;
        }

        public intent getIntent()
        {
            return currIntent;
        }

        public void addExchange(SocialExchange se)
        {
            socialKnowledge.Add(se, 1);
        }

        public List<InfluenceRule> getRules()
        {
            return influenceRules;
        }

        public void addRumor(Rumor r)
        {
            if (rumor.interest(this) > r.interest(this)) rumor = r;
        }

        public void hearRumor(Rumor r)
        {
            addRumor(r);
        }

        public bool isHappy()
        {
            return status.Contains(Status.Happy);
        }

        public void addStatus(Status s)
        {
            status.Add(s);
        }

        public void removeStatus(Status s)
        {
            status.Remove(s);
        }

        public bool isGloated()
        {
            return status.Contains(Status.Gloated);
        }
        public bool isBored()
        {
            return status.Contains(Status.Bored);
        }

        public void notBored()
        {
            status.Remove(Status.Bored);
        }

        public bool isFeared()
        {
            return status.Contains(Status.Feared);
        }

        public bool isGood()
        {
            int b = 0;
            foreach(Status s in status)
            {
                b += (int) s / Math.Abs((int)s);
            }
            return b >= 0;
        }
        
        internal float getSensitive()
        {
            return personality.sensitive;
        }
        internal float getShy()
        {
            return personality.shy;
        }
        internal float getKind()
        {
            return personality.kind;
        }

        internal bool isGay()
        {
            return personality.isGay;
        }

        internal float getCharm()
        {
            return personality.charm;
        }

        internal float getHonor()
        {
            return personality.honor;
        }

        internal class Traits
        {
            public float kind;
            public float stubborn;
            public float liar;
            public float curious;
            public float helpful;
            public float shy;
            public float careful;
            public float sensitive;
            public float honor;
            public float charm;

            public float annoying;

            public float calculating;

            public bool isGay; 

            internal Traits()
            {

                var rand = new Random();
                kind = (float) rand.NextDouble() * 2 - 1;
                stubborn = (float)rand.NextDouble() * 2 - 1;
                liar = (float)rand.NextDouble() * 2 - 1;
                curious = (float)rand.NextDouble() * 2 - 1;
                helpful = (float)rand.NextDouble() * 2 - 1;
                shy = (float)rand.NextDouble() * 2 - 1;
                careful = (float)rand.NextDouble() * 2 - 1;
                sensitive = (float)rand.NextDouble() * 2 - 1;
                honor = (float)rand.NextDouble() * 2 - 1;
                charm = (float)rand.NextDouble() * 2 - 1;

                annoying = (float)rand.NextDouble() * 2 - 1;

                calculating = (float)rand.NextDouble() * 2 - 1;

                isGay = rand.NextDouble() < 0.1;
            }

            public Traits(CharacterTraits charaterTraits)
            {
                
                var rand = new Random();
                kind = (float)(rand.NextDouble()) * charaterTraits.Generosity % 1; 
                stubborn = (float)rand.NextDouble() * 2 - 1;
                liar = (float)rand.NextDouble() * 2 - 1;
                curious = (float)rand.NextDouble() * 2 - 1;
                helpful = (float)rand.NextDouble() * charaterTraits.Mercy % 1;
                shy = (float)rand.NextDouble() * 2 - 1;
                careful = (float)rand.NextDouble() * 2 - 1;
                sensitive = (float)rand.NextDouble() * -charaterTraits.Valor % 1;
                honor = (float)rand.NextDouble() * charaterTraits.Honor % 1;
                charm = (float)rand.NextDouble() * 2 - 1;

                annoying = (float)rand.NextDouble() * 2 - 1;

                calculating = (float)rand.NextDouble() * charaterTraits.Calculating % 1;


                if(kind == 0)  kind = (float)rand.NextDouble() * 2 - 1; ;
                if(helpful == 0) helpful = (float)rand.NextDouble() * 2 - 1; ;
                if(sensitive == 0) sensitive = (float)rand.NextDouble() * 2 - 1; ;
                if(honor == 0) honor = (float)rand.NextDouble() * 2 - 1; ;
                if(calculating == 0) calculating = (float)rand.NextDouble() * 2 - 1; ;


                isGay = rand.NextDouble() < 0.1;
            }


           
        }

        

        public enum Status
        {
            Wounded=-10,
            Bored=-9,
            Tired=-8,
            Hungry=-7,

            //?
            SocialInteract=1,

            Angry=-6,
            Feared=-5,
            Sad = -4,
            Confident =2,
            Happy=3,
            Surprised=4,

            Ashamed=-3,
            Resented = -2,
            Gloated =4,
            Pity = 5,
            HappyFor =6
        }
    }

}
