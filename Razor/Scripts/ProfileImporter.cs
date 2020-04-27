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
using System.IO;
using System.Reflection;
using System.Xml;

namespace Assistant.Scripts
{
    public static class SteamProfile
    {
        private static Dictionary<string, string[]> _dataMap = new Dictionary<string, string[]>()
        {
            { "CommandPrefix", null }, // ??
            { "UseObjectsQueue", new string[2] { "QueueActions", "System.Boolean" } },
            { "ShowCorpseNames", new string[2] { "ShowCorpseNames", "System.Boolean" } },
            { "ShowCreatureNames", new string[2] { "ShowMobNames", "System.Boolean" } },
            { "OpenCorpses", new string[2] { "AutoOpenCorpses", "System.Boolean" } },
            { "OpenCorpsesRange",new string[2] { "CorpseRange", "System.Int32" } },
            { "ShowMobileHits", null }, // ??
            { "PositionInTitle", null }, // ??
            { "ForceResolution", new string[2] { "ForceSizeEnabled", "System.Boolean" } },
            { "GameWindowWidth", new string[2] { "ForceSizeX", "System.Int32" } },
            { "GameWindowHeight", new string[2] { "ForceSizeY", "System.Int32" } },
            { "HandsBeforePotions", new string[2] { "PotionEquip", "System.Boolean" } },
            { "HandsBeforeCasting", new string[2] { "SpellUnequip", "System.Boolean" } },
            { "SmartTarget", new string[2] { "SmartLastTarget", "Variant" } },
            { "SmartTargetRange", new string[2] { "RangeCheckLT", "System.Boolean" } },
            { "SmartTargetRangeValue", new string[2] { "LTRange", "System.Int32" } },
            { "HighlightCurrentTarget", null }, // ??
            { "BlockInvalidHeal", new string[2] { "BlockHealPoison", "System.Boolean" } },
            { "ActionDelay", new string[2] { "ObjectDelay", "System.Int32" } },
            { "UseObjectsLimit", null }, // ??
            { "TargetShare", null }, // ??
            { "MountSerial", null }, // ??
            { "BladeSerial", null }, // ??
            { "BoneCutter", null }, // ??
            { "AutoMount", null }, // ??
            { "AutoBandage", null }, // ??
            { "AutoBandageTarget", null }, // ??
            { "AutoBandageScale", null }, // ??
            { "AutoBandageCount", null }, // ??
            { "AutoBandageStart", null }, // ??
            { "AutoBandageStartValue", null }, // ??
            { "AutoBandageDelay", null }, // ??
            { "AutoBandageFormula", null  }, // ??
            { "AutoBandageHidden", null }, // ??
            { "OpenDoors", new string[2] { "AutoOpenDoors", "System.Boolean" } },
            { "UseDoors", null }, // ??
            { "SpellsColor", null }, // ??
            { "SpellsMode", null }, // ??
            { "SpellsTargetShare", null }, // ??
            { "OpenDoorsMode", new string[2] { "AutoOpenDoorWhenHidden", "Variant" } }, // ??
            { "OpenCorpsesMode", new string[2] { "BlockOpenCorpsesTwice", "Variant" } }, // ??
            { "ShowMobileFlags", null }, // ??
            { "StateHighlightMode", null }, // ??
            { "StaticFields", null }, // ??
            { "CountStealthSteps", new string[2] {  "CountStealthSteps", "System.Boolean" } },
            { "FriendsListOnly", null }, // ??
            { "FriendsParty", null }, // ??
            { "MoveConflictingItems", null }, // ??
            { "CustomCaption", null }, // TitleBarDisplay ?? think this is a Boolean
            { "CustomCaptionMode", null }, // TitlebarImages ?? forgot what this does
            { "CustomCaptionText", null }, // RazorTitleBarText ?? this is in a different format and I'm lazy
            { "WarnCounters", new string[2] { "CounterWarn", "System.Boolean" } },
            { "WarnCountersValue", new string[2] { "CounterWarnAmount", "System.Int32" } },
            { "HighlightReagents", new string[2] { "HighlightReagents", "System.Boolean" } },
            { "DisplayCountersName", null }, // ?? Razor seems to store this per counter
            { "CaptionUseNotoHue", null }, // ??
            { "DisplayCountersImage", null }, // ?? Razor seems to store this per counter
            { "PreventDismount", new string[2] { "BlockDismount", "System.Boolean" } },
            { "PreventAttackFriends", null }, // ??
            { "AutoSearchContainers", new string[2] { "AutoSearch", "System.Boolean" } },
            { "AutoAcceptParty", new string[2] { "AutoAcceptParty", "System.Boolean" } },
            { "StaticFieldsMode", null } // ??
        };

