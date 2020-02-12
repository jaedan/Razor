﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UOSteam;
using Assistant;

namespace Assistant.Scripts
{
    public static class Commands
    {
        private static bool DummyCommand(string command, Argument[] args, bool quiet, bool force)
        {
            Console.WriteLine("Executing command {0} {1}", command, args);

            World.Player?.SendMessage(MsgLevel.Info, $"Unimplemented command {command}");

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

        public static void Register()
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
            Interpreter.RegisterCommandHandler("togglehands", ToggleHands);
            Interpreter.RegisterCommandHandler("equipitem", EquipItem);
            Interpreter.RegisterCommandHandler("togglemounted", DummyCommand);
            Interpreter.RegisterCommandHandler("equipwand", DummyCommand);
            Interpreter.RegisterCommandHandler("buy", DummyCommand);
            Interpreter.RegisterCommandHandler("sell", DummyCommand);
            Interpreter.RegisterCommandHandler("clearbuy", DummyCommand);
            Interpreter.RegisterCommandHandler("clearsell", DummyCommand);
            Interpreter.RegisterCommandHandler("organizer", DummyCommand);
            Interpreter.RegisterCommandHandler("autoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("dress", DressCommand);
            Interpreter.RegisterCommandHandler("undress", UnDressCommand);
            Interpreter.RegisterCommandHandler("dressconfig", DressConfig);
            Interpreter.RegisterCommandHandler("toggleautoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("togglescavenger", ToggleScavenger);
            Interpreter.RegisterCommandHandler("counter", DummyCommand);
            Interpreter.RegisterCommandHandler("unsetalias", UnsetAlias);
            Interpreter.RegisterCommandHandler("setalias", SetAlias);
            Interpreter.RegisterCommandHandler("promptalias", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforgump", WaitForGump);
            Interpreter.RegisterCommandHandler("replygump", DummyCommand);
            Interpreter.RegisterCommandHandler("closegump", DummyCommand);
            Interpreter.RegisterCommandHandler("clearjournal", ClearJournal);
            Interpreter.RegisterCommandHandler("waitforjournal", WaitForJournal);
            Interpreter.RegisterCommandHandler("poplist", PopList);
            Interpreter.RegisterCommandHandler("pushlist", PushList);
            Interpreter.RegisterCommandHandler("removelist", RemoveList);
            Interpreter.RegisterCommandHandler("createlist", CreateList);
            Interpreter.RegisterCommandHandler("clearlist", ClearList);
            Interpreter.RegisterCommandHandler("info", DummyCommand);
            Interpreter.RegisterCommandHandler("pause", Pause);
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
            Interpreter.RegisterCommandHandler("msg", Msg);
            Interpreter.RegisterCommandHandler("headmsg", HeadMsg);
            Interpreter.RegisterCommandHandler("partymsg", DummyCommand);
            Interpreter.RegisterCommandHandler("guildmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("allymsg", DummyCommand);
            Interpreter.RegisterCommandHandler("whispermsg", DummyCommand);
            Interpreter.RegisterCommandHandler("yellmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("sysmsg", SysMsg);
            Interpreter.RegisterCommandHandler("chatmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("emotemsg", DummyCommand);
            Interpreter.RegisterCommandHandler("promptmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("timermsg", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforprompt", DummyCommand);
            Interpreter.RegisterCommandHandler("cancelprompt", DummyCommand);
            Interpreter.RegisterCommandHandler("addfriend", DummyCommand);
            Interpreter.RegisterCommandHandler("removefriend", DummyCommand);
            Interpreter.RegisterCommandHandler("contextmenu", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforcontext", DummyCommand);
            Interpreter.RegisterCommandHandler("ignoreobject", DummyCommand);
            Interpreter.RegisterCommandHandler("clearignorelist", DummyCommand);
            Interpreter.RegisterCommandHandler("setskill", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforproperties", DummyCommand);
            Interpreter.RegisterCommandHandler("autocolorpick", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforcontents", DummyCommand);
            Interpreter.RegisterCommandHandler("miniheal", DummyCommand);
            Interpreter.RegisterCommandHandler("bigheal", DummyCommand);
            Interpreter.RegisterCommandHandler("cast", Cast);
            Interpreter.RegisterCommandHandler("chivalryheal", DummyCommand);
            Interpreter.RegisterCommandHandler("waitfortarget", WaitForTarget);
            Interpreter.RegisterCommandHandler("canceltarget", DummyCommand);
            Interpreter.RegisterCommandHandler("target", DummyCommand);
            Interpreter.RegisterCommandHandler("targettype", DummyCommand);
            Interpreter.RegisterCommandHandler("targetground", DummyCommand);
            Interpreter.RegisterCommandHandler("targettile", DummyCommand);
            Interpreter.RegisterCommandHandler("targettileoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("targettilerelative", DummyCommand);
            Interpreter.RegisterCommandHandler("cleartargetqueue", DummyCommand);
            Interpreter.RegisterCommandHandler("settimer", DummyCommand);
            Interpreter.RegisterCommandHandler("removetimer", DummyCommand);
            Interpreter.RegisterCommandHandler("createtimer", DummyCommand);
        }

        private static bool Fly(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool Land(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static string[] abilities = new string[4] { "primary", "secondary", "stun", "disarm" };
        private static bool SetAbility(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 1 || !abilities.Contains(args[0].AsString()))
            {
                ScriptUtilities.ScriptErrorMsg("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");
                return true;
            }

            if (args.Length == 2 && args[1].AsString() == "on" || args.Length == 1)
            {
                switch (args[0].AsString())
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
            else if (args.Length == 2 && args[1].AsString() == "off")
            {
                Client.Instance.SendToServer(new UseAbility(AOSAbility.Clear));
                Client.Instance.SendToClient(ClearAbility.Instance);
            }

            return true;
        }

        private static bool Attack(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static string[] hands = new string[3] { "left", "right", "both" };
        private static bool ClearHands(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0 || !hands.Contains(args[0].AsString()))
            {
                ScriptUtilities.ScriptErrorMsg("Usage: clearhands ('left'/'right'/'both')");
                return true;
            }

            switch (args[0].AsString())
            {
                case "left":
                    Dress.Unequip(Layer.LeftHand);
                    break;
                case "right":
                    Dress.Unequip(Layer.RightHand);
                    break;
                default:
                    Dress.Unequip(Layer.LeftHand);
                    Dress.Unequip(Layer.RightHand);
                    break;
            }

            return true;
        }
        private static bool ClickObject(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: clickobject (serial)");
                return true;
            }

            uint serial = args[0].AsSerial();
            Client.Instance.SendToServer(new SingleClick(serial));

            return true;
        }

        private static bool BandageSelf(string command, Argument[] args, bool quiet, bool force)
        {
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
        private static bool UseType(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool UseObject(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: useobject (serial)");
                return true;
            }

            Serial serial = args[0].AsSerial();

            if (!serial.IsValid)
            {
                ScriptUtilities.ScriptErrorMsg("useobject - invalid serial");
                return true;
            }

            Client.Instance.SendToServer(new DoubleClick(serial));

            return true;
        }
        private static bool UseOnce(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool CleanUseQueue(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool MoveItem(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: moveitem (serial) (destination) [(x, y, z)] [amount]");
                return true;
            }

            uint serial = args[0].AsSerial();
            uint destination = args[1].AsSerial();
            if (args.Length == 2)
                DragDropManager.DragDrop(World.FindItem((uint)serial), World.FindItem((uint)destination));
            else if (args.Length == 5)
                return true;
            else if (args.Length == 6)
                return true;

            return true;
        }

        private static bool Walk(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool Turn(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool Run(string command, Argument[] args, bool quiet, bool force)
        {
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

        private static bool UseSkill(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: useskill ('skill name'/'last')");
                return true;
            }

            if (args[0].AsString() == "last")
                Client.Instance.SendToServer(new UseSkill(World.Player.LastSkill));
            else if (UsableSkills.TryGetValue(args[0].AsString(), out int skillId))
                Client.Instance.SendToServer(new UseSkill(skillId));

            return true;
        }

        private static bool Feed(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static bool Rename(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: rename (serial) ('name')");
                return true;
            }

            uint targetSerial = args[0].AsSerial();

            if (Client.Instance.ClientRunning)
                Client.Instance.SendToServer(new RenameReq(targetSerial, args[1].AsString()));
            return true;
        }

        private static bool SetAlias(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: setalias ('name') [serial]");
                return true;
            }

            Interpreter.SetAlias(args[0].AsString(), args[1].AsSerial());

            return true;
        }

        // @Jaedan this crashes as we discussed
        private static bool WaitForGump(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: waitforgump (gump id/'any') (timeout)");
                return true;
            }

            if (!ScriptManager.Pause(args[0].AsInt()))
                return true;

            bool _strict = args[0].AsString() != "any";
            uint gumpId = _strict ? args[0].AsSerial() : uint.MaxValue;
            return (World.Player.HasGump || World.Player.HasCompressedGump) && (World.Player.CurrentGumpI == gumpId || !_strict);
        }

        private static bool ClearJournal(string command, Argument[] args, bool quiet, bool force)
        {
            Journal.Clear();

            return true;
        }

        private static bool WaitForJournal(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: waitforjournal ('text') (timeout) ['author'/'system']");
                return true;
            }

            if (!ScriptManager.Pause(args[1].AsInt()))
                return true;

            if (Journal.ContainsSafe(args[0].AsString()))
            {
                ScriptManager.Unpause();
                return true;
            }

            return false;
        }

        private static bool PopList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: poplist ('list name') ('element value'/'front'/'back')");
                return true;
            }

            if (args[1].AsString() == "front")
            {
                if (force)
                    while (Interpreter.PopList(args[0].AsString(), true)) { }
                else
                    Interpreter.PopList(args[0].AsString(), true);
            }
            else if (args[1].AsString() == "back")
            {
                if (force)
                    while (Interpreter.PopList(args[0].AsString(), false)) { }
                else
                    Interpreter.PopList(args[0].AsString(), false);
            }
            else
            {
                if (force)
                    while (Interpreter.PopList(args[0].AsString(), args[1])) { }
                else
                    Interpreter.PopList(args[0].AsString(), args[1]);
            }

            return true;
        }

        private static bool PushList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: pushlist ('list name') ('element value') ['front'/'back']");
                return true;
            }

            bool front = false;
            if (args.Length == 3)
            {
                if (args[2].AsString() == "front")
                    front = true;
            }

            Interpreter.PushList(args[0].AsString(), args[1], front, force);

            return true;
        }

        private static bool RemoveList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: removelist ('list name')");
                return true;
            }

            Interpreter.DestroyList(args[0].AsString());

            return true;
        }

        private static bool CreateList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: createlist ('list name')");
                return true;
            }

            Interpreter.CreateList(args[0].AsString());

            return true;
        }

        private static bool ClearList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: clearlist ('list name')");
                return true;
            }

            Interpreter.ClearList(args[0].AsString());

            return true;
        }

        private static bool UnsetAlias(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: unsetalias (string)");
                return true;
            }

            Interpreter.SetAlias(args[0].AsString(), 0);

            return true;
        }

        private static bool ShowNames(string command, Argument[] args, bool quiet, bool force)
        {
            if (World.Player == null)
                return true;

            if (args.Length == 0 || args[0].AsString() == "mobiles")
            {
                foreach (Mobile m in World.MobilesInRange())
                {
                    if (m != World.Player)
                        Client.Instance.SendToServer(new SingleClick(m));
                }
            }
            else if (args[0].AsString() == "corpses")
            {
                foreach (Item i in World.Items.Values)
                {
                    if (i.IsCorpse)
                        Client.Instance.SendToServer(new SingleClick(i));
                }
            }

            return true;
        }

        public static bool ToggleHands(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: togglehands ('left'/'right')");
                return true;
            }

            if (args[0].AsString() == "left")
                Dress.ToggleLeft();
            else
                Dress.ToggleRight();

            return true;
        }

