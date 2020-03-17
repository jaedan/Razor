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

using System.Collections.Generic;
using UOScript;

namespace Assistant.Scripts
{
    public static class Expressions
    {
        public static void Register()
        {
            // Expressions
            Interpreter.RegisterExpressionHandler("findalias", FindAlias);
            Interpreter.RegisterExpressionHandler("contents", Contents);
            Interpreter.RegisterExpressionHandler("inregion", InRegion);
            Interpreter.RegisterExpressionHandler("skill", SkillExpression);
            Interpreter.RegisterExpressionHandler("x", X);
            Interpreter.RegisterExpressionHandler("y", Y);
            Interpreter.RegisterExpressionHandler("z", Z);
            Interpreter.RegisterExpressionHandler("physical", Physical);
            Interpreter.RegisterExpressionHandler("fire", Fire);
            Interpreter.RegisterExpressionHandler("cold", Cold);
            Interpreter.RegisterExpressionHandler("poison", Poison);
            Interpreter.RegisterExpressionHandler("energy", Energy);
            Interpreter.RegisterExpressionHandler("str", Str);
            Interpreter.RegisterExpressionHandler("dex", Dex);
            Interpreter.RegisterExpressionHandler("int", Int);
            Interpreter.RegisterExpressionHandler("hits", Hits);
            Interpreter.RegisterExpressionHandler("maxhits", MaxHits);
            Interpreter.RegisterExpressionHandler("diffhits", DiffHits);
            Interpreter.RegisterExpressionHandler("stam", Stam);
            Interpreter.RegisterExpressionHandler("maxstam", MaxStam);
            Interpreter.RegisterExpressionHandler("mana", Mana);
            Interpreter.RegisterExpressionHandler("maxmana", MaxMana);
            Interpreter.RegisterExpressionHandler("usequeue", UseQueue);
            Interpreter.RegisterExpressionHandler("dressing", Dressing);
            Interpreter.RegisterExpressionHandler("organizing", Organizing);
            Interpreter.RegisterExpressionHandler("followers", Followers);
            Interpreter.RegisterExpressionHandler("maxfollowers", MaxFollowers);
            Interpreter.RegisterExpressionHandler("gold", Gold);
            Interpreter.RegisterExpressionHandler("hidden", Hidden);
            Interpreter.RegisterExpressionHandler("luck", Luck);
            Interpreter.RegisterExpressionHandler("tithingpoints", TithingPoints);
            Interpreter.RegisterExpressionHandler("weight", Weight);
            Interpreter.RegisterExpressionHandler("maxweight", MaxWeight);
            Interpreter.RegisterExpressionHandler("diffweight", DiffWeight);
            Interpreter.RegisterExpressionHandler("serial", Serial);
            Interpreter.RegisterExpressionHandler("graphic", Graphic);
            Interpreter.RegisterExpressionHandler("color", Color);
            Interpreter.RegisterExpressionHandler("amount", Amount);
            Interpreter.RegisterExpressionHandler("name", Name);
            Interpreter.RegisterExpressionHandler("dead", Dead);
            Interpreter.RegisterExpressionHandler("direction", Direction);
            Interpreter.RegisterExpressionHandler("flying", Flying);
            Interpreter.RegisterExpressionHandler("paralyzed", Paralyzed);
            Interpreter.RegisterExpressionHandler("poisoned", Poisoned);
            Interpreter.RegisterExpressionHandler("mounted", Mounted);
            Interpreter.RegisterExpressionHandler("yellowhits", YellowHits);
            Interpreter.RegisterExpressionHandler("criminal", Criminal);
            Interpreter.RegisterExpressionHandler("enemy", Enemy);
            Interpreter.RegisterExpressionHandler("friend", Friend);
            Interpreter.RegisterExpressionHandler("gray", Gray);
            Interpreter.RegisterExpressionHandler("innocent", Innocent);
            Interpreter.RegisterExpressionHandler("invulnerable", Invulnerable);
            Interpreter.RegisterExpressionHandler("murderer", Murderer);
            Interpreter.RegisterExpressionHandler("findobject", FindObject);
            Interpreter.RegisterExpressionHandler("distance", Distance);
            Interpreter.RegisterExpressionHandler("inrange", InRange);
            Interpreter.RegisterExpressionHandler("buffexists", BuffExists);
            Interpreter.RegisterExpressionHandler("property", Property);
            Interpreter.RegisterExpressionHandler("findtype", FindType);
            Interpreter.RegisterExpressionHandler("findlayer", FindLayer);
            Interpreter.RegisterExpressionHandler("skillstate", SkillState);
            Interpreter.RegisterExpressionHandler("counttype", CountType);
            Interpreter.RegisterExpressionHandler("counttypeground", CountTypeGround);
            Interpreter.RegisterExpressionHandler("findwand", FindWand);
            Interpreter.RegisterExpressionHandler("inparty", InParty);
            Interpreter.RegisterExpressionHandler("infriendslist", InFriendsList);
            Interpreter.RegisterExpressionHandler("war", War);
            Interpreter.RegisterExpressionHandler("ingump", InGump);
            Interpreter.RegisterExpressionHandler("gumpexists", GumpExists);
            Interpreter.RegisterExpressionHandler("injournal", InJournal);
            Interpreter.RegisterExpressionHandler("listexists", ListExists);
            Interpreter.RegisterExpressionHandler("list", ListLength);
            Interpreter.RegisterExpressionHandler("inlist", InList);
            Interpreter.RegisterExpressionHandler("targetexists", TargetExists);
            Interpreter.RegisterExpressionHandler("waitingfortarget", WaitingForTarget);
            Interpreter.RegisterExpressionHandler("timer", TimerValue);
            Interpreter.RegisterExpressionHandler("timerexists", TimerExists);
        }

