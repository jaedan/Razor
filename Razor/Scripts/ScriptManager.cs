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
        private static DateTime _pauseEndTime = DateTime.MinValue;

        static ScriptManager()
        {
            _timer = new ScriptTimer();
        }

        public static void OnLogin()
        {
            Assistant.Scripts.Commands.Register();
            Assistant.Scripts.Aliases.Register();
            Assistant.Scripts.Expressions.Register();

            _timer.Start();
        }

        public static void OnLogout()
        {
            _timer.Stop();
        }

        public static bool Pause(int duration)
        {
            if (_pauseEndTime == DateTime.MinValue)
            {
                _pauseEndTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
            } else if (_pauseEndTime <= DateTime.UtcNow)
            {
                _pauseEndTime = DateTime.MinValue;
                return false;
            }

            return true;
        }

        public static void Unpause()
        {
            _pauseEndTime = DateTime.MinValue;
        }
    }
}