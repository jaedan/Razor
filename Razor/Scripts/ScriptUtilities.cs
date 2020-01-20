using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UOSteam;

namespace Assistant.Scripts
{
    public static class ScriptUtilities
    {
        public static void ScriptErrorMsg(string message, string scriptname = "")
        {
            World.Player?.SendMessage(MsgLevel.Error, $"Script {scriptname} error => {message}");
        }
    }
}
