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

        public abstract bool validate();

        protected bool validateTargets()
        {
            return numberTargets == targets.Count();
        }

    }

    public class Condition : Rule
    {
        
        public Func<bool> del;

        public Condition(string d) : base(d)
        {
        }
        public Condition(string d, int n) : base(d, n)
        {
        }

        public void setDel(Func<bool> d)
        {
            del = d;
        }

        public override bool validate()
        {
            if(validateTargets()) return del();
            return false;
        }
    }

    public class InfluenceRule : Rule
    {
        public List<Condition> conditions;

        protected Func<Character, Character, intent, float> del;
        

        public InfluenceRule(string d) : base(d,0)
        {
            conditions = new List<Condition>();
        }

        public InfluenceRule(string d, int i) : base(d,i)
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
            foreach(Condition c in conditions)
            {
                b = b && c.validate();
            }
            return b;
        }

        public float value(Character init, Character rec, intent t)
        {
            return del(init,rec,t);
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
        public TriggerRule(string d, int i) : base(d,i)
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


    public class ConditionManager
    {

        public static Condition NotIntroduced()
        {
            Condition c = new Condition("NotIntroduced");
            c.setDel(()=>Introduction.Introduced(c.targets[0],c.targets[1]));
            return c; 
        }

        public static Condition Adults()
        {
            Condition c = new Condition("Adults");
            c.setDel(() => true);
            return c;
        }

        public static Condition NoRomanceYet()
        {
            Condition c = new Condition("NoRomanceYet");
            c.setDel(() => NewCharacterRelationManager.GetRomantic(c.targets[0].HeroObject, c.targets[1].HeroObject) < NewCharacterRelationManager.relation.Good);
            return c;
        }

        public static Condition Romance()
        {
            Condition c = new Condition("Romance");
            c.setDel(() => NewCharacterRelationManager.GetRomantic(c.targets[0].HeroObject, c.targets[1].HeroObject) > NewCharacterRelationManager.relation.Neutral);
            return c;
        }

        public static Condition LordsOnly()
        {
            Condition c = new Condition("LordsOnly");
            c.setDel(() => c.targets[0].Occupation == Occupation.Lord && c.targets[1].Occupation == Occupation.Lord);
            return c;
        }

        public static Condition NotAvailable()
        {
            Condition c = new Condition("LordsOnly");
            c.setDel(() => false);
            return c;
        }
    }
}
