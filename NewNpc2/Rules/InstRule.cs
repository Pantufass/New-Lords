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


        //TODO Information
        public static void RelayRumor(List<dynamic> f)
        {

        }

        //TODO Information
        public static void NextRelay(List<dynamic> f)
        {

        }

    }
}

    
