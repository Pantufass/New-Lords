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
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
        public void OnSessionLaunched(CampaignGameStarter starter)
        {
            this.startInterction(starter);
        }

        public void startInterction(CampaignGameStarter campaign)
        {
            campaign.AddPlayerLine("startInt", "start", "step1"," ",
                null,
                null,
                1000, null);
        }
    }
}
