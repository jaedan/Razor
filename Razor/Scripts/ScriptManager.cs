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

        public static void Error(bool quiet, string statement, string message)
        {
            if (quiet)
                return;

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
                new AutocompleteItem("fly", 2, "fly", "Fly", BuildToolTip("fly", "Start or stop flying.", "fly")),
                new AutocompleteItem("land", 2, "land", "Land", BuildToolTip("land", "Stop flying.", "land")),
                new AutocompleteItem("setability", 2, "setability", "Set Ability", BuildToolTip("setability (primary/secondary/stun/disarm) [on/off]", "Toggle primary, secondary, stun, or disarm ability", "setability primary on")),
                new AutocompleteItem("attack", 2, "attack", "Attack", BuildToolTip("attack (serial/graphic/alias)", "Attack a specified target.", "attack 0x12")),  //added graphic and alias to the signature format
                new AutocompleteItem("clearhands", 2, "clearhands", "Clear Hands", BuildToolTip("clearhands ('left'/'right'/'both')", "Clear one of both of your hands.", "clearhands right")),
                new AutocompleteItem("clickobject", 2, "clickobject", "Click Object", BuildToolTip("clickobject (serial/graphic/alias) [location]", "Single click on an object.", "clickobject 0xbd2 ground")), //added graphic and alias to the signature format as well as optional location || need more definition for location, ground is from steam documentation
                new AutocompleteItem("bandageself", 2, "bandageself", "Bandage Self", BuildToolTip("bandageself", "Bandage youself", "bandageself")),
                new AutocompleteItem("usetype", 2, "usetype", "Use Type", BuildToolTip("usetype (graphic) [color] [source] [range or search level]", "Use a specific item type by graphic.", "usetype 0xe21 any ground 2")),
                new AutocompleteItem("useobject", 2, "useobject", "Use Object", BuildToolTip("useobject (serial)", "Use a specific object by serial or alias", "useobject 0x40116650")),
                new AutocompleteItem("useonce", 2, "useonce", "Use Once", BuildToolTip("useonce (graphic) [color]", "Use a specific item type by graphic, from your backpack, only once.", "useonce 0xe79 any")),
                new AutocompleteItem("clearuseonce", 2, "clearuseonce", "Clear Use Once", BuildToolTip("clearuseonce", "Clears use once", "clearuseonce")), //not really sure what this does
                new AutocompleteItem("moveitem", 2, "moveitem", "Move Item", BuildToolTip("moveitem (serial) (destination) [(x, y, z)] [amount]", "Move an item by serial from source to destination", "moveitem 0x40116650 backpack")),
                new AutocompleteItem("moveitemoffset", 2, "moveitemoffset", "Move Item Offset", BuildToolTip("moveitemoffset (serial) ground [(x, y, z)] [amount]", "Move an item to the ground to an x y z offset from character.", "moveitemoffset 0x40116650 ground '0, 1, 0' 5")), //not sure of the exact use, also not sure if xyz syntax in example is correct.
                new AutocompleteItem("movetype", 2, "movetype", "Move Type", BuildToolTip("movetype (graphic) (source) (destination) [(x, y, z)] [color] [amount] [range or search level]", "Move an item by graphic from a source to a destination xyz.  Can be specified by color and range, and set an amount to move.", "movetype 0xeed 'ground' 'backpack' 0 0 0 'any' 200 2")),
                new AutocompleteItem("movetypeoffset", 2, "movetypeoffset", "Move Type Offset", BuildToolTip("movetypeoffset (graphic) (source) 'ground' [(x, y, z)] [color] [amount] [range or search level]", "Move an item to the ground to an x y z offset from the character.", "movetypeoffset 0xeed ground '0, 1, 0' 200 2")),  //not sure of example
                new AutocompleteItem("walk", 2, "walk", "Walk", BuildToolTip("walk (direction)", "Move your character to the given direction(s)", "walk North")),
                new AutocompleteItem("turn", 2, "turn", "Turn", BuildToolTip("turn (direction)", "Turn your character in a given direction.", "turn Northeast")),
                new AutocompleteItem("run", 2, "run", "Run", BuildToolTip("run (direction)", "Make your character run in a given direction(s)", "run West")),
                new AutocompleteItem("useskill", 2, "useskill", "Use Skill", BuildToolTip("useskill ('skill name'/last)", "Use a skill by name", "useskill 'Animal Taming'")),
                new AutocompleteItem("feed", 2, "feed", "Feed", BuildToolTip("feed (serial) ('food name'/'food group'/'any'/graphic) [color] [amount]", "Feed a given alias or serial with food name, graphic, or group.  Edit Data/foods.xml in order to customize new food groups and types.", "feed mount 'Fruits and Vegetables'")),
                new AutocompleteItem("rename", 2, "rename", "Rename", BuildToolTip("rename (serial) ('name')", "Request the server to rename a mobile (by serial or alias).", "rename mount Snorlax")),
                new AutocompleteItem("shownames", 2, "shownames", "Show Names", BuildToolTip("shownames ['mobiles'/'corpses']", "Display names for corpses and/or mobiles.", "shownames mobiles")),
                new AutocompleteItem("togglehands", 2, "togglehands", "Toggle Hands", BuildToolTip("togglehands (left/right)", "Arm and disarm an item.  Item must be equiped first to successfully toggle.", "togglehands right")),
                new AutocompleteItem("equipitem", 2, "equipitem", "Equip Item", BuildToolTip("equipitem (serial) (layer)", "Equip a specific item into a given layer.", "equipitem 0x40116650 2")),
                new AutocompleteItem("togglemounted", 2, "togglemounted", "Toggle Mounted", BuildToolTip("togglemounted", "Mount and dismount.  If a mount alias is not defined the system with prompt for one.", "togglemounted")),
                new AutocompleteItem("equipwand", 2, "equipwand", "Equip Wand", BuildToolTip("equipwand ('spell name'/'any'/'undefined') [minimum charges]", "Search for a wand inside you backpack and equip it \n Spells: \n Clumsy \n Identification \n Heal \n Feeblemind \n Weaken \n Magic Arrow \n Harm \n Fireball \n Greater Heal \n Lightning \n Mana Drain", "// Equip a Greater Heal wand with a minimum of 5 charges \n equipwand 'Greater Heal' 5")),
                new AutocompleteItem("buy", 2, "buy", "Buy", BuildToolTip("buy ('list name')", "Executes the buy list from vendor agent.", "buy 'Mage Regs'")),
                new AutocompleteItem("sell", 2, "sell", "Sell", BuildToolTip("sell ('list name')", "Executes the sell list from vendor agent.", "sell Gems")),
                new AutocompleteItem("clearbuy", 2, "clearbuy", "Clear Buy", BuildToolTip("clearbuy", "Clears the buy agent.", "clearbuy")), //not sure on use for this command
                new AutocompleteItem("clearsell", 2, "clearsell", "Clear Sell", BuildToolTip("clearsell", "Clears the sell agent.", "clearsell")), //not sure on use for this command,
                new AutocompleteItem("organizer", 2, "organizer", "Organizer", BuildToolTip("organizer ('profile name') [source] [destination]", "Executes a specific organizer profile.", "organizer Reagents")),
                new AutocompleteItem("autoloot", 2, "autoloot", "Autoloot", BuildToolTip("autoloot", "Prompts a cursor to autoloot a specific corpse or container", "autoloot")),
                new AutocompleteItem("dress", 2, "dress", "Dress", BuildToolTip("dress ['profile name']", "Dress a specific or temporary profile.", "dress Weapon")),
                new AutocompleteItem("undress", 2, "undress", "Undress", BuildToolTip("undress ['profile name']", "Undress a specific or temporary profile.", "undress Weapon")),
                new AutocompleteItem("dressconfig", 2, "dressconfig", "Dress Config", BuildToolTip("dressconfig", "Setup a temporary profile.", "dressconfig")),
                new AutocompleteItem("toggleautoloot", 2, "toggleautoloot", "Toggle Autoloot", BuildToolTip("toggleautoloot", "Enable and disable the autoloot agent", "toggleautoloot")),
                new AutocompleteItem("togglescavenger", 2, "togglescavenger", "Toggle Scavenger", BuildToolTip("togglescavenger", "Enable and disable the scavenger agent", "togglescavenger")),
                new AutocompleteItem("counter", 2, "counter", "Counter", BuildToolTip("counter ('format') (operator) (value)", "Compare the amount of a specific counter format.  Go to the \"Display/Counters\" tab to setup item counters.", "if counter garlic == 0")),
                new AutocompleteItem("unsetalias", 2, "unsetalias", "Unset Alias", BuildToolTip("unsetalias ('alias name')", "Unset and remove an existing alias.", "unsetalias 'Keg Bag'")),
                new AutocompleteItem("setalias", 2, "setalias", "Set Alias", BuildToolTip("setalias ('alias name') [serial]", "Define and existing alias with a given serial or another alias value.", "setalias OldObject 0x40116650")),
                new AutocompleteItem("findalias", 2, "findalias", "Find Alias", BuildToolTip("if findalias ('alias name') \n endif", "Search if a specific custom alias name is already created", "if not findalias weapon \n promptalias weapon \n endif")),
                new AutocompleteItem("promptalias", 2, "promptalias", "Prompt Alias", BuildToolTip("promptalias ('alias name')", "Prompt in-game for a new alias and wait until it is selected.", "promptalias OldObject")),
                new AutocompleteItem("waitforgump", 2, "waitforgump", "Wait for Gump", BuildToolTip("waitforgump (gump id/'any') (timeout)", "Wait for a gump from the server", "waitforgump 0x1ec8c837 5000")),
                new AutocompleteItem("replygump", 2, "replygump", "Reply Gump", BuildToolTip("replygump (gump id/'any') (button) [option] [...]", "Reply to a server gump.", "replygump 0x1ec8c837 1")),
                new AutocompleteItem("ingump", 2, "ingump", "In Gump", BuildToolTip("ingump (gump id/'any')", "Check for text in a gump.", "if ingump 0x1ec8c837 'Home' \n replygump 0x1ec8c837 2 \n endif")),
                new AutocompleteItem("gumpexists", 2, "gumpexists", "Gump Exists", BuildToolTip("if gumpexists (gump id/'any') \n endif", "Checks if a gump id exists or not", "if gumpexists 0x3029 \n replygump 0x3029 1 \n endif")),
                new AutocompleteItem("closegump", 2, "closegump", "Close Gump", BuildToolTip("closegump ('paperdoll'/'status'/'profile'/'container') ('serial')", "Closes a specific gump type by serial.", "closegump 'paperdoll' '0x4326e98")),
                new AutocompleteItem("injournal", 2, "injournal", "In Journal", BuildToolTip("if injournal ('text') ['author'/system] \n endif", "Check for a text string in the journal.  Can definde an optional source name.", "if injournal 'outside the protection' system \n headmsg 'Outside of guard zone' \n endif")),
                new AutocompleteItem("clearjournal", 2, "clearjournal", "Clear Journal", BuildToolTip("clearjournal", "Clears the journal cache that razor reads from.", "clearjournal")),
                new AutocompleteItem("waitforjournal", 2, "waitforjournal", "Wait for Journal", BuildToolTip("waitforjournal ('text') (timeout) ['author'/system]", "Checks the journal for a given text string until found, or timeout.", "//wait 5 seconds for a journal message \n   waitforjournal 'too far away' 5000 'system'")),
                new AutocompleteItem("poplist", 2, "poplist", "Pop List", BuildToolTip("poplist ('list name') ('element value'/front/back)", "Remove an element from a named and existing list.", "createlist 'sample' \n pushlist 'sample' 'banana' \n pushlist 'sample' 'apple' \n pushlist 'sample' 'lemon' \n pushlist 'sample' 'grape' \n // Pop banana \n poplist 'sample' 'banana' \n // Now apple is our front element \n // Pop front poplist 'sample' 'front' \n // Element apple no longer exists, check output \n for 0 to 'sample' \n sysmsg sample[] \n endfor \n // Remove all bananas from the list by adding '!' suffix \n poplist! 'sample' 'banana'")),
                new AutocompleteItem("pushlist", 2, "pushlist", "Push List", BuildToolTip("pushlist ('list name') ('element value') [front/back]", "Add a new element to an existing list, default position is \"back\". Using the suffix \"!\" will only push an item to the list if it in not already present (unique). ", "createlist 'sample' \n // Apple \n pushlist 'sample' 'apple' \n // Lemon\n pushlist 'sample' 'lemon' \n // Grape \n pushlist 'sample' 'grape' \n // Insert a new Grape before all other elements \n pushlist 'sample' 'grape' 'front' \n // Use suffix '!' for unique element values \n while not pushlist! 'grape' \n // Could not push because it already exists, remove all grapes \n poplist 'sample' 'grape' \n endwhile")),
                new AutocompleteItem("removelist", 2, "removelist", "Remove List", BuildToolTip("removelist ('list name')", "Remove a named and exsting list.", "// Create and populate a new list \n createlist 'sample' \n pushlist 'sample' 'Hello' \n pushlist 'sample' 'World' \n for 0 to 'sample' \n msg sample[] \n endfor \n // Remove list \n removelist 'sample' \n if not listexists 'sample' \n sysmsg 'List removed successfully!' \n else \n // Unreachable code \n endif")),
                new AutocompleteItem("createlist", 2, "createlist", "Create List", BuildToolTip("createlist ('list name')", "Create a new named list.", "createlist sample")),
                new AutocompleteItem("clearlist", 2, "clearlist", "Clear List", BuildToolTip("clearlist ('list name')", "Clear a list by name.", "// Create and populate a list \n if not listexists 'sample' \n createlist 'sample' \n endif \n pushlist 'sample' 'Hello' \n pushlist 'sample' 'World' \n if list 'sample' > 0 \n sysmsg 'List is not empty!' \n endif \n // Clear list \n clearlist 'sample' \n // Now list is empty but still exists, use removelist command to delete! \n if listexists 'sample' \n sysmsg 'List exists!' \n endif \n if list 'sample' == 0 \n sysmsg 'List is now empty!' \n endif")),
                new AutocompleteItem("listexists", 2, "listexists", "List Exists", BuildToolTip("if listexists ('list name') \n endif", "Checks if a named list exists", "// Create a new list in case it does not exists \n if not listexists 'sample' \n createlist 'sample' \n endif")),
                new AutocompleteItem("list", 2, "list", "List Count", BuildToolTip("if list ('list name') (operator) (value) \n endif", "Compare the size of an existing list with a given value.", "// Create new list in case it does not exists \n if not listexists 'sample' \n createlist 'sample' \n endif \n // In case list is empty append values \n // Just a sample, it could be added to listexists statement block \n if list 'sample' == 0 \n pushlist 'sample' 'Hello' \n pushlist 'sample' 'World' \n endif \n for 0 to 'sample' \n msg sample[] \n endfor")),
                new AutocompleteItem("inlist", 2, "inlist", "In List", BuildToolTip("if inlist ('list name') ('element value') \n endif", "Checks whether a list contains a given element.  Case sensativity is disabled by default, to enable it append \"!\" suffix to this command.", "if not listexists 'sample' \n createlist 'sample' \n endif \n pushlist 'sample' 'Hello' \n pushlist 'sample' 'World' \n // Case sensitive disabled will return true \n if inlist 'sample' 'hello' \n sysmsg 'List contains element!' \n endif \n // Use suffix '!' to enable case sensitive \n if inlist! 'sample' 'world' \n // Unreachable code \n endif")),
                new AutocompleteItem("info", 2, "info", "Object Inspector", BuildToolTip("info", "Prompts a target to inspect in-game object.", "info")),
                new AutocompleteItem("pause", 2, "pause", "Pause", BuildToolTip("pause (timeout)", "Insert a pause/wait in milliseconds.", "//1 second \n pause 1000 \n //0.5 seconds \n pause 500")),
                new AutocompleteItem("ping", 2, "ping", "Ping Server", BuildToolTip("ping", "Retrieve an approximated ping from server", "ping")),
                new AutocompleteItem("playmacro", 2, "playmacro", "Play Macro", BuildToolTip("playmacro ('macro name')", "Run a specific macro by name.  Name parameter is case sensitive.", "playmacro 'Gate Hop'")),
                new AutocompleteItem("playsound", 2, "playsound", "Play Sound", BuildToolTip("playsound (sound ID/'file name')", "Play a sound by ID or system .wav file. **INSERT DIR FOR FILE HERE**", "// System .wav file \n playsound 'name.wav' \n // Game sound id \n playsound 25")),
                new AutocompleteItem("resync", 2, "resync", "Resynchronize", BuildToolTip("resync", "Resynchronize game data with server.  A 0.8 second delay is required between resync requests.", "resync")),
                new AutocompleteItem("snapshot", 2, "snapshot", "Snapshot", BuildToolTip("snapshot [timer]", "Allows you to create a screenshot, it is also possible to add a delay before capture.", "//Wait 5 seconds before capture \n snapshot 5000")),
                new AutocompleteItem("hotkeys", 2, "hotkeys", "Toggle Hotkeys", BuildToolTip("hotkeys", "Enable and disable hotkeys", "hotkeys")),
                new AutocompleteItem("where", 2, "where", "Where", BuildToolTip("where", "Displays coordinates and region name.", "where")),
                new AutocompleteItem("messagebox", 2, "messagebox", "Message Box", BuildToolTip("messagebox ('title') ('body')", "Shows a simple message box with a custom title and body.", "messagebox Sample 'Hello World!'")),
                new AutocompleteItem("mapuo", 2, "mapuo", "MapUO", BuildToolTip("mapuo", "Toggles MapUO visibility and starts it if not open.", "mapuo")),
                new AutocompleteItem("clickscreen", 2, "clickscreen", "Click Screen", BuildToolTip("clickscreen (x) (y) [single/double] [left/right]", "Uses your mouse to click coordinates on your screen.  You can prevent moving the mouse by using suffix \"!\".", "// Single left click moving cursor at 200, 500 \n clickscreen 200 500 \n // Single right click without moving cursor at 400, 150 \n clickscreen! 400 150 single right \n ")),
                new AutocompleteItem("paperdoll", 2, "paperdoll", "Open Paperdoll", BuildToolTip("paperdoll [serial/'alias']", "Open your paperdoll or the paperdoll of the defined mobile", "paperdoll 0xer523c2")),
                new AutocompleteItem("helpbutton", 2, "helpbutton", "Help Button", BuildToolTip("helpbutton", "Presses the help button on your paperdoll ", "helpbutton")),
                new AutocompleteItem("guildbutton", 2, "guildbutton", "Guild Button", BuildToolTip("guildbutton", "Presses the guild button on your paperdoll ", "guildbutton")),
                new AutocompleteItem("questsbutton", 2, "questsbutton", "Quests Button", BuildToolTip("questsbutton", "Presses the quest button on your paperdoll ", "questsbutton")),
                new AutocompleteItem("logoutbutton", 2, "logoutbutton", "Logout Button", BuildToolTip("logoutbutton", "Presses the logout button on your paperdoll ", "logoutbutton")),
                new AutocompleteItem("virtue", 2, "virture", "Virtues", BuildToolTip("virtue (honor/sacrifice/valor)", "Invokes the virture by name", "// Search for an ettin in range of 5 tiles \n // Prefix '@' to suppress system warnings \n if @findtype 0x12 0 0 0 5 \n autotargetobject 'found' \n // Use virtue honor \n virtue 'Honor' \n endif")),
                new AutocompleteItem("msg", 2, "msg", "Message", BuildToolTip("msg ('text') [color]", "", "")),
                new AutocompleteItem("headmsg", 2, "headmsg", "Head Message", BuildToolTip("headmsg ('text') [color] [serial]", "Sends a private over head message", "headmsg Hi 26")),
                new AutocompleteItem("partymsg", 2, "partymsg", "Party Message", BuildToolTip("partymsg ('text')", "Sends a message in party chat", "partymsg \"What's up?\"")),
                new AutocompleteItem("guildmsg", 2, "guildmsg", "Guild Message", BuildToolTip("guildmsg ('text')", "Sends a message in guild chat", "guildmessage \"Who's hunting?\"")),
                new AutocompleteItem("allymsg", 2, "allymsg", "Alliance Message", BuildToolTip("allymsg ('text')", "Sends a message in alliance chat", "allymsg \"Can someone come rez me?\"")),
                new AutocompleteItem("whispermsg", 2, "whispermsg", "Whisper Message", BuildToolTip("whispermsg ('text')", "Sends a public whisper", "whispermsg \"I see you\"")),
                new AutocompleteItem("yellmsg", 2, "yellmsg", "Yell Message", BuildToolTip("yellmsg ('text')", "Sends a public yell", "yellmsg HUAH!")),
                new AutocompleteItem("sysmsg", 2, "sysmsg", "System Message", BuildToolTip("sysmsg ('text')", "Sends an internal system message", "sysmsg 'Hello World!'")),
                new AutocompleteItem("chatmsg", 2, "chatmsg", "Chat Message", BuildToolTip("chatmsg ('text')", "Sends a chat message", "chatmsg Hello")),
                new AutocompleteItem("emotemsg", 2, "emotemsg", "Emote Message", BuildToolTip("emotemsg ('text')", "Sends a public emote", "emotemsg cries")),
                new AutocompleteItem("promptmsg", 2, "promptmsg", "Prompt Message", BuildToolTip("promptmsg ('text')", "", "")), //I don't know what this does
                new AutocompleteItem("timermsg", 2, "timermsg", "Timer Message", BuildToolTip("timermsg ('timer name') [color]", "", "")), //I don't know what this does
                new AutocompleteItem("waitforprompt", 2, "waitforprompt", "Wait for Prompt", BuildToolTip("waitforprompt (timeout)", "", "")), //I don't know what this does
                new AutocompleteItem("cancelprompt", 2, "cancelprompt", "Cancel Prompt", BuildToolTip("cancelprompt", "", "")), //I don't know what this does
                new AutocompleteItem("addfriend", 2, "addfriend", "Add Friend", BuildToolTip("addfriend", "Prompts in-game to add a mobile to friends list.", "addfriend")), //will need to be adjusted for multiple friends lists
                new AutocompleteItem("removefriend", 2, "removefriend", "Remove Friend", BuildToolTip("removefriend", "Prompts in-game to remove a mobile from friends list", "removefriend")), //will need to be adjusted for multiple friends lists
                new AutocompleteItem("contextmenu", 2, "contextmenu", "Context Menu", BuildToolTip("contextmenu", "Opens a context menu for the given serial and selects an option.", "contextmenu 0x8e86431 2")), //not sure the difference between contextmenu and waitforcontextmenu
                new AutocompleteItem("waitforcontext", 2, "waitforcontext", "Wait for Context Menu", BuildToolTip("waitforcontext (serial) (option) (timeout)", "Waits for context menu to open before responding.", "waitforcontext 'self' 6 15000")), //not sure the difference between contextmenu and waitforcontextmenu
                new AutocompleteItem("ignoreobject", 2, "ignoreobject", "Ignore Object", BuildToolTip("ignoreobject (serial)", "Add a serial to the ignore list affecting the findtype command.", "ignoreobject 0x40116650")),
                new AutocompleteItem("clearignorelist", 2, "clearignorelist", "Clear Ignore List", BuildToolTip("clearignorelist", "Clears the ignore list for the findtype command", "clearignorelist")),
                new AutocompleteItem("setskill", 2, "setskill", "Set Skill", BuildToolTip("setskill ('skill name') ('locked'/'up'/'down')", "Set a skill into a specific stat: locked, up, or down.", "if skill 'Magery' == 100 \n setskill 'Magery' 'locked' \n endif")),
                new AutocompleteItem("waitforproperties", 2, "waitforproperties", "Wait For Properties", BuildToolTip("waitforproperties (serial) (timeout)", "Request and wait for properties of an item or mobile.", "setalias 'ring' 0x409c89fa \n // Request and wait for 5 seconds \n waitforproperties 'ring' 5000 \n if property 'Faster Casting Recovery' 0x409c89fa == 3 \n moveitem 'ring' 'backpack' \n pause 1000 \n endif")),
                new AutocompleteItem("autocolorpick", 2, "autocolorpick", "Automated Color Pick", BuildToolTip("autocolorpick", "Setup an automated reply to the incoming dye color gump, allowing you to define dye tubs color. \n Command should be added prior to the action that opens the color pick gump.", "if not @findobject 'dyes' \n promptalias 'dyes' \n endif \n if not @findobject 'tub' \n prompatalias 'tub' \n endif \n autocolorpick 35 \n useobject! 'dyes' \n waitfortarget 1000 \n target! 'tub’")),
                new AutocompleteItem("waitforcontents", 2, "waitforcontents", "Wait for Contents", BuildToolTip("waitforcontents (serial) (timeout)", "Wait for the server to send container contents, it will also try opening the container once.", "// Ask for a new bag \n promptalias 'bag' \n // Try opening once and wait for contents for 2 seconds \n waitforcontents 'bag' 2000")),
                new AutocompleteItem("miniheal", 2, "miniheal", "Mini Heal", BuildToolTip("miniheal [serial]", "Cast heal, cure, greater heal or arch cure upon a mobile. \n Command uses managed casting, meaning it checks for disruptive actions and your are able to keep your hotkey pressed without checking \"Do not auto interrupt\" option.", "//miniheal self \n miniheal")),
                new AutocompleteItem("bigheal", 2, "bigheal", "Greater Heal", BuildToolTip("bigheal", "Cast heal, cure, greater heal or arch cure upon a mobile. \n Command uses managed casting, meaning it checks for disruptive actions and your are able to keep your hotkey pressed without checking \"Do not auto interrupt\" option.", "//Greater heal friend \n bigheal 'friend'")),
                new AutocompleteItem("cast", 2, "cast", "Cast", BuildToolTip("cast (spell id/'spell name'/'last')", "Cast a spell by ID or name", "// Magic Arrow and Fireball sample \n // Check 'Do not auto interrupt' option \n // Simple cast \n cast 'Magic Arrow' \n waitfortarget 650 \n target 'enemy' \n // Another simple cast \n cast 'Fireball' \n waitfortarget 1250 \n target 'enemy' \n // Check for curse and remove it \n // Prefix '@' to disable system warnings \n if @buffexists 'Curse' \n // Managed cast \n cast 'Remove Curse' 'self' \n endif \n // Automated target sample \n autotargetobject 'enemy' \n cast 'Lightning'")),
                new AutocompleteItem("chivalryheal", 2, "chivalryheal", "Chivalry Heal", BuildToolTip("chivalryheal [serial]", "Cast close wounds or cleanse by fire upon a mobile. \n Command uses managed casting, meaning it checks for disruptive actions and your are able to keep your hotkey pressed without checking \"Do not auto interrupt\" option.", "// Chivalry heal self \n chivalryheal \n // Chivalry heal friend \n chivalryheal 'friend'")),
                new AutocompleteItem("waitfortarget", 2, "waitfortarget", "Wait for Target", BuildToolTip("waitfortarget (timeout)", "Wait for a new client target cursor from server.", "cast 'Explosion' \n // Wait for 2.5 seconds until target comes \n waitfortarget 2500 \n // Not queued target on enemy \n target! 'enemy' ")),
                new AutocompleteItem("canceltarget", 2, "canceltarget", "Cancel Target", BuildToolTip("canceltarget", "Cancel an existing cursor/target.", "canceltarget")),
                new AutocompleteItem("target", 2, "target", "Target", BuildToolTip("target (serial) [timeout]", "Instantly target a given alias or serial. \n Default queue timeout is 5 seconds, use suffis \"!\" in order to bypass queue.", "cast Heal \n waitfortarget 250 \n \\Queued target \n target friend")),
                new AutocompleteItem("targettype", 2, "targettype", "Target Type", BuildToolTip("targettype (graphic) [color] [range]", "Instantly target a given type \n Default queue timeout is 5 seconds, use suffis \"!\" in order to bypass queue.", "")), //add example
                new AutocompleteItem("targetground", 2, "targetground", "Target Ground", BuildToolTip("targetground (graphic) [color] [range]", "Instantly target a give ground type \n Default queue timeout is 5 seconds, use suffis \"!\" in order to bypass queue.", "")), //add example
                new AutocompleteItem("targettile", 2, "targettile", "Target Tile", BuildToolTip("targettile ('last'/'current'/(x y z)) [graphic]", "Instantly target a given tile \n Default queue timeout is 5 seconds, use suffis \"!\" in order to bypass queue.", "targettile 125 854 0")), //verify x y z format
                new AutocompleteItem("targettileoffset", 2, "targettileoffset", "Target Tile Offset", BuildToolTip("targettileoffset (x y z) [graphic]", "", "")), //needs desc and example
                new AutocompleteItem("targettilerelative", 2, "targettilerelative", "Target Tile Relative", BuildToolTip("targettilerelative (serial) (range) [reverse = 'true' or 'false'] [graphic]", "", "")), //needs desc and example
                new AutocompleteItem("cleartargetqueue", 2, "cleartargetqueue", "Clear Target Queue", BuildToolTip("cleartargetqueue", "Clears the current target queue", "cleartargetqueue")),
                new AutocompleteItem("autotargetlast", 2, "autotargetlast", "Auto Target Last", BuildToolTip("autotargetlast", "Automatically targets the last target", " autotargetlast \n cast explosion")),
                new AutocompleteItem("autotargetself", 2, "autotargetself", "Auto Target Self", BuildToolTip("autotargetself", "Automatically targets yourself", " autotargetself \n cast 'greater heal'")),
                new AutocompleteItem("autotargetobject", 2, "autotargetobject", "Auto Target Object", BuildToolTip("autotargetobject (serial)", "Automatically targets a specific object", "")),
                new AutocompleteItem("autotargettype", 2, "autotargettype", "Auto Target Type", BuildToolTip("autotargettype (graphic) [color] [range]", "Automatically targets an object by graphic.", "")), //needs example
                new AutocompleteItem("autotargettile", 2, "autotargettile", "Auto Target Tile", BuildToolTip("autotargettile ('last'/'current'/(x y z)) [graphic]", "", "")),  //needs desc and example
                new AutocompleteItem("autotargettileoffset", 2, "autotargettileoffset", "Auto Target Tile Offset", BuildToolTip("autotargettileoffset (x y z) [graphic]", "", "")), //needs desc and example
                new AutocompleteItem("autotargettilerelative", 2, "autotargettilerelative", "Auto Target Tile Relative", BuildToolTip("autotargettilerelative (serial) (range) [reverse = 'true' or 'false'] [graphic]", "", "")), //needs desc and example
                new AutocompleteItem("autotargetghost", 2, "autotargetghost", "Auto Target Ghost", BuildToolTip("autotargetghost (range) [z-range]", "Automatically targets a ghost within range.", "autotargetghost 2 \n cast Ressurection")),
                new AutocompleteItem("autotargetground", 2, "autotargetground", "Auto Target Ground", BuildToolTip("autotargetground (graphic) [color] [range]", "", "")), //needs desc and example
                new AutocompleteItem("cancelautotarget", 2, "cancelautotarget", "Cancel Auto Target", BuildToolTip("cancelautotarget", "Cancels any auto target currently set.", "cancelautotarget")),
                new AutocompleteItem("getenemy", 2, "getenemy", "Get Enemy", BuildToolTip("/* \n Notorieties: any, friend, innocent, murderer, enemy, criminal, gray \n Filters: humanoid, transformation, closest, nearest \n */ \n getenemy ('notoriety') ['filter'] \n ", "Get and set an \"enemy\" alias according to the given parameters.  You can edit Data/bodies.xml (File) in order to customize humanoids and transformations body values filtering; by default get command will always list possible targets and switch between them every time the macro is executed, make sure to put closest or nearest filter when needed. Nearest filter will switch between the 2 closest targets.", "// Get closest humanoid enemy \n getenemy 'murderer' 'criminal' 'gray' 'closest' 'humanoid' \n if inrange 'enemy' 10 \n autotargetobject 'friend' \n cast 'Lightning' \n endif ")),
                new AutocompleteItem("getfriend", 2, "getfriend", "Get Friend", BuildToolTip("/* \n Notorieties: any, friend, innocent, murderer, enemy, criminal, gray, invulnerable \n Filters: humanoid, transformation, closest, nearest \n */ \n getfriend ('notoriety') ['filter']", "Get and set an \"friend\" alias according to the given parameters.  You can edit Data/bodies.xml (File) in order to customize humanoids and transformations body values filtering; by default get command will always list possible targets and switch between them every time the macro is executed, make sure to put closest or nearest filter when needed. Nearest filter will switch between the 2 closest targets.", "// Get a humanoid friend \n getfriend 'innocent' 'friend' 'humanoid' \n if inrange 'friend' 10 \n autotargetobject 'friend' \n cast 'Greater Heal' \n endif")),
                new AutocompleteItem("settimer", 2, "settimer", "Set Timer", BuildToolTip("settimer ('timer name') (value)", "Set a timer value and create in case it does not exist.", "if not timerexists 'sample' \n settimer 'sample' 10000 \n endif \n if skill 'Spirit Speak' < 100 and timer 'sample' >= 10000 \n useskill 'Spirit Speak' \n settimer 'sample' 0 \n endif ")),
                new AutocompleteItem("removetimer", 2, "removetimer", "Remove Timer", BuildToolTip("removetimer ('timer name')", "Remove a specific timer by name.", "")), //needs example
                new AutocompleteItem("createtimer", 2, "createtimer", "Create Timer", BuildToolTip("createtimer ('timer name')", "Create a new named timer.", "// Create a new timer and start counting \n if not timerexists 'sample' \n createtimer 'sample' \n endif")),
                new AutocompleteItem("timer", 2, "timer", "Timer", BuildToolTip("if timer ('timer name') (operator) (value) \n endif", "Check for a named timer value.", "// Create a new timer \n if not timerexists 'sample' \n createtimer 'sample' \n endif \n // Reset every 10 seconds \n if timer 'sample' > 10000 \n settimer 'sample' 0 \n endif ")),
                new AutocompleteItem("timerexists", 2, "timerexists", "Timer Exists", BuildToolTip("if timerexists ('timer name') \n endif", "Check if a named timer exists.", "if not timerexists 'sample' \n createtimer 'sample' \n endif")),
                new AutocompleteItem("contents", 2, "contents", "Contents", BuildToolTip("if contents (serial) ('operator') ('value') \n endif", "Retrieve and compare the amount of items inside a container.", "if contents 'backpack' > 10 \n sysmsg 'More than 10 items inside backpack!' \n endif")),
                new AutocompleteItem("inregion", 2, "inregion", "In Region", BuildToolTip("if inregion ('guards'/'town'/'dungeon'/'forest') [serial] [range] \n endif", "Checks whether an item or mobile region type matches. \n You can edit Data/regions.xml (File) in order to customize region names and guard zone lines.", "// Check if local player is in town \n if inregion 'town' \n msg 'bank' \n endif \n // Check if enemy is in guards zone in range of 10 tiles \n autotargetobject 'enemy' \n cast 'Lightning' \n if innocent 'enemy' and inregion 'guards' 'enemy' 10 \n cancelautotarget \n endif  ")),
                new AutocompleteItem("skill", 2, "skill", "Skills", BuildToolTip("if skill ('name') (operator) (value) \n endif", "Check for a specific local player skill value.", "// Basic train Necromancy sample \n if mana <= 10 \n // Server must support buff icons, otherwise use injournal to detect trance \n while not buffexists 'Meditation' \n useskill 'Meditation' \n pause 1000 \n endwhile \n pause 15000 \n endif \n if skill 'Necromancy' >= 99 \n cast 'Vampiric Embrace' \n elseif skill 'Necromancy' >= 75 \n cast 'Lich Form' \n else \n cast 'Horrific Beast' \n endif" )),
                new AutocompleteItem("x", 2, "x", "X Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the \'X\' coordinate of the player or object.", "")),
                new AutocompleteItem("y", 2, "y", "Y Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the \'Y\' coordinate of the player or object", "")),
                new AutocompleteItem("z", 2, "z", "Z Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the \'Z\' coordinate of the player or object", "")),
                new AutocompleteItem("physical", 2, "physical", "Physical Resistance Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the Physical Resistance of the player.", "")),
                new AutocompleteItem("fire", 2, "fire", "Fire Resistance Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the Fire Resistance of the player.", "")),
                new AutocompleteItem("cold", 2, "cold", "Cold Resistance Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the Cold Resistance of the player.", "")),
                new AutocompleteItem("poison", 2, "poison", "Poison Resistance Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the Poison Resistance of the player.", "")),
                new AutocompleteItem("energy", 2, "energy", "Energy Resistance Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "Checks the Energy Resistance of the player.", "")),
                new AutocompleteItem("str", 2, "str", "Strenght Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("dex", 2, "dex", "Dex Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("int", 2, "int", "Int Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("hits", 2, "hits", "Hits Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("maxhits", 2, "maxhits", "Max Hits Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("diffhits", 2, "diffhits", "Difference of Hits Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("stam", 2, "stam", "Stamina Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("maxstam", 2, "maxstam", "Max Stamina Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("mana", 2, "mana", "Mana Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("maxmana", 2, "maxmana", "Max Mana Attribute", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("usequeue", 2, "usequeue", "Use Queue", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("dressing", 2, "dressing", "Dressing", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("organizing", 2, "organizing", "Organizing", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("followers", 2, "followers", "Followers", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("maxfollowers", 2, "maxfollowers", "Max Followers", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("gold", 2, "gold", "Gold", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("hidden", 2, "hidden", "Hidden", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("luck", 2, "luck", "Luck", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("tithingpoints", 2, "tithingpoints", "Tithing Points", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("weight", 2, "weight", "Weight", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("maxweight", 2, "maxweight", "Max Weight", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("diffweight", 2, "diffweight", "Difference of Weight", BuildToolTip("if (attribute) [operator] [value] \n endif", "", "")),
                new AutocompleteItem("serial", 2, "serial", "Serial", BuildToolTip("", "", "")),
                new AutocompleteItem("graphic", 2, "graphic", "Graphic", BuildToolTip("", "", "")),
                new AutocompleteItem("color", 2, "color", "Color", BuildToolTip("", "", "")),
                new AutocompleteItem("amount", 2, "amount", "Amount", BuildToolTip("", "", "")),
                new AutocompleteItem("name", 2, "name", "Name", BuildToolTip("", "", "")),
                new AutocompleteItem("dead", 2, "dead", "Dead", BuildToolTip("", "", "")),
                new AutocompleteItem("direction", 2, "direction", "Direction", BuildToolTip("", "", "")),
                new AutocompleteItem("flying", 2, "flying", "Flying", BuildToolTip("", "", "")),
                new AutocompleteItem("paralyzed", 2, "paralyzed", "Paralyzed", BuildToolTip("", "", "")),
                new AutocompleteItem("poisoned", 2, "poisoned", "Poisoned", BuildToolTip("", "", "")),
                new AutocompleteItem("mounted", 2, "mounted", "Mounted", BuildToolTip("", "", "")),
                new AutocompleteItem("yellowhits", 2, "yellowhits", "Yellow Hits", BuildToolTip("", "", "")),
                new AutocompleteItem("criminal", 2, "criminal", "Criminal", BuildToolTip("", "", "")),
                new AutocompleteItem("enemy", 2, "enemy", "Enemy", BuildToolTip("", "", "")),
                new AutocompleteItem("friend", 2, "friend", "Friend", BuildToolTip("", "", "")),
                new AutocompleteItem("gray", 2, "gray", "Gray", BuildToolTip("", "", "")),
                new AutocompleteItem("innocent", 2, "innocent", "Innocent", BuildToolTip("", "", "")),
                new AutocompleteItem("invulnerable", 2, "invulnerable", "Invulnerable", BuildToolTip("", "", "")),
                new AutocompleteItem("murderer", 2, "murderer", "Murderer", BuildToolTip("", "", "")),
                new AutocompleteItem("findobject", 2, "findobject", "Find Object", BuildToolTip("", "", "")),
                new AutocompleteItem("distance", 2, "distance", "Distance", BuildToolTip("", "", "")),
                new AutocompleteItem("inrange", 2, "inrange", "In Range", BuildToolTip("", "", "")),
                new AutocompleteItem("buffexists", 2, "buffexists", "Buff Exists", BuildToolTip("", "", "")),
                new AutocompleteItem("property", 2, "property", "Property", BuildToolTip("if property ('name') (serial) [operator] [value] \n endif", "Check for a specific item or mobile property, existence and value.", "")), //need example
                new AutocompleteItem("findtype", 2, "findtype", "Find Type", BuildToolTip("", "", "")),
                new AutocompleteItem("findlayer", 2, "findlayer", "Find Layer", BuildToolTip("", "", "")),
                new AutocompleteItem("skillstate", 2, "skillstate", "Skill State", BuildToolTip("", "", "")),
                new AutocompleteItem("counttype", 2, "counttype", "Count Type", BuildToolTip("", "", "")),
                new AutocompleteItem("counttypeground", 2, "counttypeground", "Count Type Ground", BuildToolTip("", "", "")),
                new AutocompleteItem("findwand", 2, "findwand", "Find Wand", BuildToolTip("", "", "")),
                new AutocompleteItem("inparty", 2, "inparty", "In Party", BuildToolTip("", "", "")),
                new AutocompleteItem("infriendslist", 2, "infriendslist", "In Friends List", BuildToolTip("", "", "")),
                new AutocompleteItem("war", 2, "war", "War", BuildToolTip("", "", "")),
                new AutocompleteItem("targetexists", 2, "targetexists", "Target Exists", BuildToolTip("", "", "")),
                new AutocompleteItem("waitingfortarget", 2, "waitingfortarget", "Waiting for Target", BuildToolTip("waitingfortarget", "Returns true whenever the core is internally waiting for a server target. \n It is useful for creating macros that will not mess up with agents and options such as bone cutter and automated healing.", "// Search for a pouch inside backpack \n if @findtype 0xe79 'any' 'backpack' \n useobject! 'found' \n // Let's assume healing option is running, hold the cast until it applies the bandage \n while waitingfortarget or targetexists 'server' \n endwhile \n cast 'Magic Trap' \n waitfortarget 1200 \n target! 'found' \n endif ")),
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