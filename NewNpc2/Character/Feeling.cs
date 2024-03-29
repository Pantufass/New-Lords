﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace NewNpc2
{
    public class Feeling
    {
        [SaveableField(1)]
        protected Character initiator;

        [SaveableField(2)]
        protected intent intent;

        [SaveableField(3)]
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

    public enum Notion
    {
        Violent,
        Warmonger,
        Peaceful,
        Coward,
        Womanizer,
        Economist,
        Talkative

    }
}
