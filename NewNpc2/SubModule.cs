
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using System.Reflection;
using Module = TaleWorlds.MountAndBlade.Module;

namespace NewNpc2
{
    public static class CharacterManager
    {
        public static Dictionary<CharacterObject, Character> characters;

        public static void start(CulturalKnowledge ck)
        {
            //create a character for each hero
            foreach(CharacterObject h in Campaign.Current.Characters)
            {
                characters.Add(h, new Character(ck));
            }
        }

        public static Character findChar(CharacterObject h)
        {
            if(characters.TryGetValue(h, out Character c))
            {
                return c;
            }
            return null;
        }
    }

    public class SubModule : MBSubModuleBase
    {
        //TODO:
        //find a way to attach Character to every CharacterObject / or extend CharacterObject -> harmony probably solves
        //Create Social Exchanges
        //deal with the problem "only load NPCs close to player"
        //throw event when party reaches new settlement


        private static Dictionary<string, SocialExchangeType> existingExchanges;

        //TODO
        private static List<Rule> microTheories;
        //TODO
        private static List<Rule> triggerRules;

        //TODO
        public List<SocialExchange> SocialFactsDatabase;

        //Relationships in the world
        public static NewCharacterRelationManager newRelationManager;

        

        protected override void OnSubModuleLoad()
        {


            newRelationManager = new NewCharacterRelationManager();


            //typeof(Campaign).GetField("<CharacterRelationManager>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Campaign.Current, new NewCharacterRelationManager());

            existingExchanges = SocialExchangeManager.createSocialExchanges();
            createMicroTheories();
            createTriggerRules();


            CharacterManager.characters = new Dictionary<CharacterObject, Character>();
            
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign campaign && gameStarter is CampaignGameStarter campaignGameStarter)
            {

                campaignGameStarter.AddBehavior(new NewConvoBehaviour());
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