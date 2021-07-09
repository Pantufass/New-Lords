using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class InstRule : Rule
    {
        protected List<Condition> conditions;
        protected Action<List<dynamic>> del;

        public InstRule(string d, Action<List<dynamic>> f = null) : base(d)
        {
            conditions = new List<Condition>();
            del = f;
        }


        public void runEffects(List<dynamic> d = null)
        {
            if (validate(d)) del(d);
        }



        public void addCondition(Condition c)
        {
            conditions.Add(c);
        }

        public override bool validate(List<dynamic> d = null)
        {
            bool b = true;
            foreach (Condition c in conditions)
            {
                b = b && c.validate(d);
            }
            return b;
        }



    }

    public class Path : InstRule
    {
        protected int depth;
        protected string sent;
        protected SocialInteraction si;

        public Path(SocialInteraction si, string s = null, int d = 0) : base("path")
        {
            depth = d;
            conditions = new List<Condition>();
            sent = s;
            this.si = si;
        }

        public int getDepth()
        {
            return depth;
        }

        public Dialog sentence(Character c)
        {
            if (si.name.Equals("ConveyStatus"))
            {
                if (c != null) return sentence(c.getRandStatus());
                else return null;
            }
            if (si.name.Equals("RelayInformation"))
            {
                if (c != null) return sentence(c.getRumor());
                else return null;
            }
            if (sent != null) return new Dialog(sent, 0, sentenceType.Normal);
            else return null;
        }

        private Dialog sentence(Character.Status s)
        {
            StringBuilder sb = new StringBuilder(" ", 30);
            sb.Append("I am ");
            if (s != Character.Status.Normal) sb.Append(s.ToString()); 
            else sb.Append("okay");
            return new Dialog(sb.ToString(), 0, sentenceType.Normal);
        }

        private Dialog sentence(Rumor r)
        {
            if (r == null) return new Dialog("I dont know anything special",0,sentenceType.Normal);
            StringBuilder sb = new StringBuilder(" ", 50);
            if(SubModule.rand.Next(5) < 3)  sb.Append("Have you heard, ");
            if (r.info.getType() == Rumor.Information.type.Economic)
            {
                sb.Append("In ");
                sb.Append(r.info.values[0]);
                sb.Append(", ");
                sb.Append(r.info.values[1]);
                sb.Append("is at ");
                sb.Append(r.info.values[2]);

            }
            else
            {
                if (r.info.getType() == Rumor.Information.type.Warfare)
                {
                    if (r.info.war)
                    {
                        sb.Append(r.info.values[0]);
                        sb.Append(" has declared war against ");
                        sb.Append(r.info.values[1]);
                    }
                    else
                    {
                        sb.Append(r.exchange().getInitiator());
                        sb.Append(" has won a battle agaisnt ");
                        sb.Append(r.exchange().getReceiver());
                    }
                }
                else if (r.info.getType() == Rumor.Information.type.Gossip)
                {
                    switch (r.exchange().type.name)
                    {
                        case "Flirt":
                            sb.Append("They flirting");
                            break;
                        case "Date":
                            sb.Append("They dating");
                            break;
                        default :
                            break;
                    }
                }
            }


            return new Dialog(sb.ToString(),0,sentenceType.Normal);
        }

    }
}

    
