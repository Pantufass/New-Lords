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
        string intentChoice = "intent";

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
            Character c = new Character(new Culture("MainHero"), Hero.MainHero.GetHeroTraits(), Hero.MainHero.CharacterObject);
            CharacterManager.characters.Add(Hero.MainHero.CharacterObject, c);
            CharacterManager.MainCharacter = c;
        }

        private void CreateDialog(CampaignGameStarter starter)
        {
            CreateNPCResponse(starter,npcResponse, intentChoice);
            CreatePlayerDialog(starter,playerInput,npcResponse);
            AddIntentChoices(starter, intentChoice, playerInput);
        }

        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine("starter", "start", intentChoice, " ",
                null,
                () => intentStep(),
                1000, null);

        }

        private void AddIntentChoices( CampaignGameStarter campaign, string intoken, string outtoken)
        {
            string buffer = "buffer";
                    campaign.AddPlayerLine("positive", intoken, buffer, "Positive",
                        null,
                        () => {
                            runPlayerLine(intent.Positive);
                        },
                        1000, null);

            campaign.AddPlayerLine("neutral", intoken, buffer, "Neutral",
                        null,
                        () => {
                            runPlayerLine(intent.Neutral);
                        },
                        1000, null);

            campaign.AddPlayerLine("negative", intoken, buffer, "Negative",
                        null,
                        () => {
                            runPlayerLine(intent.Negative);
                        },
                        1000, null);
            campaign.AddPlayerLine("Romantic", intoken, buffer, "Romantic",
                        null,
                        () => {
                            runPlayerLine(intent.Romantic);
                        },
                        1000, null);
            campaign.AddPlayerLine("End", intoken, buffer, "Leave",
                        null,
                        () => {
                            SubModule.endInteraction();
                        },
                        1000, null);
            campaign.AddDialogLine("buffer", buffer, outtoken, " ",
                null,
                () => { },
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
                            playerChoice(sipair.Value);
                        },
                        1000, null);
                    i++;
                }
            }
        }

        private void intentStep()
        {

            
        }

        private void playerChoice(SocialInteraction si)
        {
            //change player when he does this
            //modify his character in the game on each choice 
            //tough to do
        }


        private void runPlayerLine(intent i)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            
            Character character = CharacterManager.findChar(c);
            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented(character, i))
            {
                si.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0,true);
            }
            SocialInteraction l = SocialInteractionManager.Leave();
            l.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0,true);
        }

        private void runNpcResponse(SocialInteraction social)
        {
            social.clearDialog();
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);
            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, social, CharacterManager.MainCharacter.getIntent());
            float result = se.calculateResponse();
            se.chooseResponse(result);
            SubModule.makeExchange(se);
            if (social.finish) SubModule.endInteraction();
            else intentStep();
        }



        

        
    }
}
