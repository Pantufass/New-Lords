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

        public bool IsImportant = false;

        public List<Dialog> sentences;

        protected List<InfluenceRule> initRules;
        protected List<InfluenceRule> respRules;
        protected List<InstRule> instRules;

        protected List<Path> paths;

        public bool hasPaths;

        public SocialInteraction(string n, float up, float low)
        {
            name = n;
            preconditions = new List<Condition>();
            sentences = new List<Dialog>();
            upperThresh = up;
            lowerThresh = low;
            addsentence("...");
            finish = false;
            hasPaths = false;

            initRules = new List<InfluenceRule>();
            respRules = new List<InfluenceRule>();
            instRules = new List<InstRule>();
            paths = new List<Path>();


        }



        public void addInitRule(InfluenceRule rule, bool b = true)
        {
            initRules.Add(rule);
            if (b) respRules.Add(rule);
        }

        public void addRespRule(InfluenceRule rule)
        {
            respRules.Add(rule);
        }

        public void addInstRule(InstRule rule)
        {
            instRules.Add(rule);
        }

        public void addPath(Path p)
        {
            hasPaths = true;
            paths.Add(p);
        }

        public List<InstRule> getInstRules()
        {
            return instRules;
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
        public bool validate(BasicCharacterObject c1, BasicCharacterObject c2)
        {
            List<dynamic> l = new List<dynamic>();
            l.Add(c1);
            l.Add(c2);

            bool b = true;
            foreach (Condition c in preconditions)
            {
                b = b && c.validate(l);
            }
            return b;
        }

        //return a sum of the influence rules
        public float calculateVolition(Character init, Character rec, intent intent)
        {
            List<dynamic> l = new List<dynamic>();
            l.Add(init);
            l.Add(rec);
            l.Add(intent);
            l.Add(this);

            float res = 0;
            foreach (InfluenceRule r in initRules)
            {
                if (r.validate(l)) res += r.value(l);
            }
            foreach (InfluenceRule r in init.getRules())
            {
                if (r.validate(l)) res += r.value(l);
            }

            return res;
        }


        public List<InfluenceRule> getRespRules()
        {
            return respRules;
        }

        private Dialog getTheDialog(sentenceType t, float v)
        {
            Dialog res = sentences[0];
            IEnumerable<Dialog> l = from d in sentences
                                    where d.type == t
                                    orderby d.value descending
                                    select d;
            if (l.Count() > 0)
            {
                if (Mathf.Lerp(0, 1, v) > 0.5) res = l.Last();
                else res = l.First();
            }

            return res;
        }

        public string getDialogLine(sentenceType t, float v)
        {
            return getTheDialog(t, v).sentence;
        }

        public void chooseDialog(sentenceType t, float v, bool player = false)
        {
            Dialog d = getTheDialog(t, v);
            d.playera = player;
            d.cresponse = !player;
        }

        public Dialog getDialog(sentenceType t, float v, Character r = null)
        {
            if (hasPaths) return paths.First().sentence(r);
            return getTheDialog(t, v);
        }

        public void clearDialog()
        {
            foreach (Dialog d in sentences)
            {
                d.cresponse = false;
                d.playera = false;
            }
        }

        public string getResponse(float v)
        {
            return (v > upperThresh ? getDialogLine(sentenceType.pResponse, v) : v > lowerThresh ? getDialogLine(sentenceType.normalResponse, v) : getDialogLine(sentenceType.nResponse, v));

        }

        public SocialInteraction(SocialInteraction si)
        {
            name = si.name + " changed";
            preconditions = new List<Condition>();
            sentences = new List<Dialog>();
            upperThresh = si.upperThresh;
            lowerThresh = si.lowerThresh;
            addsentence("...");
            finish = si.finish;

            initRules = new List<InfluenceRule>();
            respRules = new List<InfluenceRule>();
            instRules = new List<InstRule>();

            foreach (Condition c in si.preconditions) preconditions.Add(c);
            foreach (Dialog c in si.sentences) sentences.Add(c);
            foreach (InfluenceRule c in si.initRules) initRules.Add(c);
            foreach (InfluenceRule c in si.respRules) respRules.Add(c);
            foreach (InstRule c in si.instRules) instRules.Add(c);
        }

        public void setCultureSet(InteractionData id)
        {
            foreach (InfluenceRule ir in id.rules)
            {
                initRules.Add(ir);
                respRules.Add(ir);
            }
            foreach (InstRule ir in id.inst) instRules.Add(ir);
            foreach (Dialog d in id.dialog) sentences.Add(d);
        }

        public bool Equals(SocialInteraction obj)
        {
            return obj.name.Equals(this.name);
        }
    }


    public enum intent
    {
        Neutral,
        Positive,
        Negative,
        Romantic,
        Embellish
    }
}
