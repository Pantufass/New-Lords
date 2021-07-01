﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public static class SocialInteractionManager
    {
        public static SocialInteraction Embelish()
        {

            SocialInteraction t = new SocialInteraction("Embelish", 1, 1);
            t.addInitRule(new InfluenceRule("a", ((List<dynamic> d) =>
            {
                return 10;
            })));

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("SuckUp", out r);
            t.addInitRule(r);

            t.addsentence("You are great");
            t.addsentence("Our soldiers are the best in the world");

            return t;
        }

        public static SocialInteraction BadMouth()
        {

            SocialInteraction t = new SocialInteraction("BadMouth", 1, 1);
            t.addInitRule(new InfluenceRule("a", ((List<dynamic> d) =>
            {
                return 5;
            })));

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("NotHonor", out r);
            t.addInitRule(r);


            t.addsentence("Ugh my boss is killing me");

            return t;
        }

        public static SocialInteraction Complain()
        {

            SocialInteraction t = new SocialInteraction("Complain", 1, 1);
            t.addInitRule(new InfluenceRule("a", ((List<dynamic> d) =>
            {
                return 8;
            })));

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("LowHonor", out r);
            t.addInitRule(r);

            t.addsentence("Im so tired, I can't even");
            t.addsentence("I hate what they are doing to the town");
            t.addsentence("Back in my days things were better");
            t.addsentence("I am always hungry");
            t.addsentence("I see of outsiders around these parts");
            t.addsentence("The security in this town is terrible");
            t.addsentence("The nobles are so lazy, they do nothing all day");


            return t;
        }

        private static void Introduce(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Introduce";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            Condition c;
            SubModule.existingConditions.TryGetValue("NotIntroduced", out c);
            t.addCondition(c);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("NeedToIntroduce", out r);
            t.addInitRule(r);

            t.addInstRule(new InstRule("Introduced", (List<dynamic> d) => Introduction.Introduce((d[0] as Character).characterObject, (d[1] as Character).characterObject)));


            t.addsentence("Hello.", 1, sentenceType.Normal);
            t.addsentence("Good Evening.", 1, sentenceType.Cordial);
            t.addsentence("Hey", 1, sentenceType.Crude);

            t.addsentence("Hi back.", 1, sentenceType.pResponse);
            t.addsentence("Hi.", 1, sentenceType.normalResponse);
            t.addsentence("No.", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Compliment(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Compliment";
            SocialInteraction t = new SocialInteraction(n, 10, 0);


            InfluenceRule r;
            SubModule.existingRules.TryGetValue("ImproveRel", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("IsNice", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("BeingNice", out r);
            t.addInitRule(r);

            t.addsentence("You look good.");
            t.addsentence("You look wonderfuly", 1, sentenceType.Cordial);
            t.addsentence("You look okay", 1, sentenceType.Crude);

            t.addsentence("Thank you so much", 1, sentenceType.pResponse);
            t.addsentence("Thank You", 1, sentenceType.normalResponse);
            t.addsentence("Not really", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Insult(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Insult";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Hurtful", out r);
            t.addInitRule(r);
            SubModule.existingRules.TryGetValue("Hurtful", out r);
            t.addInitRule(r);


            t.addsentence("You look ugly.");
            t.addsentence("You should dedicate yourself to just fighting.", 1, sentenceType.Cordial);
            t.addsentence("You stupid", 1, sentenceType.Crude);


            t.addsentence("Uh", 1, sentenceType.pResponse);
            t.addsentence(" ", 1, sentenceType.normalResponse);
            t.addsentence("No", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Assault(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Assault";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Hurtful", out r);
            t.addInitRule(r);


            t.addsentence("Fight me.");
            t.addsentence("You look like you couldnt handle anyone in a fight", 1, sentenceType.Cordial);
            t.addsentence("Punch", 1, sentenceType.Crude);

            sc.Add(n, t);
        }

        private static void Brag(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Brag";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Gloated", out r);
            t.addInitRule(r,false);

            SubModule.existingRules.TryGetValue("Bored", out r);
            t.addInitRule(r);

            t.addsentence("I am the best.");
            t.addsentence("I can be anything i want.", 1, sentenceType.Cordial);
            t.addsentence("I am better than you", 1, sentenceType.Crude);

            t.addsentence("Yes", 1, sentenceType.pResponse);
            t.addsentence("Uh okay", 1, sentenceType.normalResponse);
            t.addsentence("Not really", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void ConveyStatus(Dictionary<string, SocialInteraction> sc)
        {
            string n = "ConveyStatus";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            InfluenceRule r;

            SubModule.existingRules.TryGetValue("HasStatus", out r);
            t.addInitRule(r,false);

            t.addPath(new Path());

            sc.Add(n, t);
        }

        private static void Converse(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Converse";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Bored", out r);
            t.addInitRule(r);


            SubModule.existingRules.TryGetValue("LikesToSpeak", out r);
            t.addInitRule(r);

            t.addsentence("I like pudding.");
            t.addsentence("I prefer books, not interested in those pesky wars");
            t.addsentence("You seem to enjoy some pudding", 1, sentenceType.Cordial);
            t.addsentence("Can you buy me food?", 1, sentenceType.Crude);

            t.addsentence("Oh really", 1, sentenceType.pResponse);
            t.addsentence("Okay", 1, sentenceType.normalResponse);
            t.addsentence("Dont care", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }
        private static void RunAway(Dictionary<string, SocialInteraction> sc)
        {
            string n = "RunAway";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            t.finish = true;

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Hurtful", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("Feared", out r);
            t.addRespRule(r);

            t.addsentence(" ", 1, sentenceType.Normal);

            t.addsentence(" ", 1, sentenceType.pResponse);
            t.addsentence("Running away?", 1, sentenceType.normalResponse);
            t.addsentence("RUN RUN RUN", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }
        private static void Leave(Dictionary<string, SocialInteraction> sc)
        {
            sc.Add("Leave",Leave());
        }

        public static SocialInteraction Leave()
        {
            SocialInteraction t = new SocialInteraction("Leave", 0, 0);
            t.finish = true;

            t.addInstRule(new InstRule("End", (List<dynamic> d) => PlayerEncounter.LeaveEncounter = true));


            t.addsentence("Goodbye.");
            t.addsentence("I must take my leave", 1, sentenceType.Cordial);
            t.addsentence("Cya", 1, sentenceType.Crude);

            t.addsentence("Goodbye, see you soon", 1, sentenceType.pResponse);
            t.addsentence("Goodbye", 1, sentenceType.normalResponse);
            t.addsentence(" ", 2, sentenceType.nResponse);
            return t;
        }

        //rumor 
        private static void RelayInformation(Dictionary<string, SocialInteraction> sc)
        {
            string n = "RelayInformation";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            Condition c;

            InfluenceRule r;

            SubModule.existingRules.TryGetValue("LikesToSpeak", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("InterestRumor", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("Helpful", out r);
            t.addInitRule(r);

            t.addPath(new Path("I have to tell you something."));


            t.addsentence("I see", 1, sentenceType.pResponse);
            t.addsentence(" ", 1, sentenceType.normalResponse);
            t.addsentence("Hm", 2, sentenceType.nResponse);

            sc.Add(n, t);
        }


        private static void Gossip(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Gossip";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            Condition c;
            SubModule.existingConditions.TryGetValue("NotAvailable", out c);
            t.addCondition(c);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Bored", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("LikesToSpeak", out r);
            t.addInitRule(r);


            t.addsentence("What can you tell me");

            t.addsentence("Sure", 1, sentenceType.pResponse);
            t.addsentence("Hm", 1, sentenceType.normalResponse);
            t.addsentence("No", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }


        //romance
        private static void Flirt(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Flirt";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            t.IsImportant = true;

            Condition c;
            SubModule.existingConditions.TryGetValue("Adults", out c);
            t.addCondition(c);


            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Romantic", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("Charming", out r);
            t.addRespRule(r);

            t.addsentence("Do you want to hang out?");
            t.addsentence("Either you're naked or I'm drunk, possibly both");

            t.addsentence("Yes", 1, sentenceType.pResponse);
            t.addsentence("Uh okay", 1, sentenceType.normalResponse);
            t.addsentence("Not really", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Date(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Date";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            t.IsImportant = true;
            Condition c;
            SubModule.existingConditions.TryGetValue("Adults", out c);
            t.addCondition(c);
            SubModule.existingConditions.TryGetValue("NoRomanceYet", out c);
            t.addCondition(c);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Romantic", out r);
            t.addInitRule(r);

            SubModule.existingRules.TryGetValue("Charming", out r);
            t.addRespRule(r);

            SubModule.existingRules.TryGetValue("IsLiked", out r);
            t.addInitRule(r);

            t.addInstRule(new InstRule("StartDating", (List<dynamic> d) =>
                {
                    if (d.Count < 4) return;

                    Character c1 = d[0] as Character;
                    Character c2 = d[1] as Character;

                    NewCharacterRelationManager.SetRomantic(c1.characterObject, c2.characterObject, NewCharacterRelationManager.relation.Good);
                }));

            t.addsentence("Let's date.");


            t.addsentence("Yes", 1, sentenceType.pResponse);
            t.addsentence("No", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void DeclareLove(Dictionary<string, SocialInteraction> sc)
        {
            string n = "DeclareLove";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            t.IsImportant = true;
            Condition c;
            SubModule.existingConditions.TryGetValue("Adults", out c);
            t.addCondition(c);
            SubModule.existingConditions.TryGetValue("NotAvailable", out c);
            t.addCondition(c);

            InfluenceRule r;
            SubModule.existingRules.TryGetValue("Charming", out r);
            t.addRespRule(r);


            t.addsentence("My passion for...");
            sc.Add(n, t);
        }

        private static void BreakUp(Dictionary<string, SocialInteraction> sc)
        {
            string n = "BreakUp";
            SocialInteraction t = new SocialInteraction(n, 10, 0);

            t.addInstRule(new InstRule("End", (List<dynamic> d) => PlayerEncounter.LeaveEncounter = true));

            Condition c;
            SubModule.existingConditions.TryGetValue("Adults", out c);
            t.addCondition(c);
            SubModule.existingConditions.TryGetValue("NotAvailable", out c);
            t.addCondition(c);
            SubModule.existingConditions.TryGetValue("Romance", out c);
            t.addCondition(c);


            t.addsentence("I don't like you anymore.");
            sc.Add(n, t);
        }

        //lords only
        private static void Kick(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Kick";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            Condition c;
            SubModule.existingConditions.TryGetValue("NotAvailable", out c);
            t.addCondition(c);

            t.addInstRule(new InstRule("End", (List<dynamic> d) => PlayerEncounter.LeaveEncounter = true));


            t.addsentence("Get out right now.");
            sc.Add(n, t);
        }


        private static void Exile(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Exile";
            SocialInteraction t = new SocialInteraction(n, 10, 0);
            Condition c;
            SubModule.existingConditions.TryGetValue("NotAvailable", out c);
            t.addCondition(c);

            SubModule.existingConditions.TryGetValue("LordsOnly", out c);
            t.addCondition(c);


            t.addInstRule(new InstRule("End", (List<dynamic> d) => PlayerEncounter.LeaveEncounter = true));

            t.addsentence("You are not allowed here anymore.");
            sc.Add(n, t);
        }

        public static SocialInteraction War()
        {
            string n = "War";
            SocialInteraction t = new SocialInteraction(n, 0, 0);

            return t;
        }

        public static SocialInteraction Battle()
        {

            string n = "Battle";
            SocialInteraction t = new SocialInteraction(n, 0, 0);

            return t;
        }
        public static Dictionary<string, SocialInteraction> createSocialExchanges()
        {
            Dictionary<string, SocialInteraction> existingExchanges = new Dictionary<string, SocialInteraction>();

            Introduce(existingExchanges);
            Compliment(existingExchanges);
            Insult(existingExchanges);
            Brag(existingExchanges);
            Converse(existingExchanges);
            RunAway(existingExchanges);
            Leave(existingExchanges);

            ConveyStatus(existingExchanges);
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
