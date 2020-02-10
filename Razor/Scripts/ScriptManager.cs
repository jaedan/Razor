using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UOSteam;

namespace Assistant.Scripts
{
    public static class ScriptManager
    {
        private class ScriptSource
        {
            public string Name { get; }
            public string[] Lines { get; set; }

            public ScriptSource(string path)
            {
                if (!path.EndsWith(".uos"))
                    throw new Exception("Invalid file name");

                var fname = Path.GetFileName(path);

                Name = fname.Substring(0, fname.Length - 4);
                Lines = File.ReadLines(path).ToArray();
            }
        }

        private static Dictionary<string, ScriptSource> _scripts = new Dictionary<string, ScriptSource>();

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

        public static void Initialize()
        {
            _timer = new ScriptTimer();

            string path = Config.GetUserDirectory("Scripts");

            foreach (var fname in Directory.GetFiles(path, "*.uos"))
            {
                try
                {
                    ScriptSource ss = new ScriptSource(fname);

                    _scripts[ss.Name] = ss;
                }
                catch
                {
                }
            }

            Assistant.Scripts.Commands.Register();
            Assistant.Scripts.Aliases.Register();
            Assistant.Scripts.Expressions.Register();
        }

        public static void OnLogin()
        {
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

        public static void Save(string name, string[] lines)
        {
            var dirPath = Config.GetUserDirectory("Scripts");

            Engine.EnsureDirectory(dirPath);

            foreach (var ss in _scripts.Values)
            {
                var path = Path.Combine(dirPath, ss.Name, ".uos");

                File.WriteAllLines(path, ss.Lines);
            }
        }

        public static string[] Load(string name)
        {
            return _scripts[name].Lines;
        }

        public static void Populate(ListBox list)
        {
            foreach (var ss in _scripts.Values)
            {
                list.Items.Add(ss.Name);
            }
        }
    }
}