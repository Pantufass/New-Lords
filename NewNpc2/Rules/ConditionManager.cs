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

        private static Condition NotIntroduced()
        {
            Condition c = new Condition("NotIntroduced", (List<dynamic> d) => {
                if (d.Count < 2) return false;
                if (d[0] is Character && d[1] is Character)
                    return Introduction.Introduced((d[0] as Character).characterObject, (d[1] as Character).characterObject) > 2;
                else return false;
                });
            return c;
        }

        public static Condition Adults()
        {
            Condition c = new Condition("Adults");
            c.setDel((List<dynamic> d) => true);
            return c;
        }

        public static Condition NoRomanceYet()
        {
            Condition c = new Condition("NoRomanceYet",(List<dynamic> d) => {
                if (d.Count < 2) return false;
                if (d[0] is CharacterObject && d[1] is CharacterObject)
                    return NewCharacterRelationManager.GetRomantic((d[0] as CharacterObject), (d[1] as CharacterObject)) < NewCharacterRelationManager.relation.Good;
                else return false;
                });
            return c;
        }

        public static Condition WithRomance()
        {
            Condition c = new Condition("WithRomance", (List<dynamic> d) => {
                if (d.Count < 2) return false;
                if (d[0] is CharacterObject && d[1] is CharacterObject)
                    return NewCharacterRelationManager.GetRomantic((d[0] as CharacterObject), (d[1] as CharacterObject)) > NewCharacterRelationManager.relation.Neutral;
                else return false;
            });
            return c;
        }

        public static Condition LordsOnly()
        {
            Condition c = new Condition("LordsOnly", (List<dynamic> d) => {
                if (d.Count < 2) return false;
                if (d[0] is Character && d[1] is Character)
                    return (d[0] as Character).characterObject.Occupation == Occupation.Lord && (d[1] as Character).characterObject.Occupation == Occupation.Lord;
                else return false;
            });
            return c;
        }

        

        private static Condition IsRomanceExchange()
        {
            Condition c = new Condition("IsRomanceExchange", (List<dynamic> d) => {
                if (d.Count < 3) return false;
                if (d[2] is intent)
                    return (d[2] as intent?) == intent.Romantic;
                else return false;
            });
            return c;
        }

        private static Condition IsBored()
        {
            Condition c = new Condition("IsBored", (List<dynamic> d) => {
                if (d.Count < 2) return false;
                if (d[0] is Character)
                    return (d[0] as Character).isBored();
                else return false;
            });
            return c;
        }

        private static Condition NotAvailable()
        {
            Condition c = new Condition("NotAvailable");
            c.setDel((List<dynamic> d) => false);
            return c;
        }

        internal static Dictionary<string, Condition> CreateConditions()
        {
            Dictionary<string, Condition> c = new Dictionary<string, Condition>();

            c.Add("NotIntroduced", NotIntroduced());
            c.Add("NotAvailable", NotAvailable());
            c.Add("Adults", Adults());
            c.Add("WithRomance", WithRomance());
            c.Add("NoRomanceYet", NoRomanceYet());

            c.Add("IsBored", IsBored());

            c.Add("IsRomanceExchange", IsRomanceExchange());

            return c;
        }

    }


}