        private static bool FindAlias(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: findalias (string)");

            uint serial = Interpreter.GetAlias(args[0].AsString());

            return serial != uint.MaxValue;
        }

        private static int Contents(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: contents (serial)");

            uint serial;

            serial = args[0].AsSerial();

            Item container = World.FindItem(serial);
            if (container == null || !container.IsContainer)
                throw new RunTimeError(null, "Serial not found or is not a container.");

            return container.ItemCount;
        }

        private static bool InRegion(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }

        private static double SkillExpression(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: skill (name)");

            var skill = ScriptManager.GetSkill(args[0].AsString());

            return skill.Value;
        }

        private static int X(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Position.X;
        }

        private static int Y(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Position.Y;
        }

        private static int Z(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Position.Z;
        }

        private static int Physical(string expression, Argument[] args, bool quiet)
        {
            return World.Player.AR;
        }

        private static int Fire(string expression, Argument[] args, bool quiet)
        {
            return World.Player.FireResistance;
        }

        private static int Cold(string expression, Argument[] args, bool quiet)
        {
            return World.Player.ColdResistance;
        }

        private static int Poison(string expression, Argument[] args, bool quiet)
        {
            return World.Player.PoisonResistance;
        }

        private static int Energy(string expression, Argument[] args, bool quiet)
        {
            return World.Player.EnergyResistance;
        }

        private static int Str(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Str;
        }

        private static int Dex(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Dex;
        }

        private static int Int(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Int;
        }

        private static int Hits(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Hits;
        }

        private static int MaxHits(string expression, Argument[] args, bool quiet)
        {
            return World.Player.HitsMax;
        }

        private static int DiffHits(string expression, Argument[] args, bool quiet)
        {
            return World.Player.HitsMax - World.Player.Hits;
        }

        private static int Stam(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Stam;
        }

        private static int MaxStam(string expression, Argument[] args, bool quiet)
        {
            return World.Player.StamMax;
        }

        private static int Mana(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Mana;
        }

        private static int MaxMana(string expression, Argument[] args, bool quiet)
        {
            return World.Player.ManaMax;
        }

