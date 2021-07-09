using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class DialogMatrixBehaviour : CampaignBehaviorBase
    {

        string playerInput = "player";
        string leave = "leave";
        string npcResponse = "response";
        string intentChoice = "intent";
        string fightChoice = "fight";
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
            CharacterManager.MainCharacter = new Character(Agent.Main);

            CreateDialog(starter);
            this.startInterction(starter);

        }

        public void AfterGameLoad()
        {

        }


        private void CreateDialog(CampaignGameStarter starter)
        {
            AddIntentChoices(starter, intentChoice, playerInput);
            CreatePlayerDialog(starter, playerInput, npcResponse);
            CreateNPCResponse(starter, npcResponse, intentChoice);
        }

        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine("starter", "start", intentChoice, " ",
                () => conversation(),
                () => intentStep(),
                1000, null);


        }

        private bool conversation()
        {
            bool b = false;
            b = b && CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Bandit;
            return !b;
        }

        private bool conversation_bandit()
        {
            bool b = false;
            b = b || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Bandit;
            return b;
        }

        private void AddIntentChoices( CampaignGameStarter campaign, string intoken, string outtoken)
        {
            string buffer = "buffer";
                    campaign.AddPlayerLine("positive", intoken, buffer, "Positive",
                        null,
                        () => {
                            runPlayerLine(intent.Positive);
                            playerChoice(intent.Positive);
                        },
                        1000, null);

            campaign.AddPlayerLine("neutral", intoken, buffer, "Neutral",
                        null,
                        () => {
                            runPlayerLine(intent.Neutral);
                            playerChoice(intent.Neutral);
                        },
                        1000, null);

            campaign.AddPlayerLine("negative", intoken, buffer, "Negative",
                        null,
                        () => {
                            runPlayerLine(intent.Negative);
                            playerChoice(intent.Negative);
                        },
                        1000, null);
            campaign.AddPlayerLine("Romantic", intoken, buffer, "Romantic",
                        null,
                        () => {
                            runPlayerLine(intent.Romantic);
                            playerChoice(intent.Romantic);
                        },
                        1000, null);
            campaign.AddPlayerLine("End", intoken, leave, "Leave",
                        null,
                        () => {
                            SubModule.endInteraction();
                        },
                        1000, null);
            campaign.AddDialogLine("buffer", buffer, outtoken, " ",
                null,
                () => { },
                1000, null);
            campaign.AddDialogLine("buffer", leave, "NOTHINGHERE", " ",
               null,
               () => {
                   SubModule.endInteraction();
               },
               1000, null);

        }

        public void CreateNPCResponse(CampaignGameStarter campaign, string intoken, string outtoken)
        {
            foreach (KeyValuePair<string, SocialInteraction> sipair in SubModule.existingExchanges)
            {
                int i = 0;
                foreach (Tuple<Dialog, Dialog> d in sipair.Value.sentences)
                {
                    campaign.AddDialogLine((sipair.Value.name + d.Item1.type + i), intoken, outtoken, d.Item1.sentence,
                        () => d.Item1.validateNpcLine(),
                        () => {
                            d.Item1.cresponse = false;
                        },
                        1000, null); 
                    if(d.Item2 != null)
                        campaign.AddDialogLine((sipair.Value.name + d.Item2.type + i + 2), intoken, outtoken, d.Item2.sentence,
                         () => d.Item2.validateNpcLine(),
                         () => {
                             d.Item2.cresponse = false;
                         },
                         1000, null);
                    i++;
                }
            }
        }

        public void CreatePlayerDialog(CampaignGameStarter campaign, string intoken, string outtoken)
        {
            foreach (SocialInteraction si in SocialInteractionManager.allInteractions())
            {
                int i = 0;
                foreach (Tuple<Dialog,Dialog> d in si.sentences)
                {
                    campaign.AddPlayerLine((si.name + d.Item1.type + i), intoken, outtoken, d.Item1.sentence,
                        () => d.Item1.validatePlayerLine(),
                        () => {
                            runNpcResponse(si);
                            playerChoice(si);
                            //turnalloff();
                            //si.followUp(d,campaign,outtoken);
                        }, 
                        1000, null);
                    i++;
                }
            }
        }

        private void turnalloff()
        {
            foreach (SocialInteraction si in SocialInteractionManager.allInteractions())
            {
                foreach (Tuple<Dialog,Dialog> d in si.sentences)
                {
                    d.Item1.playera = false;
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
            CharacterManager.MainCharacter.setInChoice(si);
        }
        private void playerChoice(intent i)
        {
            //change player when he does this
            //modify his character in the game on each choice 
            CharacterManager.MainCharacter.setInIntent(i);
        }

        private void runPlayerLine(intent i)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            
            Character character = CharacterManager.findChar(c);
            CharacterManager.MainCharacter.chooseDialog(i,character);

            /**
            List<SocialInteraction> l = CharacterManager.MainCharacter.getIntented(character, i);
            foreach (SocialInteraction si in l)
            {
                si.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0,true);
            }
            SocialInteraction leave = SocialInteractionManager.Leave();
            leave.chooseDialog(CharacterManager.MainCharacter.calcDialogType(),0,true);
            */
        }

        private void runNpcResponse(SocialInteraction social)
        {
            social.clearDialog();
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);
            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, social, CharacterManager.MainCharacter.getIntent());
            float result = se.calculateResponse();
            se.chooseResponse(result);

            
            SocialExchange.Last = se;

            SubModule.makeExchange(se);
            if (social.finish) SubModule.endInteraction();
            else intentStep();
        }



        

        
    }
}
