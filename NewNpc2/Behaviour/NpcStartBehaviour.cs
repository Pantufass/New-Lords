using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class NpcStartBehaviour : CampaignBehaviorBase
    {
        int step = 0;
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
            //CreateCharacter();
            loaded = true;
        }
        /**
        private void CreateCharacter()
        {
            //create MH character
            Character c = new Character(new Culture("MainHero"), Hero.MainHero.GetHeroTraits(), Hero.MainHero.CharacterObject);
            CharacterManager.characters.Add(Hero.MainHero.CharacterObject, c);
            CharacterManager.MainCharacter = c;
        }
        */
        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine(" start", "start", nextStep(), " ",
                null,
                () => startPlayerInt(campaign),
                1000, null);

            InformationManager.DisplayMessage(new InformationMessage(nextStep()));
            step++;
        }

        private string nextStep()
        {
            return "step" + (step + 1);
        }

        private string getStep()
        {
            return "step" + step;
        }

        private void startPlayerInt(CampaignGameStarter campaign)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            if (!loaded) return;

            Character character = CharacterManager.findChar(c);

            CharacterManager.MainCharacter.calcVolitions(character);

            InformationManager.DisplayMessage(new InformationMessage(CharacterManager.MainCharacter.isHappy() ? "happy" : "not"));

            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                campaign.AddPlayerLine(si.name, getStep(), nextStep(), si.getDialogLine(sentenceType.Normal, 0),
                    (() => si.validate(Hero.MainHero.CharacterObject, c)),
                    (() =>
                    {
                        newInteraction(campaign, si);
                    }),
                    1000, null);
            }

            SocialInteraction l = SocialInteractionManager.Leave();
            campaign.AddPlayerLine(l.name, getStep(), nextStep(), l.getDialogLine(sentenceType.Normal, 0),
                    (() => l.validate(Hero.MainHero.CharacterObject, c)),
                    (() =>
                    {
                        newInteraction(campaign, l);
                    }),
                    1000, null);
            step++;
        }

        private void newInteraction(CampaignGameStarter campaign, SocialInteraction si)
        { }


        private void makeExchange(SocialInteraction prev, outcome o, Character character)
        {

            //TODO spread exchange
            //todo calc exchange interest
        }

        private void endInteraction(SocialInteraction prev, outcome o, Character character)
        {
            makeExchange(prev, o, character);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
