using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;

namespace NewNpc2
{
    public class SingleInteractionBehaviour : CampaignBehaviorBase
    {
        int step = 0;
        bool loaded = false;

        List<bool> chosenExchanges;
        List<bool> chosenDialog;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.AfterGameLoad));

            //CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(Microtheory.FirstImpression));

            CampaignEvents.PlayerMetCharacter.AddNonSerializedListener(this, new Action<Hero>(this.MeetCharacter));
            CampaignEvents.CharacterDefeated.AddNonSerializedListener(this, new Action<Hero, Hero>(Microtheory.Defeat));
            CampaignEvents.CharacterInsulted.AddNonSerializedListener(this, new Action<Hero, Hero, CharacterObject, ActionNotes>(Microtheory.Insulted));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(Microtheory.HourPass));
            CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(Microtheory.QuestCompleted));
            CampaignEvents.PlayerStartTalkFromMenu.AddNonSerializedListener(this, new Action<Hero>(this.Start));
            CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(Microtheory.Raid));
            CampaignEvents.VillageLooted.AddNonSerializedListener(this, new Action<Village>(Microtheory.Raid));

        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void Start(Hero h)
        {

        }

        public void MeetCharacter(Hero c)
        {

        }


        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            this.startInterction(starter);
            chosenExchanges = new List<bool>();
            chosenDialog = new List<bool>();

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
            campaign.AddDialogLine(" start", "start","playerturn", " ",
                null,
                () => startPlayerInt(campaign),
                1000, null);
        }


        private void startPlayerInt(CampaignGameStarter campaign)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            if (!loaded) return;

            Character character = CharacterManager.findChar(c);

            intent i = CharacterManager.MainCharacter.calcVolitions(character);


            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                campaign.AddPlayerLine(si.name, "playerturn", "response", si.getDialogLine(CharacterManager.MainCharacter.calcDialogType(), 0),
                    (() => si.validate(Hero.MainHero.CharacterObject, c)),
                    (() =>
                    {
                        newInteraction(campaign, si, i);
                    }),
                    1000, null);
            }

            SocialInteraction l = SocialInteractionManager.Leave();
            campaign.AddPlayerLine(l.name, "playerturn", "response", l.getDialogLine(CharacterManager.MainCharacter.calcDialogType(), 0),
                    (() => l.validate(Hero.MainHero.CharacterObject, c)),
                    (() =>
                    {
                        newInteraction(campaign, l, i);
                    }),
                    1000, null);
            step++;
        }

        private void newInteraction(CampaignGameStarter campaign, SocialInteraction si, intent i)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);

            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, si, i);
            float result = se.calculateResponse();

            campaign.AddDialogLine(si.name, "response","end", si.getResponse(result),
                null,
                (() =>
                {
                    endInteraction();
                    makeExchange(se);
                }),
                1000, null);

            step++;
        }


        private void makeExchange(SocialExchange se)
        {
            SubModule.runTriggerRules(se.getInstRules(), se);

            if (se.type.IsImportant)
            {
                SubModule.SocialFactsDatabase.Add(se);
                se.setInCharacters();
                //TODO spread exchange
                //todo calc exchange interest
            }
        }

        private void endInteraction()
        {
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
