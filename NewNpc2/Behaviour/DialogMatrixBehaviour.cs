using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;

namespace NewNpc2
{
    public class DialogMatrixBehaviour : CampaignBehaviorBase
    {

        string playerInput = "player";
        string npcResponse = "response";

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
            CreateDialog(starter);
            this.startInterction(starter);

        }

        public void AfterGameLoad()
        {
            CreateCharacter();
        }

        private void CreateCharacter()
        {
            //create MH character
            Character c = new Character(new CulturalKnowledge("MainHero"), Hero.MainHero.GetHeroTraits(), Hero.MainHero.CharacterObject);
            CharacterManager.characters.Add(Hero.MainHero.CharacterObject, c);
            CharacterManager.MainCharacter = c;
        }

        private void CreateDialog(CampaignGameStarter starter)
        {
            CreateNPCResponse(starter,npcResponse,playerInput);
            CreatePlayerDialog(starter,playerInput,npcResponse);
        }

        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine("starter", "start", playerInput, " ",
                null,
                () => runPlayerLine(),
                1000, null);

        }

        public void CreateNPCResponse(CampaignGameStarter campaign, string intoken, string outtoken)
        {
            foreach (KeyValuePair<string, SocialInteraction> sipair in SubModule.existingExchanges)
            {
                int i = 0;
                foreach (Dialog d in sipair.Value.sentences)
                {
                    campaign.AddDialogLine((sipair.Value.name + d.type + i), intoken, outtoken, d.sentence,
                        () => d.validateNpcLine(),
                        () => {

                        },
                        1000, null);
                    i++;
                }
            }
        }

        public void CreatePlayerDialog(CampaignGameStarter campaign, string intoken, string outtoken)
        {
            foreach (KeyValuePair<string, SocialInteraction> sipair in SubModule.existingExchanges)
            {
                int i = 0;
                foreach (Dialog d in sipair.Value.sentences)
                {
                    campaign.AddPlayerLine((sipair.Value.name + d.type + i), intoken, outtoken, d.sentence,
                        () => d.validatePlayerLine(),
                        () => {
                            runNpcResponse(sipair.Value);
                        },
                        1000, null);
                    i++;
                }
            }
        }

        private void runPlayerLine()
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);
            intent i = CharacterManager.MainCharacter.calcVolitions(character);
            CharacterManager.MainCharacter.setIntent(i);
            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented())
            {
                si.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0);
            }
            SocialInteraction l = SocialInteractionManager.Leave();
            l.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0);
        }

        private void runNpcResponse(SocialInteraction social)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);
            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, social, CharacterManager.MainCharacter.getIntent());
            float result = se.calculateResponse();
            se.chooseResponse(result);
            SubModule.makeExchange(se);
            if (social.finish) SubModule.endInteraction();
            else runPlayerLine();
        }



        

        
    }
}
