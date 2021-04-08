using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{
    public class PlayerStartBehaviour : CampaignBehaviorBase
    {
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
            CreateCharacter();
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

            campaign.AddDialogLine(" start", "start", "start", " ",
                null,
                null,
                1000, null);

            foreach (SocialInteraction si in SubModule.existingExchanges.Values)
            {
                CharacterObject c = CharacterObject.OneToOneConversationCharacter;
                campaign.AddPlayerLine(si.name, "start", "step",si.getDialogLine(sentenceType.Normal,0),
                    (()=>si.validate(Hero.MainHero.CharacterObject,c)),
                    (()=>
                    {
                        newInteraction(campaign,si);
                    }),
                    1000, null);
                }
            
           
        }

        private void newInteraction(CampaignGameStarter campaign, SocialInteraction si)
        {
            CharacterObject c = CharacterObject.OneToOneConversationCharacter;
            Character character = CharacterManager.findChar(c);
            if (character == null)
            {
                if (c.IsHero)
                {
                    character = new Character(new CulturalKnowledge("a"), c.HeroObject.GetHeroTraits(),c);
                    CharacterManager.characters.Add(c, character);
                }
                else
                {
                    character = new Character(new CulturalKnowledge("a"),c);
                    CharacterManager.characters.Add(c, character);
                }
            }
            float result = si.calculateResponse(CharacterManager.MainCharacter, character, intent.Neutral);

            if (si.finish) endInteraction(si, si.GetOutcome(result), character);
            campaign.AddDialogLine(si.name, "step", "start", si.getResponse(result),
                null,
                (() =>
                {
                    makeExchange(si, si.GetOutcome(result), character);
                }),
                1000,null
                );

        }


        private void makeExchange(SocialInteraction prev, outcome o, Character character)
        {
            SocialExchange se = new SocialExchange(CharacterManager.MainCharacter, character, prev,o);
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
