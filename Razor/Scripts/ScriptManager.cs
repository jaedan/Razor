using System;
using UOSteam;

namespace Assistant.Scripts
{
    public static class ScriptManager
    {
        private class ScriptTimer : Timer
        {
            // Only run scripts once every 25ms to avoid spamming.
            public ScriptTimer() : base(TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(25))
            {
            }

            protected override void OnTick()
            {
                Interpreter.ExecuteScripts();
            }
        }

        private static ScriptTimer _timer;

        static ScriptManager()
        {
            _timer = new ScriptTimer();
        }

        public static void OnLogin()
        {
            _timer.Start();
        }

        public static void OnLogout()
        {
            _timer.Stop();
        }
    }
}