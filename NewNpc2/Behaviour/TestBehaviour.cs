using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class TestBehaviour : CampaignBehaviorBase
    {

        string playerInput = "player";
        string npcResponse = "response";
        string intentChoice = "intent";
        string fightChoice = "fight";
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));

        }

        public override void SyncData(IDataStore dataStore)
        {
        }



        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            CharacterManager.MainCharacter = new Character(Agent.Main);

            this.startInterction(starter);

        }


        private void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddDialogLine("starter", "start", "start", " *Random shit guedo is saying on discord* ",
                null,
                null,
                10000, null);
            campaign.AddPlayerLine("starter", "start", "start", "GUEDO",
                null,
                null,
                10000, null);

        }


        public void CreateNPCResponse(CampaignGameStarter campaign, string intoken, string outtoken)
        {
            foreach (KeyValuePair<string, SocialInteraction> sipair in SubModule.existingExchanges)
            {
                int i = 0;
                foreach (Tuple<Dialog,Dialog> d in sipair.Value.sentences)
                {
                    campaign.AddDialogLine((sipair.Value.name + d.Item1.type + i), intoken, outtoken, d.Item1.sentence,
                        () => d.Item1.validateNpcLine(),
                        () => {

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
                foreach (Dialog d in si.getSentences())
                {
                    campaign.AddPlayerLine((si.name + d.type + i), intoken, outtoken, d.sentence,
                        () => d.validatePlayerLine(),
                        () => {
                            runNpcResponse(si);
                            playerChoice(si);
                            si.followUp(d,campaign,outtoken);
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

        }


        private void runPlayerLine(intent i)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;

            Character character = CharacterManager.findChar(c);
            foreach (SocialInteraction si in CharacterManager.MainCharacter.getIntented(character, i))
            {
                si.chooseDialog(CharacterManager.MainCharacter.calcDialogType(), 0, true);
            }
            SocialInteraction l = SocialInteractionManager.Leave();
            l.chooseDialog(CharacterManager.MainCharacter.calcDialogType(), 0, true);
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
