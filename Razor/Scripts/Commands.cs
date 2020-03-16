#region license

// Razor: An Ultima Online Assistant
// Copyright (C) 2020 Razor Development Community on GitHub <https://github.com/markdwags/Razor>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UOScript;
using Assistant;
using Assistant.Core;
using Assistant.Agents;

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

        private static bool Deprecated(string command, Argument[] args, bool quiet, bool force)
        {
            World.Player?.SendMessage(MsgLevel.Info, $"Deprecated command {command}");

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
            Interpreter.RegisterCommandHandler("clearuseonce", CleanUseQueue);
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
            Interpreter.RegisterCommandHandler("autoloot", Deprecated);
            Interpreter.RegisterCommandHandler("dress", DressCommand);
            Interpreter.RegisterCommandHandler("undress", UnDressCommand);
            Interpreter.RegisterCommandHandler("dressconfig", DressConfig);
            Interpreter.RegisterCommandHandler("toggleautoloot", Deprecated);
            Interpreter.RegisterCommandHandler("togglescavenger", ToggleScavenger);
            Interpreter.RegisterCommandHandler("counter", DummyCommand);
            Interpreter.RegisterCommandHandler("unsetalias", UnsetAlias);
            Interpreter.RegisterCommandHandler("setalias", SetAlias);
            Interpreter.RegisterCommandHandler("promptalias", PromptAlias);
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
            Interpreter.RegisterCommandHandler("paperdoll", Paperdoll);
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
            Interpreter.RegisterCommandHandler("autotargetlast", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargetself", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargetobject", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargettype", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargettile", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargettileoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargettilerelative", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargetghost", DummyCommand);
            Interpreter.RegisterCommandHandler("autotargetground", DummyCommand);
            Interpreter.RegisterCommandHandler("cancelautotarget", DummyCommand);
            Interpreter.RegisterCommandHandler("getenemy", DummyCommand);
            Interpreter.RegisterCommandHandler("getfriend", DummyCommand);
            Interpreter.RegisterCommandHandler("settimer", SetTimer);
            Interpreter.RegisterCommandHandler("removetimer", RemoveTimer);
            Interpreter.RegisterCommandHandler("createtimer", CreateTimer);
        }

        private static bool Fly(string command, Argument[] args, bool quiet, bool force)
        {
            throw new RunTimeError(null, "Command is not yet implemented");
        }

        private static bool Land(string command, Argument[] args, bool quiet, bool force)
        {
            return true;
        }

        private static string[] abilities = new string[4] { "primary", "secondary", "stun", "disarm" };
        private static bool SetAbility(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 1 || !abilities.Contains(args[0].AsString()))
                throw new RunTimeError(null, "Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

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
                throw new RunTimeError(null, "Usage: clearhands ('left'/'right'/'both')");

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
                throw new RunTimeError(null, "Usage: clickobject (serial)");

            uint serial = args[0].AsSerial();
            Client.Instance.SendToServer(new SingleClick(serial));

            return true;
        }

        private static bool BandageSelf(string command, Argument[] args, bool quiet, bool force)
        {
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
                throw new RunTimeError(null, "Usage: useobject (serial)");

            Serial serial = args[0].AsSerial();

            if (!serial.IsValid)
                throw new RunTimeError(null, "useobject - invalid serial");

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
                throw new RunTimeError(null, "Usage: moveitem (serial) (destination) [(x, y, z)] [amount]");

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
                throw new RunTimeError(null, "Usage: useskill ('skill name'/'last')");

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
                throw new RunTimeError(null, "Usage: rename (serial) ('name')");

            uint targetSerial = args[0].AsSerial();

            Client.Instance.SendToServer(new RenameReq(targetSerial, args[1].AsString()));
            return true;
        }

        private static bool SetAlias(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
                throw new RunTimeError(null, "Usage: setalias ('name') [serial]");

            Interpreter.SetAlias(args[0].AsString(), args[1].AsSerial());

            return true;
        }

        private static bool _hasPrompt = false;
        private static string _nextPromptAliasName = "";
        private static void OnPromptAliasTarget(bool location, Serial serial, Point3D p, ushort gfxid)
        {
            Interpreter.SetAlias(_nextPromptAliasName, serial);
        }

        private static bool PromptAlias(string command, Argument[] args, bool quiet, bool force)
        {
            if (!_hasPrompt)
            {
                _hasPrompt = true;
                _nextPromptAliasName = args[0].AsString();
                Targeting.OneTimeTarget(OnPromptAliasTarget);
                return false;
            }

            if (!Targeting.HasTarget)
            {
                _hasPrompt = false;
                _nextPromptAliasName = "";
                return true;
            }

            return false;
        }

        private static bool WaitForGump(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
                throw new RunTimeError(null, "Usage: waitforgump (gump id/'any') (timeout)");

            bool any = args[0].AsString() == "any";

            if (any)
            {
                if (World.Player.HasGump || World.Player.HasCompressedGump)
                    return true;
            }
            else
            {
                uint gumpId = args[0].AsSerial();

                if (World.Player.CurrentGumpI == gumpId)
                    return true;
            }

            Interpreter.Timeout(args[1].AsUInt());
            return false;
        }

        private static bool ClearJournal(string command, Argument[] args, bool quiet, bool force)
        {
            Journal.Clear();

            return true;
        }

        private static bool WaitForJournal(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
                throw new RunTimeError(null, "Usage: waitforjournal ('text') (timeout) ['author'/'system']");

            if (!Journal.ContainsSafe(args[0].AsString()))
            {
                Interpreter.Timeout(args[1].AsUInt());
                return false;
            }

            return true;
        }

        private static bool PopList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
                throw new RunTimeError(null, "Usage: poplist ('list name') ('element value'/'front'/'back')");

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
                throw new RunTimeError(null, "Usage: pushlist ('list name') ('element value') ['front'/'back']");

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
                throw new RunTimeError(null, "Usage: removelist ('list name')");

            Interpreter.DestroyList(args[0].AsString());

            return true;
        }

        private static bool CreateList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: createlist ('list name')");

            Interpreter.CreateList(args[0].AsString());

            return true;
        }

        private static bool ClearList(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: clearlist ('list name')");

            Interpreter.ClearList(args[0].AsString());

            return true;
        }

        private static bool UnsetAlias(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: unsetalias (string)");

            Interpreter.SetAlias(args[0].AsString(), 0);

            return true;
        }

        private static bool ShowNames(string command, Argument[] args, bool quiet, bool force)
        {
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
                throw new RunTimeError(null, "Usage: togglehands ('left'/'right')");

            if (args[0].AsString() == "left")
                Dress.ToggleLeft();
            else
                Dress.ToggleRight();

            return true;
        }

        public static bool EquipItem(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
                throw new RunTimeError(null, "Usage: equipitem (serial) (layer)");

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
                throw new RunTimeError(null, "Usage: pause (timeout)");

            Interpreter.Pause(args[0].AsUInt());
            return true;
        }

        private static bool Ping(string command, Argument[] args, bool quiet, bool force)
        {
            Assistant.Ping.StartPing(5);

            return true;
        }

        private static bool Resync(string command, Argument[] args, bool quiet, bool force)
        {
            Client.Instance.SendToServer(new ResyncReq());

            return true;
        }

        private static bool MessageBox(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
                throw new RunTimeError(null, "Usage: messagebox ('title') ('body')");

            System.Windows.Forms.MessageBox.Show(args[0].AsString(), args[1].AsString());

            return true;
        }

        public static bool Msg(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: msg ('text') [color]");

            if (args.Length == 1)
                World.Player.Say(Config.GetInt("SysColor"), args[0].AsString());
            else
                World.Player.Say(Utility.ToInt32(args[1].AsString(), 0), args[0].AsString());

            return true;
        }

        private static bool Paperdoll(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length > 1)
                throw new RunTimeError(null, "Usage: paperdoll [serial]");

            uint serial = args.Length == 0 ? World.Player.Serial.Value : args[0].AsSerial();
            Client.Instance.SendToServer(new DoubleClick(serial));

            return true;
        }

        public static bool Cast(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: cast 'spell' [serial]");

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
                        throw new RunTimeError(null, "cast - invalid serial or alias");
                }
            }
            else if (!quiet)
                throw new RunTimeError(null, "cast - spell name or number not valid");

            return true;
        }

        private static bool WaitForTarget(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: waitfortarget (timeout)");

            if (Targeting.HasTarget)
                return true;

            Interpreter.Timeout(args[0].AsUInt());
            return false;
        }

        public static bool HeadMsg(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: headmsg ('text') [color] [serial]");

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
                throw new RunTimeError(null, "Usage: sysmsg ('text') [color]");

            if (args.Length == 1)
                World.Player.SendMessage(Config.GetInt("SysColor"), args[0].AsString());
            else if (args.Length == 2)
                World.Player.SendMessage(Utility.ToInt32(args[1].AsString(), 0), args[0].AsString());

            return true;
        }

        public static bool DressCommand(string command, Argument[] args, bool quiet, bool force)
        {
            //we're using a named dresslist or a temporary dresslist?
            if (args.Length == 0)
            {
                if (DressList._Temporary != null)
                    DressList._Temporary.Dress();
                else if (!quiet)
                    throw new RunTimeError(null, "No dresslist specified and no temporary dressconfig present - usage: dress ['dresslist']");
            }
            else
            {
                var d = DressList.Find(args[0].AsString());
                if (d != null)
                    d.Dress();
                else if (!quiet)
                    throw new RunTimeError(null, $"dresslist {args[0].AsString()} not found");
            }

            return true;
        }

        public static bool UnDressCommand(string command, Argument[] args, bool quiet, bool force)
        {
            //we're using a named dresslist or a temporary dresslist?
            if (args.Length == 0)
            {
                if (DressList._Temporary != null)
                    DressList._Temporary.Undress();
                else if (!quiet)
                    throw new RunTimeError(null, "No dresslist specified and no temporary dressconfig present - usage: undress ['dresslist']");
            }
            else
            {
                var d = DressList.Find(args[0].AsString());
                if (d != null)
                    d.Undress();
                else if (!quiet)
                    throw new RunTimeError(null, $"dresslist {args[0].AsString()} not found");
            }

            return true;
        }

        public static bool DressConfig(string command, Argument[] args, bool quiet, bool force)
        {
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

        private static bool SetTimer(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
                throw new RunTimeError(null, "Usage: settimer (timer name) (value)");


            Interpreter.SetTimer(args[0].AsString(), args[1].AsInt());
            return true;
        }

        private static bool RemoveTimer(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: removetimer (timer name)");

            Interpreter.RemoveTimer(args[0].AsString());
            return true;
        }

        private static bool CreateTimer(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: createtimer (timer name)");

            Interpreter.CreateTimer(args[0].AsString());
            return true;
        }
    }
}
