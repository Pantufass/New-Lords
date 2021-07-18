using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.TwoDimension;

namespace NewNpc2
{
    public class Character
    {
        #region Consts

        protected const float FRIEND_STEP = 1;
        protected const float ENERGY_STEP = 0.01f;
        protected const float SPEND_ENERGY_STEP = 0.4f;
        protected const float START_ENERGY = 0.4f;
        protected const int INT_MAX_EXCHANGES = 5;
        protected const float CORDIAL_D = 0.08f;
        protected const float CRUDE_D = 0.92f;
        protected const float CHANCE_VALUE = 1;
        protected const float CHANCE_VALUE2 = 0.15f;
        protected const float POSITIVE_THRESH = 10;
        protected const float NEGATIVE_THRESH = 0;
        protected const float OWN_BELIEF_SURENESS = 1;
        protected const float START_BASE_THRESH = 0.55f;
        protected const float MAX_BASE_THRESH = 1.05f;
        protected const float MIN_BASE_THRESH = 0.45f;
        protected const float PREVIOUS_TRAITS_WEIGHT = 0.5f;
        protected const float BORED_TIMER = 10f;
        protected const float TARGET_DIST = 2.5f;
        protected const int TIME_RESPONSE = 3;


        #endregion

        public string Name
        {
            get
            {
                if (hero != null) return hero.Name.ToString();
                if (agent != null) return agent.Name.ToString();
                if (characterObject != null) return characterObject.Name.ToString();
                return "no name";
            }
        }

        //[SaveableField(1)]
        //class with the personality traits
        protected Traits personality;

        //[SaveableField(2)]
        //List of possible statuses
        protected List<Status> status;

        private float _energy;
        public float energy
        {
            get
            {
                return _energy;
            }
            set
            {
                if (value > 1) _energy = 1;
                else if (value < 0) _energy = 0;
                else _energy = value;
            }
        }
        public float threshold
        {
            get
            {
                float res = START_BASE_THRESH;

                res += (personality.annoying * START_BASE_THRESH * -0.4f);
                res += (personality.calculating * START_BASE_THRESH * 0.2f);

                if (res > MAX_BASE_THRESH) return MAX_BASE_THRESH;
                if (res < MIN_BASE_THRESH) return MIN_BASE_THRESH;
                return res;

            }
        }


        [SaveableField(3)]
        private CharacterObject _characterObject;
        public CharacterObject characterObject
        {
            get
            {
                if (_characterObject == null && agent != null) _characterObject = agent.Character as CharacterObject;
                if (_characterObject == null && hero != null) _characterObject = hero.CharacterObject;
                return _characterObject;
            }
            set
            {
                _characterObject = value;
            }
        }


        [SaveableField(4)]
        protected Agent _agent;
        public Agent agent
        {
            get { return _agent; }
            set
            {
                initialState();
                _agent = value;
            }
        }
        [SaveableField(5)]
        public Hero hero;

        //social network
        //3 unidirectional relations between this and other characters
        //[SaveableField(6)]
        protected List<Feeling> friendlyFeelings;
        //[SaveableField(7)]
        protected List<Feeling> romanticFeelings;
        //[SaveableField(8)]
        protected List<Feeling> admiration;

        //[SaveableField(9)]
        //belief network
        protected Dictionary<Character,List<Feeling>> beliefs;

        protected Dictionary<Character, List<Notion>> notions;

        protected intent currIntent;

        //list of social exchanges known
        //each exchange is paired with the respective believability 
        //[SaveableField(10)]
        protected Dictionary<SocialExchange, float> memory;

        [SaveableField(11)]
        private Culture _culture;
        public Culture Culture
        {
            get
            {
                if (_culture == null) setCulture();
                return _culture;
            }
        }

        public SocialInteraction npcIntended;


        public bool performing;
        public bool onResponse;
        public bool initResponse;
        public bool onStand;

        private List<SocialInteraction> last;

        protected Tuple<Rumor,float> rumor;

        public float timeSinceLast;

