using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UOSteam;

namespace Assistant.Scripts
{
    public static class ScriptUtilities
    {
        public static List<ASTNode> ParseArguments(ref ASTNode node)
        {
            List<ASTNode> args = new List<ASTNode>();
            while (node != null)
            {
                args.Add(node);
                node = node.Next();
            }
            return args;
        }

        public static Serial GetSerial(ref ASTNode target)
        {
            Serial targetSerial = Serial.MinusOne;
            if (target.Type == ASTNodeType.STRING)
                targetSerial = (uint)Interpreter.GetAlias(ref target);
            else if (target.Type == ASTNodeType.SERIAL)
                targetSerial = Utility.ToUInt32(target.Lexeme, Serial.MinusOne);

            return targetSerial;
        }

        public static void ScriptErrorMsg(string message, string scriptname = "")
        {
            World.Player?.SendMessage(MsgLevel.Error, $"Script {scriptname} error => {message}");
        }
    }
}
