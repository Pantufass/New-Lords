using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class TriggerRule : Rule
    {
        public List<Condition> conditions;
        public List<Effect> effects;

        public TriggerRule(string d) : base(d)
        {
            conditions = new List<Condition>();
            effects = new List<Effect>();
        }
        public TriggerRule(string d, int i) : base(d, i)
        {
            conditions = new List<Condition>();
            effects = new List<Effect>();
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
    }
}
