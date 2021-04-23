using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class DialogFlowBehaviour : CampaignBehaviorBase
    {
        DialogFlow df;
        bool loaded = false;

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
            df = DialogFlow.CreateDialogFlow(null, 10000);
            df.NpcLine(" ",null);
            df.Consequence(() =>startPlayerInt(campaign));
            df.CloseDialog();
            Campaign.Current.ConversationManager.AddDialogFlow(df);
        }

        private void startPlayerInt(CampaignGameStarter campaign)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            if (!loaded) return;

            Character character = CharacterManager.findChar(c);
            if (character == null)
            {
                if (c.IsHero)
                {
                    character = new Character(new CulturalKnowledge("a"), c.HeroObject.GetHeroTraits(), c);
                    CharacterManager.characters.Add(c, character);
                }
                else
                {
                    character = new Character(new CulturalKnowledge("a"), c);
                    CharacterManager.characters.Add(c, character);
                }
            }

            df.BeginPlayerOptions();

            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                df.PlayerOption(si.getDialogLine(sentenceType.Normal, 0)).Consequence(() =>
                {
                    newInteraction(campaign, si,character);
                });
            }

            df.CloseDialog();
            SocialInteraction l = SocialInteractionManager.Leave();
            df.PlayerOption(l.getDialogLine(sentenceType.Normal, 0)).Consequence(() =>
            {
                newInteraction(campaign, l,character);
            });
            df.EndPlayerOptions();
        }

        private void newInteraction(CampaignGameStarter campaign, SocialInteraction si, Character c)
        {


        }

        private void makeExchange(SocialInteraction prev, outcome o, Character character)
        {

            //TODO spread exchange
            //todo calc exchange interest
        }

        private void endInteraction(SocialInteraction prev, outcome o, Character character)
        {

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            df.CloseDialog();
        }
    }
}
