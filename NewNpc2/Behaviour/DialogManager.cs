using SandBox.Source.Missions;
using SandBox.Source.Objects.SettlementObjects;
using SandBox.ViewModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NewNpc2
{
    public class DialogManager : ViewModel
    {
        private Counter secCounter;

        public DialogManager(Mission mission, Camera c) 
        {
            this.Targets = new MBBindingList<DialogTarget>();
            this._distanceComparer = new DialogManager.MarkerDistanceComparer();
            this._missionCamera = c;
            this._mission = mission;

            secCounter = new Counter();

        }

        public void showDialog(Dialog d, Character c)
        {
            EnableTarget(d, c);

        }

        private void EnableTarget(Dialog d, Character c)
        {
            foreach(DialogTarget dt in Targets)
            {
                if (dt.TargetAgent == c.agent)
                {
                    dt.Name = d.sentence;
                    dt.IsEnabled = true;
                    break;
                }
            }
        }

        private void Tick()
        {
            characterEnergy();
        }

        private void characterEnergy()
        {
            foreach (Character c in CharacterManager.getCharacters(_mission.Agents))
            {
                c.raiseEnergy();
            }
        }

        public void MissionTick(float dt)
        {
            if(_mission != null)
            {
                Tick(dt);
                if (secCounter.second(dt))
                {
                    Tick();
                }
                foreach (DialogTarget dialog in Targets)
                {
                    dialog.Tick(dt);
                }
            }
            
        }
        private void RemoveAgentTarget(Agent agent)
        {
            if (this.Targets.SingleOrDefault((DialogTarget t) => t.TargetAgent == agent) != null)
            {
                this.Targets.Remove(this.Targets.Single((DialogTarget t) => t.TargetAgent == agent));
            }
        }

        private void AddAgentTarget(Agent agent)
        {
            if (agent != Agent.Main && agent.Character != null && agent.IsActive() && !this.Targets.Any((DialogTarget t) => t.TargetAgent == agent))
            {
                if (agent.IsHuman)
                {
                    DialogTarget item = new DialogTarget(agent);
                    this.Targets.Add(item);
                }
            }
        }

        public void Tick(float dt)
        {
            if (this._firstTick)
            {
                if (this._mission.MainAgent != null)
                {
                    foreach (Agent agent in this._mission.Agents)
                    {
                        this.AddAgentTarget(agent);
                    }
                }
                this._firstTick = false;
            }
            if (this.IsEnabled)
            {
                this.UpdateTargetScreenPositions();
                this._fadeOutTimerStarted = false;
                this._fadeOutTimer = 0f;
                this._prevEnabledState = this.IsEnabled;
            }
            else
            {
                if (this._prevEnabledState)
                {
                    this._fadeOutTimerStarted = true;
                }
                if (this._fadeOutTimerStarted)
                {
                    this._fadeOutTimer += dt;
                }
                if (this._fadeOutTimer < 2f)
                {
                    this.UpdateTargetScreenPositions();
                }
                else
                {
                    this._fadeOutTimerStarted = false;
                }
            }
            this._prevEnabledState = this.IsEnabled;
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            this.Targets.ApplyActionOnAllItems(delegate (DialogTarget x)
            {
                x.RefreshValues();
            });
        }

        private void UpdateTargetScreenPositions()
        {
            foreach (DialogTarget dialogTarget in this.Targets)
            {
                float a = -100f;
                float b = -100f;
                float num = 0f;
                MBWindowManager.WorldToScreenInsideUsableArea(this._missionCamera, dialogTarget.WorldPosition + this._heightOffset, ref a, ref b, ref num);
                if (num > 0f){
                    dialogTarget.ScreenPosition = new Vec2(a, b);
                    dialogTarget.Distance = (int)(dialogTarget.WorldPosition - this._missionCamera.Position).Length;
                }
                else{
                    dialogTarget.Distance = -1;
                    dialogTarget.ScreenPosition = new Vec2(-100f, -100f);
                }
            }
            this.Targets.Sort(this._distanceComparer);
        }

        public void OnConversationEnd()
        {

        }

        public void OnAgentBuild(Agent agent)
        {
            this.AddAgentTarget(agent);
        }

        public void OnAgentRemoved(Agent agent)
        {
            this.RemoveAgentTarget(agent);
        }

        public void OnAgentDeleted(Agent agent)
        {
            this.RemoveAgentTarget(agent);
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
                    this.UpdateTargetStates(value);
                    Game.Current.EventManager.TriggerEvent<MissionNameMarkerToggleEvent>(new MissionNameMarkerToggleEvent(value));
                }
            }
        }


        private void UpdateTargetStates(bool state)
        {
            foreach (DialogTarget dialogt in this.Targets)
            {
                dialogt.IsEnabled = state;
            }
        }

        [DataSourceProperty]
        public MBBindingList<DialogTarget> Targets
        {
            get
            {
                return this._targets;
            }
            set
            {
                if (value != this._targets)
                {
                    this._targets = value;
                    base.OnPropertyChangedWithValue(value, "Targets");
                }
            }
        }
        private readonly Camera _missionCamera;

        // Token: 0x0400002C RID: 44
        private bool _firstTick = true;

        // Token: 0x0400002D RID: 45
        private readonly Mission _mission;

        // Token: 0x0400002E RID: 46
        private Vec3 _heightOffset = new Vec3(0f, 0f, 2f, -1f);

        // Token: 0x0400002F RID: 47
        private bool _prevEnabledState;

        // Token: 0x04000030 RID: 48
        private bool _fadeOutTimerStarted;

        // Token: 0x04000031 RID: 49
        private float _fadeOutTimer;

        private readonly DialogManager.MarkerDistanceComparer _distanceComparer;

        // Token: 0x04000033 RID: 51
        private readonly List<string> PassagePointFilter = new List<string>
        {
            "Empty Shop"
        };

        private MBBindingList<DialogTarget> _targets;

        private bool _isEnabled;
        private class MarkerDistanceComparer : IComparer<DialogTarget>
        {
            // Token: 0x060003A7 RID: 935 RVA: 0x0000FABC File Offset: 0x0000DCBC
            public int Compare(DialogTarget x, DialogTarget y)
            {
                return y.Distance.CompareTo(x.Distance);
            }
        }
    }
}
