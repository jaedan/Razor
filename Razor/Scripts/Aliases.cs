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
using System.Linq;
using System.Text;
using UOScript;
using Assistant;

namespace Assistant.Scripts
{
    public static class Aliases
    {

        public static void Register()
        {
            Interpreter.RegisterAliasHandler("backpack", Backpack);
            Interpreter.RegisterAliasHandler("bank", Bank);
            Interpreter.RegisterAliasHandler("enemy", Enemy);
            Interpreter.RegisterAliasHandler("last", Last);
            Interpreter.RegisterAliasHandler("lasttarget", Last);
            Interpreter.RegisterAliasHandler("lastobject", LastObject);
            Interpreter.RegisterAliasHandler("self", Self);
            Interpreter.RegisterAliasHandler("mount", Mount);
            Interpreter.RegisterAliasHandler("righthand", RightHand);
            Interpreter.RegisterAliasHandler("lefthand", LeftHand);
        }

        private static uint Mount(string alias)
        {
            if (World.Player == null)
                return 0;

            var mount = World.Player.GetItemOnLayer(Layer.Mount);

            if (mount == null)
                return 0;

            return mount.Serial;
        }

        private static uint RightHand(string alias)
        {
            if (World.Player == null)
                return 0;

            Item i = World.Player.GetItemOnLayer(Layer.RightHand);

            if (i == null)
                return 0;

            return i.Serial;
        }

        private static uint LeftHand(string alias)
        {
            if (World.Player == null)
                return 0;

            Item i = World.Player.GetItemOnLayer(Layer.LeftHand);

            if (i == null)
                return 0;

            return i.Serial;
        }

        private static uint Backpack(string alias)
        {
            if (World.Player == null || World.Player.Backpack == null)
                return 0;

            return World.Player.Backpack.Serial;
        }

        private static uint Bank(string alias)
        {
            // unsupported?  I can't find a reference to the bankbox in the player
            return uint.MaxValue;
        }

        private static uint Enemy(string alias)
        {
            // we will need to modify the PlayerData class to keep track of the current enemy to make this work
            return uint.MaxValue;
        }

        private static uint Friend(string alias)
        {
            // we will need to modify the PlayerData class to keep track of the current enemy to make this work
            return uint.MaxValue;
        }

        private static uint Ground(string alias)
        {
            // not sure how to return the serial of the ground at your current position
            return uint.MaxValue;
        }

        private static uint Last(string alias)
        {
            if (Targeting.LastTargetInfo == null)
                return 0;

            return Targeting.LastTargetInfo.Serial;
        }

        private static uint LastObject(string alias)
        {
            if (World.Player.LastObject != null)
                return World.Player.LastObject;

            return 0;
        }

        private static uint Self(string alias)
        {
            if (World.Player == null)
                return 0;

            return World.Player.Serial;
        }
    }
}
