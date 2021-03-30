using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public abstract class Rule
    {
        public string description;
        public List<CharacterObject> targets;
        public int numberTargets;

        public Rule(string d)
        {
            description = d;
            targets = new List<CharacterObject>();
            numberTargets = 2;
        }

        public Rule(string d,int n)
        {
            description = d;
            targets = new List<CharacterObject>();
            numberTargets = n;
        }

        public void addTarget(CharacterObject target)
        {
            targets.Add(target);
        }

        protected bool validateTargets()
        {
            return numberTargets == targets.Count();
        }

    }

    public class Condition : Rule
    {
        protected conditions name;
        //TODO
        protected Func<bool> del;

        public Condition(conditions name, string d) : base(d)
        {
            this.name = name;
        }
        public Condition(conditions name,string d, int n) : base(d,n)
        {
            this.name = name;
        }

        public bool validate()
        {
            if(validateTargets()) return ConditionManager.Run(name, targets[1], targets[2]);
            return false;
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

        public bool validate()
        {
            bool b = true;
            foreach(Condition c in conditions)
            {
                b = b && c.validate();
            }
            return b;
        }

        public float value()
        {
            return 0;
        }
    }

    public class TriggerRule : Rule
    {
        public List<Condition> conditions;
        public List<Effect> effects;

        public TriggerRule(string d) : base(d)
        {
            conditions = new List<Condition>();
            effects = new List<Effect>();
        }
    }

    public enum conditions
    {
        LordsOnly,
        Romance,
        Adults,
        NoRomance,
        NotIntroduced

    }

    public static class ConditionManager
    {
        public static bool Run(conditions c, CharacterObject target1, CharacterObject target2)
        {
            switch (c)
            {
                case conditions.LordsOnly:
                    return target1.Occupation == Occupation.Lord && target2.Occupation == Occupation.Lord;
                case conditions.Romance:
                    NewCharacterRelationManager.relation r1 = NewCharacterRelationManager.GetRomantic(target1.HeroObject, target2.HeroObject);
                    return r1 == NewCharacterRelationManager.relation.Good || r1 == NewCharacterRelationManager.relation.Great;
                case conditions.Adults:
                    return true;
                case conditions.NoRomance:
                    NewCharacterRelationManager.relation r2 = NewCharacterRelationManager.GetRomantic(target1.HeroObject, target2.HeroObject);
                    return r2 == NewCharacterRelationManager.relation.Terrible || r2 == NewCharacterRelationManager.relation.Bad || r2 == NewCharacterRelationManager.relation.Neutral;
                case conditions.NotIntroduced:
                    return Introduction.Introduced(target1, target2);
                default:
                    return false;
            }
        }

        public static bool Run(conditions c, Character target1, Character target2)
        {

            return false;
        }
    }
}