        public Settlement lastLocation;
        public bool hasRumor;

        public Character target;
        public bool hasTarget;

        public SocialExchange onExchange;

        private MissionTimer respTimer;
        private MissionTimer initTimer;
        private MissionTimer standTimer;


        public Rumor.Information.type preference
        {
            get
            {
                float cur = personality.curious;
                float calc = personality.calculating;
                float car = personality.careful;

                if (personality.curious > 0.7f) return Rumor.Information.type.Gossip;
                if (agent != null && agent.IsHero) car += 0.3f;
                if(cur > calc && cur > car) return Rumor.Information.type.Gossip;
                if(calc > car) return Rumor.Information.type.Economic;
                return Rumor.Information.type.Warfare;
            }
        }
        public Character(Hero h)
        {
            hero = h;
            if (h == Hero.MainHero)
            {
                personality = new Traits(true);

            }
            personality = new Traits(h.GetHeroTraits());
            characterObject = h.CharacterObject;
            setCulture(h.Culture);
            CreateChar();
        }

        public Character(CharacterObject co)
        {
            personality = new Traits();
            characterObject = co;
            setCulture(co.Culture);
            CreateChar();
        }

        public Character(Agent a)
        {
            if (a == Agent.Main)
            {
                personality = new Traits(true);
            }
            personality = new Traits();
            if (a != null) characterObject = a.Character as CharacterObject;
            CreateChar();
            agent = a;
        }


        public void setHero(Hero h)
        {
            hero = h;
        }

        public void setSet(Settlement s)
        {
            lastLocation = s;
        }

        private void CreateChar()
        {
            status = new List<Status>();

            friendlyFeelings = new List<Feeling>();
            romanticFeelings = new List<Feeling>();
            admiration = new List<Feeling>();

            beliefs = new Dictionary<Character, List<Feeling>>();
            notions = new Dictionary<Character, List<Notion>>();
            memory = new Dictionary<SocialExchange, float>();

            last = new List<SocialInteraction>();

            initialState();

            energy = InitialEnergy();
            performing = false;
            onResponse = false;

            timeSinceLast = 0;

            if(Mission.Current != null)
            {
                respTimer = new MissionTimer(TIME_RESPONSE);
                initTimer = new MissionTimer(TIME_RESPONSE * 2);
                standTimer = new MissionTimer(TIME_RESPONSE);
            }
        }

        public List<SocialInteraction> getLast()
        {
            return last;
        }

        public bool hasLast()
        {
            return last.Count > 0;
        }

        public void Tick(float dt)
        {
            if (timeSinceLast > BORED_TIMER && !isBored())
            {
                status.Add(Status.Bored);
                timeSinceLast = 0;
            }
            else timeSinceLast += dt;

            if (hasTarget && agent.Position.Distance(target.agent.Position) < TARGET_DIST)
            {
                reached();
            }
            if (onResponse && respTimer.Check(false))
            {
                respond();
            }
            if(initResponse && initTimer.Check(false))
            {
                initRespond();
            }
            if(onStand && standTimer.Check(false))
            {
                unstand();
            }

        }

        private void initRespond()
        {
            stand();
            initResponse = false;
            if (initTimer == null) initTimer = new MissionTimer(TIME_RESPONSE * 2);
            initTimer.Check(true);
            if(onExchange.resp != null)
            {
                Dialog d = npcIntended.getDialog(calcDialogType(),onExchange.resp.value, 2, this);
                if (d.value != -1) showDialog(d);
            }
        }

        private void respond()
        {

            respTimer.Check(true);
            onResponse = false;

            float result = onExchange.calculateResponse();

            Dialog d = onExchange.getResponse(result);

            stand();

            showDialog(d);

            SubModule.makeExchange(onExchange);
        }

        private void OnResponse()
        {
            if (respTimer == null) respTimer = new MissionTimer(TIME_RESPONSE);
            respTimer.Check(true);
            onResponse = true;
        }

