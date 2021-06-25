﻿
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using System.Reflection;
using Module = TaleWorlds.MountAndBlade.Module;
using HarmonyLib;
using System;

namespace NewNpc2
{
    

    public class SubModule : MBSubModuleBase
    {
        #region Consts

        private const int PATTERN_MIN = 5;

        #endregion

        public static Dictionary<string, Condition> existingConditions;
        public static Dictionary<string, SocialInteraction> existingExchanges;
        public static Dictionary<string, InfluenceRule> existingRules;

        public static List<TriggerRule> triggerRules;

        public static List<SocialExchange> SocialFactsDatabase;

        public static NPCDialogBehaviour npc;

        //Relationships in the world
        public static NewCharacterRelationManager newRelationManager;

        public static void DoPatching()
        {
            var harmony = new Harmony("example.patch");
            harmony.PatchAll();
        }

        protected override void OnSubModuleLoad()
        {
            SocialFactsDatabase = new List<SocialExchange>();

            newRelationManager = new NewCharacterRelationManager();


            //typeof(Campaign).GetField("<CharacterRelationManager>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Campaign.Current, new NewCharacterRelationManager());

            existingConditions = ConditionManager.CreateConditions();
            existingRules = InfluenceRuleManager.createRules();
            existingExchanges = SocialInteractionManager.createSocialExchanges();

            triggerRules = TriggerRuleManager.createRules();


            CharacterManager.characters2 = new Dictionary<CharacterObject, Character>();
            CharacterManager.startAgents();
            
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign campaign && gameStarter is CampaignGameStarter campaignGameStarter)
            {
         
                //campaignGameStarter.AddBehavior(new NewConvoBehaviour());
                //campaignGameStarter.AddBehavior(new PlayerStartBehaviour());
                //campaignGameStarter.AddBehavior(new NpcStartBehaviour());
                //campaignGameStarter.AddBehavior(new DialogFlowBehaviour());
                //campaignGameStarter.AddBehavior(new SingleInteractionBehaviour());
                campaignGameStarter.AddBehavior(new DialogMatrixBehaviour());

                campaignGameStarter.AddBehavior(new NPCDialogBehaviour());

                campaignGameStarter.AddBehavior(new RumorBehaviour());

            }
        }


        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
            mission.MissionBehaviours.Add(new MissionViewBehaviour());
        }



        public static void makeExchange(SocialExchange se)
        {
            runTriggerRules(se.getInstRules(), se);

            SocialFactsDatabase.Add(se);
            if (se.type.IsImportant)
            {
                se.setInCharacters();
                se.spendEnergy();
                //TODO spread exchange
                //todo calc exchange interest
            }
        }

        public static void findPatter()
        {
            Dictionary<Tuple<Character, SocialInteraction>, float> list = new Dictionary<Tuple<Character, SocialInteraction>, float>();
            foreach(SocialExchange si in SocialFactsDatabase)
            {
                Tuple<Character,SocialInteraction> t = new Tuple<Character, SocialInteraction>(si.getInitiator(), si.type);
                if (list.ContainsKey(t)) list[t]++;
                else list.Add(t, 1);
            }
            foreach(KeyValuePair<Tuple<Character,SocialInteraction>,float> pair in list)
            {
                if (pair.Value > PATTERN_MIN) pair.Key.Item1.setPattern(pair.Key.Item2);
            }
        }

        public static void endInteraction()
        {
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        public static void runTriggerRules(List<InstRule> interactionRules, SocialExchange se)
        {
            List<dynamic> l = new List<dynamic>();
            l.Add(se.getInitiator());
            l.Add(se.getReceiver());
            l.Add(se.getIntent());
            l.Add(se.outcome);
            l.Add(se.type);

            foreach (InstRule r in interactionRules)
            {
                r.runEffects(l);
            }

            foreach (TriggerRule r in triggerRules)
            {
                r.runEffects(l);
            }

        }

        //update function
        //TODO: run this every x 
        private void update()
        {
            desireFormation();
            intentFormation();
            performExchanges();
            exchangeEffects();
        }

        //TODO
        private void desireFormation()
        {

        }

        //maybe not this one /merge with up?
        private void intentFormation()
        {

        }

        //make each character perform their desired exchange
        private void performExchanges()
        {

        }

        //trigger rules and instantiations 
        private void exchangeEffects()
        {

        }


        //TODO
        private void createMicroTheories()
        {

        }
    }
}