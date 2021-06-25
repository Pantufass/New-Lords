

using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NewNpc2
{

    [SaveableClass(200180)]
    public class NewCharacterRelationManager
    {

        [SaveableField(2)]
        private NewCharacterRelationManager.Friendship friendship;
        [SaveableField(3)]
        private NewCharacterRelationManager.Romantic romantic;

        public static NewCharacterRelationManager Instance
        {
            get
            {
                return SubModule.newRelationManager;
            }
        }

        // Token: 0x0600093A RID: 2362 RVA: 0x0002C64C File Offset: 0x0002A84C
        public NewCharacterRelationManager() : base()
        {
            this.friendship = new NewCharacterRelationManager.Friendship();
            this.romantic = new NewCharacterRelationManager.Romantic();

        }

        // Token: 0x0600093B RID: 2363 RVA: 0x0002C65F File Offset: 0x0002A85F
        public static relation GetFriendship(BasicCharacterObject Character1, BasicCharacterObject Character2)
        {
            return (NewCharacterRelationManager.Instance).friendship.GetRelation(Character1, Character2);
        }

        // Token: 0x0600093C RID: 2364 RVA: 0x0002C672 File Offset: 0x0002A872
        public static void SetFriendship(BasicCharacterObject Character1, BasicCharacterObject Character2, relation value)
        {
            (NewCharacterRelationManager.Instance).friendship.SetRelation(Character1, Character2, value);
        }

        public static relation GetRomantic(BasicCharacterObject Character1, BasicCharacterObject Character2)
        {
            return (NewCharacterRelationManager.Instance).romantic.GetRelation(Character1, Character2);
        }

        // Token: 0x0600093C RID: 2364 RVA: 0x0002C672 File Offset: 0x0002A872
        public static void SetRomantic(BasicCharacterObject Character1, BasicCharacterObject Character2, relation value)
        {
            (NewCharacterRelationManager.Instance).romantic.SetRelation(Character1, Character2, value);
        }

        [SaveableClass(200182)]
        internal class Friendship : Relations { }

        [SaveableClass(200183)]
        internal class Romantic : Relations { }

        public enum relation
        {
            Great=5,
            Good=4,
            Neutral=3,
            Bad=2,
            Terrible=1
        }

        [SaveableClass(200181)]
        internal class Relations
        {

            // Token: 0x0600332E RID: 13102 RVA: 0x000E748C File Offset: 0x000E568C
            public virtual relation GetRelation(BasicCharacterObject Character1, BasicCharacterObject Character2)
            {
                long hash = MBGUID.GetHash2(Character1.Id, Character2.Id);
                relation result;
                if (this._relations.TryGetValue(hash, out result))
                {
                    return result;
                }
                return 0;
            }

            // Token: 0x0600332F RID: 13103 RVA: 0x000E74C0 File Offset: 0x000E56C0
            public virtual void SetRelation(BasicCharacterObject Character1, BasicCharacterObject Character2, relation v)
            {
                long hash = MBGUID.GetHash2(Character1.Id, Character2.Id);
                this._relations[hash] = v;
            }

            // Token: 0x040011BF RID: 4543
            [SaveableField(0)]
            private readonly Dictionary<long, relation> _relations = new Dictionary<long, relation>();
        }


    }
}