        private void reached()
        {
            hasTarget = false;
            target.OnResponse();

            if(agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
            agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().RemoveBehavior<FollowAgentBehavior>();

            onExchange = new SocialExchange(this, target, npcIntended, getIntent());
            target.calcResponse(onExchange);

            stand();
            if (npcIntended == null) 
                return;
            if (npcIntended.hasPaths)
            {
                if (initTimer == null) initTimer = new MissionTimer(TIME_RESPONSE * 2);
                initTimer.Check(true);
                initResponse = true;
                showDialog(npcIntended.getDialog(calcDialogType(), 0, 0, this));
            }
            else showDialog(npcIntended.getDialog(calcDialogType(), 0));

        }

        public void exchange(Character tar,SocialInteraction inten)
        {
            hasTarget = true;
            target = tar;
            npcIntended = inten;

            performing = true;
            target.performing = true;

            moveTo();
        }

        public void moveTo()
        {
            if (hasTarget && agent.Position.Distance(target.agent.Position) > TARGET_DIST)
            {
                if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
                {
                    DailyBehaviorGroup bg = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
                    bg.AddBehavior<FollowAgentBehavior>().SetTargetAgent(target.agent);

                    bg.SetScriptedBehavior<FollowAgentBehavior>();

                    agent.SetLookAgent(target.agent);
                }
            }
        }

        private void stand()
        {
            onStand = true;
            if(standTimer == null) standTimer = new MissionTimer(TIME_RESPONSE);
            if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
            {
                DailyBehaviorGroup bg = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
                bg.AddBehavior<StandGuardBehavior>();

                bg.SetScriptedBehavior<StandGuardBehavior>();
            }
        }

        private void unstand()
        {
            if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
                agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().RemoveBehavior<StandGuardBehavior>();
            onStand = false;

        }

        public void showDialog(Dialog d)
        {
            foreach (MissionBehaviour mb in Mission.Current.MissionBehaviours)
            {
                if (mb is MissionViewBehaviour)
                    (mb as MissionViewBehaviour).dialog(d, this);
            }
        }

        public void calcResponse(SocialExchange se)
        {
            onExchange = se;
        }


        private void setExchange(SocialInteraction si)
        {
            if (si == null) return;
            if (last.Count < 5) last.Add(si);
            if (last.Count > 4) last[4] = last[3];
            if (last.Count > 3) last[3] = last[2];
            if (last.Count > 2) last[2] = last[1];
            if (last.Count > 1) last[1] = last[0];
            last[0] = si;
        }


        public void calcRumor()
        {
            Settlement s = Settlement.CurrentSettlement;
            if (s == null) throw new ArgumentException();

            RumorHolder rh = SubModule.rb.findSettlement(s);
            
            rumor = rh.getRumor(this);
            if (rumor != null) 
                hasRumor = true;
        }

        public Rumor getRumor()
        {
            if (rumor == null) return null;
            return rumor.Item1;
        }

        public float getRumorValue()
        {
            if (rumor == null) return 0;
            return rumor.Item2;
        }

        private float InitialEnergy()
        {

            return threshold * START_ENERGY + threshold * personality.annoying * START_ENERGY + SubModule.rand.NextFloat() * START_ENERGY;
        }


        internal Feeling getStrongestFeeling(Character c)
        {
            Feeling res = getFeeling(friendlyFeelings, c);
            Feeling tem = getFeeling(romanticFeelings, c);
            if (tem == null && res != null) return res;
            if (res == null && tem != null) return tem;
            if (tem == null && res == null) return null;
            if (tem.getIntensity() > res.getIntensity() / 2) res = tem;
            return res;
        }

        internal Feeling getLowestFeeling(Character c = null)
        {
            if (c == null)
            {
                Feeling res = new Feeling(c,1,intent.Neutral);
                foreach(Feeling f in friendlyFeelings)
                {
                    if (f.getIntensity() < res.getIntensity()) res = f;
                }
                foreach (Feeling f in romanticFeelings)
                {
                    if (f.getIntensity() < res.getIntensity()) res = f;
                }
                return res;
            }
            else
            {
                Feeling res = getFeeling(friendlyFeelings, c);
                Feeling tem = getFeeling(romanticFeelings, c);
                if (tem == null && res != null) return res;
                if (res == null && tem != null) return tem;
                if (tem == null && res == null) return null;
                if (tem.getIntensity() < res.getIntensity() / 2) res = tem;
                return res;
            }
        }

        internal List<Feeling> getBeliefs(Character character)
        {
            List<Feeling> l;
            beliefs.TryGetValue(character, out l);
            return l;
        }

        internal List<Notion> getNotions(Character character)
        {
            if(notions.TryGetValue(character, out List<Notion> notion))
                return notion;
            return new List<Notion>();
        }


        private void initialState()
        {
            status.Clear();
            var rand = SubModule.rand;
            var match = Enum.GetValues(typeof(Status)).Cast<Status>().ToArray();
            //one random status
            addStatus(match[rand.Next(match.Length)]);
            //two 50% chance
            if (rand.NextDouble() > 0.5)
                addStatus(match[rand.Next(match.Length)]);
            //three 5%
            if (rand.NextDouble() > 0.05)
                addStatus(match[rand.Next(match.Length)]);

        }

        private void addStatus(Status s)
        {
            if (!status.Contains(s)) status.Add(s);
        }


        public float calcNpcVolition(Character r)
        {
            currIntent = calcIntent(r);

            return calcNpcIntended(r, currIntent);
        }

        public intent calcVolitions(Character r)
        {
            currIntent = calcIntent(r);

            calcNpcIntended(r, currIntent);

            return currIntent;
        }

        private void setCulture()
        {
            setCulture(characterObject.Culture);
            
        }

        private void setCulture(BasicCultureObject c)
        {
            string s = c.Name.ToString();
            switch (s)
            {
                case "Empire":
                    _culture = CultureManager.createEmpire();
                    break;
                case "Khuzaits":
                    _culture = CultureManager.createKhuzaits();
                    break;
                case "Vlandia":
                    _culture = CultureManager.createVlandia();
                    break;
                case "Battania":
                    _culture = CultureManager.createBattania();
                    break;
                case "Aserai":
                    _culture = CultureManager.createAserai();
                    break;
                case "Sturgians":
                    _culture = CultureManager.createSturgians();
                    break;
                default:
                    _culture = CultureManager.createEmpire();
                    break;
            }
        }

        protected void addCulture(List<SocialInteraction> interactions)
        {
            foreach(SocialInteraction i in interactions)
            {
                foreach(InteractionData id in Culture.getIntData())
                {
                    if(id.compare(i.name))
                    {
                        interactions.Remove(i);
                        SocialInteraction si = new SocialInteraction(i);
                        si.setCultureSet(id);
                        interactions.Add(si);
                        break;
                    }
                }
            }
        }

        private float calcNpcIntended(Character r, intent intent)
        {
            Random rand = SubModule.rand;

            KeyValuePair<float, SocialInteraction> d = new KeyValuePair<float, SocialInteraction>(0,new SocialInteraction("test",1,1));

            List<SocialInteraction> interactions = new List<SocialInteraction>();
            interactions.AddRange(Culture.CulturalExchanges());
            interactions.AddRange(SubModule.existingExchanges.Values);
            addCulture(interactions);

            foreach (SocialInteraction si in interactions)
            {
                if (si.validate(this, r))
                {
                    float res = (float)(si.calculateVolition(this, r, intent) + (2 * CHANCE_VALUE * rand.NextDouble() - CHANCE_VALUE));
                    if (res > d.Key) d = new KeyValuePair<float, SocialInteraction>(res, si);
                }
            }
            npcIntended = d.Value;
            if (d.Value.Equals("RelayInformation"))
            {
                _ = d.Key;
            }

            return d.Key;
        }

        public SocialInteraction getNpcIntended()
        {
            return npcIntended;
        }


        internal sentenceType calcDialogType()
        {
            if(hero == Hero.MainHero || agent == Agent.Main) return sentenceType.Normal;
            Random r = SubModule.rand;
            float v = Mathf.Lerp(0,1,(personality.calculating + personality.careful + personality.honor + personality.shy) * ((float)r.NextDouble() * (2*CHANCE_VALUE2 - CHANCE_VALUE2)));
            if (v < CORDIAL_D) return sentenceType.Cordial;
            else if (v > CRUDE_D) return sentenceType.Crude;
            else return sentenceType.Normal;
        }
        
        private intent calcIntent(Character r)
        {
            intent i = intent.Neutral;

            Feeling friendly = getFeeling(friendlyFeelings, r);
            Feeling romantic = getFeeling(romanticFeelings, r);
            Feeling admire = getFeeling(admiration, r);

            if(friendly != null) 
                i = friendly.getIntent();

            if (romantic != null && romantic.getIntensity() > (friendly != null ? friendly.getIntensity(): 0))
                i = romantic.getIntent();

            if (admire != null)
                if (admire.getIntensity() > (friendly != null ? friendly.getIntensity() : 0) &&
                    admire.getIntensity() > (romantic != null ? romantic.getIntensity() : 0))
                    i = admire.getIntent();
                    
            return i;
        }

        public void raiseRomantic(Character c, float v = FRIEND_STEP)
        {
            float value = v + ((v / 5) * personality.stubborn);
            Feeling f = null;
            for(int i = 0; i < romanticFeelings.Count; i++)
            {
                if (romanticFeelings[i].getInitiator() == c)
                {
                    romanticFeelings[i].addIntensity(value);
                    f = romanticFeelings[i];
                }
            }
            if (f == null) romanticFeelings.Add(new Feeling(c,v,intent.Romantic));
        }

        public void lowerRomantic(Character c, float v = FRIEND_STEP)
        {
            raiseRomantic(c, -v);

        }

        internal void overpowered(Character c, float v = FRIEND_STEP)
        {
            Feeling f = null;
            for (int i = 0; i < admiration.Count; i++)
            {
                if (admiration[i].getInitiator() == c)
                {
                    admiration[i].addIntensity(v);
                    f = admiration[i];
                    if (getFeeling(friendlyFeelings, c).getIntent() == intent.Positive) admiration[i].setIntent(intent.Embellish);
                }
            }
            if (f == null) admiration.Add(new Feeling(c, v, intent.Neutral));

        }

        internal void raiseFriendly(Character c, float v = FRIEND_STEP)
        {
            Random r = SubModule.rand;
            float value = v - ((v / 4) * personality.stubborn *(float) r.NextDouble());
            Feeling f = null;
            for (int i = 0; i < friendlyFeelings.Count; i++)
            {
                if (friendlyFeelings[i].getInitiator() == c)
                {
                    f = friendlyFeelings[i];
                    friendlyFeelings[i].addIntensity(value);
                    if(friendlyFeelings[i].getIntensity() > POSITIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Positive);
                    else if(friendlyFeelings[i].getIntensity() > NEGATIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Negative)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < POSITIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Positive)
                        friendlyFeelings[i].setIntent(intent.Neutral);
                    else if (friendlyFeelings[i].getIntensity() < NEGATIVE_THRESH && friendlyFeelings[i].getIntent() == intent.Neutral)
                        friendlyFeelings[i].setIntent(intent.Negative);
                }
            }
            if (f == null) friendlyFeelings.Add(new Feeling(c, v, intent.Positive));
        }

