using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public abstract class Rule
    {
        public string description;
        public List<Character> targets; 

        public Rule(string d)
        {
            description = d;
            targets = new List<Character>();
        }

        public void addTarget(Character target)
        {
            targets.Add(target);
        }

    }

    public class Condition : Rule
    {
        public Condition(string d) : base(d)
        {

        }
    }

    public class InfluenceRule : Rule
    {
        public List<Condition> conditions;

        public InfluenceRule(string d) : base(d)
        {
            conditions = new List<Condition>();
        }

        public void addCondition(Condition c)
        {
            conditions.Add(c);
        }

        public float value()
        {
            return 0;
        }
    }
}
