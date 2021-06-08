﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;

namespace NewNpc2
{
    public class Character
    {
        #region Consts

        private const float FRIEND_STEP = 1;
        private const float ENERGY_STEP = 0.05f;
        private const float SPEND_ENERGY_STEP = 0.2f;
        private const int INT_MAX_EXCHANGES = 3;
        private const float CORDIAL_D = 0.1f;
        private const float CRUDE_D = 0.85f;
        private const float CHANCE_VALUE = 2;
        private const float CHANCE_VALUE2 = 0.15f;
        private const float POSITIVE_THRESH = 10;
        private const float NEGATIVE_THRESH = 0;
        private const float OWN_BELIEF_SURENESS = 1;
        private const float START_BASE_THRESH = 0.5f;
        private const float PREVIOUS_TRAITS_WEIGHT = 0.5f;


        #endregion


        //class with the personality traits
        private Traits personality;

        //List of possible statuses
        private List<Status> status;

        private float energy;
        private float threshold;

        public CharacterObject characterObject;
        public Agent agent;

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
        private Culture culture;

        private List<SocialInteraction> intendedSocialExchange;
        private SocialInteraction npcIntended;

        //TODO rumor
        private Rumor rumor;

        //TODO influence rules
        private List<InfluenceRule> influenceRules;


        public Character(Culture culture,CharacterObject co)
        {
            personality = new Traits();
            CreateChar(culture, co);
        }

        public Character(Culture culture, CharacterTraits characterTraits,CharacterObject co)
        {
            personality = new Traits(characterTraits);
            CreateChar(culture, co);
        }

        public Character(Agent a)
        {
            agent = a;
            personality = new Traits();
            CreateChar(null, null);
        }

        public void setAgent(Agent a)
        {
            agent = a;
        }

        public void setCulture(Culture c)
        {
            culture = c;
        }

        public void setCharacterObject(CharacterObject co)
        {
            characterObject = co;
        }

        private void CreateChar(Culture culture, CharacterObject co)
        {
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

            energy = InitialEnergy();
            threshold = exchangeThreshold();

        }

        private float InitialEnergy()
        {

            return 0;
        }

