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
        private Traits personality;

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
        private List<Belief> beliefs;

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
        private List<Rule> influenceRules;

        public Character(CulturalKnowledge culture)
        {
            personality = new Traits();
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new List<Belief>();

            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<Rule>();

            this.culture = culture;
        }

        public Character(CulturalKnowledge culture, CharacterTraits characterTraits)
        {
            personality = new Traits(characterTraits);
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new List<Belief>();

            socialKnowledge = new Dictionary<SocialExchange, float>();

            influenceRules = new List<Rule>();

            this.culture = culture;
        }

        public void addInfluenceRule(Rule r)
        {

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

        public double isNice()
        {
            return personality.kind;
        }

        internal class Traits
        {
            public double kind;
            public double stubborn;
            public double liar;
            public double curious;
            public double helpful;
            public double shy;
            public double careful;
            public double sensitive;
            public double honor;
            public double charm;

            public double annoying;

            public double calculating;

            public Traits()
            {

                var rand = new Random();
                kind = rand.NextDouble() * 2 - 1;
                stubborn = rand.NextDouble() * 2 - 1;
                liar = rand.NextDouble() * 2 - 1;
                curious = rand.NextDouble() * 2 - 1;
                helpful = rand.NextDouble() * 2 - 1;
                shy = rand.NextDouble() * 2 - 1;
                careful = rand.NextDouble() * 2 - 1;
                sensitive = rand.NextDouble() * 2 - 1;
                honor = rand.NextDouble() * 2 - 1;
                charm = rand.NextDouble() * 2 - 1;

                annoying = rand.NextDouble() * 2 - 1;

                calculating = rand.NextDouble() * 2 - 1;
            }

            public Traits(CharacterTraits charaterTraits)
            {
                
                var rand = new Random();
                kind = (rand.NextDouble()) * charaterTraits.Generosity % 1; 
                stubborn = rand.NextDouble() * 2 - 1;
                liar = rand.NextDouble() * 2 - 1;
                curious = rand.NextDouble() * 2 - 1;
                helpful = rand.NextDouble() * charaterTraits.Mercy % 1;
                shy = rand.NextDouble() * 2 - 1;
                careful = rand.NextDouble() * 2 - 1;
                sensitive = rand.NextDouble() * -charaterTraits.Valor % 1;
                honor = rand.NextDouble() * charaterTraits.Honor % 1;
                charm = rand.NextDouble() * 2 - 1;

                annoying = rand.NextDouble() * 2 - 1;

                calculating = rand.NextDouble() * charaterTraits.Calculating % 1;


                if(kind == 0)  kind = rand.NextDouble() * 2 - 1; ;
                if(helpful == 0) helpful = rand.NextDouble() * 2 - 1; ;
                if(sensitive == 0) sensitive = rand.NextDouble() * 2 - 1; ;
                if(honor == 0) honor = rand.NextDouble() * 2 - 1; ;
                if(calculating == 0) calculating = rand.NextDouble() * 2 - 1; ;
            }


           
        }

        

        enum Status
        {
            Wounded,
            Bored,
            Tired,
            Hungry,

            //?
            SocialInteract,

            Angry,
            Feared,
            Confident,
            Happy,
            Sad,
            Surprised,

            Ashamed,
            Pity,
            Gloated,
            Resented,
            HappyFor
        }
    }

}
