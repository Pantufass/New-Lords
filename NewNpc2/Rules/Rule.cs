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

        public Rule(string d)
        {
            description = d;
        }


        public abstract bool validate(List<dynamic> d);


    }

    

    
}