        internal void lowerFriendly(Character c, float v = FRIEND_STEP)
        {
            raiseFriendly(c, -v);
        }
         
        private Feeling getFeeling(List<Feeling> list, Character c)
        {
            Feeling f = null;
            foreach(Feeling f2 in list)
            {
                if (f2.getInitiator() == c) f = f2;
            }
            return f;
        }


        internal Character getUnliked()
        {
            return getLowestFeeling().getInitiator();
        }
        internal bool isHero()
        {
            return (hero != null);
        }
        //TODO
        internal bool isFalse(SocialExchange se)
        {
            if (memory.TryGetValue(se, out float v)) return true;
            return false;
        }

        public intent getIntent()
        {
            return currIntent;
        }

        public void addExchange(SocialExchange se)
        {
            memory.Add(se, OWN_BELIEF_SURENESS);
        }

        public List<InfluenceRule> getRules()
        {
            return NPCDialogBehaviour.characterManager.generalRules();
        }

        //TODO
        public void hearRumor(Rumor r)
        {
            //determine lie
            //process beliefs
            //process feelings
            
        }

        internal float calcCharacterInterest(Character c)
        {
            float res = 0;
            Feeling temp = getFeeling(friendlyFeelings, c);
            if(temp != null) res += temp.getIntensity() * 0.8f;
            temp = getFeeling(romanticFeelings, c);
            if(temp != null) res += temp.getIntensity() * 2f;
            temp = getFeeling(admiration, c);
            if(temp != null) res += temp.getIntensity() * 0.5f;
            return res;
        }

