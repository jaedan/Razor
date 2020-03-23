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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Assistant.Macros;
using UOScript;
using Assistant.UI;
using FastColoredTextBoxNS;

namespace Assistant.Scripts
{
    public static class ScriptManager
    {
        public static bool Recording { get; set; }

        private static Script _queuedScript = null;

        public static bool Running { get; set; }

        public static DateTime LastMove { get; set; }

        public static bool SetLastTargetActive { get; set; }

        public static bool SetVariableActive { get; set; }

        public static string ScriptPath => Config.GetUserDirectory("Scripts");

        private static FastColoredTextBox ScriptEditor { get; set; }

        private static ListBox ScriptList { get; set; }

        private class ScriptTimer : Timer
        {
            // Only run scripts once every 25ms to avoid spamming.
            public ScriptTimer() : base(TimeSpan.FromMilliseconds(25), TimeSpan.FromMilliseconds(25))
            {
            }

            protected override void OnTick()
            {
                try
                {
                    if (!Client.Instance.ClientRunning || World.Player == null)
                    {
                        if (ScriptManager.Running)
                        {
                            ScriptManager.Running = false;
                            Interpreter.StopScript();
                        }
                        return;
                    }

                    bool running;

                    if (_queuedScript != null)
                    {
                        // Starting a new script. This relies on the atomicity for references in CLR
                        var script = _queuedScript;

                        running = Interpreter.StartScript(script);

                        _queuedScript = null;
                    }
                    else
                    {
                        running = Interpreter.ExecuteScript();
                    }

                    if (running)
                    {
                        if (ScriptManager.Running == false)
                        {
                            World.Player?.SendMessage(LocString.ScriptPlaying);
                            Assistant.Engine.MainWindow.LockScriptUI(true);
                            ScriptManager.Running = true;
                        }
                    }
                    else
                    {
                        if (ScriptManager.Running)
                        {
                            World.Player?.SendMessage(LocString.ScriptFinished);
                            Assistant.Engine.MainWindow.LockScriptUI(false);
                            ScriptManager.Running = false;
                        }
                    }
                }
                catch(RunTimeError ex)
                {
                    if (ex.Node != null)
                        World.Player?.SendMessage(MsgLevel.Error, string.Format("Script Error On Line {0}: {1}", ex.Node.LineNumber, ex.Message));
                    else
                        World.Player?.SendMessage(MsgLevel.Error, string.Format("Script Error: {0}", ex.Message));
                    Interpreter.StopScript();
                }
                catch (Exception ex)
                {
                    World.Player?.SendMessage(MsgLevel.Error, ex.Message);
                    Interpreter.StopScript();
                }
            }
        }

        private static readonly HotKeyCallbackState HotkeyCallback = OnHotKey;

        /// <summary>
        /// This is called via reflection when the application starts up
        /// </summary>
        public static void Initialize()
        {
            HotKey.Add(HKCategory.Scripts, LocString.StopScript, new HotKeyCallback(HotkeyStopScript));

            Scripts = new List<SteamScript>();

            LoadScripts();

            foreach (SteamScript script in Scripts)
            {
                AddHotkey(script.Name);
            }
        }

        private static void HotkeyStopScript()
        {
            StopScript();
        }

        public static void AddHotkey(string script)
        {
            HotKey.Add(HKCategory.Scripts, HKSubCat.None, Language.Format(LocString.PlayScript, script), HotkeyCallback,
                script);
        }

        public static void RemoveHotkey(string script)
        {
            HotKey.Remove(Language.Format(LocString.PlayScript, script));
        }

        public static void OnHotKey(ref object state)
        {
            PlayScript((string) state);
        }

        public static void StopScript()
        {
            Running = false;
            Interpreter.StopScript();
        }

        public static void PlayScript(string scriptName)
        {
            foreach (SteamScript script in Scripts)
            {
                if (script.Name.Equals(scriptName, StringComparison.OrdinalIgnoreCase))
                {
                    PlayScript(script.Lines);
                    break;
                }
            }
        }

        public static void PlayScript(string[] lines)
        {
            if (World.Player == null || ScriptEditor == null || lines == null)
                return;

            if (MacroManager.Playing || MacroManager.StepThrough)
                MacroManager.Stop();

            StopScript(); // be sure nothing is running

            SetLastTargetActive = false;
            SetVariableActive = false;

            if (_queuedScript != null)
                return;

            if (!Client.Instance.ClientRunning)
                return;

            if (World.Player == null)
                return;

            Script script = new Script(Lexer.Lex(lines));

            _queuedScript = script;
        }

