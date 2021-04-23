using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public static class SocialInteractionManager
    {

        private static void Introduce(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Introduce";
            SocialInteraction t = new SocialInteraction(n, 1, 1);

            t.addCondition(ConditionManager.NotIntroduced());

            InfluenceRule r = new InfluenceRule("NeedtoIntroduce");
            r.setDel((Character c1, Character c2, intent it) => 100);
            t.addInitRule(r);

            t.addTriggerRule(new TriggerRule("Introduced", (Character c1, Character c2, intent it, outcome o) => Introduction.Introduce(c1.characterObject, c2.characterObject)));


            t.addsentence("Hello.", 1, sentenceType.Normal);

            t.addsentence("Hi back.", 1, sentenceType.pResponse);
            t.addsentence("No.", 2, sentenceType.nResponse);
            sc.Add(n, t);
        }

        private static void Compliment(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Compliment";
            SocialInteraction t = new SocialInteraction(n, 1, 1);


            InfluenceRule r = new InfluenceRule("ImproveRel"); //is positive
            r.setDel((Character c1, Character c2, intent it) => 5 * (it == intent.Positive ? 1 : -1));
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

        private static void Insult(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Insult";
            SocialInteraction t = new SocialInteraction(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Hurtful");//return 5 if not good
            r.setDel((Character c1, Character c2, intent it) => c1.isGood() ? 0 : 10);
            t.addInitRule(r);


            t.addsentence("You look stupid.");
            sc.Add(n, t);
        }

        private static void Brag(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Brag";
            SocialInteraction t = new SocialInteraction(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Gloated");
            r.setDel((Character c1, Character c2, intent it) => c1.isGloated() ? 10 : 0);
            t.addInitRule(r);

            t.addsentence("I am the best.");
            sc.Add(n, t);
        }

        private static void Converse(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Converse";
            SocialInteraction t = new SocialInteraction(n, 1, 1);

            InfluenceRule r = new InfluenceRule("Bored");
            r.setDel((Character c1, Character c2, intent it) => c1.isBored() ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("Entertain");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Entertain ? 10 : 0);
            t.addInitRule(r);

            r = new InfluenceRule("LikesToSpeak");
            r.setDel((Character c1, Character c2, intent it) => 5);
            t.addInitRule(r);

            t.addTriggerRule(new TriggerRule("Raise", (Character c1, Character c2, intent it, outcome o) => CharacterManager.MainCharacter.addStatus(Character.Status.Happy)));


            t.addsentence("I like pudding.");
            sc.Add(n, t);
        }
        private static void RunAway(Dictionary<string, SocialInteraction> sc)
        {
            string n = "RunAway";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.finish = true;

            InfluenceRule r = new InfluenceRule("Hurtful");//return 5 if not good
            r.setDel((Character c1, Character c2, intent it) => c1.isGood() ? 0 : 10);
            t.addInitRule(r);

            r = new InfluenceRule("Feared");
            r.setDel((Character c1, Character c2, intent it) => c1.isFeared() ? 10 : 0);
            t.addInitRule(r);

            sc.Add(n, t);
        }
        private static void Leave(Dictionary<string, SocialInteraction> sc)
        {
            sc.Add("Leave",Leave());
        }

        public static SocialInteraction Leave()
        {
            SocialInteraction t = new SocialInteraction("Leave", 1, 1);
            t.finish = true;
            t.addsentence("Goodbye.");
            return t;
        }

        //rumor
        private static void RelayInformation(Dictionary<string, SocialInteraction> sc)
        {
            string n = "RelayInformation";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.NotAvailable());

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

        private static void Gossip(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Gossip";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.NotAvailable());

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
        private static void Flirt(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Flirt";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.NotAvailable());

            InfluenceRule r = new InfluenceRule("Romantic");
            r.setDel((Character c1, Character c2, intent it) => it == intent.Romantic ? 10 : 0);
            t.addInitRule(r);

            t.addsentence("Do you want to hang out?");
            sc.Add(n, t);
        }

        private static void Date(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Date";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.NoRomanceYet());
            t.addCondition(ConditionManager.NotAvailable());

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

        private static void DeclareLove(Dictionary<string, SocialInteraction> sc)
        {
            string n = "DeclareLove";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.NotAvailable());



            t.addsentence("My passion for...");
            sc.Add(n, t);
        }

        private static void BreakUp(Dictionary<string, SocialInteraction> sc)
        {
            string n = "BreakUp";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.Adults());
            t.addCondition(ConditionManager.Romance());
            t.addCondition(ConditionManager.NotAvailable());



            t.addsentence("I don't like you anymore.");
            sc.Add(n, t);
        }

        //lords only
        private static void Kick(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Kick";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.NotAvailable());



            t.addsentence("Get out right now.");
            sc.Add(n, t);
        }


        private static void Exile(Dictionary<string, SocialInteraction> sc)
        {
            string n = "Exile";
            SocialInteraction t = new SocialInteraction(n, 1, 1);
            t.addCondition(ConditionManager.LordsOnly());
            t.addCondition(ConditionManager.NotAvailable());


            t.addsentence("You are not allowed here anymore.");
            sc.Add(n, t);
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
