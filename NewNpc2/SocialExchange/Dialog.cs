using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Dialog
    {
        public string sentence;

        //more value means more confidence
        public float value;

        public sentenceType type;

        public Dialog(string s, float v, sentenceType t)
        {
            sentence = s;
            value = v;
            this.type = t;
        }

        public void updateValue(float v)
        {
            value = v;
        }
    }

    public enum sentenceType
    {
        Cordial = 1,
        Normal = 1,
        Crude = 1,
        pResponse = -1,
        nResponse = -1,
        normalResponse = -1
    }
}
