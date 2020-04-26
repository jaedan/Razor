﻿#region license

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
        private static bool UnimplementedCommand(string command, Argument[] args, bool quiet, bool force)
        {
            ScriptManager.Error(quiet, command, "Unimplemented command");
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
            Interpreter.RegisterCommandHandler("fly", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("land", UnimplementedCommand);
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
            Interpreter.RegisterCommandHandler("moveitemoffset", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("movetype", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("movetypeoffset", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("walk", Walk);
            Interpreter.RegisterCommandHandler("turn", Turn);
            Interpreter.RegisterCommandHandler("run", Run);
            Interpreter.RegisterCommandHandler("useskill", UseSkill);
            Interpreter.RegisterCommandHandler("feed", Feed);
            Interpreter.RegisterCommandHandler("rename", Rename);
            Interpreter.RegisterCommandHandler("shownames", ShowNames);
            Interpreter.RegisterCommandHandler("togglehands", ToggleHands);
            Interpreter.RegisterCommandHandler("equipitem", EquipItem);
            Interpreter.RegisterCommandHandler("togglemounted", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("equipwand", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("buy", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("sell", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("clearbuy", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("clearsell", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("organizer", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autoloot", Deprecated);
            Interpreter.RegisterCommandHandler("dress", DressCommand);
            Interpreter.RegisterCommandHandler("undress", UnDressCommand);
            Interpreter.RegisterCommandHandler("dressconfig", DressConfig);
            Interpreter.RegisterCommandHandler("toggleautoloot", Deprecated);
            Interpreter.RegisterCommandHandler("togglescavenger", ToggleScavenger);
            Interpreter.RegisterCommandHandler("counter", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("unsetalias", UnsetAlias);
            Interpreter.RegisterCommandHandler("setalias", SetAlias);
            Interpreter.RegisterCommandHandler("promptalias", PromptAlias);
            Interpreter.RegisterCommandHandler("waitforgump", WaitForGump);
            Interpreter.RegisterCommandHandler("replygump", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("closegump", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("clearjournal", ClearJournal);
            Interpreter.RegisterCommandHandler("waitforjournal", WaitForJournal);
            Interpreter.RegisterCommandHandler("poplist", PopList);
            Interpreter.RegisterCommandHandler("pushlist", PushList);
            Interpreter.RegisterCommandHandler("removelist", RemoveList);
            Interpreter.RegisterCommandHandler("createlist", CreateList);
            Interpreter.RegisterCommandHandler("clearlist", ClearList);
            Interpreter.RegisterCommandHandler("info", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("pause", Pause);
            Interpreter.RegisterCommandHandler("ping", Ping);
            Interpreter.RegisterCommandHandler("playmacro", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("playsound", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("resync", Resync);
            Interpreter.RegisterCommandHandler("snapshot", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("hotkeys", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("where", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("messagebox", MessageBox);
            Interpreter.RegisterCommandHandler("mapuo", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("clickscreen", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("paperdoll", Paperdoll);
            Interpreter.RegisterCommandHandler("helpbutton", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("guildbutton", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("questsbutton", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("logoutbutton", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("virtue", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("msg", Msg);
            Interpreter.RegisterCommandHandler("headmsg", HeadMsg);
            Interpreter.RegisterCommandHandler("partymsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("guildmsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("allymsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("whispermsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("yellmsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("sysmsg", SysMsg);
            Interpreter.RegisterCommandHandler("chatmsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("emotemsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("promptmsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("timermsg", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("waitforprompt", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("cancelprompt", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("addfriend", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("removefriend", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("contextmenu", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("waitforcontext", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("ignoreobject", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("clearignorelist", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("setskill", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("waitforproperties", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autocolorpick", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("waitforcontents", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("miniheal", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("bigheal", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("cast", Cast);
            Interpreter.RegisterCommandHandler("chivalryheal", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("waitfortarget", WaitForTarget);
            Interpreter.RegisterCommandHandler("canceltarget", CancelTarget);
            Interpreter.RegisterCommandHandler("target", Target);
            Interpreter.RegisterCommandHandler("targettype", TargetType);
            Interpreter.RegisterCommandHandler("targetground", TargetGround);
            Interpreter.RegisterCommandHandler("targettile", TargetTile);
            Interpreter.RegisterCommandHandler("targettileoffset", TargetTileOffset);
            Interpreter.RegisterCommandHandler("targettilerelative", TargetTileRelative);
            Interpreter.RegisterCommandHandler("cleartargetqueue", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargetlast", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargetself", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargetobject", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargettype", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargettile", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargettileoffset", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargettilerelative", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargetghost", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("autotargetground", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("cancelautotarget", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("getenemy", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("getfriend", UnimplementedCommand);
            Interpreter.RegisterCommandHandler("settimer", SetTimer);
            Interpreter.RegisterCommandHandler("removetimer", RemoveTimer);
            Interpreter.RegisterCommandHandler("createtimer", CreateTimer);
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
                ScriptManager.Error(quiet, command, "Object not found.");
            else
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

        private static Dictionary<string, Direction> _directions = new Dictionary<string, Direction>()
        {
            { "north", Direction.North },
            { "northeast", Direction.Right },
            { "right", Direction.Right },
            { "east", Direction.East },
            { "southeast", Direction.Down },
            { "down", Direction.Down },
            { "south", Direction.South },
            { "southwest", Direction.Left },
            { "left", Direction.Left },
            { "west", Direction.West },
            { "northwest", Direction.Up },
            { "up", Direction.Up }
        };

        // Milliseconds per tile
        private static readonly TimeSpan WALK_MS = TimeSpan.FromMilliseconds(400);
        private static readonly TimeSpan RUN_MS = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan MOUNTED_WALK_MS = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan MOUNTED_RUN_MS = TimeSpan.FromMilliseconds(100);

        private static bool Walk(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: walk ('direction name')");

            if (!_directions.TryGetValue(args[0].AsString().ToLower(), out var dir))
                throw new RunTimeError(null, "Usage: walk ('direction name')");

            var delay = WALK_MS;
            if (World.Player.GetItemOnLayer(Layer.Mount) != null)
                delay = MOUNTED_WALK_MS;

            if (ScriptManager.LastMove + delay >= DateTime.UtcNow)
                return false;

            ScriptManager.LastMove = DateTime.UtcNow;

            Client.Instance?.RequestMove(dir);

            return true;
        }

        private static bool Turn(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: turn ('direction name')");

            if (!_directions.TryGetValue(args[0].AsString().ToLower(), out var dir))
                throw new RunTimeError(null, "Usage: turn ('direction name')");

            if ((World.Player.Direction & Direction.Mask) != dir)
                Client.Instance?.RequestMove(dir);

            return true;
        }

        private static bool Run(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: run ('direction name')");

            if (!_directions.TryGetValue(args[0].AsString().ToLower(), out var dir))
                throw new RunTimeError(null, "Usage: run ('direction name')");

            var delay = RUN_MS;
            if (World.Player.GetItemOnLayer(Layer.Mount) != null)
                delay = MOUNTED_RUN_MS;

            if (ScriptManager.LastMove + delay >= DateTime.UtcNow)
                return false;

            ScriptManager.LastMove = DateTime.UtcNow;

            Client.Instance?.RequestMove(dir & Direction.Running);

            return true;
        }

        private static HashSet<int> _usableSkills = new HashSet<int>()
        {
            1, // anatomy
            2, // animal lore
            3, // item identification
            4, // arms lore
            6, // begging
            9, // peacemaking
            12, // cartography
            14, // detect hidden
            15, // Discordance
            16, // evaluate intelligence
            19, // forensic evaluation
            21, // hiding
            22, // provocation
            23, // inscription
            30, // poisoning
            32, // spirit speak
            33, // stealing
            35, // taming
            36, // taste id
            38, // tracking
            46, // Meditation
            47, // Stealth
            48 // RemoveTrap
        };

        private static bool UseSkill(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: useskill ('skill name'/'last')");

            if (args[0].AsString() == "last")
            {
                Client.Instance.SendToServer(new UseSkill(World.Player.LastSkill));
                return true;
            }

            var skill = ScriptManager.GetSkill(args[0].AsString());

            if (!_usableSkills.Contains(skill.Index))
                throw new RunTimeError(null, "That skill  is not usable");

            Client.Instance.SendToServer(new UseSkill(skill.Index));

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

        private static bool PromptAlias(string command, Argument[] args, bool quiet, bool force)
        {
            Interpreter.Pause(60000);

            if (!_hasPrompt)
            {
                _hasPrompt = true;
                Targeting.OneTimeTarget((location, serial, p, gfxid) =>
                {
                    Interpreter.SetAlias(args[0].AsString(), serial);
                    Interpreter.Unpause();
                });
                return false;
            }

            _hasPrompt = false;
            return true;
        }

        private static bool WaitForGump(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 2)
                throw new RunTimeError(null, "Usage: waitforgump (gump id/'any') (timeout)");

            Interpreter.Timeout(args[1].AsUInt(), () => { return true; });
            bool any = args[0].AsString() == "any";

            if (any)
            {
                if (World.Player.HasGump || World.Player.HasCompressedGump)
                {
                    Interpreter.ClearTimeout();
                    return true;
                }
            }
            else
            {
                uint gumpId = args[0].AsSerial();

                if (World.Player.CurrentGumpI == gumpId)
                {
                    Interpreter.ClearTimeout();
                    return true;
                }
            }
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
                Interpreter.Timeout(args[1].AsUInt(), () => { return true; });
                return false;
            }
            else
                Interpreter.ClearTimeout();

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
            switch (args.Length)
            {
                case 1:
                    World.Player.Say(Config.GetInt("SysColor"), args[0].AsString());
                    break;
                case 2:
                    World.Player.Say(args[1].AsInt(), args[0].AsString());
                    break;
                default:
                    throw new RunTimeError(null, "Usage: msg ('text') [color]");
            }

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

            Interpreter.Timeout(args[0].AsUInt(), () => { return true; });
            if (Targeting.HasTarget)
            {
                Interpreter.ClearTimeout();
                return true;
            }
            return false;
        }

        private static bool CancelTarget(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 0)
                throw new RunTimeError(null, "Usage: canceltarget");

            if (Targeting.HasTarget)
                Targeting.CancelOneTimeTarget();

            return true;
        }

        private static bool Target(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: target (serial)");

            if (!Targeting.HasTarget)
                ScriptManager.Error(quiet, command, "No target cursor available. Consider using waitfortarget.");
            else
                Targeting.Target(args[0].AsSerial());

            return true;
        }

        private static bool TargetType(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 1 || args.Length > 3)
                throw new RunTimeError(null, "Usage: targettype (graphic) [color] [range]");

            if (!Targeting.HasTarget)
            {
                ScriptManager.Error(quiet, command, "No target cursor available. Consider using waitfortarget.");
                return true;
            }

            var graphic = args[0].AsInt();

            uint serial = Serial.MinusOne;

            switch (args.Length)
            {
                case 1:
                    // Only graphic
                    serial = World.FindItemByType(graphic).Serial;
                    break;
                case 2:
                    {
                        // graphic and color
                        var color = args[1].AsUShort();
                        foreach (var item in World.Items.Values)
                        {
                            if (item.ItemID.Value == graphic && item.Hue == color)
                            {
                                serial = item.Serial;
                                break;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        // graphic, color, range
                        var color = args[1].AsUShort();
                        var range = args[2].AsInt();
                        foreach (var item in World.Items.Values)
                        {
                            if (item.ItemID.Value == graphic && item.Hue == color && Utility.Distance(item.Position, World.Player.Position) < range)
                            {
                                serial = item.Serial;
                                break;
                            }
                        }
                        break;
                    }

            }

            if (serial == Serial.MinusOne)
                throw new RunTimeError(null, "Unable to find suitable target");

            Targeting.Target(serial);
            return true;
        }

        private static bool TargetGround(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length < 1 || args.Length > 3)
                throw new RunTimeError(null, "Usage: targetground (graphic) [color] [range]");

            throw new RunTimeError(null, $"Unimplemented command {command}");
        }

        private static bool TargetTile(string command, Argument[] args, bool quiet, bool force)
        {
            if (!(args.Length == 1 || args.Length == 3))
                throw new RunTimeError(null, "Usage: targettile ('last'/'current'/(x y z))");

            if (!Targeting.HasTarget)
            {
                ScriptManager.Error(quiet, command, "No target cursor available. Consider using waitfortarget.");
                return true;
            }

            Point3D position = Point3D.MinusOne;

            switch (args.Length)
            {
                case 1:
                    {
                        var alias = args[0].AsString();
                        if (alias == "last")
                        {
                            if (Targeting.LastTargetInfo.Type != 1)
                                throw new RunTimeError(null, "Last target was not a ground target");

                            position = new Point3D(Targeting.LastTargetInfo.X, Targeting.LastTargetInfo.Y, Targeting.LastTargetInfo.Z);
                        }
                        else if (alias == "current")
                        {
                            position = World.Player.Position;
                        }
                        break;
                    }
                case 3:
                    position = new Point3D(args[0].AsInt(), args[1].AsInt(), args[2].AsInt());
                    break;
            }

            if (position == Point3D.MinusOne)
                throw new RunTimeError(null, "Usage: targettile ('last'/'current'/(x y z))");

            Targeting.Target(position);
            return true;
        }

        private static bool TargetTileOffset(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 3)
                throw new RunTimeError(null, "Usage: targettileoffset (x y z)");

            if (!Targeting.HasTarget)
            {
                ScriptManager.Error(quiet, command, "No target cursor available. Consider using waitfortarget.");
                return true;
            }

            var position = World.Player.Position;

            position.X += args[0].AsInt();
            position.Y += args[1].AsInt();
            position.Z += args[2].AsInt();

            Targeting.Target(position);
            return true;
        }

        private static bool TargetTileRelative(string command, Argument[] args, bool quiet, bool force)
        {
            if (args.Length != 2)
                throw new RunTimeError(null, "Usage: targettilerelative (serial) (range). Range may be negative.");

            if (!Targeting.HasTarget)
            {
                ScriptManager.Error(quiet, command, "No target cursor available. Consider using waitfortarget.");
                return true;
            }

            var serial = args[0].AsSerial();
            var range = args[1].AsInt();

            var mobile = World.FindMobile(serial);

            if (mobile == null)
            {
                /* TODO: Search items if mobile not found. Although this isn't very useful. */
                ScriptManager.Error(quiet, command, "item or mobile not found.");
                return true;
            }

            var position = mobile.Position;

            switch (mobile.Direction)
            {
                case Direction.North:
                    position.Y -= range;
                    break;
                case Direction.Right:
                    position.X += range;
                    position.Y -= range;
                    break;
                case Direction.East:
                    position.X += range;
                    break;
                case Direction.Down:
                    position.X += range;
                    position.Y += range;
                    break;
                case Direction.South:
                    position.Y += range;
                    break;
                case Direction.Left:
                    position.X -= range;
                    position.Y += range;
                    break;
                case Direction.West:
                    position.X -= range;
                    break;
                case Direction.Up:
                    position.X -= range;
                    position.Y -= range;
                    break;
            }

            Targeting.Target(position);

            return true;
        }

        public static bool HeadMsg(string command, Argument[] args, bool quiet, bool force)
        {
            switch (args.Length)
            {
                case 1:
                    World.Player.OverheadMessage(Config.GetInt("SysColor"), args[0].AsString());
                    break;
                case 2:
                    World.Player.OverheadMessage(args[1].AsInt(), args[0].AsString());
                    break;
                case 3:
                    Mobile m = World.FindMobile(args[2].AsSerial());

                    if (m != null)
                        m.OverheadMessage(args[1].AsInt(), args[0].AsString());
                    break;
                default:
                    throw new RunTimeError(null, "Usage: headmsg (text) [color] [serial]");
            }

            return true;
        }

        public static bool SysMsg(string command, Argument[] args, bool quiet, bool force)
        {
            switch (args.Length)
            {
                case 1:
                    World.Player.SendMessage(Config.GetInt("SysColor"), args[0].AsString());
                    break;
                case 2:
                    World.Player.SendMessage(args[1].AsInt(), args[0].AsString());
                    break;
                default:
                    throw new RunTimeError(null, "Usage: sysmsg ('text') [color]");
            }

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