        internal virtual void FinishedExchange(SocialInteraction si = null)
        {
            setExchange(si);
            performing = false;
        }

        internal float getEnergy()
        {
            return energy;
        }

        internal void raiseEnergy(float e = ENERGY_STEP)
        {
            if (performing) return;
            energy += e - e * (personality.stubborn * 0.05f) -
                e * (personality.calculating * 0.2f) +
                e * (personality.curious * 0.3f) -
                e * (personality.shy * 0.3f) +
                e * (SubModule.rand.NextFloat() * 0.4f - 0.2f);
            
            if(energy >= threshold) 
                SubModule.npc.NPCReady(this);
            //InformationManager.DisplayMessage(new InformationMessage(characterObject.Name + " - " +  energy));
        }

        internal void spendEnergy(float e = SPEND_ENERGY_STEP)
        {
            energy -= e + e * (personality.annoying * 0.2f);
        }

        internal void setInChoice(SocialInteraction si)
        {
            //TODO
        }

        internal void setInIntent(intent i)
        {
            switch (i)
            {
                case intent.Positive:
                    personality.kind += 0.05f * SubModule.rand.NextFloat();
                    if (SubModule.rand.NextDouble() < 0.1f) addStatus(Status.Happy);
                    break;
                case intent.Neutral:
                    personality.curious += 0.05f * SubModule.rand.NextFloat();
                    break;
                case intent.Negative:
                    personality.kind -= 0.05f * SubModule.rand.NextFloat();
                    if (SubModule.rand.NextDouble() < 0.05f) addStatus(Status.Angry);
                    if (SubModule.rand.NextDouble() < 0.05f) addStatus(Status.Sad);
                    break;
                case intent.Romantic:
                    personality.charm -= 0.05f * SubModule.rand.NextFloat();
                    break;
                default:
                    break;
            }
        }

