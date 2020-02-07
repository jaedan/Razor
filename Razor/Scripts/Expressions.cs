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
            Interpreter.RegisterExpressionHandler("findtype", DummyExpression);
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

        private static int InJournal(string expression, Argument[] args, bool quiet)
        {
            if (args.Length == 0)
            {
                ScriptUtilities.ScriptErrorMsg("Usage: injournal ('text') ['author'/'system']");
                return 0;
            }

            if (args.Length == 1 && Journal.Contains(args[0].AsString()))
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
