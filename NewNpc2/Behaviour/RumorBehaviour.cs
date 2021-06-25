using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace NewNpc2
{

    public class RumorBehaviour : CampaignBehaviorBase
    {
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
        }

        private void OnLoad()
        {
            foreach (Settlement s in Settlement.All)
            {

            }

        }

        private void OnSettlement(MobileParty mp, Settlement s, Hero h)
        {
            if (h == Hero.MainHero) MainHeroOnSet(s);
            RumorParty party;
            if (!parties.TryGetValue(mp, out party))
            {
                party = new RumorParty(mp.LeaderHero);
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
                party = new RumorParty(mp.LeaderHero);
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
                party = new RumorParty(mp.LeaderHero);
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

        public void WorldEvent(Rumor r)
        {
            foreach(RumorHolder rh in settlements.Values)
            {

            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new NotImplementedException();
        }
    }
}
