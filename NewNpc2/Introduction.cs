using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace NewNpc2
{
    public static class Introduction
    {
        private static Dictionary<long,bool> intro = new Dictionary<long, bool>();

        public static void Introduce(CharacterObject c1, CharacterObject c2)
        {
            long hash = MBGUID.GetHash2(c1.Id, c2.Id);
            intro[hash] = true;
        }

        public static bool Introduced(CharacterObject c1, CharacterObject c2)
        {
            long hash = MBGUID.GetHash2(c1.Id, c2.Id);
            bool result;
            return intro.TryGetValue(hash, out result);
        }
    }
}
