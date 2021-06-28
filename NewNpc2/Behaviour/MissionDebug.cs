using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace NewNpc2
{
    internal class MissionDebug : MissionView
    {
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            InformationManager.DisplayMessage(new InformationMessage(Settlement.GetFirst.GetName().ToString()));
            
        }

    }
}