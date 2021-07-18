using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class InstRule : Rule
    {
        protected List<Condition> conditions;
        protected Action<List<dynamic>> del;

        public InstRule(string d, Action<List<dynamic>> f = null) : base(d)
        {
            conditions = new List<Condition>();
            del = f;
        }


        public void runEffects(List<dynamic> d = null)
        {
            if (validate(d)) del(d);
        }



        public void addCondition(Condition c)
        {
            conditions.Add(c);
        }

        public override bool validate(List<dynamic> d = null)
        {
            bool b = true;
            foreach (Condition c in conditions)
            {
                b = b && c.validate(d);
            }
            return b;
        }



    }

    public class Path : InstRule
    {
        protected int depth;
        protected string sent;
        protected SocialInteraction si;

        public Path(SocialInteraction si, string s = null, int d = 0) : base("path")
        {
            depth = d;
            conditions = new List<Condition>();
            sent = s;
            this.si = si;
        }

        public int getDepth()
        {
            return depth;
        }

        public Dialog sentence(Character c, int ch = 0)
        {
                if (si.name.Equals("ConveyStatus"))
                {
                    if (c != null) return sentence(c.getRandStatus());
                }
                if (si.name.Equals("RelayInformation"))
                {
                    if (c != null) return sentence(c.getRumor(),ch,c.Culture);
                }
                if (si.name.Equals("BadMouth"))
                {
                    if (c != null) return sentenceUnliked(c.getUnliked());
                }
            
            if (sent != null) return new Dialog(sent, 0, sentenceType.Normal);
            else return null;
        }

        //TODO error 
        private Dialog sentenceUnliked(Character c)
        {
            StringBuilder sb = new StringBuilder(" ", 30);
            if(c == null) return new Dialog("There are so many people I dont like... I dont even know where to start", 0, sentenceType.Normal);
            switch (SubModule.rand.Next(5))
            {
                case 0:
                    sb.Append("I really dont like ");
                    sb.Append(c.Name);
                    break;
                case 1:
                    sb.Append(c.Name);
                    sb.Append(" irritates me so much");
                    break;
                case 2:
                    sb.Append(c.Name);
                    sb.Append(" is annoying...");
                    break;
                case 3:
                    sb.Append("Ugh, ");
                    sb.Append(c.Name);
                    sb.Append("... Even the name is troublesome");
                    break;
                default:
                    sb.Append("I really dont like ");
                    sb.Append(c.Name);
                    break;
            }
            return new Dialog(sb.ToString(), 0, sentenceType.Normal);
        }

        private Dialog sentence(Character.Status s)
        {
            StringBuilder sb = new StringBuilder(" ", 30);
            sb.Append("I am ");
            if (s != Character.Status.Normal) sb.Append(s.ToString()); 
            else sb.Append("okay");
            return new Dialog(sb.ToString(), 0, sentenceType.Normal);
        }

        private Dialog sentence(Rumor r, int ch, Culture c = null)
        {
            float v = -1;
            if (r == null && depth == 0) 
                return new Dialog("I dont know anything special",0,sentenceType.Normal);
            if (r == null)
                return null;
            StringBuilder sb = new StringBuilder(" ", 50);
            if(depth == 0)
            switch (SubModule.rand.Next(7))
            {
                case 0:
                    sb.Append("Have you heard, ");
                    break;
                case 1:
                    sb.Append("Have you heard, ");
                    break;
                case 2:
                    sb.Append("Did you know, ");
                    break;
                case 3:
                    sb.Append("I heard... ");
                    break;
                default:
                    break;
            }
            if (r.info.getType() == Rumor.Information.type.Economic)
            {
                sb.Append("In ");
                sb.Append(r.info.values[0]);
                sb.Append(", ");
                sb.Append(r.info.values[1]);
                sb.Append("is at ");
                sb.Append(r.info.values[2]);

            }
            else
            {
                if (r.info.getType() == Rumor.Information.type.Warfare)
                {
                    if (r.info.war)
                    {
                        if (depth == 0)
                            switch (SubModule.rand.Next(2))
                            {
                                case 0:
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" has declared war against ");
                                    sb.Append(r.info.values[1]);
                                    break;
                                case 1:
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" and ");
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" have waged war against each other");
                                    break;
                                default:
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" has declared war against ");
                                    sb.Append(r.info.values[1]);
                                    break;
                            }
                        else if (depth == 1)
                            switch (SubModule.rand.Next(13))
                            {
                                case 0:
                                    sb.Append("I believe ");
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" has the upper hand");
                                    v = 1;
                                    break;
                                case 1:
                                    sb.Append("I believe ");
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" has the upper hand");
                                    v = 2;
                                    break;
                                case 2:
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" will win for sure");
                                    v = 1;
                                    break;
                                case 3:
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" will win for sure");

                                    v = 2;
                                    break;
                                case 4:
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" is doomed...");
                                    v = 3;
                                    break;
                                case 5:
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" is doomed...");
                                    v = 4;
                                    break;
                                case 6:
                                    sb.Append("War is terrible");
                                    break;
                                case 7:
                                    sb.Append("Who is going to count the bodies...");
                                    break;
                                case 8:
                                    sb.Append("And as usual the people suffer...");
                                    break;
                                case 9:
                                    sb.Append(r.exchange().getInitiator());
                                    sb.Append(" is a terrible leader... No wonder war is being declared");
                                    v = 5;
                                    break;
                                case 10:
                                    sb.Append(r.exchange().getInitiator());
                                    sb.Append(" knows what is right");
                                    v = 6;
                                    break;
                                case 11:
                                    sb.Append(r.exchange().getReceiver());
                                    sb.Append(" is a terrible leader... No wonder war is being declared");
                                    v = 6;
                                    break;
                                case 12:
                                    sb.Append(r.exchange().getReceiver());
                                    sb.Append(" knows what is right");
                                    v = 5;
                                    break;
                                default:
                                    sb.Append("That is so unfortunate");
                                    break;
                            }
                        else if (depth == 2)
                        {
                            switch (ch)
                            {
                                case 1:
                                    sb.Append("Yeah I agree, ");
                                    sb.Append(r.exchange().getInitiator().Name);
                                    sb.Append(" is a good leader");
                                    break;
                                case 2:
                                    sb.Append("Yeah I agree, ");
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" has the upper hand");
                                    break;
                                case 5:
                                    sb.Append("Yeah, ");
                                    sb.Append(r.info.values[1]);
                                    sb.Append(" will win the war");
                                    break;
                                case 6:
                                    sb.Append("Yeah, ");
                                    sb.Append(r.info.values[0]);
                                    sb.Append(" will win the war");
                                    break;
                                default:
                                    if (SubModule.rand.Next(3) > 1)
                                    {
                                        sb.Append("Yeah I agree");
                                    }
                                    break;
                            }
                            v = 1;
                        }
                    }
                    else
                    {
                        if(depth == 0)
                        {
                            sb.Append(r.exchange().getInitiator());
                            sb.Append(" has won a battle agaisnt ");
                            sb.Append(r.exchange().getReceiver());
                        }
                        else if(depth == 1)
                            switch (SubModule.rand.Next(5))
                            {
                                case 0:
                                    sb.Append("Oh my god is ");
                                    sb.Append(r.exchange().getReceiver().Name);
                                    sb.Append(" alright??");
                                    v = 1;
                                    break;
                                case 1:
                                    sb.Append("That must have been a surprising victory");
                                    v = 2;
                                    break;
                                case 2:
                                    sb.Append("Who would have thought uh");
                                    break;
                                case 3:
                                    sb.Append("I always believed in ");
                                    sb.Append(r.exchange().getInitiator().Name);
                                    v = 3;
                                    break;
                                default:
                                    sb.Append("That is impressoive");
                                    break;
                            }
                        else if(depth == 2)
                        {
                            switch (ch)
                            {
                                case 1:
                                    sb.Append("I have no ideia, I just heard about it");
                                    break;
                                case 3:
                                    sb.Append("Me too");
                                    break;
                                default:
                                    break;

                        }
                           //v = 1;
                        }

                        
                    }
                }
                else if (r.info.getType() == Rumor.Information.type.Gossip)
                {
                    Character c1 = r.exchange().getInitiator();
                    Character c2 = r.exchange().getReceiver();
                    if(depth == 0)
                    switch (r.exchange().type.name)
                    {
                        
                        case "Flirt":
                            sb.Append(c1.Name);
                            if (c2 != null || !c2.isHero())
                            {
                                sb.Append(" and ");
                                sb.Append(c2.Name);
                                sb.Append(", they flirting");
                            }
                            else
                            {
                                sb.Append(" has been flirting a lot");
                            }
                            break;
                        case "Date":
                            sb.Append(c1.Name);
                            sb.Append(" and ");
                            sb.Append(c2.Name);
                            sb.Append(" started dating");
                            break;
                        case "Insult":
                            sb.Append(c1.Name);
                            if (c2 != null || !c2.isHero())
                            {
                                sb.Append(" is insulting ");
                                sb.Append(c2.Name);
                                sb.Append(" so often... It's terrible");
                            }
                            sb.Append(" is so insulting");
                            break;
                        case "Complain":

                            switch (SubModule.rand.Next(4))
                            {
                                case 0:
                                    sb.Append("Look out, ");
                                    sb.Append(c1.Name);
                                    sb.Append(" likes to complain a little too much");
                                    break;
                                case 1:
                                    sb.Append(c1.Name);
                                    sb.Append(" is too annoying, i cannot be near ");
                                    if (c1.characterObject.IsFemale)
                                        sb.Append(" her");
                                    else sb.Append(" him");
                                    break;
                                default:
                                    sb.Append(c1.Name);
                                    sb.Append(" is so annoying, always complaining");
                                    break;
                            }
                            break;
                        case "Compliment":
                            switch (SubModule.rand.Next(2))
                            {
                                case 0:
                                    sb.Append(c1.Name);
                                    sb.Append(" is so complimenting");
                                    break;
                                default:
                                    sb.Append(c1.Name);
                                    sb.Append(" is great! So complimenting");
                                    break;
                            }
                            break;
                        default :
                            break;
                    }
                }
            }


            return new Dialog(sb.ToString(),v,sentenceType.Normal);
        }

    }
}

    
