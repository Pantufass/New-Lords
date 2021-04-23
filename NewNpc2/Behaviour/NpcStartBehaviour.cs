using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class NpcStartBehaviour : CampaignBehaviorBase
    {
        DialogFlow df;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.AfterGameLoad));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            this.startInterction(starter);
        }

        public void AfterGameLoad()
        {
            CreateCharacter();
            loaded = true;
        }

        private void CreateCharacter()
        {
            //create MH character
            Character c = new Character(new CulturalKnowledge("MainHero"), Hero.MainHero.GetHeroTraits(), Hero.MainHero.CharacterObject);
            CharacterManager.characters.Add(Hero.MainHero.CharacterObject, c);
            CharacterManager.MainCharacter = c;
        }

        private void startInterction(CampaignGameStarter campaign)
        {
            df = DialogFlow.CreateDialogFlow(null, 1000);
            df.NpcLine(" ",null);
            //df.Consequence(() =>startPlayerInt(campaign));
            Campaign.Current.ConversationManager.AddDialogFlow(df);
        }


        


        private void makeExchange(SocialInteraction prev, outcome o, Character character)
        {
            SubModule.runTriggerRules(prev.getTrigger(), CharacterManager.MainCharacter, character, intent.Neutral, o);

            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, prev, o);
            SubModule.SocialFactsDatabase.Add(se);
            //TODO spread exchange
            //todo calc exchange interest
        }

        private void endInteraction(SocialInteraction prev, outcome o, Character character)
        {
            makeExchange(prev, o, character);

            df.CloseDialog();
        }
    }
}
