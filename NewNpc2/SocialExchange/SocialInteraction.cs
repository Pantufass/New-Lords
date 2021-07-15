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

        public List<Tuple<Dialog,Dialog>> sentences;

        protected List<InfluenceRule> initRules;
        protected List<InfluenceRule> respRules;
        protected List<InstRule> instRules;

        protected List<Path> paths;

        public bool hasPaths;
        public bool turnOff;

        public SocialInteraction(string n, float up, float low)
        {
            name = n;
            preconditions = new List<Condition>();
            sentences = new List<Tuple<Dialog, Dialog>>();
            upperThresh = up;
            lowerThresh = low;

            addsentence("Okay", 0, sentenceType.normalResponse);
            addsentence("Hm", 0, sentenceType.normalResponse);
            addsentence("I don't care", 0, sentenceType.nResponse);

            finish = false;
            hasPaths = false;

            initRules = new List<InfluenceRule>();
            respRules = new List<InfluenceRule>();
            instRules = new List<InstRule>();
            paths = new List<Path>();

            turnOff = true;
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

        public void followUp(Dialog d, CampaignGameStarter c,string token)
        {
            //TODO
            Dialog res = findResponse(d);
            if(res != null)
            {
                res.cresponse = true;
                c.AddDialogLine(name+"followUp", token, "npcfollowup" , res.sentence,
                        () => res.cresponse,
                        () => {
                            res.cresponse = false;
                            nextFollowUp();
                        },
                        1001, null);
            }
        }

        private void nextFollowUp()
        {

        }

        private Dialog findResponse(Dialog d)
        {
            foreach(Tuple<Dialog,Dialog> pair in sentences)
            {
                if (pair.Item1 == d) return pair.Item2;
            }
            return null;
        }


        public void addsentence(string sentence, string sentence2 = null)
        {
            Dialog d = null;
            if (sentence2 != null) d = new Dialog(sentence2, 0, sentenceType.normalResponse);
            sentences.Add(new Tuple<Dialog, Dialog> (new Dialog(sentence, 0, sentenceType.Normal),d));
        }
        public void addsentence(string sentence, float v, sentenceType t, string sentence2 = null, float v2 = 0, sentenceType t2 = sentenceType.normalResponse)
        {
            Dialog d = null;
            if (sentence2 != null) d = new Dialog(sentence2, v2, t2);
            sentences.Add(new Tuple<Dialog,Dialog>(new Dialog(sentence, v, t),d));
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
            Dialog res = sentences[0].Item1;

            IEnumerable<Dialog> l = (from d in sentences
                                    where d.Item1.type == t
                                    orderby d.Item1.value descending
                                    select d.Item1);

            if (l.Count() > 0)
            {
                int index = SubModule.rand.Next(0, l.Count());
                res = l.ElementAt(index);
            }

            else if (t == sentenceType.Cordial || t == sentenceType.Crude)
                return getTheDialog(sentenceType.Normal, v);

            return res;
        }

        public string getDialogLine(sentenceType t, float v)
        {
            return getTheDialog(t, v).sentence;
        }

        public void chooseDialog(sentenceType t, float v, bool player = false)
        {
            Dialog d = getTheDialog(t, v);
            foreach(Tuple<Dialog,Dialog> dd in sentences)
            {
                if(d == dd.Item1)
                {
                    dd.Item1.playera = player; 
                    dd.Item1.cresponse = !player;
                }
            }
        }

        public Dialog chooseDialog(sentenceType t, Character r = null)
        {
            if(hasPaths) return paths.First().sentence(r);
            return getTheDialog(t, 0);
        }

        public Dialog getDialog(sentenceType t, float v, Character r = null)
        {

            if (hasPaths)
            {
                Dialog d = paths.First().sentence(r);
                if (d != null) return d;
            }
            return getTheDialog(t, v);
        }

        public void clearDialog()
        {
            foreach (Tuple<Dialog,Dialog> d in sentences)
            {
                d.Item1.cresponse = false;
                d.Item1.playera = false;
            }
        }

        public string getResponse(float v,Character c = null)
        {
            if (hasPaths && paths.Count > 1)
            {
                Dialog d = paths[1].sentence(c);
                if (d != null) return d.sentence;
            }
            return (v > upperThresh ? getDialogLine(sentenceType.pResponse, v) : v > lowerThresh ? getDialogLine(sentenceType.normalResponse, v) : getDialogLine(sentenceType.nResponse, v));

        }

        public SocialInteraction(SocialInteraction si)
        {
            name = si.name + " changed";
            preconditions = new List<Condition>();
            sentences = new List<Tuple<Dialog, Dialog>>();
            upperThresh = si.upperThresh;
            lowerThresh = si.lowerThresh;
            addsentence("...");
            finish = si.finish;

            initRules = new List<InfluenceRule>();
            respRules = new List<InfluenceRule>();
            instRules = new List<InstRule>();

            foreach (Condition c in si.preconditions) preconditions.Add(c);
            foreach (Tuple<Dialog, Dialog> c in si.sentences) sentences.Add(c);
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
            foreach (Dialog d in id.dialog) sentences.Add(new Tuple<Dialog,Dialog>(d,null));
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
