using SandBox.Source.Objects.SettlementObjects;
using SandBox.ViewModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class DialogTarget : ViewModel
    {
        public bool IsMovingTarget { get; }
        public Agent TargetAgent { get; }
        public CommonAreaMarker TargetCommonAreaMarker { get; }
        public CommonArea TargetCommonArea { get; }
        public PassageUsePoint TargetPassageUsePoint { get; }
        public Vec3 TargetWorkshopPosition { get; private set; }

		protected Counter counter;

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public int MarkerType
		{
			get
			{
				return this._markerType;
			}
			set
			{
				if (value != this._markerType)
				{
					this._markerType = value;
					base.OnPropertyChangedWithValue(value, "MarkerType");
				}
			}
		}
		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}
		[DataSourceProperty]
		public int QuestMarkerType
		{
			get
			{
				return this._questMarkerType;
			}
			set
			{
				if (value != this._questMarkerType)
				{
					this._questMarkerType = value;
					base.OnPropertyChangedWithValue(value, "QuestMarkerType");
				}
			}
		}
		[DataSourceProperty]
		public int IssueMarkerType
		{
			get
			{
				return this._issueMarkerType;
			}
			set
			{
				if (value != this._issueMarkerType)
				{
					this._issueMarkerType = value;
					base.OnPropertyChangedWithValue(value, "IssueMarkerType");
				}
			}
		}

        internal void Tick(float dt)
        {
            if (IsEnabled)
            {
                if (counter.severalSeconds(dt,3))
                {
					IsEnabled = false;
                }
            }
        }

        [DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}
		[DataSourceProperty]
		public bool IsAgentInPrison
		{
			get
			{
				return this._isAgentInPrison;
			}
			set
			{
				if (value != this._isAgentInPrison)
				{
					this._isAgentInPrison = value;
					base.OnPropertyChangedWithValue(value, "IsAgentInPrison");
				}
			}
		}
		[DataSourceProperty]
		public bool IsQuestMainStory
		{
			get
			{
				return this._isQuestMainStory;
			}
			set
			{
				if (value != this._isQuestMainStory)
				{
					this._isQuestMainStory = value;
					base.OnPropertyChangedWithValue(value, "IsQuestMainStory");
				}
			}
		}
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		[DataSourceProperty]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChangedWithValue(value, "IsTracked");
				}
			}
		}
		private Vec3 _worldPosition;

		private Vec2 _screenPosition;

		private int _distance;

		private string _name;

		private int _markerType;

		private int _questMarkerType;

		private int _issueMarkerType;

		private bool _isEnabled;

		private bool _isTracked;

		private bool _isAgentInPrison;

		private bool _isQuestMainStory;

		public DialogTarget(CommonAreaMarker cam)
        {
			TargetCommonAreaMarker = cam;
			this.IsAgentInPrison = false;
			this.IsMovingTarget = false;
			this.MarkerType = 4;
			this.QuestMarkerType = 0;
			this.IssueMarkerType = 0;
			this.TargetCommonArea = Hero.MainHero.CurrentSettlement.CommonAreas[cam.AreaIndex - 1];
			this.Name = this.TargetCommonArea.Name.ToString();
			CommonAreaPartyComponent commonAreaPartyComponent = this.TargetCommonArea.CommonAreaPartyComponent;
			if (commonAreaPartyComponent != null && commonAreaPartyComponent.MobileParty.MemberRoster.TotalManCount > 0 && Hero.MainHero.GetRelation(this.TargetCommonArea.Owner) <= 0)
			{
				this.MarkerType = 3;
			}
			this.QuestMarkerType = (Campaign.Current.VisualTrackerManager.CheckTracked(this.TargetCommonArea) ? 2 : 0);
			counter = new Counter();
		}
		public DialogTarget(WorkshopType workshopType, Vec3 signPosition)
		{
			this.TargetWorkshopPosition = signPosition;
			this.IsAgentInPrison = false;
			this.IsMovingTarget = false;
			this.Name = workshopType.Name.ToString();
			this.QuestMarkerType = 0;
			this.IssueMarkerType = 0;
			this.MarkerType = 22;
			this.QuestMarkerType = 0;
			counter = new Counter();
		}
		public DialogTarget(PassageUsePoint passageUsePoint)
		{
			counter = new Counter();
			this.TargetPassageUsePoint = passageUsePoint;
			this.IsAgentInPrison = false;
			this.IsMovingTarget = false;
			this.Name = passageUsePoint.ToLocation.Name.ToString();
			this.QuestMarkerType = 0;
			this.IssueMarkerType = 0;
			if (passageUsePoint.ToLocation.Name.Contains("Lords Hall"))
			{
				this.MarkerType = 8;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Arena"))
			{
				this.MarkerType = 6;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Dungeon"))
			{
				this.MarkerType = 7;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Tavern"))
			{
				this.MarkerType = 5;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Smithy"))
			{
				this.MarkerType = 15;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Stable"))
			{
				this.MarkerType = 16;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Empty Shop"))
			{
				this.MarkerType = 11;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Velvet Weavery"))
			{
				this.MarkerType = 20;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Linen Weavery"))
			{
				this.MarkerType = 19;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Brewery"))
			{
				this.MarkerType = 9;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Mill"))
			{
				this.MarkerType = 12;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Pottery"))
			{
				this.MarkerType = 14;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Olive"))
			{
				this.MarkerType = 13;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Weavery"))
			{
				this.MarkerType = 18;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Tannery"))
			{
				this.MarkerType = 17;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Butcher"))
			{
				this.MarkerType = 10;
				return;
			}
			if (passageUsePoint.ToLocation.Name.Contains("Wood Workshop"))
			{
				this.MarkerType = 21;
				return;
			}
			this.MarkerType = 5;
		}

		public DialogTarget(Agent agent)
		{
			counter = new Counter();
			this.IsMovingTarget = true;
			this.TargetAgent = agent;
			this.Name = " ";
			this.MarkerType = 1;
			this.QuestMarkerType = 0;
			this.IssueMarkerType = 0;
			this.IsAgentInPrison = false;
			CharacterObject characterObject = (CharacterObject)agent.Character;
			if (characterObject != null)
			{
				Hero heroObject = characterObject.HeroObject;
				if (heroObject != null && heroObject.IsNoble)
				{
					if (FactionManager.IsAtWarAgainstFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
					{
						this.MarkerType = 2;
					}
					else if (FactionManager.IsAlliedWithFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
					{
						this.MarkerType = 0;
					}
					else
					{
						this.MarkerType = 1;
					}
				}
				if (characterObject.HeroObject != null)
				{
					this.IsAgentInPrison = characterObject.HeroObject.IsPrisoner;
				}
				if (agent.IsHuman && characterObject.IsHero && agent != Agent.Main)
				{
					this.UpdateQuestStatus();
				}
			}
		}

		public void UpdateQuestStatus()
		{
			this.QuestMarkerType = 0;
			this.IssueMarkerType = 0;
			QuestType questType = QuestType.None;
			if (this.TargetAgent != null && (CharacterObject)this.TargetAgent.Character != null && ((CharacterObject)this.TargetAgent.Character).HeroObject != null)
			{
				Tuple<SandBoxUIHelper.QuestType, SandBoxUIHelper.QuestState> questStateOfHero = SandBoxUIHelper.GetQuestStateOfHero(((CharacterObject)this.TargetAgent.Character).HeroObject);
				if (questStateOfHero.Item2 == SandBoxUIHelper.QuestState.Active)
				{
					questType = QuestType.Active;
				}
				else if (questStateOfHero.Item2 == SandBoxUIHelper.QuestState.Available)
				{
					questType = QuestType.Available;
				}
				if (questStateOfHero.Item1 == SandBoxUIHelper.QuestType.Issue)
				{
					this.IssueMarkerType = (int)questType;
					return;
				}
				if (questStateOfHero.Item1 == SandBoxUIHelper.QuestType.Main)
				{
					this.QuestMarkerType = (int)questType;
				}
				return;
			}
			else
			{
				if (this.TargetCommonAreaMarker != null && this.TargetCommonArea != null)
				{
					questType = (Campaign.Current.VisualTrackerManager.CheckTracked(this.TargetCommonArea) ? QuestType.Active : QuestType.None);
					this.IssueMarkerType = (int)questType;
					return;
				}
				Agent targetAgent = this.TargetAgent;
				if (targetAgent != null && !targetAgent.IsHero)
				{
					questType = (Settlement.CurrentSettlement.LocationComplex.FindCharacter(this.TargetAgent).IsVisualTracked ? QuestType.Active : QuestType.None);
					this.IssueMarkerType = (int)questType;
					return;
				}
				return;
			}
		}


		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		public Vec3 WorldPosition
		{
			get
			{
				switch (this.MarkerType)
				{
					case 0:
					case 1:
					case 2:
						return this.TargetAgent.Position;
					case 3:
					case 4:
						return this.TargetCommonAreaMarker.GetPosition();
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
						return this.TargetPassageUsePoint.GameEntity.GlobalPosition;
					case 22:
						return this.TargetWorkshopPosition;
					default:
						return Vec3.One;
				}
			}
			set
			{
				this._worldPosition = value;
			}
		}
	}

	public enum EntitiyType
	{
		// Token: 0x040001EA RID: 490
		FriendlyNobleAgent,
		// Token: 0x040001EB RID: 491
		NeutralNobleAgent,
		// Token: 0x040001EC RID: 492
		EnemyNobleAgent,
		// Token: 0x040001ED RID: 493
		EnemyCommonArea,
		// Token: 0x040001EE RID: 494
		NeutralCommonArea,
		// Token: 0x040001EF RID: 495
		TavernPassage,
		// Token: 0x040001F0 RID: 496
		ArenaPassage,
		// Token: 0x040001F1 RID: 497
		DungeonPassage,
		// Token: 0x040001F2 RID: 498
		LordsHallPassage,
		// Token: 0x040001F3 RID: 499
		Brewery,
		// Token: 0x040001F4 RID: 500
		Butcher,
		// Token: 0x040001F5 RID: 501
		EmptyShop,
		// Token: 0x040001F6 RID: 502
		Mill,
		// Token: 0x040001F7 RID: 503
		OlivePress,
		// Token: 0x040001F8 RID: 504
		Pottery,
		// Token: 0x040001F9 RID: 505
		Smithy,
		// Token: 0x040001FA RID: 506
		Stable,
		// Token: 0x040001FB RID: 507
		Tannery,
		// Token: 0x040001FC RID: 508
		Weavery,
		// Token: 0x040001FD RID: 509
		WeaveryLinen,
		// Token: 0x040001FE RID: 510
		WeaveryVelvet,
		// Token: 0x040001FF RID: 511
		WoodWorkshop,
		// Token: 0x04000200 RID: 512
		GenericWorkshop
	}

	// Token: 0x02000035 RID: 53
	public enum QuestType
	{
		// Token: 0x04000202 RID: 514
		None,
		// Token: 0x04000203 RID: 515
		Available,
		// Token: 0x04000204 RID: 516
		Active
	}
}
