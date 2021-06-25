using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace NewNpc2
{
    public static class Introduction
    {
        // 0 = not introd
        // 1 = seen
        // 2 = introd
        // 3 = not introd

        private static Dictionary<long,int> intro = new Dictionary<long, int>();

        public static bool Introduce(BasicCharacterObject c1, BasicCharacterObject c2)
        {
            long hash = MBGUID.GetHash2(c1.Id, c2.Id);
            intro[hash] ++;
            return true;
        }

        public static int Introduced(BasicCharacterObject c1, BasicCharacterObject c2)
        {
            long hash = MBGUID.GetHash2(c1.Id, c2.Id);
            int result = 0;
            intro.TryGetValue(hash, out result);
            return result;
        }

    }
}
