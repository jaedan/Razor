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
            public string FilePath { get;}
            public string Name { get; }
            public string[] Lines { get; set; }

            public ScriptSource(string path)
            {
                if (!path.EndsWith(".uos"))
                    throw new Exception("Invalid file name");

                var fname = Path.GetFileName(path);

                FilePath = path;
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

        public static void Save(string name)
        {
            var dirPath = Config.GetUserDirectory("Scripts");

            Engine.EnsureDirectory(dirPath);

            if (_scripts.TryGetValue(name, out ScriptSource ss))
            {
                File.WriteAllLines(Path.Combine(dirPath, $"{ss.Name}.uos"), ss.Lines);
            }
        }

        public static void New(string name)
        {
            var dirPath = Config.GetUserDirectory("Scripts");

            Engine.EnsureDirectory(dirPath);

            var path = Path.Combine(dirPath, $"{name}.uos");

            if (!File.Exists(path))
                File.Create(path).Close();
            else
            {
                MessageBox.Show($"A script with that name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ScriptSource ss = new ScriptSource(path);

            _scripts.Add(name, ss);
        }

        public static string[] Load(string name)
        {
            return _scripts[name].Lines;
        }

        public static void Update(string name, string[] lines)
        {
            if (_scripts.TryGetValue(name, out ScriptSource ss))
                ss.Lines = lines;
        }

        public static void Delete(string name)
        {
            if (_scripts.TryGetValue(name, out ScriptSource ss))
            {
                File.Delete(ss.FilePath);
                _scripts.Remove(name);
            }
        }

        public static void Populate(ListBox list)
        {
            list.Items.Clear();
            foreach (var ss in _scripts.Values)
                list.Items.Add(ss.Name);
        }
    }
}