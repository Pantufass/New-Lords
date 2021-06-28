using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace NewNpc2
{

    public class RumorBehaviour : CampaignBehaviorBase
    {

        protected const float CLOSE_DISTANCE = 30;

        public Dictionary<MobileParty, RumorParty> parties;
        public Dictionary<Settlement, RumorHolder> settlements;
        public RumorHolder currentSet;

        public RumorBehaviour()
        {
            parties = new Dictionary<MobileParty, RumorParty>();
            settlements = new Dictionary<Settlement, RumorHolder>();

        }

        public override void RegisterEvents()
        {
            CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlement));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyCreated));
            CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.PartyDestroyed));
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.PartyLeave));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction>(this.OnWarDeclared));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnLoad));

            CampaignEvents.CharacterDefeated.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnDefeat));
        }

        private void OnLoad()
        {
            foreach (Settlement s in Settlement.All)
            {
                settlements.Add(s, new RumorHolder());
            }

        }

        private void OnSettlement(MobileParty mp, Settlement s, Hero h)
        {
            if (h == Hero.MainHero) MainHeroOnSet(s);
            RumorParty party;
            if (!parties.TryGetValue(mp, out party))
            {
                party = new RumorParty(mp.Leader);
                parties.Add(mp, party);
            }
            RumorHolder curr;
            if (!settlements.TryGetValue(s, out curr))
            {
                curr = new RumorHolder();
                settlements.Add(s, curr);
            }

            curr.setRumors(party.getRumors());
        }

        private void MainHeroOnSet(Settlement s)
        {
            RumorHolder holder;
            if (settlements.TryGetValue(s, out holder)) currentSet = holder;
            else
            {
                holder = new RumorHolder();
                settlements.Add(s, holder);
            }

        }

        private void OnPartyCreated(MobileParty mp)
        {
            if (!parties.TryGetValue(mp, out RumorParty party))
            {
                party = new RumorParty(mp.Leader);
                parties.Add(mp, party);
            }
        }

        private void PartyDestroyed(MobileParty mp)
        {
            parties.Remove(mp);
        }

        private void PartyLeave(MobileParty mp, Settlement s)
        {
            if (!parties.TryGetValue(mp, out RumorParty party))
            {
                party = new RumorParty(mp.Leader);
                parties.Add(mp, party);
            }
            if (!settlements.TryGetValue(s, out RumorHolder curr))
            {
                curr = new RumorHolder();
                settlements.Add(s, curr);
            }
            party.setRumors(curr.getRumors());
        }

        private void WeeklyTick()
        {
            SubModule.findPatter();
        }

        private void OnWarDeclared(IFaction declarer, IFaction declared)
        {
            SocialExchange se = new SocialExchange(CharacterManager.findChar(declarer.Leader), CharacterManager.findChar(declared.Leader), SocialInteractionManager.War(), intent.Negative);

            List<string> l = new List<string>();
            l.Add(declarer.Name.ToString());
            l.Add(declared.Name.ToString());
            Rumor r = new Rumor(new Rumor.Information(l, true), 0.7f, se);

            WorldEvent(r);
        }

        private void OnDefeat(Hero winner, Hero loser)
        {
            SocialExchange se = new SocialExchange(CharacterManager.findChar(winner.CharacterObject), CharacterManager.findChar(loser.CharacterObject), SocialInteractionManager.Battle(), intent.Neutral);
            Rumor r = new Rumor(new Rumor.Information(Rumor.Information.type.Warfare), se);

            
            CloseWorldEvent(r, winner.GetPosition());
        }

        public void CloseWorldEvent(Rumor r, Vec3 pos)
        {
            foreach (KeyValuePair<Settlement,RumorHolder> rh in settlements)
            {
                if(rh.Key.GetPosition().Distance(pos) < CLOSE_DISTANCE)
                    rh.Value.addRumor(r);
            }

        }

        public void WorldEvent(Rumor r)
        {
            foreach(RumorHolder rh in settlements.Values)
            {
                rh.addRumor(r);
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
