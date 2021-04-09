
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using System.Reflection;
using Module = TaleWorlds.MountAndBlade.Module;

namespace NewNpc2
{
    

    public class SubModule : MBSubModuleBase
    {
        //TODO:
        //find a way to attach Character to every CharacterObject / or extend CharacterObject -> harmony probably solves
        //Create Social Exchanges
        //deal with the problem "only load NPCs close to player"
        //throw event when party reaches new settlement


        public static Dictionary<string, SocialInteraction> existingExchanges;

        //TODO
        private static List<Rule> microTheories;
        //TODO
        private static List<Rule> triggerRules;

        //TODO
        public static List<SocialExchange> SocialFactsDatabase;

        //Relationships in the world
        public static NewCharacterRelationManager newRelationManager;

        

        protected override void OnSubModuleLoad()
        {
            SocialFactsDatabase = new List<SocialExchange>();

            newRelationManager = new NewCharacterRelationManager();


            //typeof(Campaign).GetField("<CharacterRelationManager>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Campaign.Current, new NewCharacterRelationManager());

            existingExchanges = SocialInteractionManager.createSocialExchanges();
            
            createTriggerRules();


            CharacterManager.characters = new Dictionary<CharacterObject, Character>();

            
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign campaign && gameStarter is CampaignGameStarter campaignGameStarter)
            {
         
                campaignGameStarter.AddBehavior(new NewConvoBehaviour());
                campaignGameStarter.AddBehavior(new PlayerStartBehaviour());
            }
        }


        //TODO
        private void createMicroTheories()
        {

        }

        //TODO
        private void createTriggerRules()
        {

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
    }
}