        private static int GetSpellID(int circle, int num)
        {
            if (circle <= 8) // Mage
                return 3002011 + ((circle - 1) * 8) + (num - 1);
            if (circle == 10) // Necr
                return 1060509 + num - 1;
            if (circle == 20) // Chiv
                return 1060585 + num - 1;
            if (circle == 40) // Bush
                return 1060595 + num - 1;
            if (circle == 50) // Ninj
                return 1060610 + num - 1;
            if (circle == 60) // Elfs
                return 1071026 + num - 1;

            return -1;
        }

        private static Dictionary<string, LocString> _hotkeyMap = new Dictionary<string, LocString>()
        {
            // main.pingserver
            { "main.resynchronize", LocString.Resync },
            // main.snapshot
            { "actions.grabitem", LocString.GrabItem },
            // main.togglemounted
            { "actions.use.lastobject", LocString.LastObj },
            { "actions.use.lefthand", LocString.UseLeftHand },
            { "actions.use.righthand", LocString.UseRightHand },
            { "actions.shownames.all", LocString.AllNames },
            { "actions.shownames.corpses", LocString.AllCorpses },
            { "actions.shownames.mobiles", LocString.AllMobiles },
            { "actions.creatures.come", LocString.AllCome },
            { "actions.creatures.follow", LocString.AllFollow },
            { "actions.creatures.guard", LocString.AllGuard },
            { "actions.creatures.kill", LocString.AllKill },
            { "actions.creatures.stay", LocString.AllStay },
            { "actions.creatures.stop", LocString.AllStop },
            { "combat.abilities.primary", LocString.SetPrimAb },
            { "combat.abilities.secondary", LocString.SetSecAb },
            { "combat.abilities.stun", LocString.ToggleStun },
            { "combat.abilities.disarm", LocString.ToggleDisarm },
            // { "combat.attack.enemy", ?? }
            { "combat.attack.lasttarget", LocString.AttackLastTarg },
            { "combat.attack.lastcombatant", LocString.AttackLastComb },
            { "combat.bandage.self", LocString.BandageSelf },
            { "combat.bandage.last", LocString.BandageLT },
            { "combat.bandage.target", LocString.UseBandage },
            { "combat.consume.potions.agility", LocString.DrinkAg },
            { "combat.consume.potions.cure", LocString.DrinkCure },
            { "combat.consume.potions.explosion", LocString.DrinkExp },
            { "combat.consume.potions.heal", LocString.DrinkHeal },
            { "combat.consume.potions.refresh", LocString.DrinkRef },
            { "combat.consume.potions.strength", LocString.DrinkStr },
            { "combat.consume.potions.nightsight", LocString.DrinkNS },
            { "combat.togglehands.left", LocString.ArmDisarmLeft },
            { "combat.togglehands.right", LocString.ArmDisarmRight },
            { "skills.last", LocString.LastSkill },
            { "skills.anatomy", (LocString)(1044060 + 1) },
            { "skills.animallore", (LocString)(1044060 + 2) },
            { "skills.itemidentification",  (LocString)(1044060 + 3) },
            { "skills.armslore", (LocString)(1044060 + 4) },
            { "skills.begging", (LocString)(1044060 + 6) },
            { "skills.peacemaking", (LocString)(1044060 + 9) },
            { "skills.detectinghidden", (LocString)(1044060 + 14) },
            { "skills.discordance", (LocString)(1044060 + 15) },
            { "skills.evaluatingintelligence", (LocString)(1044060 + 16) },
            { "skills.hiding", (LocString)(1044060 + 21) },
            { "skills.provocation", (LocString)(1044060 + 22) },
            { "skills.inscription", (LocString)(1044060 + 23) },
            { "skills.poisoning", (LocString)(1044060 + 30) },
            { "skills.spiritspeak", (LocString)(1044060 + 32) },
            { "skills.stealing", (LocString)(1044060 + 33) },
            { "skills.animaltaming", (LocString)(1044060 + 35) },
            { "skills.tasteidentification", (LocString)(1044060 + 36) },
            { "skills.tracking", (LocString)(1044060 + 38) },
            { "skills.meditation", (LocString)(1044060 + 46) },
            { "skills.stealth", (LocString)(1044060 + 47) },
            { "skills.removetrap", (LocString)(1044060 + 48) },
            { "spells.last", LocString.LastSpell },
            { "spells.bigheal.self", LocString.GHealOrCureSelf },
            { "spells.miniheal.self", LocString.MiniHealOrCureSelf },
            { "spells.magery.clumsy", (LocString)GetSpellID(1, 1) },
            { "spells.magery.createfood", (LocString)GetSpellID(1, 2) },
            { "spells.magery.feeblemind", (LocString)GetSpellID(1, 3) },
            { "spells.magery.heal", (LocString)GetSpellID(1, 4) },
            { "spells.magery.magicarrow", (LocString)GetSpellID(1, 5) },
            { "spells.magery.nightsight", (LocString)GetSpellID(1, 6) },
            { "spells.magery.reactivearmor", (LocString)GetSpellID(1, 7) },
            { "spells.magery.weaken", (LocString)GetSpellID(1, 8) },
            { "spells.magery.agility", (LocString)GetSpellID(2, 1) },
            { "spells.magery.cunning", (LocString)GetSpellID(2, 2) },
            { "spells.magery.cure", (LocString)GetSpellID(2, 3) },
            { "spells.magery.harm", (LocString)GetSpellID(2, 4) },
            { "spells.magery.magictrap", (LocString)GetSpellID(2, 5) },
            { "spells.magery.magicuntrap", (LocString)GetSpellID(2, 6) },
            { "spells.magery.protection", (LocString)GetSpellID(2, 7) },
            { "spells.magery.strength", (LocString)GetSpellID(2, 8) },
            { "spells.magery.bless", (LocString)GetSpellID(3, 1) },
            { "spells.magery.fireball", (LocString)GetSpellID(3, 2) },
            { "spells.magery.magiclock", (LocString)GetSpellID(3, 3) },
            { "spells.magery.poison", (LocString)GetSpellID(3, 4) },
            { "spells.magery.telekinesis", (LocString)GetSpellID(3, 5) },
            { "spells.magery.teleport", (LocString)GetSpellID(3, 6) },
            { "spells.magery.unlock", (LocString)GetSpellID(3, 7) },
            { "spells.magery.wallofstone", (LocString)GetSpellID(3, 8) },
            { "spells.magery.archcure", (LocString)GetSpellID(4, 1) },
            { "spells.magery.archprotection", (LocString)GetSpellID(4, 2) },
            { "spells.magery.curse", (LocString)GetSpellID(4, 3) },
            { "spells.magery.firefield", (LocString)GetSpellID(4, 4) },
            { "spells.magery.greaterheal", (LocString)GetSpellID(4, 5) },
            { "spells.magery.lightning", (LocString)GetSpellID(4, 6) },
            { "spells.magery.manadrain", (LocString)GetSpellID(4, 7) },
            { "spells.magery.recall", (LocString)GetSpellID(4, 8) },
            { "spells.magery.bladespirits", (LocString)GetSpellID(5, 1) },
            { "spells.magery.dispelfield", (LocString)GetSpellID(5, 2) },
            { "spells.magery.incognito", (LocString)GetSpellID(5, 3) },
            { "spells.magery.magicreflection", (LocString)GetSpellID(5, 4) },
            { "spells.magery.mindblast", (LocString)GetSpellID(5, 5) },
            { "spells.magery.paralyze", (LocString)GetSpellID(5, 6) },
            { "spells.magery.poisonfield", (LocString)GetSpellID(5, 7) },
            { "spells.magery.summoncreature", (LocString)GetSpellID(5, 8) },
            { "spells.magery.dispel", (LocString)GetSpellID(6, 1) },
            { "spells.magery.energybolt", (LocString)GetSpellID(6, 2) },
            { "spells.magery.explosion", (LocString)GetSpellID(6, 3) },
            { "spells.magery.invisibility", (LocString)GetSpellID(6, 4) },
            { "spells.magery.mark", (LocString)GetSpellID(6, 5) },
            { "spells.magery.masscurse", (LocString)GetSpellID(6, 6) },
            { "spells.magery.paralyzefield", (LocString)GetSpellID(6, 7) },
            { "spells.magery.reveal", (LocString)GetSpellID(6, 8) },
            { "spells.magery.chainlightning", (LocString)GetSpellID(7, 1) },
            { "spells.magery.energyfield", (LocString)GetSpellID(7, 2) },
            { "spells.magery.flamestrike", (LocString)GetSpellID(7, 3) },
            { "spells.magery.gatetravel", (LocString)GetSpellID(7, 4) },
            { "spells.magery.manavampire", (LocString)GetSpellID(7, 5) },
            { "spells.magery.massdispel", (LocString)GetSpellID(7, 6) },
            { "spells.magery.meteorswarm", (LocString)GetSpellID(7, 7) },
            { "spells.magery.polymorph", (LocString)GetSpellID(7, 8) },
            { "spells.magery.summonairelemental", (LocString)GetSpellID(8, 1) },
            { "spells.magery.summonearthelemental", (LocString)GetSpellID(8, 2) },
            { "spells.magery.earthquake", (LocString)GetSpellID(8, 3) },
            { "spells.magery.energyvortex", (LocString)GetSpellID(8, 4) },
            { "spells.magery.summonfireelemental", (LocString)GetSpellID(8, 5) },
            { "spells.magery.resurrection", (LocString)GetSpellID(8, 6) },
            { "spells.magery.summondaemon", (LocString)GetSpellID(8, 7) },
            { "spells.magery.summonwaterelemental", (LocString)GetSpellID(8, 8) },
            // { "targeting.attack.enemy", ?? },
            { "targeting.attack.last", LocString.AttackLastTarg },
            { "targeting.friends.add", LocString.AddFriend },
            { "targeting.friends.remove", LocString.RemoveFriend },
            { "targeting.get.enemy.closest.any", LocString.TargCloseEnemy },
            { "targeting.get.enemy.closest.criminal.any", LocString.TargCloseCriminal },
            // { "targeting.get.enemy.closest.criminal.both", ?? },
            { "targeting.get.enemy.closest.criminal.humanoid", LocString.TargCloseCriminalHuman },
            // { "targeting.get.enemy.closest.criminal.transformation", ?? },
            { "targeting.get.enemy.closest.enemy.any", LocString.TargCloseEnemy },
            // { "targeting.get.enemy.closest.enemy.both", ?? },
            { "targeting.get.enemy.closest.enemy.humanoid", LocString.TargCloseEnemyHuman },
            // { "targeting.get.enemy.closest.enemy.transformation", ?? },
            { "targeting.get.enemy.closest.gray.any", LocString.TargCloseGrey },
            // { "targeting.get.enemy.closest.gray.both", ?? },
            { "targeting.get.enemy.closest.gray.humanoid", LocString.TargCloseGreyHuman },
            // { "targeting.get.enemy.closest.gray.transformation", ?? },
            { "targeting.get.enemy.closest.innocent.any", LocString.TargCloseBlue },
            // { "targeting.get.enemy.closest.innocent.both", ?? },
            { "targeting.get.enemy.closest.innocent.humanoid", LocString.TargCloseInnocentHuman },
            // { "targeting.get.enemy.closest.innocent.transformation", ?? },           
            { "targeting.get.enemy.closest.murderer.any", LocString.TargCloseRed },
            // { "targeting.get.enemy.closest.murderer.both", ?? },
            { "targeting.get.enemy.closest.murderer.humanoid", LocString.TargCloseRedHumanoid },
            // { "targeting.get.enemy.closest.murderer.transformation", ?? },
            { "targeting.get.enemy.closest.non-friendly.any", LocString.TargCloseNFriend },
            // { "targeting.get.enemy.closest.non-friendly.both", ?? },
            { "targeting.get.enemy.closest.non-friendly.humanoid", LocString.TargClosestNFriendlyHuman },
            // { "targeting.get.enemy.closest.non-friendly.transformation", ?? },


            { "targeting.set.enemy", LocString.SetLTHarm },
            { "targeting.set.friend", LocString.SetLTBene },
            { "targeting.set.last", LocString.SetLT },
            // { "targeting.set.mount", ?? }
        };

