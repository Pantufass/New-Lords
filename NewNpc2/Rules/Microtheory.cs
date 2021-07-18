using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;

namespace NewNpc2
{
    public static class Microtheory
    {
        private static Random r = SubModule.rand;

        //fix to use mobile party
        public static void FirstImpression(List<Character> list)
        {

            foreach (Character c1 in list)
            {
                if(c1.agent == Agent.Main)  InformationManager.DisplayMessage(new InformationMessage("THERE IS A MAIN IN LIST" ));
                foreach (Character c2 in list)
                {
                    FirstImpression(c1, c2);
                }
            }
        }

        //usar proprios traits para calcular
        //mais ou menos chance de nao gostar
        private static void FirstImpression(Character c1, Character c2)
        {
            if(Introduction.Introduced(c1.characterObject,c2.characterObject) == 0)
            {
                float kindness = c1.getKind();
                float charm = c2.getCharm();
                float honor = c1.getHonor();
                float chance = 0.05f;
                double res = r.NextDouble();

                float v = chance + chance * charm;
                float v2 = chance + chance * kindness;
                float v3 = chance - chance * honor;
                Introduction.Introduce(c1.characterObject, c2.characterObject);
                if (res < v) LoveAtFirstSight(c1, c2, charm);
                else if (res - v < v2) LooksCool(c1, c2, kindness);
                else if (res - v - v2 < v3) HatredAtFirstSight(c1, c2, honor);
            }
        }


        private static void LoveAtFirstSight(Character c1, Character c2, float charm)
        {
            float n = 3;
            float value = 2 * n + n * charm;
            if(!c1.characterObject.IsFemale && c2.characterObject.IsFemale ||
                c1.characterObject.IsFemale && !c2.characterObject.IsFemale ||
                !c1.characterObject.IsFemale && !c2.characterObject.IsFemale && c1.isGay() ||
                c1.characterObject.IsFemale && c2.characterObject.IsFemale && c1.isGay())
                c1.raiseRomantic(c2, value);
        }

        private static void LooksCool(Character c1, Character c2, float kindness)
        {
            float n = 3;
            float value = 2 * n + n * kindness;
            c1.raiseFriendly(c2, value);
        }

        private static void HatredAtFirstSight(Character c1, Character c2, float honor)
        {
            float n = 3;
            float value = 2 * n - n * honor;
            c1.lowerFriendly(c2, value);
        }

        public static void Defeat(Hero w, Hero l)
        {
            Character w1 = NPCDialogBehaviour.characterManager.findChar(w);
            Character l1 = NPCDialogBehaviour.characterManager.findChar(l);
            l1.overpowered(w1);
        }


        public static void QuestCompleted(QuestBase qb, QuestBase.QuestCompleteDetails qd)
        {
            float value = 8;
            Character qc = NPCDialogBehaviour.characterManager.findChar(qb.QuestGiver.CharacterObject);
            qc.overpowered(CharacterManager.MainCharacter, 0.2f);
            qc.raiseFriendly(CharacterManager.MainCharacter,value);
        }

        //village raid? how to get the characters?
        public static void Raid(Village v)
        {
            
        }

        //i have no ideia when this is triggered, never seen it ingame
        public static void Insulted(Hero w, Hero l, CharacterObject co, ActionNotes an)
        {

        }

        public static void HourPass()
        {

        }
    }
}
