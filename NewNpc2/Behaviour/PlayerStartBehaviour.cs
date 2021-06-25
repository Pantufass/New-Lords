using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;

namespace NewNpc2
{
    public class PlayerStartBehaviour : CampaignBehaviorBase
    {
        int step = 0;
        bool loaded = false;


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

        }

        public void AfterGameLoad()
        {
            loaded = true;
        }


        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine(" start", "start", nextStep(), " ",
                null,
                ()=>startPlayerInt(campaign),
                1000, null);

            InformationManager.DisplayMessage(new InformationMessage(nextStep()));
            step++;
        }

        private string nextStep()
        {
            return "step" + (step+1);
        }

        private string getStep()
        {
            return "step" + step;
        }

        private void startPlayerInt(CampaignGameStarter campaign)
        {

            Campaign.Current.ConversationManager.ClearCurrentOptions();
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            if (!loaded) return;

            Character character = CharacterManager.findChar(c);

            intent i = CharacterManager.MainCharacter.calcVolitions(character);


            InformationManager.DisplayMessage(new InformationMessage(CharacterManager.MainCharacter.isBored() ? "bored" : "not"));


            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                campaign.AddPlayerLine(si.name, getStep(), nextStep(), si.getDialogLine(sentenceType.Normal, 0),
                    (() => si.validate(Hero.MainHero.CharacterObject, c)),
                    (() =>
                    {
                        newInteraction(campaign, si, i);
                    }),
                    1000, null);
            }

            SocialInteraction l = SocialInteractionManager.Leave();
            campaign.AddPlayerLine(l.name, getStep(), nextStep(), l.getDialogLine(sentenceType.Normal, 0),
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

            campaign.AddDialogLine(si.name, getStep(), nextStep(), si.getResponse(result),
                null,
                (() =>
                {
                    makeExchange(se);
                    if (si.finish) endInteraction();
                    else
                    {
                        startPlayerInt(campaign);
                    }
                }),
                1000,null );

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
