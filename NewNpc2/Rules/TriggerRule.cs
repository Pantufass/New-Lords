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
        protected List<Effect> effects;

        public TriggerRule(string d, Action<Character, Character, intent, outcome> f) : base(d)
        {
            conditions = new List<Condition>();
            effects = new List<Effect>();
            effects.Add(new Effect(f));
        }
        public TriggerRule(string d, int i) : base(d, i)
        {
            conditions = new List<Condition>();
            effects = new List<Effect>();
        }

        public void addEffect(Action<Character, Character, intent, outcome> f)
        {
            effects.Add(new Effect(f));
        }

        public void runEffects(Character c1, Character c2, intent i, outcome o)
        {
            foreach(Effect e in effects)
            {
                e.run(c1, c2, i,o);
            }
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

        protected class Effect
        {
            protected Action<Character, Character, intent, outcome> del;
            public Effect(Action<Character, Character, intent, outcome> d)
            {
                del = d;
            }

            public void run(Character c1, Character c2, intent i, outcome o)
            {
                del(c1,c2,i,o);
            }
        }
    }

    

}
