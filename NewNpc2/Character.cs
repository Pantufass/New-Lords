using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace NewNpc2
{
    public class Character
    {
        //class with the personality traits
        public Traits personality;

        //List of possible statuses
        private List<Status> status;

        //likes?

        //social network
        //3 unidirectional relations between this and other characters
        private List<Feeling> friendlyFeelings;
        private List<Feeling> romanticFeelings;
        private List<Feeling> admiration;

        //belief network
        //it is a list of relations with 2 characters as reference as well as a goal and intensity (how strongly he believes in that)
        public Dictionary<Character,Feeling> beliefs;

        //list of social exchanges known
        //each exchange is paired with the respective believability 
        private Dictionary<SocialExchange, float> socialKnowledge;

        //cultural knowledge TODO probably 
        private CulturalKnowledge culture;

        //TODO
        //intention type
        //calc intent/goals
        private SocialExchangeType intendedSocialExchange;

        //TODO rumor
        private Rumor rumor;

        //TODO influence rules
        private List<InfluenceRule> influenceRules;

        public Character(CulturalKnowledge culture)
        {
            personality = new Traits();
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new Dictionary<Character, Feeling>();
            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<InfluenceRule>();

            rumor = null;
            this.culture = culture;

            initialState();
        }

        public Character(CulturalKnowledge culture, CharacterTraits characterTraits)
        {
            personality = new Traits(characterTraits);
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new Dictionary<Character, Feeling>();

            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<InfluenceRule>();
            rumor = null;
            this.culture = culture;

            initialState();
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

        }

        public void addInfluenceRule(InfluenceRule r)
        {
            influenceRules.Add(r);
        }

        public List<InfluenceRule> getRules()
        {
            return influenceRules;
        }

        public void addIntendedSocialExchange(SocialExchangeType se)
        {
            this.intendedSocialExchange = se;
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
        public bool isGloated()
        {
            return status.Contains(Status.Gloated);
        }
        public bool isBored()
        {
            return status.Contains(Status.Bored);
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
                b += (int) s;
            }
            return b >= 0;
        }
        

        public class Traits
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

            public Traits()
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
            }


           
        }

        

        enum Status : int
        {
            Wounded=-1,
            Bored=-1,
            Tired=-1,
            Hungry=-1,

            //?
            SocialInteract=0,

            Angry=-1,
            Feared=0,
            Confident=1,
            Happy=1,
            Sad=-1,
            Surprised=0,

            Ashamed=0,
            Pity=0,
            Gloated=0,
            Resented=-1,
            HappyFor=1
        }
    }

}
