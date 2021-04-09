using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class ConditionManager
    {

        public static Condition NotIntroduced()
        {
            Condition c = new Condition("NotIntroduced");
            c.setDel(() => !(Introduction.Introduced(c.target1, c.target2)));
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
            c.setDel(() => NewCharacterRelationManager.GetRomantic(c.target1.HeroObject, c.target2.HeroObject) < NewCharacterRelationManager.relation.Good);
            return c;
        }

        public static Condition Romance()
        {
            Condition c = new Condition("Romance");
            c.setDel(() => NewCharacterRelationManager.GetRomantic(c.target1.HeroObject, c.target2.HeroObject) > NewCharacterRelationManager.relation.Neutral);
            return c;
        }

        public static Condition LordsOnly()
        {
            Condition c = new Condition("LordsOnly");
            c.setDel(() => c.target1.Occupation == Occupation.Lord && c.target2.Occupation == Occupation.Lord);
            return c;
        }

        public static Condition NotAvailable()
        {
            Condition c = new Condition("NotAvailable");
            c.setDel(() => false);
            return c;
        }
    }
}
