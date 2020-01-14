using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assistant;

namespace UOSteam
{
    public static class Commands
    {
        private static void DummyCommand(ref ASTNode node, bool quiet, bool force)
        {
            Console.WriteLine("Executing command {0} {1}", node.Type, node.Lexeme);

            node = null;
        }

        static Commands()
        {
            // Commands. From UOSteam Documentation
            Interpreter.RegisterCommandHandler("fly", DummyCommand);
            Interpreter.RegisterCommandHandler("land", DummyCommand);
            Interpreter.RegisterCommandHandler("setability", SetAbility);
            Interpreter.RegisterCommandHandler("attack", DummyCommand);
            Interpreter.RegisterCommandHandler("clearhands", ClearHands);
            Interpreter.RegisterCommandHandler("clickobject", ClickObject);
            Interpreter.RegisterCommandHandler("bandageself", BandageSelf);
            Interpreter.RegisterCommandHandler("usetype", UseType);
            Interpreter.RegisterCommandHandler("useobject", UseObject);
            Interpreter.RegisterCommandHandler("useonce", DummyCommand);
            Interpreter.RegisterCommandHandler("cleanusequeue", DummyCommand);
            Interpreter.RegisterCommandHandler("moveitem", DummyCommand);
            Interpreter.RegisterCommandHandler("moveitemoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("movetype", DummyCommand);
            Interpreter.RegisterCommandHandler("movetypeoffset", DummyCommand);
            Interpreter.RegisterCommandHandler("walk", Walk);
            Interpreter.RegisterCommandHandler("turn", Turn);
            Interpreter.RegisterCommandHandler("run", Run);
            Interpreter.RegisterCommandHandler("useskill", UseSkill);
            Interpreter.RegisterCommandHandler("feed", DummyCommand);
            Interpreter.RegisterCommandHandler("rename", DummyCommand);
            Interpreter.RegisterCommandHandler("shownames", DummyCommand);
            Interpreter.RegisterCommandHandler("togglehands", DummyCommand);
            Interpreter.RegisterCommandHandler("equipitem", DummyCommand);
            Interpreter.RegisterCommandHandler("togglemounted", DummyCommand);
            Interpreter.RegisterCommandHandler("equipwand", DummyCommand);
            Interpreter.RegisterCommandHandler("buy", DummyCommand);
            Interpreter.RegisterCommandHandler("sell", DummyCommand);
            Interpreter.RegisterCommandHandler("clearbuy", DummyCommand);
            Interpreter.RegisterCommandHandler("clearsell", DummyCommand);
            Interpreter.RegisterCommandHandler("organizer", DummyCommand);
            Interpreter.RegisterCommandHandler("autoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("dress", DummyCommand);
            Interpreter.RegisterCommandHandler("undress", DummyCommand);
            Interpreter.RegisterCommandHandler("dressconfig", DummyCommand);
            Interpreter.RegisterCommandHandler("toggleautoloot", DummyCommand);
            Interpreter.RegisterCommandHandler("togglescavenger", DummyCommand);
            Interpreter.RegisterCommandHandler("counter", DummyCommand);
            Interpreter.RegisterCommandHandler("unsetalias", DummyCommand);
            Interpreter.RegisterCommandHandler("setalias", SetAlias);
            Interpreter.RegisterCommandHandler("promptalias", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforgump", DummyCommand);
            Interpreter.RegisterCommandHandler("replygump", DummyCommand);
            Interpreter.RegisterCommandHandler("closegump", DummyCommand);
            Interpreter.RegisterCommandHandler("clearjournal", DummyCommand);
            Interpreter.RegisterCommandHandler("waitforjournal", DummyCommand);
            Interpreter.RegisterCommandHandler("poplist", DummyCommand);
            Interpreter.RegisterCommandHandler("pushlist", DummyCommand);
            Interpreter.RegisterCommandHandler("removelist", DummyCommand);
            Interpreter.RegisterCommandHandler("createlist", DummyCommand);
            Interpreter.RegisterCommandHandler("clearlist", DummyCommand);
            Interpreter.RegisterCommandHandler("info", DummyCommand);
            Interpreter.RegisterCommandHandler("pause", DummyCommand);
            Interpreter.RegisterCommandHandler("ping", DummyCommand);
            Interpreter.RegisterCommandHandler("playmacro", DummyCommand);
            Interpreter.RegisterCommandHandler("playsound", DummyCommand);
            Interpreter.RegisterCommandHandler("resync", DummyCommand);
            Interpreter.RegisterCommandHandler("snapshot", DummyCommand);
            Interpreter.RegisterCommandHandler("hotkeys", DummyCommand);
            Interpreter.RegisterCommandHandler("where", DummyCommand);
            Interpreter.RegisterCommandHandler("messagebox", DummyCommand);
            Interpreter.RegisterCommandHandler("mapuo", DummyCommand);
            Interpreter.RegisterCommandHandler("clickscreen", DummyCommand);
            Interpreter.RegisterCommandHandler("paperdoll", DummyCommand);
            Interpreter.RegisterCommandHandler("helpbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("guildbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("questsbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("logoutbutton", DummyCommand);
            Interpreter.RegisterCommandHandler("virtue", DummyCommand);
            Interpreter.RegisterCommandHandler("msg", DummyCommand);
            Interpreter.RegisterCommandHandler("headmsg", DummyCommand);
            Interpreter.RegisterCommandHandler("sysmsg", DummyCommand);

        }

        private static string[] abilities = new string[4] { "primary", "secondary", "stun", "disarm" };
        private static void SetAbility(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            // expect two STRING nodes

            ASTNode ability = node.Next();
            ASTNode state = node.Next();

            if (ability == null || state == null)
                throw new ArgumentException("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

            if (!abilities.Contains(ability.Lexeme))
                throw new ArgumentException("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

            if (state.Lexeme != "on" && state.Lexeme != "off")
                throw new ArgumentException("Usage: setability ('primary'/'secondary'/'stun'/'disarm') ['on'/'off']");

            if (state.Lexeme == "off")
            {
                Client.Instance.SendToServer(new UseAbility(AOSAbility.Clear));
                Client.Instance.SendToClient(ClearAbility.Instance);
            }
            else
            {
                switch (ability.Lexeme)
                {
                    case "primary":
                        SpecialMoves.SetPrimaryAbility();
                        break;
                    case "secondary":
                        SpecialMoves.SetSecondaryAbility();
                        break;
                    case "stun":
                        Client.Instance.SendToServer(new StunRequest());
                        break;
                    case "disarm":
                        Client.Instance.SendToServer(new DisarmRequest());
                        break;
                    default:
                        break;
                }
            }
        }

        private static string[] hands = new string[3] { "left", "right", "both" };
        private static void ClearHands(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            // expect one STRING node

            ASTNode hand = node.Next();

            if (hand == null)
                throw new ArgumentException("Usage: clearhands ('left'/'right'/'both')");

            if (!hands.Contains(hand.Lexeme))
                throw new ArgumentException("Usage: clearhands ('left'/'right'/'both')");


            Item leftHand = World.Player.GetItemOnLayer(Layer.LeftHand);
            Item rightHand = World.Player.GetItemOnLayer(Layer.RightHand);

            switch (hand.Lexeme)
            {
                case "left":
                    if (leftHand != null)
                        DragDropManager.DragDrop(leftHand, World.Player.Backpack);
                    break;
                case "right":
                    if (rightHand != null)
                        DragDropManager.DragDrop(rightHand, World.Player.Backpack);
                    break;
                default:
                    if (leftHand != null)
                        DragDropManager.DragDrop(leftHand, World.Player.Backpack);
                    if (rightHand != null)
                        DragDropManager.DragDrop(rightHand, World.Player.Backpack);
                    break;
            }
        }

        private static void ClickObject(ref ASTNode node, bool quiet, bool force)
        {
            node.Next(); // walk past COMMAND

            // expect one SERIAL node

            ASTNode obj = node.Next();

            if (obj == null)
                throw new ArgumentException("Usage: clickobject (serial)");

            Serial serial = Serial.Parse(obj.Lexeme);

            if (!serial.IsValid)
                throw new ArgumentException("Invalid Serial in clickobject");

            Client.Instance.SendToServer(new SingleClick(serial));
        }
        private static bool UseItem(Item cont, ushort find)
        {
            if (!Client.Instance.AllowBit(FeatureBit.PotionHotkeys))
                return false;

            for (int i = 0; i < cont.Contains.Count; i++)
            {
                Item item = (Item)cont.Contains[i];

                if (item.ItemID == find)
                {
                    PlayerData.DoubleClick(item);
                    return true;
                }
                else if (item.Contains != null && item.Contains.Count > 0)
                {
                    if (UseItem(item, find))
                        return true;
                }
            }

            return false;
        }

        private static void BandageSelf(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            if (World.Player == null)
                return;

            Item pack = World.Player.Backpack;
            if (pack != null)
            {
                if (!UseItem(pack, 3617))
                {
                    World.Player.SendMessage(MsgLevel.Warning, LocString.NoBandages);
                }
                else
                {
                    if (force)
                    {
                        Targeting.ClearQueue();
                        Targeting.TargetSelf(true);
                    }
                    else
                        Targeting.TargetSelf(true);
                }
            }
        }

        private static void UseType(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // variable args here
                
        }

        private static void UseObject(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // expect a SERIAL node

            ASTNode obj = node.Next();

            if (obj == null)
                throw new ArgumentException("Usage: useobject (serial)");

            Serial serial = Serial.Parse(obj.Lexeme);

            if (!serial.IsValid)
                throw new ArgumentException("Invalid Serial in useobject");

            Client.Instance.SendToServer(new DoubleClick(serial));
        }

        private static void Walk(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();
        }
        private static void Turn(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();
        }
        private static void Run(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();
        }

        private static Dictionary<string, int> UsableSkills = new Dictionary<string, int>()
        {
            { "anatomy", 1 }, // anatomy
            { "animallore", 2 }, // animal lore
            { "itemidentification", 3 }, // item identification
            { "armslore", 4 }, // arms lore
            { "begging", 6 }, // begging
            { "peacemaking", 9 }, // peacemaking
            { "cartography", 12 }, // cartography
            { "detectinghidden", 14 }, // detect hidden
            { "discordance", 15 }, // Discordance
            { "evaluatingintelligence", 16 }, // evaluate intelligence
            { "forensicevaluation", 19 }, // forensic evaluation
            { "hiding", 21 }, // hiding
            { "provocation", 22 }, // provocation
            { "inscription", 23 }, // inscription
            { "poisoning", 30 }, // poisoning
            { "spiritspeak", 32 }, // spirit speak
            { "stealing", 33 }, // stealing
            { "taming", 35 }, // taming
            { "tasteidentification", 36 }, // taste id
            { "tracking", 38 }, // tracking
            { "meditation", 46 }, // Meditation
            { "stealth", 47 }, // Stealth
            { "removetrap", 48 } // RemoveTrap
        };

        private static void UseSkill(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            // expect one string node or "last"

            ASTNode skill = node.Next();

            if (node.Lexeme == "last")
                Client.Instance.SendToServer(new UseSkill(World.Player.LastSkill));
            else if(UsableSkills.TryGetValue(node.Lexeme, out int skillId))
            {
                Client.Instance.SendToServer(new UseSkill(skillId));
            }
        }

        private static void SetAlias(ref ASTNode node, bool quiet, bool force)
        {
            node.Next();

            ASTNode alias = node.Next();
            ASTNode value = node.Next();

            if (alias == null || value == null)
                throw new ArgumentException("Usage: setalias (string) (serial/string)");

            int obj;

            if (value.Type == ASTNodeType.STRING)
                obj = Interpreter.GetAlias(ref value);
            else
                obj = Convert.ToInt32(value.Lexeme);

            Interpreter.SetAlias(alias.Lexeme, obj);
        }
    }
}
