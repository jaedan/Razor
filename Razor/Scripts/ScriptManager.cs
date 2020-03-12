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

        public static bool Running => ScriptRunning;

        private static bool ScriptRunning { get; set; }

        public static DateTime LastWalk { get; set; }

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
                    if (Interpreter.ExecuteScript())
                    {
                        if (ScriptRunning == false)
                        {
                            World.Player?.SendMessage(LocString.ScriptPlaying);
                            Assistant.Engine.MainWindow.LockScriptUI(true);
                            ScriptRunning = true;
                        }
                    }
                    else
                    {
                        if (ScriptRunning)
                        {
                            World.Player?.SendMessage(LocString.ScriptFinished);
                            Assistant.Engine.MainWindow.LockScriptUI(false);
                            ScriptRunning = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
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

            Script script = new Script(Lexer.Lex(lines));
            Interpreter.StartScript(script);
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

        public static void Error(string message, string scriptname = "")
        {
            World.Player?.SendMessage(MsgLevel.Error, $"Script '{scriptname}' error => {message}");
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
                new AutocompleteItem("fly") { ImageIndex = 2, ToolTipTitle = "fly", ToolTipText = BuildToolTip("fly", "Start or stop flying.", "fly") },
                new AutocompleteItem("land") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("setability") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("attack") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearhands") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clickobject") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("bandageself") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("usetype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("useobject") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("useonce") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearuseonce") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("moveitem") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("moveitemoffset") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("movetype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("movetypeoffset") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("walk") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("turn") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("run") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("useskill") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("feed") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("rename") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("shownames") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("togglehands") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("equipitem") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("togglemounted") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("equipwand") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("buy") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("sell") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearbuy") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearsell") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("organizer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autoloot") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("dress") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("undress") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("dressconfig") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("toggleautoloot") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("togglescavenger") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("counter") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("unsetalias") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("setalias") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("promptalias") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforgump") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("replygump") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("closegump") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearjournal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforjournal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("poplist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("pushlist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("removelist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("createlist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearlist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("info") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("pause") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("ping") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("playmacro") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("playsound") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("resync") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("snapshot") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("hotkeys") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("where") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("messagebox") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("mapuo") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clickscreen") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("paperdoll") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("helpbutton") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("guildbutton") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("questsbutton") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("logoutbutton") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("virtue") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("msg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("headmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("partymsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("guildmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("allymsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("whispermsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("yellmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("sysmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("chatmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("emotemsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("promptmsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("timermsg") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforprompt") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("cancelprompt") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("addfriend") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("removefriend") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("contextmenu") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforcontext") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("ignoreobject") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("clearignorelist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("setskill") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforproperties") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autocolorpick") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitforcontents") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("miniheal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("bigheal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("cast") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("chivalryheal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitfortarget") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("canceltarget") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("target") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targettype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targetground") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targettile") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targettileoffset") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targettilerelative") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("cleartargetqueue") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargetlast") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargetself") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargetobject") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargettype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargettile") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargettileoffset") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargettilerelative") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargetghost") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("autotargetground") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("cancelautotarget") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("getenemy") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("getfriend") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("settimer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("removetimer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("createtimer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("findalias") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("contents") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("inregion") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("skill") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("x") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("y") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("z") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("physical") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("fire") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("cold") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("poison") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("energy") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("str") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("dex") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("int") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("hits") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("maxhits") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("diffhits") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("stam") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("maxstam") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("mana") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("maxmana") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("usequeue") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("dressing") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("organizing") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("followers") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("maxfollowers") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("gold") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("hidden") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("luck") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("tithingpoints") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("weight") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("maxweight") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("diffweight") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("serial") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("graphic") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("color") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("amount") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("name") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("dead") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("direction") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("flying") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("paralyzed") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("poisoned") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("mounted") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("yellowhits") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("criminal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("enemy") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("friend") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("gray") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("innocent") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("invulnerable") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("murderer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("findobject") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("distance") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("inrange") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("buffexists") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("property") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("findtype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("findlayer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("skillstate") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("counttype") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("counttypeground") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("findwand") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("inparty") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("infriendslist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("war") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("ingump") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("gumpexists") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("injournal") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("listexists") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("list") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("inlist") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("targetexists") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("waitingfortarget") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("timer") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
                new AutocompleteItem("timerexists") { ImageIndex = 2, ToolTipTitle = "", ToolTipText = "" },
            };

            _autoCompleteMenu.Items.SetAutocompleteItems(autocompletes);
            _autoCompleteMenu.Items.MaximumSize =
                new Size(_autoCompleteMenu.Items.Width + 20, _autoCompleteMenu.Items.Height);
            _autoCompleteMenu.Items.Width = _autoCompleteMenu.Items.Width + 20;

            ScriptEditor.Language = FastColoredTextBoxNS.Language.Razor;
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
    }
}