        public static void Error(string statement, string message)
        {
            World.Player?.SendMessage(MsgLevel.Error, $"{statement}: {message}");
        }

        private static ScriptTimer Timer { get; }

        static ScriptManager()
        {
            Timer = new ScriptTimer();
        }

        public static void SetControls(FastColoredTextBox scriptEditor, ListBox scriptList)
        {
            ScriptEditor = scriptEditor;
            ScriptList = scriptList;
        }

        public static void OnLogin()
        {
            Commands.Register();
            AgentCommands.Register();
            Aliases.Register();
            Expressions.Register();

            Timer.Start();
        }

        public static void OnLogout()
        {
            Timer.Stop();
        }

        public static void StartEngine()
        {
            Timer.Start();
        }

        public static void StopEngine()
        {
            Timer.Stop();
        }

        public class SteamScript
        {
            public string Path { get; set; }
            public string[] Lines { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public static List<SteamScript> Scripts { get; set; }

        public static void LoadScripts()
        {
            Scripts.Clear();

            foreach (string file in Directory.GetFiles(ScriptPath, "*.uos"))
            {
                Scripts.Add(new SteamScript
                {
                    Lines = File.ReadAllLines(file),
                    Name = Path.GetFileNameWithoutExtension(file),
                    Path = file
                });
            }
        }

        public static void DisplayScriptVariables(ListBox list)
        {
            list.SafeAction(s =>
            {
                s.BeginUpdate();
                s.Items.Clear();

                foreach (ScriptVariables.ScriptVariable at in ScriptVariables.ScriptVariableList)
                {
                    s.Items.Add($"'{at.Name}' ({at.TargetInfo.Serial})");
                }

                s.EndUpdate();
                s.Refresh();
                s.Update();
            });
        }

        public static bool AddToScript(string command)
        {
            if (Recording)
            {
                ScriptEditor?.AppendText(command + Environment.NewLine);

                return true;
            }

            return false;
        }

        private delegate void SetHighlightLineDelegate(int iline, Color color);

        private static void SetHighlightLine(int iline, Color background)
        {
            for (int i = 0; i < ScriptEditor.LinesCount; i++)
            {
                ScriptEditor[i].BackgroundBrush = ScriptEditor.BackBrush;
            }

            ScriptEditor[iline].BackgroundBrush = new SolidBrush(background);
            ScriptEditor.Invalidate();
        }

        private static AutocompleteMenu _autoCompleteMenu;

        private static string BuildToolTip(string signature, string description,
                                           string example)
        {
            string tooltip = string.Empty;

            tooltip += "Signature: ";

            tooltip += "\n    " + signature;

            tooltip += "\nDescription:";

            tooltip += "\n    " + description;

            tooltip += "\nExample:";

            tooltip += "\n    " + example;

            return tooltip;
        }

        public static void InitScriptEditor()
        {
            _autoCompleteMenu = new AutocompleteMenu(ScriptEditor);
            _autoCompleteMenu.SearchPattern = @"[\w\.:=!<>]";
            _autoCompleteMenu.AllowTabKey = true;
            _autoCompleteMenu.ToolTipDuration = 5000;
            _autoCompleteMenu.AppearInterval = 100;

            var autocompletes = new List<AutocompleteItem>()
            {
                new AutocompleteItem("if"),
                new AutocompleteItem("elseif"),
                new AutocompleteItem("else"),
                new AutocompleteItem("endif"),
                new AutocompleteItem("while"),
                new AutocompleteItem("endwhile"),
                new AutocompleteItem("for"),
                new AutocompleteItem("foreach"),
                new AutocompleteItem("in"),
                new AutocompleteItem("endfor"),
                new AutocompleteItem("break"),
                new AutocompleteItem("continue"),
                new AutocompleteItem("stop"),
                new AutocompleteItem("replay"),
                new AutocompleteItem("loop"),
                new AutocompleteItem("not"),
                new AutocompleteItem("and"),
                new AutocompleteItem("or"),
                new AutocompleteItem("fly", 2, "", "fly", BuildToolTip("fly", "Start or stop flying.", "fly")),
                new AutocompleteItem("land", 2, "", "", ""),
                new AutocompleteItem("setability", 2, "", "", ""),
                new AutocompleteItem("attack", 2, "", "", ""),
                new AutocompleteItem("clearhands", 2, "", "", ""),
                new AutocompleteItem("clickobject", 2, "", "", ""),
                new AutocompleteItem("bandageself", 2, "", "", ""),
                new AutocompleteItem("usetype", 2, "", "", ""),
                new AutocompleteItem("useobject", 2, "", "", ""),
                new AutocompleteItem("useonce", 2, "", "", ""),
                new AutocompleteItem("clearuseonce", 2, "", "", ""),
                new AutocompleteItem("moveitem", 2, "", "", ""),
                new AutocompleteItem("moveitemoffset", 2, "", "", ""),
                new AutocompleteItem("movetype", 2, "", "", ""),
                new AutocompleteItem("movetypeoffset", 2, "", "", ""),
                new AutocompleteItem("walk", 2, "", "", ""),
                new AutocompleteItem("turn", 2, "", "", ""),
                new AutocompleteItem("run", 2, "", "", ""),
                new AutocompleteItem("useskill", 2, "", "", ""),
                new AutocompleteItem("feed", 2, "", "", ""),
                new AutocompleteItem("rename", 2, "", "", ""),
                new AutocompleteItem("shownames", 2, "", "", ""),
                new AutocompleteItem("togglehands", 2, "", "", ""),
                new AutocompleteItem("equipitem", 2, "", "", ""),
                new AutocompleteItem("togglemounted", 2, "", "", ""),
                new AutocompleteItem("equipwand", 2, "", "", ""),
                new AutocompleteItem("buy", 2, "", "", ""),
                new AutocompleteItem("sell", 2, "", "", ""),
                new AutocompleteItem("clearbuy", 2, "", "", ""),
                new AutocompleteItem("clearsell", 2, "", "", ""),
                new AutocompleteItem("organizer", 2, "", "", ""),
                new AutocompleteItem("autoloot", 2, "", "", ""),
                new AutocompleteItem("dress", 2, "", "", ""),
                new AutocompleteItem("undress", 2, "", "", ""),
                new AutocompleteItem("dressconfig", 2, "", "", ""),
                new AutocompleteItem("toggleautoloot", 2, "", "", ""),
                new AutocompleteItem("togglescavenger", 2, "", "", ""),
                new AutocompleteItem("counter", 2, "", "", ""),
                new AutocompleteItem("unsetalias", 2, "", "", ""),
                new AutocompleteItem("setalias", 2, "", "", ""),
                new AutocompleteItem("promptalias", 2, "", "", ""),
                new AutocompleteItem("waitforgump", 2, "", "", ""),
                new AutocompleteItem("replygump", 2, "", "", ""),
                new AutocompleteItem("closegump", 2, "", "", ""),
                new AutocompleteItem("clearjournal", 2, "", "", ""),
                new AutocompleteItem("waitforjournal", 2, "", "", ""),
                new AutocompleteItem("poplist", 2, "", "", ""),
                new AutocompleteItem("pushlist", 2, "", "", ""),
                new AutocompleteItem("removelist", 2, "", "", ""),
                new AutocompleteItem("createlist", 2, "", "", ""),
                new AutocompleteItem("clearlist", 2, "", "", ""),
                new AutocompleteItem("info", 2, "", "", ""),
                new AutocompleteItem("pause", 2, "", "", ""),
                new AutocompleteItem("ping", 2, "", "", ""),
                new AutocompleteItem("playmacro", 2, "", "", ""),
                new AutocompleteItem("playsound", 2, "", "", ""),
                new AutocompleteItem("resync", 2, "", "", ""),
                new AutocompleteItem("snapshot", 2, "", "", ""),
                new AutocompleteItem("hotkeys", 2, "", "", ""),
                new AutocompleteItem("where", 2, "", "", ""),
                new AutocompleteItem("messagebox", 2, "", "", ""),
                new AutocompleteItem("mapuo", 2, "", "", ""),
                new AutocompleteItem("clickscreen", 2, "", "", ""),
                new AutocompleteItem("paperdoll", 2, "", "", ""),
                new AutocompleteItem("helpbutton", 2, "", "", ""),
                new AutocompleteItem("guildbutton", 2, "", "", ""),
                new AutocompleteItem("questsbutton", 2, "", "", ""),
                new AutocompleteItem("logoutbutton", 2, "", "", ""),
                new AutocompleteItem("virtue", 2, "", "", ""),
                new AutocompleteItem("msg", 2, "", "", ""),
                new AutocompleteItem("headmsg", 2, "", "", ""),
                new AutocompleteItem("partymsg", 2, "", "", ""),
                new AutocompleteItem("guildmsg", 2, "", "", ""),
                new AutocompleteItem("allymsg", 2, "", "", ""),
                new AutocompleteItem("whispermsg", 2, "", "", ""),
                new AutocompleteItem("yellmsg", 2, "", "", ""),
                new AutocompleteItem("sysmsg", 2, "", "", ""),
                new AutocompleteItem("chatmsg", 2, "", "", ""),
                new AutocompleteItem("emotemsg", 2, "", "", ""),
                new AutocompleteItem("promptmsg", 2, "", "", ""),
                new AutocompleteItem("timermsg", 2, "", "", ""),
                new AutocompleteItem("waitforprompt", 2, "", "", ""),
                new AutocompleteItem("cancelprompt", 2, "", "", ""),
                new AutocompleteItem("addfriend", 2, "", "", ""),
                new AutocompleteItem("removefriend", 2, "", "", ""),
                new AutocompleteItem("contextmenu", 2, "", "", ""),
                new AutocompleteItem("waitforcontext", 2, "", "", ""),
                new AutocompleteItem("ignoreobject", 2, "", "", ""),
                new AutocompleteItem("clearignorelist", 2, "", "", ""),
                new AutocompleteItem("setskill", 2, "", "", ""),
                new AutocompleteItem("waitforproperties", 2, "", "", ""),
                new AutocompleteItem("autocolorpick", 2, "", "", ""),
                new AutocompleteItem("waitforcontents", 2, "", "", ""),
                new AutocompleteItem("miniheal", 2, "", "", ""),
                new AutocompleteItem("bigheal", 2, "", "", ""),
                new AutocompleteItem("cast", 2, "", "", ""),
                new AutocompleteItem("chivalryheal", 2, "", "", ""),
                new AutocompleteItem("waitfortarget", 2, "", "", ""),
                new AutocompleteItem("canceltarget", 2, "", "", ""),
                new AutocompleteItem("target", 2, "", "", ""),
                new AutocompleteItem("targettype", 2, "", "", ""),
                new AutocompleteItem("targetground", 2, "", "", ""),
                new AutocompleteItem("targettile", 2, "", "", ""),
                new AutocompleteItem("targettileoffset", 2, "", "", ""),
                new AutocompleteItem("targettilerelative", 2, "", "", ""),
                new AutocompleteItem("cleartargetqueue", 2, "", "", ""),
                new AutocompleteItem("autotargetlast", 2, "", "", ""),
                new AutocompleteItem("autotargetself", 2, "", "", ""),
                new AutocompleteItem("autotargetobject", 2, "", "", ""),
                new AutocompleteItem("autotargettype", 2, "", "", ""),
                new AutocompleteItem("autotargettile", 2, "", "", ""),
                new AutocompleteItem("autotargettileoffset", 2, "", "", ""),
                new AutocompleteItem("autotargettilerelative", 2, "", "", ""),
                new AutocompleteItem("autotargetghost", 2, "", "", ""),
                new AutocompleteItem("autotargetground", 2, "", "", ""),
                new AutocompleteItem("cancelautotarget", 2, "", "", ""),
                new AutocompleteItem("getenemy", 2, "", "", ""),
                new AutocompleteItem("getfriend", 2, "", "", ""),
                new AutocompleteItem("settimer", 2, "", "", ""),
                new AutocompleteItem("removetimer", 2, "", "", ""),
                new AutocompleteItem("createtimer", 2, "", "", ""),
                new AutocompleteItem("findalias", 2, "", "", ""),
                new AutocompleteItem("contents", 2, "", "", ""),
                new AutocompleteItem("inregion", 2, "", "", ""),
                new AutocompleteItem("skill", 2, "", "", ""),
                new AutocompleteItem("x", 2, "", "", ""),
                new AutocompleteItem("y", 2, "", "", ""),
                new AutocompleteItem("z", 2, "", "", ""),
                new AutocompleteItem("physical", 2, "", "", ""),
                new AutocompleteItem("fire", 2, "", "", ""),
                new AutocompleteItem("cold", 2, "", "", ""),
                new AutocompleteItem("poison", 2, "", "", ""),
                new AutocompleteItem("energy", 2, "", "", ""),
                new AutocompleteItem("str", 2, "", "", ""),
                new AutocompleteItem("dex", 2, "", "", ""),
                new AutocompleteItem("int", 2, "", "", ""),
                new AutocompleteItem("hits", 2, "", "", ""),
                new AutocompleteItem("maxhits", 2, "", "", ""),
                new AutocompleteItem("diffhits", 2, "", "", ""),
                new AutocompleteItem("stam", 2, "", "", ""),
                new AutocompleteItem("maxstam", 2, "", "", ""),
                new AutocompleteItem("mana", 2, "", "", ""),
                new AutocompleteItem("maxmana", 2, "", "", ""),
                new AutocompleteItem("usequeue", 2, "", "", ""),
                new AutocompleteItem("dressing", 2, "", "", ""),
                new AutocompleteItem("organizing", 2, "", "", ""),
                new AutocompleteItem("followers", 2, "", "", ""),
                new AutocompleteItem("maxfollowers", 2, "", "", ""),
                new AutocompleteItem("gold", 2, "", "", ""),
                new AutocompleteItem("hidden", 2, "", "", ""),
                new AutocompleteItem("luck", 2, "", "", ""),
                new AutocompleteItem("tithingpoints", 2, "", "", ""),
                new AutocompleteItem("weight", 2, "", "", ""),
                new AutocompleteItem("maxweight", 2, "", "", ""),
                new AutocompleteItem("diffweight", 2, "", "", ""),
                new AutocompleteItem("serial", 2, "", "", ""),
                new AutocompleteItem("graphic", 2, "", "", ""),
                new AutocompleteItem("color", 2, "", "", ""),
                new AutocompleteItem("amount", 2, "", "", ""),
                new AutocompleteItem("name", 2, "", "", ""),
                new AutocompleteItem("dead", 2, "", "", ""),
                new AutocompleteItem("direction", 2, "", "", ""),
                new AutocompleteItem("flying", 2, "", "", ""),
                new AutocompleteItem("paralyzed", 2, "", "", ""),
                new AutocompleteItem("poisoned", 2, "", "", ""),
                new AutocompleteItem("mounted", 2, "", "", ""),
                new AutocompleteItem("yellowhits", 2, "", "", ""),
                new AutocompleteItem("criminal", 2, "", "", ""),
                new AutocompleteItem("enemy", 2, "", "", ""),
                new AutocompleteItem("friend", 2, "", "", ""),
                new AutocompleteItem("gray", 2, "", "", ""),
                new AutocompleteItem("innocent", 2, "", "", ""),
                new AutocompleteItem("invulnerable", 2, "", "", ""),
                new AutocompleteItem("murderer", 2, "", "", ""),
                new AutocompleteItem("findobject", 2, "", "", ""),
                new AutocompleteItem("distance", 2, "", "", ""),
                new AutocompleteItem("inrange", 2, "", "", ""),
                new AutocompleteItem("buffexists", 2, "", "", ""),
                new AutocompleteItem("property", 2, "", "", ""),
                new AutocompleteItem("findtype", 2, "", "", ""),
                new AutocompleteItem("findlayer", 2, "", "", ""),
                new AutocompleteItem("skillstate", 2, "", "", ""),
                new AutocompleteItem("counttype", 2, "", "", ""),
                new AutocompleteItem("counttypeground", 2, "", "", ""),
                new AutocompleteItem("findwand", 2, "", "", ""),
                new AutocompleteItem("inparty", 2, "", "", ""),
                new AutocompleteItem("infriendslist", 2, "", "", ""),
                new AutocompleteItem("war", 2, "", "", ""),
                new AutocompleteItem("ingump", 2, "", "", ""),
                new AutocompleteItem("gumpexists", 2, "", "", ""),
                new AutocompleteItem("injournal", 2, "", "", ""),
                new AutocompleteItem("listexists", 2, "", "", ""),
                new AutocompleteItem("list", 2, "", "", ""),
                new AutocompleteItem("inlist", 2, "", "", ""),
                new AutocompleteItem("targetexists", 2, "", "", ""),
                new AutocompleteItem("waitingfortarget", 2, "", "", ""),
                new AutocompleteItem("timer", 2, "", "", ""),
                new AutocompleteItem("timerexists", 2, "", "", ""),
            };

            _autoCompleteMenu.Items.SetAutocompleteItems(autocompletes);
            _autoCompleteMenu.Items.MaximumSize =
                new Size(_autoCompleteMenu.Items.Width + 20, _autoCompleteMenu.Items.Height);
            _autoCompleteMenu.Items.Width = _autoCompleteMenu.Items.Width + 20;

            ScriptEditor.Language = FastColoredTextBoxNS.Language.Custom;
        }

        public static void RedrawScripts()
        {
            ScriptList.SafeAction(s =>
            {
                int curIndex = 0;

                if (s.SelectedIndex > -1)
                    curIndex = s.SelectedIndex;

                s.BeginUpdate();
                s.Items.Clear();

                LoadScripts();

                foreach (SteamScript script in Scripts)
                {
                    if (script != null)
                        s.Items.Add(script);
                }

                if (s.Items.Count > 0 && (curIndex - 1 != -1))
                    s.SelectedIndex = curIndex - 1;

                s.EndUpdate();
            });
        }

        private static Dictionary<string, int> _skillMap = new Dictionary<string, int>()
        {
            { "alchemy", 0 },
            { "anatomy", 1 },
            { "animallore", 2 }, { "animal lore", 2 },
            { "itemidentification", 3 }, {"itemid", 3 }, { "item identification", 3 }, { "item id", 3 },
            { "armslore", 4 }, { "arms lore", 4 },
            { "parry", 5 }, { "parrying", 5 },
            { "begging", 6 },
            { "blacksmith", 7 }, { "blacksmithing", 7 },
            { "fletching", 8 }, { "bowcraft", 8 },
            { "peacemaking", 9 }, { "peace", 9 }, { "peacemake", 9 },
            { "camping", 10 }, { "camp", 10 },
            { "carpentry", 11 },
            { "cartography", 12 },
            { "cooking", 13 }, { "cook", 13 },
            { "detectinghidden", 14 }, { "detect", 14 }, { "detecthidden", 14 }, { "detecting hidden", 14 }, { "detect hidden", 14 },
            { "discordance", 15 }, { "discord", 15 }, { "enticement", 15 }, { "entice", 15 },
            { "evaluatingintelligence", 16 }, { "evalint", 16 }, { "eval", 16 }, { "evaluating intelligence", 16 },
            { "healing", 17 },
            { "fishing", 18 },
            { "forensicevaluation", 19 }, { "forensiceval", 19 }, { "forensics", 19 },
            { "herding", 20 },
            { "hiding", 21 },
            { "provocation", 22 }, { "provo", 22 },
            { "inscription", 23 }, { "scribe", 23 },
            { "lockpicking", 24 },
            { "magery", 25 }, { "mage", 25 },
            { "magicresist", 26 }, { "resist", 26 }, { "resistingspells", 26 },
            { "tactics", 27 },
            { "snooping", 28 }, { "snoop", 28 },
            { "musicianship", 29 }, { "music", 29 },
            { "poisoning", 30 },
            { "archery", 31 },
            { "spiritspeak", 32 },
            { "stealing", 33 },
            { "tailoring", 34 },
            { "taming", 35 }, { "animaltaming", 35 }, { "animal taming", 35 },
            { "tasteidentification", 36 }, { "tasteid", 36 },
            { "tinkering", 37 },
            { "tracking", 38 },
            { "veterinary", 39 }, { "vet", 39 },
            { "swords", 40 }, { "swordsmanship", 40 },
            { "macing", 41 }, { "macefighting", 41 }, { "mace fighting", 41 },
            { "fencing", 42 },
            { "wrestling", 43 },
            { "lumberjacking", 44 },
            { "mining", 45 },
            { "meditation", 46 },
            { "stealth", 47 },
            { "removetrap", 48 },
            { "necromancy", 49 }, { "necro", 49 },
            { "focus", 50 },
            { "chivalry", 51 },
            { "bushido", 52 },
            { "ninjitsu", 53 },
            { "spellweaving", 54 },
        };

        // Convert steam-compatible skill names to Skills
        public static Skill GetSkill(string skillName)
        {
            if (_skillMap.TryGetValue(skillName.ToLower(), out var id))
                return World.Player.Skills[id];

            throw new RunTimeError(null, $"Unknown skill name: {skillName}");
        }
    }
}