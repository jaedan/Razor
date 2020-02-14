using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UOSteam;
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
            Interpreter.RegisterExpressionHandler("contents", DummyExpression);
            Interpreter.RegisterExpressionHandler("inregion", DummyExpression);
            Interpreter.RegisterExpressionHandler("skill", SkillExpression);
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
            Interpreter.RegisterExpressionHandler("timer", DummyExpression);
            Interpreter.RegisterExpressionHandler("timerexists", DummyExpression);

            // Player Attributes
            Interpreter.RegisterExpressionHandler("mana", Mana);
            Interpreter.RegisterExpressionHandler("x", X);
            Interpreter.RegisterExpressionHandler("y", Y);
            Interpreter.RegisterExpressionHandler("z", Z);
        }

        private static int FindAlias(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                ScriptUtilities.ScriptErrorMsg("Usage: findalias (string)");

            uint serial = Interpreter.GetAlias(args[0].AsString());

            if (serial == uint.MaxValue)
                return 0;

            return 1;
        }

        private static int FindType(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: findtype (graphic) [color] [source] [amount] [range or search level]");
            }

            string graphicString = args[0].AsString();
            uint graphicId;
            try
            {
                graphicId = args[0].AsUInt();
            }
            catch (RunTimeError)
            {
                ScriptUtilities.ScriptErrorMsg("Invalid: graphic id");
                return 0;
            }

            uint? color = null;
            if (args.Length >= 2 && args[1].AsString().ToLower() != "any")
            {
                try
                {
                    color = args[1].AsUInt();
                }
                catch (RunTimeError)
                {
                    ScriptUtilities.ScriptErrorMsg("Invalid: color 'any' if unsure.");
                    return 0;
                }
            }

            string sourceStr = null;
            Serial source = 0;

            if (args.Length >= 3)
            {
                sourceStr = args[2].AsString().ToLower();
                if (sourceStr != "world" && sourceStr != "any" && sourceStr != "ground")
                {
                    try
                    {
                        source = args[2].AsSerial();
                    }
                    catch (RunTimeError)
                    {
                        ScriptUtilities.ScriptErrorMsg("Invalid: source id (serial).");
                        return 0;
                    }
                }
            }

            uint? amount = null;
            if (args.Length >= 4 && args[3].AsString().ToLower() != "any")
            {
                try
                {
                    amount = args[3].AsUInt();
                }
                catch (RunTimeError)
                {
                    ScriptUtilities.ScriptErrorMsg("Invalid: amount 'any' if unsure.");
                    return 0;
                }
            }

            uint? range = null;
            if (args.Length >= 5 && args[4].AsString().ToLower() != "any")
            {
                try
                {
                    range = args[4].AsUInt();
                }
                catch (RunTimeError)
                {
                    ScriptUtilities.ScriptErrorMsg("Invalid: range 'any' or remove if unsure.");
                    return 0;
                }
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
                {
                    ScriptUtilities.ScriptErrorMsg($"Script Error: Couldn't find source '{sourceStr}'");
                }
                else if (!container.IsContainer)
                {
                    ScriptUtilities.ScriptErrorMsg($"Script Error: Source '{sourceStr}' is not a container!");
                }
            }

            if (list.Count > 0)
            {
                Serial found = list[Utility.Random(list.Count)];
                Interpreter.RegisterAliasHandler("found", delegate { return found; });
                return 1;
            }

            if (!quiet)
            {
                ScriptUtilities.ScriptErrorMsg($"Script Error: Couldn't find '{graphicString}'");
            }

            return 0;
        }

        private static int InJournal(string expression, Argument[] args, bool quiet)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: injournal ('text') ['author'/'system']");
                return 0;
            }

            if (args.Length == 1 && Journal.ContainsSafe(args[0].AsString()))
                return 1;

            // TODO:
            // handle second argument

            return 0;
        }

        private static int ListExists(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: listexists ('list name')");
                return 0;
            }

            if (Interpreter.ListExists(args[0].AsString()))
                return 1;

            return 0;
        }

        private static int ListLength(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: list ('list name') (operator) (value)");
                return 0;
            }

            return Interpreter.ListLength(args[0].AsString());
        }

        private static int InList(string expression, Argument[] args, bool quiet)
        {
            if (args.Length != 1)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: inlist ('list name')");
                return 0;
            }

            if (Interpreter.ListContains(args[0].AsString(), args[1]))
                return 1;

            return 0;
        }

        private static int Mana(string expression, Argument[] args, bool quiet)
        {
            if (World.Player == null)
                return 0;

            return World.Player.Mana;
        }

        private static int X(string expression, Argument[] args, bool quiet)
        {
            if (World.Player == null)
                return 0;

            return World.Player.Position.X;
        }

        private static int Y(string expression, Argument[] args, bool quiet)
        {
            if (World.Player == null)
                return 0;

            return World.Player.Position.Y;
        }

        private static int Z(string expression, Argument[] args, bool quiet)
        {
            if (World.Player == null)
                return 0;

            return World.Player.Position.Z;
        }

        // WIP
        private static int SkillExpression(string expression, Argument[] args, bool quiet)
        {
            if (args.Length < 1)
                throw new ArgumentException("Usage: skill (name)");

            if (World.Player == null)
                return 0;

            return 0;
        }
    }
}
