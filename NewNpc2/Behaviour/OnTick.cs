using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace NewNpc2
{
    public class OnTick : MissionView
    {


        

    }

    public class Counter
    {
        private float counter;

        public Counter()
        {
            counter = 0;
        }

        public bool second(float dt)
        {
            return severalSeconds(dt, 1);
        }

        public bool severalSeconds(float dt, int seconds)
        {
            counter += dt;
            if (counter >= seconds)
            {
                counter = 0;
                return true;
            }
            return false;
        }
    }
}
