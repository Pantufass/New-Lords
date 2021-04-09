using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace NewNpc2
{
    public abstract class Rule
    {
        public string description;
        public CharacterObject target1;
        public CharacterObject target2;
        public int numberTargets;

        public Rule(string d)
        {
            description = d;
            numberTargets = 2;
        }

        public Rule(string d,int n)
        {
            description = d;
            numberTargets = n;
        }


        public abstract bool validate();

        protected bool validateTargets()
        {
            return target1 != null && target2 != null;
           // return numberTargets == targets.Count();
        }

    }

    

    
}