        #region Status
        public bool isHappy()
        {
            return status.Contains(Status.Happy);
        }

        public void removeStatus(Status s)
        {
            status.Remove(s);
        }

        public bool isGloated()
        {
            return status.Contains(Status.Gloated);
        }
        public bool isBored()
        {
            return status.Contains(Status.Bored);
        }
        public void notBored()
        {
            status.Remove(Status.Bored);
        }

        public bool isFeared()
        {
            return status.Contains(Status.Feared);
        }

        public bool isGood()
        {
            int b = 0;
            foreach(Status s in status)
            {
                if (s == Status.Normal) continue;
                b += (int) s / Math.Abs((int)s);
            }
            return b >= 0;
        }

        public Status getRandStatus()
        {
            if (status.Count > 0)
                return status[SubModule.rand.Next(status.Count - 1)];
            else return Status.Normal;
        }

        public bool hasStatus()
        {
            return status.Count > 0;
        }

        internal float getCalc()
        {
            return personality.calculating;
        }
        
        internal float getAnoy()
        {
            return personality.annoying;
        }
        internal float getCurious()
        {
            return personality.curious;
        }
        internal float getSensitive()
        {
            return personality.sensitive;
        }
        internal float getShy()
        {
            return personality.shy;
        }
        internal float getKind()
        {
            return personality.kind;
        }

