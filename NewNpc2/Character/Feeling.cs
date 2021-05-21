using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Feeling
    {
        protected Character initiator;

        protected intent intent;

        protected float intensity;

        public Feeling(Character init)
        {
            initiator = init;
        }

        public Feeling(Character init,float i, intent intent)
        {
            initiator = init;
            intensity = i;
            this.intent = intent;
        }

        public Character getInitiator()
        {
            return initiator;
        }

        public void setIntensity(float i)
        {
            intensity = i;
        }
        public float getIntensity()
        {
            return intensity;
        }
        public void setIntent(intent i)
        {
            intent = i;
        }

        public intent getIntent()
        {
            return intent;
        }

        internal void addIntensity(float v)
        {
            intensity += v;
        }
    }
}