        private float exchangeThreshold()
        {
            float res = START_BASE_THRESH;

            res += (personality.annoying * START_BASE_THRESH);
            res += (personality.calculating * START_BASE_THRESH * 0.3f);

            return res;
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

        public List<SocialInteraction> getIntented(Character c, intent i)
        {
            calcIntended(c, i);
            setIntent(i);
            return intendedSocialExchange;
        }

        public void addInfluenceRule(InfluenceRule r)
        {
            influenceRules.Add(r);
        }
        public float calcNpcVolition(Character r)
        {
            intent intent = calcIntent(r);

            setIntent(intent);

            return calcNpcIntended(r, intent);
        }

        public intent calcVolitions(Character r)
        {
            intent intent = calcIntent(r);

            setIntent(intent);

            calcNpcIntended(r, intent);

            return intent;
        }

        private float calcNpcIntended(Character r, intent intent)
        {
            Random rand = new Random();

            KeyValuePair<float, SocialInteraction> d = new KeyValuePair<float, SocialInteraction>(0,new SocialInteraction("test",1,1));

            List<SocialInteraction> interactions = new List<SocialInteraction>();
            interactions.AddRange(culture.CulturalExchanges());
            interactions.AddRange(SubModule.existingExchanges.Values);

            foreach (SocialInteraction si in interactions)
            {
                if (si.validate(this.characterObject, r.characterObject))
                {
                    float res = (float)(si.calculateVolition(this, r, intent) + (2 * CHANCE_VALUE * rand.NextDouble() - CHANCE_VALUE));
                    if (res > d.Key) d = new KeyValuePair<float, SocialInteraction>(res, si);
                }
            }
            npcIntended = d.Value;
            return d.Key;
        }

        public SocialInteraction getNpcIntended()
        {
            return npcIntended;
        }

        private void calcIntended(Character r, intent intent)
        {
            Random rand = new Random();

            Dictionary<float, SocialInteraction> d = new Dictionary<float, SocialInteraction>();

            List<SocialInteraction> interactions = new List<SocialInteraction>();
            interactions.AddRange(culture.CulturalExchanges());
            interactions.AddRange(SubModule.existingExchanges.Values);

            foreach (SocialInteraction si in interactions)
            {
                if (si.validate(this.characterObject, r.characterObject))
                {
                    float res = (float)(si.calculateVolition(this, r, intent) + (2 * CHANCE_VALUE * rand.NextDouble() - CHANCE_VALUE));
                    //TDODO fix
                    while (d.ContainsKey(res))
                        res += (float)rand.NextDouble() * (CHANCE_VALUE * 0.05f); 
                    d.Add(res, si);
                }
            }
            List<float> l = d.Keys.ToList();
            l.Sort();
            l.Reverse();

            for (int i = 0; i < INT_MAX_EXCHANGES; i++)
            {
                intendedSocialExchange.Add(d[l[i]]);
            }
        }

        internal sentenceType calcDialogType()
        {
            Random r = new Random();
            float v = Mathf.Lerp(0,1,(personality.calculating + personality.careful + personality.honor + personality.shy) * ((float)r.NextDouble() * (2*CHANCE_VALUE2 - CHANCE_VALUE2)));
            if (v < CORDIAL_D) return sentenceType.Crude;
            else if (v > CRUDE_D) return sentenceType.Cordial;
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

        public void raiseRomantic(Character c, float v = FRIEND_STEP)
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

        public void lowerRomantic(Character c, float v = FRIEND_STEP)
        {
            raiseRomantic(c, -v);

        }

        internal void overpowered(Character c, float v = FRIEND_STEP)
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

        internal void raiseFriendly(Character c, float v = FRIEND_STEP)
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
                    if(friendlyFeelings[i].getIntensity() > POSITIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Positive);
                    else if(friendlyFeelings[i].getIntensity() > NEGATIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Negative)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < POSITIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Positive)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < NEGATIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Negative);
                }
            }
            if (f == null) friendlyFeelings.Add(new Feeling(c, v, intent.Positive));
        }

        internal void lowerFriendly(Character c, float v = FRIEND_STEP)
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
            socialKnowledge.Add(se, OWN_BELIEF_SURENESS);
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


        internal void raiseEnergy(float e = ENERGY_STEP)
        {
            energy += e - e * (personality.stubborn * 0.05f) - 
                e * (personality.calculating * 0.2f)+ 
                e * (personality.curious * 0.3f) - 
                e * (personality.shy * 0.3f);
            if(energy >= threshold) 
                SubModule.npc.NPCReady(this);
            if (energy > 1) energy = 1;
        }

        internal void spendEnergy(float e = SPEND_ENERGY_STEP)
        {
            energy -= e + e * (personality.shy * 0.3f) -
                e * (personality.annoying * 0.2f);
            if (energy < 0) energy = 0;
        }

        #region Status
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

        #endregion

        internal class Traits
        {
            public float kind; //likeness
            public float stubborn; //chances in the beliefs
            public float liar; //inventing rumors
            public float curious; //wants to know/asks more stuff
            public float helpful; //helps out
            public float shy; //more or ness energy for exchanges 
            public float careful; //i dont know yet
            public float sensitive; //changes in the beliefs
            public float honor; //likeness
            public float charm; //love at first sight

            public float annoying; //energy for exchanges

            public float calculating; //i dont know yet

            public bool isGay; 

            internal Traits()
            {

                var rand = new Random();
                kind = (float) rand.NextDouble() * 2 - 1;
                helpful = (float)rand.NextDouble() * 2 - 1;
                sensitive = (float)rand.NextDouble() * 2 - 1;
                honor = (float)rand.NextDouble() * 2 - 1;
                calculating = (float)rand.NextDouble() * 2 - 1;

                normalTraits(rand);
            }

            public Traits(CharacterTraits charaterTraits)
            {
                
                var rand = new Random();
                
                kind = (float)(rand.NextDouble()) + (charaterTraits.Generosity % 1 / PREVIOUS_TRAITS_WEIGHT); 
                helpful = (float)rand.NextDouble() + (charaterTraits.Mercy % 1 / PREVIOUS_TRAITS_WEIGHT);
                sensitive = (float)rand.NextDouble() - (charaterTraits.Valor % 1 / PREVIOUS_TRAITS_WEIGHT);
                honor = (float)rand.NextDouble() + (charaterTraits.Honor % 1 / PREVIOUS_TRAITS_WEIGHT);
                calculating = (float)rand.NextDouble() + (charaterTraits.Calculating % 1 / PREVIOUS_TRAITS_WEIGHT);


                if(kind == 0)  kind = (float)rand.NextDouble() * 2 - 1; ;
                if(helpful == 0) helpful = (float)rand.NextDouble() * 2 - 1; ;
                if(sensitive == 0) sensitive = (float)rand.NextDouble() * 2 - 1; ;
                if(honor == 0) honor = (float)rand.NextDouble() * 2 - 1; ;
                if(calculating == 0) calculating = (float)rand.NextDouble() * 2 - 1; ;

                normalTraits(rand);
            }

            private void normalTraits(Random rand)
            {
                stubborn = (float)rand.NextDouble() * 2 - 1;
                liar = (float)rand.NextDouble() * 2 - 1;
                curious = (float)rand.NextDouble() * 2 - 1;
                shy = (float)rand.NextDouble() * 2 - 1;
                careful = (float)rand.NextDouble() * 2 - 1;
                charm = (float)rand.NextDouble() * 2 - 1;

                annoying = (float)rand.NextDouble() * 2 - 1;

                isGay = rand.NextDouble() < 0.1;
            }


           
        }

        

        public enum Status
        {
            Wounded=-10,
            Bored=-9,
            Tired=-8,
            Hungry=-7,

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
