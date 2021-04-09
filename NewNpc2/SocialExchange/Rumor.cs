using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Rumor
    {
        private SocialExchange s;

        public information info;

        public Rumor(SocialExchange social, information i)
        {
            s = social;
            info = i;
        }

        public int interest(Character c)
        {
            return 0;
        }

        public enum information
        {
            Gossip,
            News,
            Lie
        }
    }
}
