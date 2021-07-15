using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class NPCDialogBehaviour : CampaignBehaviorBase
    {

        private Mission currentMission;
        private bool missionReady = false;
        private bool firstDone = false;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.AfterGameLoad));
            CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStart));
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnd));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunched(CampaignGameStarter campaign)
        {
        }

        private void AfterGameLoad()
        {

        }

        private void OnMissionEnd(IMission mission)
        {
            currentMission = null;
            missionReady = false;
        }

        private void OnMissionStart(IMission mission)
        {
            this.currentMission = (Mission) mission;
            missionReady = true;


            Microtheory.FirstImpression(CharacterManager.getCharacters(currentMission.Agents));
        }


        public void NPCReady(Character character)
        {

            InformationManager.DisplayMessage(new InformationMessage(character.agent.Name +" - " +character.getEnergy()));

            if (missionReady)  characterVolition(character);
        }

        private bool notIntoExchange(Character character)
        {
            return character.hero == Hero.MainHero || character.agent == Agent.Main ||
                character.agent.Character != null && character.agent.Character.Name.ToString() == "Training Master";
        }
        private void characterVolition(Character character)
        {
            if (notIntoExchange(character)) 
                
                return;

            character.calcRumor();

            if (character.characterObject.Occupation == Occupation.Tavernkeeper || character.characterObject.Occupation == Occupation.Guard || character.characterObject.Occupation == Occupation.PrisonGuard)
            {
                stayVolition(character);
                return;
            }

            Tuple<float, Character, SocialInteraction> pair = new Tuple<float, Character, SocialInteraction>(-10,CharacterManager.MainCharacter,null);
            foreach(Character c in CharacterManager.getCharacters(currentMission.Agents))
            {
                if (c == character) continue;
                float r = character.calcNpcVolition(c);
                if (r > pair.Item1) 
                    pair = new Tuple<float, Character, SocialInteraction>(r, c, character.getNpcIntended());
            }

            /*
            moveTo(character, pair.Item2);

            if(pair.Item3.hasPaths) showDialog(pair.Item3.getDialog(character.calcDialogType(), pair.Item1, character), character);
            else showDialog(pair.Item3.getDialog(character.calcDialogType(), pair.Item1), character);

            */


            character.exchange(pair.Item2, pair.Item3);


        }

        private void stayVolition(Character character)
        {

        }

        private bool isHostile()
        {
            return false;
        }

        private void calcCharacterResponse(Character init, Character res, SocialInteraction si, intent i)
        {
            SocialExchange se = new SocialExchange(init, res, si, i);
            float result = se.calculateResponse();
            Dialog d = se.getResponse(result);
            res.showDialog(d);

            SubModule.makeExchange(se);
        }


        public static bool moveTo(Character mover, Character target)
        {
            //how to get agent
            //maybe replace object with agent
            if (mover.agent.Position.Distance(target.agent.Position) < 2.5f)
            {
                if (mover.performing)
                {
                    
                }
                mover.agent.SetLookAgent(target.agent);
                return true;
            }

            if (!mover.performing)
            {
                if (mover.agent.GetComponent<CampaignAgentComponent>().AgentNavigator == null) return false;


                DailyBehaviorGroup bg = mover.agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
                bg.AddBehavior<FollowAgentBehavior>().SetTargetAgent(target.agent);

                bg.SetScriptedBehavior<FollowAgentBehavior>();

                mover.agent.SetLookAgent(target.agent);
            }



            return true;

        }
        
    }
}