        private static bool UseQueue(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Dressing(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Organizing(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int Followers(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Followers;
        }

        private static int MaxFollowers(string expression, Argument[] args, bool quiet)
        {
            return World.Player.FollowersMax;
        }

        private static uint Gold(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Gold;
        }

        private static bool Hidden(string expression, Argument[] args, bool quiet)
        {
            return !World.Player.Visible;
        }

        private static int Luck(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Luck;
        }

        private static int TithingPoints(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Tithe;
        }

        private static int Weight(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Weight;
        }

        private static int MaxWeight(string expression, Argument[] args, bool quiet)
        {
            return World.Player.MaxWeight;
        }

        private static int DiffWeight(string expression, Argument[] args, bool quiet)
        {
            return World.Player.MaxWeight - World.Player.Weight;
        }

        private static uint Serial(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int Graphic(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int Color(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int Amount(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static string Name(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Name;
        }

        private static bool Dead(string expression, Argument[] args, bool quiet)
        {
            return World.Player.IsGhost;
        }

        private static int Direction(string expression, Argument[] args, bool quiet)
        {
            return (int)World.Player.Direction;
        }

        private static bool Flying(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Paralyzed(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Poisoned(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Poisoned;
        }

        private static bool Mounted(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool YellowHits(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Criminal(string expression, Argument[] args, bool quiet)
        {
            return World.Player.CriminalTime > 0;
        }

        private static bool Enemy(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Friend(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Gray(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Innocent(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Invulnerable(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Murderer(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool FindObject(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int Distance(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool InRange(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool BuffExists(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool Property(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }

        private static bool FindType(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: findtype (graphic) [color] [source] [amount] [range or search level]");

            string graphicString = args[0].AsString();
            uint graphicId = args[0].AsUInt();

            uint? color = null;
            if (args.Length >= 2 && args[1].AsString().ToLower() != "any")
            {
                color = args[1].AsUInt();
            }

            string sourceStr = null;
            Serial source = 0;

            if (args.Length >= 3)
            {
                sourceStr = args[2].AsString().ToLower();
                if (sourceStr != "world" && sourceStr != "any" && sourceStr != "ground")
                {
                    source = args[2].AsSerial();
                }
            }

            uint? amount = null;
            if (args.Length >= 4 && args[3].AsString().ToLower() != "any")
            {
                amount = args[3].AsUInt();
            }

            uint? range = null;
            if (args.Length >= 5 && args[4].AsString().ToLower() != "any")
            {
                range = args[4].AsUInt();
            }

            List<Serial> list = new List<Serial>();

            if (args.Length < 3 || source == 0)
            {
                // No source provided or invalid. Treat as world.
                foreach (Mobile find in World.MobilesInRange())
                {
                    if (find.Body == graphicId)
                    {
                        if (color.HasValue && find.Hue != color.Value)
                        {
                            continue;
                        }

                        // This expression does not support checking if mobiles on ground or an amount of mobiles.

                        if (range.HasValue && !Utility.InRange(World.Player.Position, find.Position, (int)range.Value))
                        {
                            continue;
                        }

                        list.Add(find.Serial);
                    }
                }

                if (list.Count == 0)
                {
                    foreach (Item i in World.Items.Values)
                    {
                        if (i.ItemID == graphicId && !i.IsInBank)
                        {
                            if (color.HasValue && i.Hue != color.Value)
                            {
                                continue;
                            }

                            if (sourceStr == "ground" && !i.OnGround)
                            {
                                continue;
                            }

                            if (i.Amount < amount)
                            {
                                continue;
                            }

                            if (range.HasValue && !Utility.InRange(World.Player.Position, i.Position, (int)range.Value))
                            {
                                continue;
                            }

                            list.Add(i.Serial);
                        }
                    }
                }
            }
            else if (source != 0)
            {
                Item container = World.FindItem(source);
                if (container != null && container.IsContainer)
                {
                    // TODO need an Argument.ToUShort() in interpreter as ItemId stores ushort.
                    Item item = container.FindItemByID(new ItemID((ushort)graphicId));
                    if (item != null &&
                        (!color.HasValue || item.Hue == color.Value) &&
                        (sourceStr != "ground" || item.OnGround) &&
                        (!amount.HasValue || item.Amount >= amount) &&
                        (!range.HasValue || Utility.InRange(World.Player.Position, item.Position, (int)range.Value)))
                    {
                        list.Add(item.Serial);
                    }
                }
                else if (container == null)
                    throw new RunTimeError(null, $"Script Error: Couldn't find source '{sourceStr}'");
                else if (!container.IsContainer)
                    throw new RunTimeError(null, $"Script Error: Source '{sourceStr}' is not a container!");
            }

            if (list.Count > 0)
            {
                Serial found = list[Utility.Random(list.Count)];
                Interpreter.SetAlias("found", found);
                return true;
            }

            if (!quiet)
                World.Player?.SendMessage(MsgLevel.Warning, $"Script Error: Couldn't find '{graphicString}'");

            return false;
        }

        private static bool FindLayer(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static string SkillState(string expression, Argument[] args, bool quiet)
        {
            var skill = ScriptManager.GetSkill(args[0].AsString());

            switch (skill.Lock)
            {
                case LockType.Down:
                    return "down";
                case LockType.Up:
                    return "up";
                case LockType.Locked:
                    return "locked";
            }

            return "unknown";
        }

        private static int CountType(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static int CountTypeGround(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool FindWand(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool InParty(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool InFriendsList(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool War(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool InGump(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool GumpExists(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }

        private static bool InJournal(string expression, Argument[] args, bool quiet)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: injournal ('text') ['author'/'system']");

            if (args.Length == 1 && Journal.ContainsSafe(args[0].AsString()))
                return true;

            // TODO:
            // handle second argument

            return false;
        }

        private static bool ListExists(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: listexists ('list name')");

            if (Interpreter.ListExists(args[0].AsString()))
                return true;

            return false;
        }

        private static int ListLength(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: list ('list name') (operator) (value)");

            return Interpreter.ListLength(args[0].AsString());
        }

        private static bool InList(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: inlist ('list name')");

            if (Interpreter.ListContains(args[0].AsString(), args[1]))
                return true;

            return false;
        }

        private static bool TargetExists(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }
        private static bool WaitingForTarget(string expression, Argument[] args, bool quiet) { throw new RunTimeError(null, $"Expression {expression} not yet supported."); }

        private static int TimerValue(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: timer ('timer name')");

            var ts = Interpreter.GetTimer(args[0].AsString());

            return (int)ts.TotalMilliseconds;
        }

        private static bool TimerExists(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: timerexists ('timer name')");

            return Interpreter.TimerExists(args[0].AsString());
        }
    }
}