        public static bool Import(string steamProfileFile, Profile profile)
        {
            if (!File.Exists(steamProfileFile))
                return false;

            XmlDocument doc = new XmlDocument();

            doc.Load(steamProfileFile);

            XmlElement root = doc["profile"];
            if (root == null)
                return false;

            Assembly exe = Assembly.GetCallingAssembly();
            if (exe == null)
                return false;

            foreach (XmlElement el in root.GetElementsByTagName("data"))
            {
                string steamName = el.GetAttribute("name");

                if (!_dataMap.TryGetValue(steamName, out string[] razorMapping) || razorMapping == null)
                    continue;

                string typeStr = razorMapping[1];
                string val = el.InnerText;

                if (typeStr == "Variant")
                {
                    int value = Convert.ToInt32(val, 16);
                    profile.SetProperty(razorMapping[0], Convert.ToBoolean(value));
                }
                else
                {
                    Type type = Type.GetType(typeStr);
                    if (type == null)
                        type = exe.GetType(typeStr);

                    if (profile.HasProperty(razorMapping[0]))
                    {
                        if (type == null)
                            profile.RemoveProperty(razorMapping[0]);
                        else if (type.Name == "Int32")
                            profile.SetProperty(razorMapping[0], Convert.ToInt32(val, 16));
                        else
                            profile.SetProperty(razorMapping[0], profile.GetObjectFromString(val, type));
                    }
                }
            }

            foreach (XmlElement el in root["hotkeys"].GetElementsByTagName("hotkey"))
            {
                try
                {
                    string action = el.GetAttribute("action");

                    if (!_hotkeyMap.TryGetValue(action, out LocString razorAction))
                        continue;

                    int keyMask = Convert.ToInt32(el.GetAttribute("key"), 16);
                    int key = keyMask & 0xFF;

                    if (key == 4) key = -3;
                    else if (key == 259) key = -4;
                    else if (key == 260) key = -5;

                    ModKeys mod = ModKeys.None;

                    if ((keyMask & 0x200) != 0)
                        mod = ModKeys.Shift;
                    else if ((keyMask & 0x400) != 0)
                        mod = ModKeys.Control;
                    else if ((keyMask & 0x800) != 0)
                        mod = ModKeys.Alt;

                    string pass = el.GetAttribute("pass");

                    KeyData k = HotKey.Get((int)razorAction);

                    if (k != null)
                    {
                        k.Mod = mod;
                        k.Key = key;
                        k.SendToUO = Convert.ToBoolean(pass);
                    }
                }
                catch
                {
                }
            }

            return false;
        }
    }
}