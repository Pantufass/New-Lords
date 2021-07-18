using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using static NewNpc2.Character;
using static NewNpc2.NewCharacterRelationManager;

namespace NewNpc2
{
    public class CustomSave : SaveableTypeDefiner
    {

        public CustomSave() : base(637757009) { }

        
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(Character), 1);
            AddClassDefinition(typeof(Feeling), 2);
            AddClassDefinition(typeof(RumorParty), 3);
            AddClassDefinition(typeof(RumorHolder), 4);
            AddClassDefinition(typeof(SocialExchange), 5);
            AddClassDefinition(typeof(SocialInteraction), 6);
            AddClassDefinition(typeof(Rumor), 7);
            AddClassDefinition(typeof(Culture), 8);
            AddClassDefinition(typeof(Traits), 9);
            AddClassDefinition(typeof(CharacterManager), 10);

            AddClassDefinition(typeof(NewCharacterRelationManager), 11);
            AddClassDefinition(typeof(Relations), 12);

        }

        
        protected override void DefineContainerDefinitions()
        {
            
            ConstructContainerDefinition(typeof(Dictionary<MobileParty, RumorParty>));
            ConstructContainerDefinition(typeof(Dictionary<Settlement, RumorHolder>));
            ConstructContainerDefinition(typeof(List<Feeling>));
            ConstructContainerDefinition(typeof(Dictionary<Character, List<Feeling>>));
            ConstructContainerDefinition(typeof(Dictionary<SocialExchange, float>));
            ConstructContainerDefinition(typeof(Dictionary<CharacterObject, Character>));
            ConstructContainerDefinition(typeof(List<SocialExchange>));

            //ConstructContainerDefinition(typeof(Dictionary<Character, Agent>));


        }
    }
}
