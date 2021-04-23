using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class TriggerRule : Rule
    {
        protected List<Condition> conditions; 
        protected Action<Character, Character, intent, outcome> del;

        public TriggerRule(string d, Action<Character, Character, intent, outcome> f) : base(d)
        {
            conditions = new List<Condition>();
            del = f;
        }

        public void runEffects(Character c1, Character c2, intent i, outcome o)
        {
            if(validate()) del(c1, c2, i,o);
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
