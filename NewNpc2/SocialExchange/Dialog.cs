using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Dialog
    {
        public double id;
        public string sentence;

        //more value means more confidence
        public float value;

        public sentenceType type;

        private bool _playera;
        public bool playera
        {
            get { return _playera; }
            set { _playera = value; }
        }
        private bool _cresponse;
        public bool cresponse
        {
            get { return _cresponse; }
            set { _cresponse = value; }
        }
        public Dialog(string s, float v, sentenceType t, bool c = false)
        {
            sentence = s;
            value = v;
            this.type = t;
            playera = false;
            cresponse = false;
            id = SubModule.rand.NextDouble();
        }

        public void updateValue(float v)
        {
            value = v;
        }

        public bool validateNpcLine()
        {
            if (SocialExchange.Last.resp == this) return true;
            if (SocialExchange.Last.resp.id == id) return true;
            if (SocialExchange.Last.resp.sentence == sentence)
            {
                return true;
            }
            return cresponse;
        }

        public bool validatePlayerLine()
        {
            foreach(Dialog d in CharacterManager.MainCharacter.dialogs)
            {
                if (d.sentence == this.sentence) return true;
            }
            return playera;
        }
    }

    public enum sentenceType
    {
        Cordial = 3,
        Normal = 1,
        Crude = 2,
        pResponse = -2,
        nResponse = -3,
        normalResponse = -1
    }
}