        internal bool isGay()
        {
            return personality.isGay;
        }

        internal float getCharm()
        {
            return personality.charm;
        }

        internal float getHonor()
        {
            return personality.honor;
        }

        internal float getHelp()
        {
            return personality.helpful;
        }

        internal float getCare()
        {
            return personality.careful;
        }

        #endregion

        public class Traits
        {

            [SaveableField(1)]
            public float kind; //likeness

            [SaveableField(2)]
            public float stubborn; //chances in the beliefs

            [SaveableField(3)]
            public float liar; //inventing rumors

            [SaveableField(4)]
            public float curious; //wants to know/asks more stuff

            [SaveableField(5)]
            public float helpful; //helps out

            [SaveableField(6)]
            public float shy; //more or ness energy for exchanges 

            [SaveableField(7)]
            public float careful; //i dont know yet

            [SaveableField(8)]
            public float sensitive; //changes in the beliefs

            [SaveableField(9)]
            public float honor; //likeness

            [SaveableField(10)]
            public float charm; //love at first sight

            [SaveableField(11)]
            public float annoying; //energy for exchanges

            [SaveableField(12)]
            public float calculating; //i dont know yet

            [SaveableField(13)]
            public bool isGay; 

            internal Traits()
            {

                var r = SubModule.rand;
                kind = (float) r.NextDouble() * 2 - 1;
                helpful = (float)r.NextDouble() * 2 - 1;
                sensitive = (float)r.NextDouble() * 2 - 1;
                honor = (float)r.NextDouble() * 2 - 1;
                calculating = (float)r.NextDouble() * 2 - 1;

                normalTraits(r);
            }

            public Traits(bool mc = true)
            {

            }
            public Traits(CharacterTraits charaterTraits)
            {

                var r = SubModule.rand;

                kind = (float)(r.NextDouble()) + (charaterTraits.Generosity % 1 / PREVIOUS_TRAITS_WEIGHT); 
                helpful = (float)r.NextDouble() + (charaterTraits.Mercy % 1 / PREVIOUS_TRAITS_WEIGHT);
                sensitive = (float)r.NextDouble() - (charaterTraits.Valor % 1 / PREVIOUS_TRAITS_WEIGHT);
                honor = (float)r.NextDouble() + (charaterTraits.Honor % 1 / PREVIOUS_TRAITS_WEIGHT);
                calculating = (float)r.NextDouble() + (charaterTraits.Calculating % 1 / PREVIOUS_TRAITS_WEIGHT);


                if(kind == 0)  kind = (float)r.NextDouble() * 2 - 1; ;
                if(helpful == 0) helpful = (float)r.NextDouble() * 2 - 1; ;
                if(sensitive == 0) sensitive = (float)r.NextDouble() * 2 - 1; ;
                if(honor == 0) honor = (float)r.NextDouble() * 2 - 1; ;
                if(calculating == 0) calculating = (float)r.NextDouble() * 2 - 1; ;

                normalTraits(r);
            }

            private void normalTraits(Random rand)
            {
                stubborn = (float)rand.NextDouble() * 2 - 1;
                liar = (float)rand.NextDouble() * 2 - 1;
                curious = (float)rand.NextDouble() * 2 - 1;
                shy = (float)rand.NextDouble() * 2 - 1;
                careful = (float)rand.NextDouble() * 2 - 1;
                charm = (float)rand.NextDouble() * 2 - 1;

                annoying = (float)rand.NextDouble() * 2 - 1;

                isGay = rand.NextDouble() < 0.1;
            }


           
        }

        
        
        public enum Status
        {
            Wounded=-10,
            Bored=-9,
            Tired=-8,

