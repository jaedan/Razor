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
using System.ComponentModel;
using System.Linq;
using System.Text;
using UOScript;
using Assistant;

namespace Assistant.Scripts
{
    public static class Expressions
    {
        private static int DummyExpression(string expression, Argument[] args, bool quiet)
        {
            Console.WriteLine("Executing expression {0} {1}", expression, args);

            return 0;
        }

        public static void Register()
        {
            // Expressions
            Interpreter.RegisterExpressionHandler("findalias", FindAlias);
            Interpreter.RegisterExpressionHandler("contents", Contents);
            Interpreter.RegisterExpressionHandler("inregion", DummyExpression);
            Interpreter.RegisterExpressionHandler("skill", SkillExpression);
            Interpreter.RegisterExpressionHandler("x", X);
            Interpreter.RegisterExpressionHandler("y", Y);
            Interpreter.RegisterExpressionHandler("z", Z);
            Interpreter.RegisterExpressionHandler("physical", DummyExpression);
            Interpreter.RegisterExpressionHandler("fire", DummyExpression);
            Interpreter.RegisterExpressionHandler("cold", DummyExpression);
            Interpreter.RegisterExpressionHandler("poison", DummyExpression);
            Interpreter.RegisterExpressionHandler("energy", DummyExpression);
            Interpreter.RegisterExpressionHandler("str", DummyExpression);
            Interpreter.RegisterExpressionHandler("dex", DummyExpression);
            Interpreter.RegisterExpressionHandler("int", DummyExpression);
            Interpreter.RegisterExpressionHandler("hits", DummyExpression);
            Interpreter.RegisterExpressionHandler("maxhits", DummyExpression);
            Interpreter.RegisterExpressionHandler("diffhits", DummyExpression);
            Interpreter.RegisterExpressionHandler("stam", DummyExpression);
            Interpreter.RegisterExpressionHandler("maxstam", DummyExpression);
            Interpreter.RegisterExpressionHandler("mana", Mana);
            Interpreter.RegisterExpressionHandler("maxmana", DummyExpression);
            Interpreter.RegisterExpressionHandler("usequeue", DummyExpression);
            Interpreter.RegisterExpressionHandler("dressing", DummyExpression);
            Interpreter.RegisterExpressionHandler("organizing", DummyExpression);
            Interpreter.RegisterExpressionHandler("followers", DummyExpression);
            Interpreter.RegisterExpressionHandler("maxfollowers", DummyExpression);
            Interpreter.RegisterExpressionHandler("gold", DummyExpression);
            Interpreter.RegisterExpressionHandler("hidden", DummyExpression);
            Interpreter.RegisterExpressionHandler("luck", DummyExpression);
            Interpreter.RegisterExpressionHandler("tithingpoints", DummyExpression);
            Interpreter.RegisterExpressionHandler("weight", DummyExpression);
            Interpreter.RegisterExpressionHandler("maxweight", DummyExpression);
            Interpreter.RegisterExpressionHandler("diffweight", DummyExpression);
            Interpreter.RegisterExpressionHandler("serial", DummyExpression);
            Interpreter.RegisterExpressionHandler("graphic", DummyExpression);
            Interpreter.RegisterExpressionHandler("color", DummyExpression);
            Interpreter.RegisterExpressionHandler("amount", DummyExpression);
            Interpreter.RegisterExpressionHandler("name", DummyExpression);
            Interpreter.RegisterExpressionHandler("dead", DummyExpression);
            Interpreter.RegisterExpressionHandler("direction", DummyExpression);
            Interpreter.RegisterExpressionHandler("flying", DummyExpression);
            Interpreter.RegisterExpressionHandler("paralyzed", DummyExpression);
            Interpreter.RegisterExpressionHandler("poisoned", DummyExpression);
            Interpreter.RegisterExpressionHandler("mounted", DummyExpression);
            Interpreter.RegisterExpressionHandler("yellowhits", DummyExpression);
            Interpreter.RegisterExpressionHandler("criminal", DummyExpression);
            Interpreter.RegisterExpressionHandler("enemy", DummyExpression);
            Interpreter.RegisterExpressionHandler("friend", DummyExpression);
            Interpreter.RegisterExpressionHandler("gray", DummyExpression);
            Interpreter.RegisterExpressionHandler("innocent", DummyExpression);
            Interpreter.RegisterExpressionHandler("invulnerable", DummyExpression);
            Interpreter.RegisterExpressionHandler("murderer", DummyExpression);
            Interpreter.RegisterExpressionHandler("findobject", DummyExpression);
            Interpreter.RegisterExpressionHandler("distance", DummyExpression);
            Interpreter.RegisterExpressionHandler("inrange", DummyExpression);
            Interpreter.RegisterExpressionHandler("buffexists", DummyExpression);
            Interpreter.RegisterExpressionHandler("property", DummyExpression);
            Interpreter.RegisterExpressionHandler("findtype", FindType);
            Interpreter.RegisterExpressionHandler("findlayer", DummyExpression);
            Interpreter.RegisterExpressionHandler("skillstate", DummyExpression);
            Interpreter.RegisterExpressionHandler("counttype", DummyExpression);
            Interpreter.RegisterExpressionHandler("counttypeground", DummyExpression);
            Interpreter.RegisterExpressionHandler("findwand", DummyExpression);
            Interpreter.RegisterExpressionHandler("inparty", DummyExpression);
            Interpreter.RegisterExpressionHandler("infriendslist", DummyExpression);
            Interpreter.RegisterExpressionHandler("war", DummyExpression);
            Interpreter.RegisterExpressionHandler("ingump", DummyExpression);
            Interpreter.RegisterExpressionHandler("gumpexists", DummyExpression);
            Interpreter.RegisterExpressionHandler("injournal", InJournal);
            Interpreter.RegisterExpressionHandler("listexists", ListExists);
            Interpreter.RegisterExpressionHandler("list", ListLength);
            Interpreter.RegisterExpressionHandler("inlist", InList);
            Interpreter.RegisterExpressionHandler("targetexists", DummyExpression);
            Interpreter.RegisterExpressionHandler("waitingfortarget", DummyExpression);
            Interpreter.RegisterExpressionHandler("timer", TimerValue);
            Interpreter.RegisterExpressionHandler("timerexists", TimerExists);


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

        private static bool FindAlias(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: findalias (string)");

            uint serial = Interpreter.GetAlias(args[0].AsString());

            return serial != uint.MaxValue;
        }

        private static int FindType(string expression, Argument[] args, bool quiet)
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
                Interpreter.RegisterAliasHandler("found", delegate { return found; });
                return 1;
            }

            if (!quiet)
                throw new RunTimeError(null, $"Script Error: Couldn't find '{graphicString}'");

            return 0;
        }

        private static int InJournal(string expression, Argument[] args, bool quiet)
        {
            if (args.Length == 0)
                throw new RunTimeError(null, "Usage: injournal ('text') ['author'/'system']");

            if (args.Length == 1 && Journal.ContainsSafe(args[0].AsString()))
                return 1;

            // TODO:
            // handle second argument

            return 0;
        }

        private static int ListExists(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: listexists ('list name')");

            if (Interpreter.ListExists(args[0].AsString()))
                return 1;

            return 0;
        }

        private static int ListLength(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: list ('list name') (operator) (value)");

            return Interpreter.ListLength(args[0].AsString());
        }

        private static int InList(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
                throw new RunTimeError(null, "Usage: inlist ('list name')");

            if (Interpreter.ListContains(args[0].AsString(), args[1]))
                return 1;

            return 0;
        }

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

        private static int Mana(string expression, Argument[] args, bool quiet)
        {
            return World.Player.Mana;
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

        private static double SkillExpression(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new RunTimeError(null, "Usage: skill (name)");

            var skill = ScriptManager.GetSkill(args[0].AsString());

            return skill.Value;
        }
    }
}
