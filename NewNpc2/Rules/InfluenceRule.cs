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

        protected Func<Character, Character, intent, float> del;


        public InfluenceRule(string d) : base(d, 0)
        {
            conditions = new List<Condition>();
        }

        public InfluenceRule(string d, int i) : base(d, i)
        {
            conditions = new List<Condition>();

        }


        public void addCondition(Condition c)
        {
            conditions.Add(c);
        }

        public void setDel(Func<Character, Character, intent, float> d)
        {
            del = d;
        }

        public override bool validate()
        {
            bool b = true;
            foreach (Condition c in conditions)
            {
                b = b && c.validate();
            }
            return b;
        }

        public float value(Character init, Character rec, intent t)
        {
            return del(init, rec, t);
        }
    }
}
