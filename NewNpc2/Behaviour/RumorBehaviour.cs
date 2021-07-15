using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NewNpc2
{

    public class RumorBehaviour : CampaignBehaviorBase
    {

        protected const float CLOSE_DISTANCE = 40;

        public Dictionary<MobileParty, RumorParty> parties;
        public Dictionary<Settlement, RumorHolder> settlements;
        public RumorHolder currentSet;

        private bool init = false;

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
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction>(this.OnWarDeclared));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnLoad));

            //CampaignEvents.CharacterDefeated.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnDefeat));

            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(OnMapEvent));
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.OnTick));
        }

        private void OnLoad()
        {
            foreach (Settlement s in Settlement.All)
            {
                settlements.Add(s, new RumorHolder());
            }

        }


        private void OnTick(float dt)
        {

            //InformationManager.DisplayMessage(new InformationMessage(Settlement.GetFirst.GetPosition().Distance(Hero.MainHero.GetPosition()).ToString()));
        }

        private void OnSettlement(MobileParty mp, Settlement s, Hero h)
        {
            if (mp == null || s == null || h == null) return;
            Character c = CharacterManager.findChar(h);
            c.setSet(s);
            if (h == Hero.MainHero) MainHeroOnSet(s);
            RumorParty party;
            if (!parties.TryGetValue(mp, out party))
            {
                party = new RumorParty(mp);
                parties.Add(mp, party);
            }
            RumorHolder curr;
            if (!settlements.TryGetValue(s, out curr))
            {
                curr = new RumorHolder();
                settlements.Add(s, curr);
            }

            initSet(curr);
            curr.setRumors(party.getRumors());
        }

        private void initSet(RumorHolder rh)
        {
            List<string> l = new List<string>();
            l.Add("asdrubal");
            l.Add("letirio");

            rh.addRumor(new Rumor(new Rumor.Information(l,true)));

        }

        
        private void MainHeroOnSet(Settlement s)
        {
            RumorHolder holder;
            if (settlements.TryGetValue(s, out holder)) currentSet = holder;
            else
            {
                holder = new RumorHolder();
                settlements.Add(s, holder);
                currentSet = holder;
            }

        }

        private void OnPartyCreated(MobileParty mp)
        {
            findParty(mp);
        }

        private void PartyDestroyed(MobileParty mp)
        {
            parties.Remove(mp);
        }

        public RumorHolder findSettlement(Settlement s)
        {
            if (!settlements.TryGetValue(s, out RumorHolder curr))
            {
                curr = new RumorHolder();
                settlements.Add(s, curr);
            }
            return curr;
        }

        public RumorParty findParty(MobileParty mp)
        {
            if (!parties.TryGetValue(mp, out RumorParty party))
            {
                party = new RumorParty(mp);
                parties.Add(mp, party);
            }
            return party;
        }

        private void PartyLeave(MobileParty mp, Settlement s)
        {
            RumorParty party = findParty(mp);
            RumorHolder curr = findSettlement(s);
            party.setRumors(curr.getRumors());
        }

        private void WeeklyTick()
        {
            SubModule.findPatter();
        }

        public static void setPattern(Character c, SocialInteraction si)
        {
            
        }

        private void OnWarDeclared(IFaction declarer, IFaction declared)
        {
            SocialExchange se = new SocialExchange(CharacterManager.findChar(declarer.Leader), CharacterManager.findChar(declared.Leader), SocialInteractionManager.War(), intent.Negative);

            List<string> l = new List<string>();
            l.Add(declarer.Name.ToString());
            l.Add(declared.Name.ToString());
            Rumor r = new Rumor(new Rumor.Information(l, true), se, 0.7f);

            WorldEvent(r);
        }


        private void OnMapEvent(MapEvent me)
        {
            if (me.Winner.LeaderParty.LeaderHero == null) return;
            Character winner = CharacterManager.findChar(me.Winner.LeaderParty.LeaderHero);
            Character loser = CharacterManager.findChar(me.GetLeaderParty(me.DefeatedSide));


            SocialExchange se = new SocialExchange(winner, loser, SocialInteractionManager.Battle(), intent.Neutral);
            Rumor r = new Rumor(new Rumor.Information(Rumor.Information.type.Warfare), se);

            CloseWorldEvent(r, me.Winner.LeaderParty.LeaderHero.GetPosition(), winner.hero == Hero.MainHero);
        }

        private void OnDefeat(Hero winner, Hero loser)
        {
            SocialExchange se = new SocialExchange(CharacterManager.findChar(winner.CharacterObject), CharacterManager.findChar(loser.CharacterObject), SocialInteractionManager.Battle(), intent.Neutral);
            Rumor r = new Rumor(new Rumor.Information(Rumor.Information.type.Warfare), se);

            if (winner == Hero.MainHero) CloseWorldEvent(r, winner.GetPosition(), true);
            else CloseWorldEvent(r, winner.GetPosition());
        }

        private void DailyTick()
        {
            Settlement s = Settlement.All[SubModule.rand.Next(Settlement.All.Count)];
            CreateGossip(settlements[s],s);
        }

        private void CreateGossip(RumorHolder rh, Settlement s)
        {

        }

        public static void CreateGossip(SocialExchange se)
        {
            if (se.getInitiator().agent != null)
            {

            }
            else if (se.getReceiver().agent != null)
            {

            }
        }

        public void CloseWorldEvent(Rumor r, Vec3 pos, bool isMain = false)
        {
            float d = CLOSE_DISTANCE;
            if (isMain) d *= 2;
            InformationManager.DisplayMessage(new InformationMessage("new close rumor pos - " + pos));
            foreach (KeyValuePair<Settlement,RumorHolder> rh in settlements)
            {
                if(rh.Key.GetPosition().Distance(pos) < d)
                    rh.Value.addRumor(r);
            }

        }

        public void WorldEvent(Rumor r)
        {
            InformationManager.DisplayMessage(new InformationMessage("new rumor"));
            foreach (RumorHolder rh in settlements.Values)
            {
                rh.addRumor(r);
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
           
        }
    }
}
