using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class SocialInteraction
    {
        public string name;
        //list of preconditions
        private List<Condition> preconditions;

        //threshold for a positive response
        public float upperThresh;
        //treshhold for a neutral response
        public float lowerThresh;

        public bool finish;

        public List<Dialog> sentences;

        protected List<InfluenceRule> initRules;
        protected List<InfluenceRule> respRules;

        public SocialInteraction(string n, float up, float low)
        {
            name = n;
            preconditions = new List<Condition>();
            sentences = new List<Dialog>();
            upperThresh = up;
            lowerThresh = low;
            addsentence("...");
            finish = false;

            initRules = new List<InfluenceRule>();
            respRules = new List<InfluenceRule>();
        }

        public void addInitRule(InfluenceRule rule)
        {
            initRules.Add(rule);
            respRules.Add(rule);
        }

        public void addInitRule(InfluenceRule rule, bool b)
        {
            initRules.Add(rule);
            if (b) respRules.Add(rule);
        }

        public void addRespRule(InfluenceRule rule)
        {
            respRules.Add(rule);
        }

        public void addsentence(string sentence)
        {
            sentences.Add(new Dialog(sentence, 0, sentenceType.Normal));
        }
        public void addsentence(string sentence, float v, sentenceType t)
        {
            sentences.Add(new Dialog(sentence, v, t));
        }
        public void addCondition(Condition c)
        {
            preconditions.Add(c);
        }

        //return a sum of the preconditions
        public bool validate(CharacterObject c1, CharacterObject c2)
        {
            bool b = true;
            foreach (Condition c in preconditions)
            {
                c.target1 = c1;
                c.target2 = c2;
                b = b && c.validate();
            }
            return b;
        }

        //TODO calc intent
        //return a sum of the influence rules
        public float calculateVolition(Character init, Character rec, intent intent)
        {
            float res = 0;
            foreach (InfluenceRule r in initRules)
            {
                if (r.validate()) res += r.value(init, rec, intent);
            }
            foreach (InfluenceRule r in init.getRules())
            {
                if (r.validate()) res += r.value(init, rec, intent);
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

        //TODO pick sentence
        public string getDialogLine(sentenceType t, float v)
        {
            return (sentences.Count > 1 ? sentences[1].sentence : sentences[0].sentence);
        }

        public string getResponse(float v)
        {
            return (v > upperThresh ? getDialogLine(sentenceType.pResponse, v) : v > lowerThresh ? getDialogLine(sentenceType.normalResponse, v) : getDialogLine(sentenceType.nResponse, v));

        }

        public outcome GetOutcome(float v)
        {
            return (v > upperThresh ? outcome.Positive : v > lowerThresh ? outcome.Neutral : outcome.Negative);
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
}
