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
        public Path(int d = 0) : base("path")
        {
            depth = d;
            conditions = new List<Condition>();
        }

        public int getDepth()
        {
            return depth;
        }

        public string sentence(Rumor r)
        {
            if (r == null) return "I dont know anything special";
            StringBuilder sb = new StringBuilder(" ", 50);
            sb.Append("Have you heard, ");
            if (r.info.getType() == Rumor.Information.type.Economic)
            {
                sb.Append("In ");
                sb.Append(r.info.economic[0]);
                sb.Append(", ");
                sb.Append(r.info.economic[1]);
                sb.Append("is at ");
                sb.Append(r.info.economic[2]);

            }
            else
            {
                if (r.info.getType() == Rumor.Information.type.Warfare)
                {
                    if (r.info.war)
                    {
                        sb.Append(r.info.warfare[0]);
                        sb.Append(" has declared war against ");
                        sb.Append(r.info.warfare[1]);
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
                            break;
                        case "Date":
                            break;
                        default :
                            break;
                    }
                }
            }


            return sb.ToString();
        }

    }
}

    
