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
                if(!settlements.ContainsKey(s))
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
            Character c = NPCDialogBehaviour.characterManager.findChar(h);
            c.setSet(s);
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

            //initSet(curr);
            curr.setRumors(party.getRumors());
        }

        private void initSet(RumorHolder rh)
        {
            List<string> l = new List<string>();
            l.Add("asdrubal");
            l.Add("letirio");

            rh.addRumor(new Rumor(new Rumor.Information(l,true)));

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
            SocialExchange se = new SocialExchange(NPCDialogBehaviour.characterManager.findChar(declarer.Leader), NPCDialogBehaviour.characterManager.findChar(declared.Leader), SocialInteractionManager.War(), intent.Negative);

            List<string> l = new List<string>();
            l.Add(declarer.Name.ToString());
            l.Add(declared.Name.ToString());
            Rumor r = new Rumor(new Rumor.Information(l,true), se, 0.7f);

            WorldEvent(r);
        }


        private void OnMapEvent(MapEvent me)
        {
            if (!me.HasWinner || me.Winner.LeaderParty.LeaderHero == null || me.GetNumberOfInvolvedMen() < 30) return;
            Character winner = NPCDialogBehaviour.characterManager.findChar(me.Winner.LeaderParty.LeaderHero);
            Character loser = NPCDialogBehaviour.characterManager.findChar(me.GetLeaderParty(me.DefeatedSide).LeaderHero);


            SocialExchange se = new SocialExchange(winner, loser, SocialInteractionManager.Battle(), intent.Neutral);
            Rumor r = new Rumor(new Rumor.Information(Rumor.Information.type.Warfare), se);

            CloseWorldEvent(r, me.Winner.LeaderParty.LeaderHero.GetPosition(), winner.hero == Hero.MainHero);
        }

        private void OnDefeat(Hero winner, Hero loser)
        {
            SocialExchange se = new SocialExchange(NPCDialogBehaviour.characterManager.findChar(winner.CharacterObject), NPCDialogBehaviour.characterManager.findChar(loser.CharacterObject), SocialInteractionManager.Battle(), intent.Neutral);
            Rumor r = new Rumor(new Rumor.Information(Rumor.Information.type.Warfare), se);

            if (winner == Hero.MainHero) CloseWorldEvent(r, winner.GetPosition(), true);
            else CloseWorldEvent(r, winner.GetPosition());
        }

        private void DailyTick()
        {
            Settlement s = Settlement.All[SubModule.rand.Next(Settlement.All.Count)];
            CreateRandomGossip(s);
        }

        private bool CreateRandomGossip(Settlement s)
        {
            
            List<Character> l = new List<Character>();
            l.AddRange(NPCDialogBehaviour.characterManager.getCharacters(s.HeroesWithoutParty));
            foreach (MobileParty p in s.Parties)
            {
                if (p.LeaderHero != null) l.Add(NPCDialogBehaviour.characterManager.findChar(p.LeaderHero));
                else if (p.Leader != null) l.Add(NPCDialogBehaviour.characterManager.findChar(p.Leader));
            }
            Character c = null;
            if (l.Count == 0) return false;
            if (SubModule.rand.Next(5) < 1) c = l[SubModule.rand.Next(l.Count)];
            List<SocialInteraction> si = SocialInteractionManager.rumorInteractions();
            SocialExchange se = new SocialExchange(l[SubModule.rand.Next(l.Count)],c,si[SubModule.rand.Next(si.Count)],intent.Neutral);

            CreateGossip(se,s);
            return true;
        }

        public void CreateGossip(SocialExchange se, Settlement s = null)
        {
            if(s != null)
            {
                if (!settlements.TryGetValue(s, out RumorHolder rh))
                {
                    rh = new RumorHolder();
                    settlements.Add(s, rh);
                }
                List<Rumor> l = new List<Rumor>();
                l.Add(new Rumor(se));
                rh.addRumor(new Rumor(se));
                return;
            }
            Character c = se.getInitiator();
            if (c.isHero())
            {
                if (!settlements.TryGetValue(c.hero.CurrentSettlement, out RumorHolder rh))
                {
                    rh = new RumorHolder();
                    settlements.Add(s, rh);
                }
                rh.addRumor(new Rumor(se));
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
           dataStore.SyncData("Parties", ref parties);
           dataStore.SyncData("RumorHolders", ref settlements);
        }
    }
}
