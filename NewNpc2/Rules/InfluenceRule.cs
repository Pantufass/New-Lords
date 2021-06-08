using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class InfluenceRule : Rule
    {
        public List<Condition> conditions;

        protected Func<List<dynamic>, float> del;


        public InfluenceRule(string d) : base(d)
        {
            conditions = new List<Condition>();
        }

        public InfluenceRule(string d, Func<List<dynamic>, float> dele) : base(d)
        {
            conditions = new List<Condition>();
            del = dele;
        }

        public void addCondition(Condition c)
        {
            conditions.Add(c);
        }

        public void setDel(Func<List<dynamic>, float> d)
        {
            del = d;
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

        public float value(List<dynamic> d)
        {
            return del(d);
        }
    }
}