        public static bool EquipItem(string command, Argument[] args, bool quiet, bool force)
        {
            if (!Client.Instance.ClientRunning || World.Player == null)
                return true;

            if (args.Length < 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: equipitem (serial) (layer)");
                return true;
            }

            Item equip = World.FindItem(args[0].AsSerial());
            byte layer = (byte)Utility.ToInt32(args[1].AsString(), 0);
            if (equip != null && (Layer)layer != Layer.Invalid)
                Dress.Equip(equip, (Layer)layer);

            return true;
        }

        public static bool ToggleScavenger(string command, Argument[] args, bool quiet, bool force)
        {
            ScavengerAgent.Instance.ToggleEnabled();

            return true;
        }

        private static bool Pause(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: pause (timeout)");
                return true;
            }

            if (!ScriptManager.Pause(args[0].AsInt()))
                return true;

            return false;
        }

        private static bool Ping(string command, Argument[] args, bool quiet, bool force)
        {
            Assistant.Ping.StartPing(5);

            return true;
        }

        private static bool Resync(string command, Argument[] args, bool quiet, bool force)
        {
            if (Client.Instance.ClientRunning)
                Client.Instance.SendToServer(new ResyncReq());

            return true;
        }

        private static bool MessageBox(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: messagebox ('title') ('body')");
                return true;
            }

