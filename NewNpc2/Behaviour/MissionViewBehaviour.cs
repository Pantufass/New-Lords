using SandBox.GauntletUI;
using SandBox.View.Missions;
using SandBox.ViewModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace NewNpc2
{
    [OverrideView(typeof(MissionNameMarkerUIHandler))]
    public class MissionViewBehaviour : MissionView
    {

        protected GauntletLayer _gauntletLayer;
        protected DialogManager _dataSource;


        public MissionViewBehaviour() : base()
        {
            this.ViewOrderPriorty = 1;
        }

        public override void OnMissionScreenInitialize()
        {

			CharacterManager.main(Agent.Main);

			base.OnMissionScreenInitialize();

            _dataSource = new DialogManager(base.Mission, base.MissionScreen.CombatCamera);
            this._gauntletLayer = new GauntletLayer(ViewOrderPriorty, "GauntletLayer");
            this._gauntletLayer.LoadMovie("NameMarker", this._dataSource);
            base.MissionScreen.AddLayer(this._gauntletLayer);
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<CharacterObject>(this.OnConversationEnd));
            CampaignEvents.SetupPreConversationEvent.AddNonSerializedListener(this, new Action(this.OnConversationStart));

			_dataSource.IsEnabled = true;
        }


        public void OnConversationStart()
        {
           // _dataSource.IsEnabled = true;
            
        }

        public void OnConversationEnd(CharacterObject co)
        {

            this._dataSource.OnConversationEnd();
        }

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			CampaignEvents.ConversationEnded.ClearListeners(this);
			InformationManager.ClearAllMessages();

		}

		public void dialog(Dialog d, Character c)
        {
			 _dataSource.showDialog(d, c);
        }

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.IsEnabled = true;
			_dataSource.MissionTick(dt);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000077B2 File Offset: 0x000059B2
		public override void OnAgentBuild(Agent affectedAgent, Banner banner)
		{
			base.OnAgentBuild(affectedAgent, banner);
			DialogManager dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnAgentBuild(affectedAgent);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000077CD File Offset: 0x000059CD
		public override void OnAgentDeleted(Agent affectedAgent)
		{
			this._dataSource.OnAgentDeleted(affectedAgent);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000077DB File Offset: 0x000059DB
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			this._dataSource.OnAgentRemoved(affectedAgent);
		}


		// Token: 0x060000E8 RID: 232 RVA: 0x000077F6 File Offset: 0x000059F6
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007813 File Offset: 0x00005A13
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

	}
}
