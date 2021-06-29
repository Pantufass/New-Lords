using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class NPCDialogBehaviour : CampaignBehaviorBase
    {

        private List<Character> currCharacters;
        private Mission currentMission;
        private bool missionReady = false;

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
            currCharacters = new List<Character>();
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
            //queue first impression here
            //Microtheory.FirstImpression(mission);
        }


        public void updateCharacters(List<Character> c)
        {
            currCharacters = c;
        }

        public void NPCReady(Character character)
        {

            InformationManager.DisplayMessage(new InformationMessage(character.agent.Name +" - " +character.getEnergy()));
            //pop up a sentence when ready
            //readyvol(character);
            if (missionReady)  characterVolition(character);
        }

        private void readyvol(Character c)
        {

            if (missionReady)
            {
                foreach (MissionBehaviour mb in currentMission.MissionBehaviours)
                {
                    if (mb is MissionViewBehaviour)
                        (mb as MissionViewBehaviour).dialog(new Dialog("Ready", 0, sentenceType.Normal), c);
                }

            }
        }

        private void characterVolition(Character character)
        {
            character.spendEnergy();
            character.getRumor();
            KeyValuePair<float, Character> pair = new KeyValuePair<float, Character>(-10,CharacterManager.MainCharacter);
            List<Character> availableCharacters = CharacterManager.getCharacters(currentMission.Agents);
            foreach(Character c in availableCharacters)
            {
                float r = character.calcNpcVolition(c);
                if (r > pair.Key) pair = new KeyValuePair<float, Character>(r, c);
            }
            SocialInteraction si = character.getNpcIntended();

            moveTo(character, pair.Value);

            showDialog(si.getDialog(character.calcDialogType(), pair.Key), character);

            calcCharacterResponse(character, pair.Value,si, character.getIntent());

            
        }

        private void showDialog(Dialog d, Character c)
        {
            if (missionReady)
            {
                foreach (MissionBehaviour mb in currentMission.MissionBehaviours)
                {
                    if (mb is MissionViewBehaviour)
                        (mb as MissionViewBehaviour).dialog(d, c);
                }

            }
        }


        private void calcCharacterResponse(Character init, Character res, SocialInteraction si, intent i)
        {
            SocialExchange se = new SocialExchange(init, res, si, i);
            float result = se.calculateResponse();
            Dialog d = se.getResponse(result);
            showDialog(d, res);

            SubModule.makeExchange(se);
            SubModule.endInteraction();
        }


        public static void moveTo(Character mover, Character target)
        {
            //how to get agent
            //maybe replace object with agent

            mover.OnExchange();
            target.OnExchange();
            if (mover.agent.GetComponent<CampaignAgentComponent>().AgentNavigator == null) return;
            DailyBehaviorGroup bg = mover.agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
            bg.AddBehavior<FollowAgentBehavior>().SetTargetAgent(target.agent);
            
            bg.SetScriptedBehavior<FollowAgentBehavior>();

            mover.agent.SetLookAgent(target.agent);
        }
        
    }
}
