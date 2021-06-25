using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace NewNpc2
{
    public class DialogFlowBehaviour : CampaignBehaviorBase
    {
        int step = 0;
        bool loaded = false;

        Dictionary<string, DialogFlow> dialogs;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.AfterGameLoad));

           // CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(Microtheory.FirstImpression));

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
            dialogs = new Dictionary<string, DialogFlow>();

        }

        public void AfterGameLoad()
        {
            loaded = true;
        }


        private void startInterction(CampaignGameStarter campaign)
        {
            string s = " ";
            DialogFlow df = DialogFlow.CreateDialogFlow("start", 1000);
            df.GetOutputToken(out s);
            df.NpcLine(new TextObject("AAAAAAAAAAAAAAAAAAAAAAA")).Consequence(() => startPlayerInt(campaign, s));
            Campaign.Current.ConversationManager.AddDialogFlow(df);
        }


        private void startPlayerInt(CampaignGameStarter campaign, string outToken)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            if (!loaded) return;

            Character character = CharacterManager.findChar(c);

            intent i = CharacterManager.MainCharacter.calcVolitions(character);

            DialogFlow df = DialogFlow.CreateDialogFlow(outToken, 1000);

            string s = " ";
            df.GetOutputToken(out s);
            df.BeginPlayerOptions();

            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                df.PlayerOption(new TextObject(si.getDialogLine(sentenceType.Normal, 0)))
                    .Condition(() => si.validate(Hero.MainHero.CharacterObject, c))
                    .Consequence(() => newInteraction(campaign, si, i,s));

            }

            SocialInteraction l = SocialInteractionManager.Leave();
            df.PlayerOption(new TextObject(l.getDialogLine(sentenceType.Normal, 0)))
                   .Condition(() => l.validate(Hero.MainHero.CharacterObject, c))
                   .Consequence(() => newInteraction(campaign, l, i,s));
        }

        private void newInteraction(CampaignGameStarter campaign, SocialInteraction si, intent i, string outToken)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);

            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, si, i);
            float result = se.calculateResponse();

            DialogFlow df = DialogFlow.CreateDialogFlow(outToken, 1000);

            string s = " ";
            df.GetOutputToken(out s);
            df.BeginNpcOptions();
            df.NpcLine(new TextObject(si.getResponse(result))).Consequence(() =>
            {
                makeExchange(se);
                if (si.finish) {
                    endInteraction();
                    df.CloseDialog();
                    }
                else startPlayerInt(campaign, s);
            }); ;
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
