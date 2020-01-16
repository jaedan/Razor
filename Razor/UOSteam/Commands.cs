using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assistant;

namespace UOSteam
{
    public static class Commands
    {
        private static List<ASTNode> ParseArguments(ref ASTNode node)
        {
            List<ASTNode> args = new List<ASTNode>();
            while (node != null)
            {
                args.Add(node);
                node = node.Next();
            }
            return args;
        }
        private static int GetSerial(ref ASTNode target)
        {
            int targetSerial = -1;
            if (target.Type == ASTNodeType.STRING)
                targetSerial = Interpreter.GetAlias(ref target);
            else if (target.Type == ASTNodeType.SERIAL)
                targetSerial = Convert.ToInt32(target.Lexeme, 16);

            return targetSerial;
        }
        private static bool DummyCommand(ref ASTNode node, bool quiet, bool force)
        {
            Console.WriteLine("Executing command {0} {1}", node.Type, node.Lexeme);

            node = null;

            return true;
        }
        private static bool UseItem(Item cont, ushort find)
        {
            for (int i = 0; i < cont.Contains.Count; i++)
            {
                Item item = cont.Contains[i];

                if (item.ItemID == find)
                {
                    PlayerData.DoubleClick(item);
                    return true;
                }
                else if (item.Contains != null && item.Contains.Count > 0)
                {
                    if (UseItem(item, find))
                        return true;
                }
            }

            return false;
        }
        static Commands()
        {
            // Commands. From UOSteam Documentation
            Interpreter.RegisterCommandHandler("fly", Fly);
            Interpreter.RegisterCommandHandler("land", Land);
            Interpreter.RegisterCommandHandler("setability", SetAbility);
            Interpreter.RegisterCommandHandler("attack", Attack);
            Interpreter.RegisterCommandHandler("clearhands", ClearHands);
            Interpreter.RegisterCommandHandler("clickobject", ClickObject);
            Interpreter.RegisterCommandHandler("bandageself", BandageSelf);
            Interpreter.RegisterCommandHandler("usetype", UseType);
            Interpreter.RegisterCommandHandler("useobject", UseObject);
            Interpreter.RegisterCommandHandler("useonce", UseOnce);
            Interpreter.RegisterCommandHandler("cleanusequeue", CleanUseQueue);
            Interpreter.RegisterCommandHandler("moveitem", MoveItem);
            Interpreter.RegisterCommandHandler("moveitemoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("movetype", DummyCommand);
            Interpreter.RegisterCommandHandler("movetypeoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("walk", Walk);
            Interpreter.RegisterCommandHandler("turn", Turn);
            Interpreter.RegisterCommandHandler("run", Run);
            Interpreter.RegisterCommandHandler("useskill", UseSkill);
            Interpreter.RegisterCommandHandler("feed", Feed);
            Interpreter.RegisterCommandHandler("rename", Rename);
            Interpreter.RegisterCommandHandler("shownames", ShowNames);
            Interpreter.RegisterCommandHandler("togglehands", DummyCommand);
            Interpreter.RegisterCommandHandler("equipitem", DummyCommand);
            Interpreter.RegisterCommandHandler("togglemounted", DummyCommand);
            Interpreter.RegisterCommandHandler("equipwand", DummyCommand);
            Interpreter.RegisterCommandHandler("buy", DummyCommand);
            Interpreter.RegisterCommandHandler("sell", DummyCommand);
            Interpreter.RegisterCommandHandler("clearbuy", DummyCommand);
            Interpreter.RegisterCommandHandler("clearsell", DummyCommand);
            Interpreter.RegisterCommandHandler("organizer", DummyCommand);
            Interpreter.RegisterCommandHandler("autoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("dress", DummyCommand);
            Interpreter.RegisterCommandHandler("undress", DummyCommand);
            Interpreter.RegisterCommandHandler("dressconfig", DummyCommand);
            Interpreter.RegisterCommandHandler("toggleautoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("togglescavenger", DummyCommand);
            Interpreter.RegisterCommandHandler("counter", DummyCommand);
            Interpreter.RegisterCommandHandler("unsetalias", UnsetAlias);
            Interpreter.RegisterCommandHandler("setalias", SetAlias);
            Interpreter.RegisterCommandHandler("promptalias", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforgump", DummyCommand);
            Interpreter.RegisterCommandHandler("replygump", DummyCommand);
            Interpreter.RegisterCommandHandler("closegump", DummyCommand);
            Interpreter.RegisterCommandHandler("clearjournal", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforjournal", DummyCommand);
            Interpreter.RegisterCommandHandler("poplist", DummyCommand);
            Interpreter.RegisterCommandHandler("pushlist", DummyCommand);
            Interpreter.RegisterCommandHandler("removelist", DummyCommand);
            Interpreter.RegisterCommandHandler("createlist", DummyCommand);
            Interpreter.RegisterCommandHandler("clearlist", DummyCommand);
            Interpreter.RegisterCommandHandler("info", DummyCommand);
            Interpreter.RegisterCommandHandler("pause", DummyCommand);
            Interpreter.RegisterCommandHandler("ping", Ping);
            Interpreter.RegisterCommandHandler("playmacro", DummyCommand);
            Interpreter.RegisterCommandHandler("playsound", DummyCommand);
            Interpreter.RegisterCommandHandler("resync", Resync);
            Interpreter.RegisterCommandHandler("snapshot", DummyCommand);
            Interpreter.RegisterCommandHandler("hotkeys", DummyCommand);
            Interpreter.RegisterCommandHandler("where", DummyCommand);
            Interpreter.RegisterCommandHandler("messagebox", MessageBox);
            Interpreter.RegisterCommandHandler("mapuo", DummyCommand);
            Interpreter.RegisterCommandHandler("clickscreen", DummyCommand);
            Interpreter.RegisterCommandHandler("paperdoll", DummyCommand);
            Interpreter.RegisterCommandHandler("helpbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("guildbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("questsbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("logoutbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("virtue", DummyCommand);
            Interpreter.RegisterCommandHandler("msg", DummyCommand);
            Interpreter.RegisterCommandHandler("headmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("sysmsg", DummyCommand);

        }
        private static bool Fly(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }
        private static bool Land(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }
        private static string[] abilities = new string[4] { "primary", "secondary", "stun", "disarm" };
        private static bool SetAbility(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            List<ASTNode> args = ParseArguments(ref node);

            if (args.Count < 1)
                throw new ArgumentException("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

            if (!abilities.Contains(args[0].Lexeme))
                throw new ArgumentException("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

            if (args.Count == 2 && args[1].Lexeme == "on" || args.Count == 1)
            {
                switch (args[0].Lexeme)
                {
                    case "primary":
                        SpecialMoves.SetPrimaryAbility();
                        break;
                    case "secondary":
                        SpecialMoves.SetSecondaryAbility();
                        break;
                    case "stun":
                        Client.Instance.SendToServer(new StunRequest());
                        break;
                    case "disarm":
                        Client.Instance.SendToServer(new DisarmRequest());
                        break;
                    default:
                        break;
                }
            }
            else if (args.Count == 2 && args[1].Lexeme == "off")
            {
                Client.Instance.SendToServer(new UseAbility(AOSAbility.Clear));
                Client.Instance.SendToClient(ClearAbility.Instance);
            }

            return true;
        }
        private static bool Attack(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            node.Next(); // walk past argument

            return true;
        }
        private static string[] hands = new string[3] { "left", "right", "both" };
        private static bool ClearHands(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            // expect one STRING node

            ASTNode hand = node.Next();

            if (hand == null)
                throw new ArgumentException("Usage: clearhands ('left'/'right'/'both')");

            if (!hands.Contains(hand.Lexeme))
                throw new ArgumentException("Usage: clearhands ('left'/'right'/'both')");


            Item leftHand = World.Player.GetItemOnLayer(Layer.LeftHand);
            Item rightHand = World.Player.GetItemOnLayer(Layer.RightHand);

            switch (hand.Lexeme)
            {
                case "left":
                    if (leftHand != null)
                        DragDropManager.DragDrop(leftHand, World.Player.Backpack);
                    break;
                case "right":
                    if (rightHand != null)
                        DragDropManager.DragDrop(rightHand, World.Player.Backpack);
                    break;
                default:
                    if (leftHand != null)
                        DragDropManager.DragDrop(leftHand, World.Player.Backpack);
                    if (rightHand != null)
                        DragDropManager.DragDrop(rightHand, World.Player.Backpack);
                    break;
            }

            return true;
        }
        private static bool ClickObject(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            // expect one SERIAL node

            ASTNode obj = node.Next();

            if (obj == null)
                throw new ArgumentException("Usage: clickobject (serial)");

            int serial = GetSerial(ref obj);

            if (serial == -1)
                throw new ArgumentException("Invalid Serial in clickobject");

            Client.Instance.SendToServer(new SingleClick(serial));

            return true;
        }
        private static bool BandageSelf(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            if (World.Player == null)
                return true;

            Item pack = World.Player.Backpack;
            if (pack != null)
            {
                if (!UseItem(pack, 3617))
                {
                    World.Player.SendMessage(MsgLevel.Warning, LocString.NoBandages);
                }
                else
                {
                    if (force)
                    {
                        Targeting.ClearQueue();
                        Targeting.TargetSelf(true);
                    }
                    else
                        Targeting.TargetSelf(true);
                }
            }

            return true;
        }
        private static bool UseType(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // variable args here

            return true;
        }
        private static bool UseObject(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // expect a SERIAL node

            ASTNode obj = node.Next();

            if (obj == null)
                throw new ArgumentException("Usage: useobject (serial)");

            Serial serial = Serial.Parse(obj.Lexeme);

            if (!serial.IsValid)
                throw new ArgumentException("Invalid Serial in useobject");

            Client.Instance.SendToServer(new DoubleClick(serial));

            return true;
        }
        private static bool UseOnce(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            node.Next(); // item ID
            node.Next(); // ?color

            return true;
        }
        private static bool CleanUseQueue(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }
        private static bool MoveItem(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            node.Next(); // item alias or serial
            node.Next(); // target alias or serial
            node.Next(); // (x, y, z)?
            node.Next(); // amount?

            return true;
        }
        private static bool Walk(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }
        private static bool Turn(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }
        private static bool Run(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            return true;
        }

        private static Dictionary<string, int> UsableSkills = new Dictionary<string, int>()
        {
            { "anatomy", 1 }, // anatomy
            { "animallore", 2 }, // animal lore
            { "itemidentification", 3 }, // item identification
            { "armslore", 4 }, // arms lore
            { "begging", 6 }, // begging
            { "peacemaking", 9 }, // peacemaking
            { "cartography", 12 }, // cartography
            { "detectinghidden", 14 }, // detect hidden
            { "discordance", 15 }, // Discordance
            { "evaluatingintelligence", 16 }, // evaluate intelligence
            { "forensicevaluation", 19 }, // forensic evaluation
            { "hiding", 21 }, // hiding
            { "provocation", 22 }, // provocation
            { "inscription", 23 }, // inscription
            { "poisoning", 30 }, // poisoning
            { "spiritspeak", 32 }, // spirit speak
            { "stealing", 33 }, // stealing
            { "taming", 35 }, // taming
            { "tasteidentification", 36 }, // taste id
            { "tracking", 38 }, // tracking
            { "meditation", 46 }, // Meditation
            { "stealth", 47 }, // Stealth
            { "removetrap", 48 } // RemoveTrap
        };
        private static bool UseSkill(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // expect one string node or "last"

            ASTNode skill = node.Next();

            if (node.Lexeme == "last")
                Client.Instance.SendToServer(new UseSkill(World.Player.LastSkill));
            else if(UsableSkills.TryGetValue(node.Lexeme, out int skillId))
            {
                Client.Instance.SendToServer(new UseSkill(skillId));
            }

            return true;
        }
        private static bool Feed(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            node.Next(); // target alias or serial
            node.Next(); // food string
            node.Next(); // ?color
            node.Next(); // ?amount

            return true;
        }
        private static bool Rename(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            List<ASTNode> args = ParseArguments(ref node);

            if (args.Count != 2)
                throw new ArgumentException("Usage: rename (serial) ('name')");

            ASTNode target = args[0];

            int targetSerial = GetSerial(ref target);

            if (Client.Instance.ClientRunning && targetSerial != -1)
                Client.Instance.SendToServer(new RenameReq((uint)targetSerial, args[1].Lexeme));

            return true;
        }
        private static bool SetAlias(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            List<ASTNode> args = ParseArguments(ref node);

            if (args.Count != 2)
                throw new ArgumentException("Usage: setalias ('name') [serial]");

            ASTNode value = args[1]; // can't pass ref to this

            int serial = GetSerial(ref value);

            if (serial == -1)
                return true;

            Interpreter.SetAlias(args[0].Lexeme, serial);

            return true;
        }
        private static bool UnsetAlias(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            ASTNode alias = node.Next();

            if (alias == null)
                throw new ArgumentException("Usage: unsetalias (string)");

            Interpreter.SetAlias(alias.Lexeme, 0);

            return true;
        }

        private static bool ShowNames(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            ASTNode type = node.Next();

            if (World.Player == null)
                return true;

            if (type == null || type.Lexeme == "mobiles")
            {
                foreach (Mobile m in World.MobilesInRange())
                {
                    if (m != World.Player)
                        Client.Instance.SendToServer(new SingleClick(m));
                }
            }
            else if (type.Lexeme == "corpses")
            {
                foreach (Item i in World.Items.Values)
                {
                    if (i.IsCorpse)
                        Client.Instance.SendToServer(new SingleClick(i));
                }
            }

            return true;
        }
        private static bool Ping(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            Assistant.Ping.StartPing(5);

            return true;
        }
        private static bool Resync(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            if (Client.Instance.ClientRunning)
                Client.Instance.SendToServer(new ResyncReq());

            return true;
        }

        private static bool MessageBox(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            List<ASTNode> args = ParseArguments(ref node);

            if (args.Count != 2)
                throw new ArgumentException("Usage: messagebox ('title') ('body')");

            System.Windows.Forms.MessageBox.Show(args[0].Lexeme, args[1].Lexeme);

            return true;
        }
    }
}