            Angry=-6,
            Feared=-5,
            Sad = -4,
            Confident =2,
            Happy=3,

            Ashamed=-3,

            Gloated =4,

            Normal = 0
        }
    }

    public class MainCharacter : Character
    {

        public List<Dialog> dialogs;
        protected List<SocialInteraction> intendedSocialExchange;
        //protected List<SocialInteraction> allInteractions;

        public MainCharacter(Hero h) : base(h)
        {
            dialogs = new List<Dialog>();
            intendedSocialExchange = new List<SocialInteraction>();
        }

        public MainCharacter(Agent a) : base(a)
        {
            dialogs = new List<Dialog>();
            intendedSocialExchange = new List<SocialInteraction>();
        }

        internal override void FinishedExchange(SocialInteraction si = null)
        {
            base.FinishedExchange(si);
            dialogs.Clear();
        }

        private void calcIntended(Character r, intent intent)
        {
            Random rand = SubModule.rand;

            Dictionary<float, SocialInteraction> d = new Dictionary<float, SocialInteraction>();

            List<SocialInteraction> interactions = new List<SocialInteraction>();
            interactions.AddRange(Culture.CulturalExchanges());
            interactions.AddRange(SubModule.existingExchanges.Values);
            addCulture(interactions);

            foreach (SocialInteraction si in interactions)
            {
                if (si.validate(this, r))
                {
                    float res = (float)(si.calculateVolition(this, r, intent) + (2 * CHANCE_VALUE * rand.NextDouble() - CHANCE_VALUE));
                    //TDODO fix
                    while (d.ContainsKey(res))
                        res += (float)rand.NextDouble() * (CHANCE_VALUE * 0.05f);
                    d.Add(res, si);
                }
            }
            List<float> l = d.Keys.ToList();
            l.Sort();
            l.Reverse();

            for (int i = 0; i < INT_MAX_EXCHANGES; i++)
            {
                intendedSocialExchange.Add(d[l[i]]);
            }
        }

        public void clearIntented()
        {
            dialogs.Clear();
            intendedSocialExchange.Clear();
        }

        public void setIntendedRule(SocialInteraction si)
        {
            intendedSocialExchange.Add(si);
        }

        public List<SocialInteraction> getIntented()
        {
            return intendedSocialExchange;
        }

        public List<SocialInteraction> getIntented(Character c, intent i)
        {
            calcIntended(c, i);
            currIntent = i;
            return intendedSocialExchange;
        }


        public void chooseDialog(intent i, Character c)
        {
            Random rand = SubModule.rand;

            Dictionary<float, SocialInteraction> d = new Dictionary<float, SocialInteraction>();

            List<SocialInteraction> interactions = new List<SocialInteraction>();
            interactions.AddRange(SocialInteractionManager.allInteractions());
            addCulture(interactions);

            foreach (SocialInteraction si in interactions)
            {
                if (si.validate(this, c))
                {
                    float res = (float)(si.calculateVolition(this, c, i) + (2 * CHANCE_VALUE * rand.NextDouble() - CHANCE_VALUE));
                    while (d.ContainsKey(res))
                        res += (float)rand.NextDouble() * (CHANCE_VALUE * 0.05f);
                    d.Add(res, si);
                }
            }
            List<float> l = d.Keys.ToList();
            l.Sort();
            l.Reverse();

            for (int j = 0; j < INT_MAX_EXCHANGES; j++)
            {
                dialogs.Add(d[l[j]].chooseDialog(sentenceType.Normal, this));
                dialogs[j].playera = true;
            }
            foreach (Dialog x in dialogs)
            {
                if (x.playera)
                {
                    if (x.cresponse) { }
                }
            }
            foreach (SocialInteraction si in SocialInteractionManager.allInteractions())
            {
                foreach (Tuple<Dialog, Dialog> dia in si.sentences)
                {
                    if (dia.Item1.playera)
                    {
                        if (dia.Item1.cresponse) { }
                    }
                }
            }
        }


    }


}
