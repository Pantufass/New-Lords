﻿using System;

using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace NewNpc2
{
    public class NewConvoBehaviour : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            this.AddDialogs(starter);
        }

        protected void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("test_id", "start", "test_out", "{=*}heelo",
                new ConversationSentence.OnConditionDelegate(this.conversation_test),
                new ConversationSentence.OnConsequenceDelegate(this.conversation_on_consequence),
                10000012, null);
            starter.AddDialogLine("test_id", "test_out", "test_out2", "{=*}hello",
                 null,
                 null,
                 10000012, null);
            starter.AddDialogLine("test_id", "test_out2", "test_out3", "{=*}very words",
                 null,
                 null,
                 10000012, null);
            starter.AddPlayerLine("test_id", "test_out3", "close_window", "{=*}leave",
                  null,
                  new ConversationSentence.OnConsequenceDelegate(this.conversation_leave_on_consequence),
                  10000012, null);
            starter.AddPlayerLine("test_id", "test_out3", "test_out4", "{=*}player option",
                new ConversationSentence.OnConditionDelegate(this.conversation_new_relation_test),
                null,
                10000022, null);
            starter.AddPlayerLine("test_id", "test_out3", "test_out4", "{=*}raise relation 2",
                null,
                 new ConversationSentence.OnConsequenceDelegate(this.raise2),
                10000022, null);
            starter.AddPlayerLine("test_id", "test_out3", "test_out4", "{=*}player option2",
                new ConversationSentence.OnConditionDelegate(this.conversation_new_relation_test2),
                null,
                10000022, null);
            starter.AddDialogLine("test_id", "test_out4", "close_window", "{=*}no",
                 null,
                 new ConversationSentence.OnConsequenceDelegate(this.conversation_leave_on_consequence),
                 10000012, null);
        }

        private void raise2()
        {

        }
        private bool conversation_new_relation_test()
        {

            return true;
        }

        private bool conversation_new_relation_test2()
        {
            return false;

        }

        private bool conversation_new_relation_test3()
        {

            return CharacterRelationManager.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero) > 1;

        }

        private bool conversation_test()
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            bool b = c.Occupation == Occupation.Townsfolk;
            Character chara = CharacterManager.findChar(c);
            if (chara  != null)
            {
                InformationManager.DisplayMessage(new InformationMessage(chara.isNice().ToString("F5")));
            }
            else
            {
                chara = new Character(new CulturalKnowledge());
                CharacterManager.characters.Add(c, chara);

                InformationManager.DisplayMessage(new InformationMessage(chara.isNice().ToString("F5")));
            }

            return b;
        } 

        private void conversation_on_consequence()
        {
            if(Hero.OneToOneConversationHero != null)
            {
                Hero.MainHero.SetPersonalRelation(Hero.OneToOneConversationHero, 10);

                NewCharacterRelationManager.SetFriendship(Hero.MainHero, Hero.OneToOneConversationHero, NewCharacterRelationManager.relation.Good);
            }
        }

        private void conversation_leave_on_consequence()
        {
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