            System.Windows.Forms.MessageBox.Show(args[0].AsString(), args[1].AsString());

            return true;
        }

        public static bool Msg(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: msg ('text') [color]");
                return true;
            }

            if (!Client.Instance.ClientRunning)
                return true;

            if (args.Length == 1)
                World.Player.Say(Config.GetInt("SysColor"), args[0].AsString());
            else
                World.Player.Say(Utility.ToInt32(args[1].AsString(), 0), args[0].AsString());

            return true;
        }

        public static bool Cast(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: cast 'spell' [serial]");
                return true;
            }

            if (!Client.Instance.ClientRunning)
            {
                return true;
            }

            Spell spell;

            if (int.TryParse(args[0].AsString(), out int spellnum))
                spell = Spell.Get(spellnum);
            else
                spell = Spell.GetByName(args[0].AsString());
            if (spell != null)
            {
                if (args.Length > 1)
                {
                    Serial s = args[1].AsSerial();
                    if (force)
                        Targeting.ClearQueue();
                    if (s > Serial.Zero && s != Serial.MinusOne)
                    {
                        Targeting.Target(s);
                    }
                    else if (!quiet)
                        ScriptUtilities.ScriptErrorMsg("cast - invalid serial or alias");
                }
            }
            else if (!quiet)
                ScriptUtilities.ScriptErrorMsg("cast - spell name or number not valid");

            return true;
        }

        public static bool WaitForTarget(string command, Argument[] args, bool quiet, bool force)
        {
            if (!ScriptManager.Pause(args[0].AsInt()))
                return true;

            if (Targeting.HasTarget)
            {
                ScriptManager.Unpause();
                return true;
            }

            return false;
        }

        public static bool HeadMsg(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: headmsg ('text') [color] [serial]");
                return true;
            }

            if (!Client.Instance.ClientRunning)
                return true;

            if (args.Length == 1)
                World.Player.OverheadMessage(Config.GetInt("SysColor"), args[0].AsString());
            else
            {
                int hue = Utility.ToInt32(args[1].AsString(), 0);

                if (args.Length == 3)
                {
                    uint serial = args[2].AsSerial();
                    Mobile m = World.FindMobile((uint)serial);

                    if (m != null)
                        m.OverheadMessage(hue, args[0].AsString());
                }
                else
                    World.Player.OverheadMessage(hue, args[0].AsString());
            }

            return true;
        }

        public static bool SysMsg(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: sysmsg ('text') [color]");
                return true;
            }

            if (!Client.Instance.ClientRunning)
                return true;

            if (args.Length == 1)
                World.Player.SendMessage(Config.GetInt("SysColor"), args[0].AsString());
            else if (args.Length == 2)
                World.Player.SendMessage(Utility.ToInt32(args[1].AsString(), 0), args[0].AsString());

            return true;
        }

        public static bool DressCommand(string command, Argument[] args, bool quiet, bool force)
        {
            if (!Client.Instance.ClientRunning)
                return true;

            //we're using a named dresslist or a temporary dresslist?
            if (args.Length == 0)
            {
                if (DressList._Temporary != null)
                    DressList._Temporary.Dress();
                else if (!quiet)
                    ScriptUtilities.ScriptErrorMsg("No dresslist specified and no temporary dressconfig present - usage: dress ['dresslist']");
            }
            else
            {
                var d = DressList.Find(args[0].AsString());
                if (d != null)
                    d.Dress();
                else if (!quiet)
                    ScriptUtilities.ScriptErrorMsg($"dresslist {args[0].AsString()} not found");
            }

            return true;
        }

        public static bool UnDressCommand(string command, Argument[] args, bool quiet, bool force)
        {
            if (!Client.Instance.ClientRunning)
                return true;

            //we're using a named dresslist or a temporary dresslist?
            if (args.Length == 0)
            {
                if (DressList._Temporary != null)
                    DressList._Temporary.Undress();
                else if (!quiet)
                    ScriptUtilities.ScriptErrorMsg("No dresslist specified and no temporary dressconfig present - usage: undress ['dresslist']");
            }
            else
            {
                var d = DressList.Find(args[0].AsString());
                if (d != null)
                    d.Undress();
                else if (!quiet)
                    ScriptUtilities.ScriptErrorMsg($"dresslist {args[0].AsString()} not found");
            }

            return true;
        }

        public static bool DressConfig(string command, Argument[] args, bool quiet, bool force)
        {
            if (!Client.Instance.ClientRunning)
                return true;

            if (DressList._Temporary == null)
                DressList._Temporary = new DressList("dressconfig");

            DressList._Temporary.Items.Clear();
            for (int i = 0; i < World.Player.Contains.Count; i++)
            {
                Item item = World.Player.Contains[i];
                if (item.Layer <= Layer.LastUserValid && item.Layer != Layer.Backpack && item.Layer != Layer.Hair &&
                    item.Layer != Layer.FacialHair)
                    DressList._Temporary.Items.Add(item.Serial);
            }

            return true;
        }
    }
}
