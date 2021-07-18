using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace NewNpc2
{
    public class DialogMatrixBehaviour : CampaignBehaviorBase
    {

        string playerInput = "player";
        string leave = "leave";
        string npcResponse = "response";
        string intentChoice = "intent";
        string fightChoice = "fight";
        string start = "startnode";
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
            CharacterManager.MainCharacter = new MainCharacter(Hero.MainHero);

            CreateDialog(starter);

        }

        public void AfterGameLoad()
        {

        }


        private void CreateDialog(CampaignGameStarter starter)
        {
            AddStartConv(starter, start, intentChoice);
            AddIntentChoices(starter, intentChoice, playerInput);
            CreatePlayerDialog(starter, playerInput, npcResponse);
            CreateNPCResponse(starter, npcResponse, intentChoice);
        }


        private void AddStartConv(CampaignGameStarter starter, string intoken, string outtoken)
        {
            string buffer = "bufferstart";
            /*
            starter.AddDialogLine("set_vars", "start", "lord_intro", "{=IWKmmImm}Never see this", new ConversationSentence.OnConditionDelegate(this.conversation_set_first_on_condition), null, 1000, null);
            starter.AddDialogLine("parley", "start", "lord_intro", "{=!}{STR_PARLEY_COMMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_siege_parley_unmet_on_condition), null, 1000, null);
            starter.AddDialogLine("parley", "start", "lord_start", "{=!}{STR_PARLEY_COMMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_siege_parley_met_on_condition), null, 1000, null);
            starter.AddDialogLine("start_attacking_unmet", "start", "lord_meet_player_response", "{=EPpTmCXw}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_attacking_lord_set_meeting_meet_on_condition), null, 100, null);
            starter.AddDialogLine("start_lord_unmet", "start", "lord_meet_player_response", "{=EPpTmCXw}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_on_condition), null, 1100, null);
            starter.AddDialogLine("unmet_in_main_mobile_party", "start", "lord_meet_in_main_party_player_response", "{=EPpTmCXw}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_unmet_lord_main_party_on_condition), null, 110, null);
            starter.AddDialogLine("start_wanderer_unmet", "start", "wanderer_meet_player_response", "{=EPpTmCXw}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_meet_on_condition), null, 1100, null);
            starter.AddDialogLine("start_default_under_24_hours", "start", "lord_start", "{=!}{SHORT_ABSENCE_GREETING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_greets_under_24_hours_on_condition), null, 1000, null);
            */
            starter.AddDialogLine("start_wanderer_unmet", "start", "wanderer_meet_player_response2", "{=EPpTmCXw}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_meet_on_condition), null, 1100, null);
            starter.AddDialogLine("town_or_village_start", "start", "town_or_village_talk2", "Hello, Can i help you?", new ConversationSentence.OnConditionDelegate(this.conversation_town_or_village_start_on_condition), null, 1000, null);

            starter.AddPlayerLine("tavernmaid_order_food", "taverngamehost_talk", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);
            starter.AddPlayerLine("tavernmaid_order_food", "tavernmaid_talk", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);
            starter.AddPlayerLine("talk_bard_player_leave", "talk_bard_player", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);

            starter.AddPlayerLine("town_or_village_player", "hero_main_options", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);
            
            /*
            starter.AddPlayerLine("town_or_village_player", "lord_start", buffer, "Converse", null, null, 1000, null, null);
            starter.AddPlayerLine("town_or_village_player", "lord_meet_player_response", buffer, "Converse", null, null, 1000, null, null);
            starter.AddPlayerLine("town_or_village_player", "lord_meet_in_main_party_player_response", buffer, "Converse", null, null, 1000, null, null);
            */

            starter.AddPlayerLine("town_or_village_player", "wanderer_meet_player_response2", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);

            starter.AddPlayerLine("town_or_village_player", "town_or_village_talk2", buffer, "Converse", null, () => converse_consequence(), 1000, null, null);

            starter.AddDialogLine("town_or_village_player", buffer, outtoken, " ", null, null, 1000, null);


            starter.AddPlayerLine("player_is_leaving_neutral_or_friendly", "town_or_village_talk2", "hero_leave", "{=9mBy0qNW}I must leave now.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_neutral_or_friendly_on_condition), null, 1, null, null);
            starter.AddPlayerLine("player_is_leaving_neutral_or_friendly", "wanderer_meet_player_response2", "hero_leave", "{=9mBy0qNW}I must leave now.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_neutral_or_friendly_on_condition), null, 1, null, null);
        }

        private void converse_consequence()
        {
            SubModule.talkedCounter++;
        }
        private bool conversation_town_or_village_start_on_condition()
        {
            return (CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Villager || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Townsfolk) && PlayerEncounter.Current != null && PlayerEncounter.InsideSettlement;

        }
        private bool conversation_wanderer_meet_on_condition()
        {
            return conversation_wanderer_on_condition() && conversationUseMeetingDialogs();
        }
        private bool conversationUseMeetingDialogs()
        {
            if (Hero.OneToOneConversationHero != null)
            {
                StringHelpers.SetCharacterProperties("CONVERSATION_NPC", Hero.OneToOneConversationHero.CharacterObject, null);
            }
            if (Campaign.Current.CurrentConversationContext == ConversationContext.FreedLord || Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord)
            {
                return false;
            }
            if (Hero.OneToOneConversationHero == null)
            {
                return false;
            }
            if (Hero.OneToOneConversationHero.HasMet)
            {
                Campaign.Current.ConversationManager.CurrentConversationIsFirst = false;
                return false;
            }
            Campaign.Current.ConversationManager.CurrentConversationIsFirst = true;
            Hero.OneToOneConversationHero.HasMet = true;
            if (Campaign.Current.CurrentConversationContext != ConversationContext.Default && Campaign.Current.CurrentConversationContext != ConversationContext.PartyEncounter)
            {
                return false;
            }
            return true;
        }
        public static bool conversation_wanderer_on_condition()
        {
            return CharacterObject.OneToOneConversationCharacter != null && CharacterObject.OneToOneConversationCharacter.IsHero && CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Wanderer && CharacterObject.OneToOneConversationCharacter.HeroObject.HeroState != Hero.CharacterStates.Prisoner;
        }
        public bool conversation_player_is_leaving_enemy_on_condition()
        {
            return Hero.OneToOneConversationHero != null && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
        }

        public bool conversation_player_is_leaving_neutral_or_friendly_on_condition()
        {
            return Hero.OneToOneConversationHero != null && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
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
                        1001, null);

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
            campaign.AddPlayerLine("End", intoken, "hero_leave", "Leave",
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
            foreach (SocialInteraction si in SocialInteractionManager.allInteractions())
            {
                int i = 0;
                foreach (Tuple<Dialog, Dialog> d in si.sentences)
                {
                    campaign.AddDialogLine((si.name + d.Item1.type + i), intoken, outtoken, d.Item1.sentence,
                        () => d.Item1.validateNpcLine(),
                        () => {
                            d.Item1.cresponse = false;
                        },
                        1000, null); 
                    if(d.Item2 != null)
                        campaign.AddDialogLine((si.name + d.Item2.type + i + 2), intoken, outtoken, d.Item2.sentence,
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
                if (si.name == "AskAbout")
                {
                    int i = 0;
                    foreach (Tuple<Dialog, Dialog> d in si.sentences)
                    {
                        campaign.AddPlayerLine((si.name + d.Item1.type + i), intoken, "wanderer_preintroduction", d.Item1.sentence,
                            () => d.Item1.validatePlayerLine(),
                            () =>
                            {
                                clearIntended();
                                playerChoice(si);
                            },
                            1000, null);
                        i++;
                    }
                }
                else if(si.name == "InviteParty")
                {
                    int i = 0;
                    foreach (Tuple<Dialog, Dialog> d in si.sentences)
                    {
                        campaign.AddPlayerLine((si.name + d.Item1.type + i), intoken, "companion_hire", d.Item1.sentence,
                            () => d.Item1.validatePlayerLine(),
                            () =>
                            {
                                clearIntended();
                                playerChoice(si);
                            },
                            1000, null);
                        i++;
                    }
                }
                else
                {
                    int i = 0;
                    foreach (Tuple<Dialog, Dialog> d in si.sentences)
                    {

                        campaign.AddPlayerLine((si.name + d.Item1.type + i), intoken, outtoken, d.Item1.sentence,
                            () => d.Item1.validatePlayerLine(),
                            () =>
                            {
                                runNpcResponse(si);
                                playerChoice(si);
                            //turnalloff();
                            //si.followUp(d.Item1,campaign,outtoken);
                        },
                            1000, null);
                        i++;
                    }
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

        private void clearIntended()
        {
            CharacterManager.MainCharacter.clearIntented();
        }
        private void intentStep()
        {

            
        }
        //TODO give gold gifts
        public static void giftMoney(Character c, Character c2)
        {
            int price = 100;
            GiveGoldAction.ApplyBetweenCharacters(c.hero, c2.hero, price, false);
            //this._hasBoughtTunToParty = true;
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
            
            Character character = NPCDialogBehaviour.characterManager.findChar(c);
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
            Character character = NPCDialogBehaviour.characterManager.findChar(c);
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
