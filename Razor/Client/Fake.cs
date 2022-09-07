#region license
// Razor: An Ultima Online Assistant
// Copyright (c) 2022 Razor Development Community on GitHub <https://github.com/markdwags/Razor>
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
using System.Diagnostics;
using System.Net;

namespace Assistant
{
    public unsafe class FakeClient : Client
    {
        public enum UONetMessage
        {
            Send = 1,
            Recv = 2,
            Ready = 3,
            NotReady = 4,
            Connect = 5,
            Disconnect = 6,
            KeyDown = 7,
            Mouse = 8,
            Activate = 9,
            Focus = 10,
            Close = 11,
            StatBar = 12,
            NotoHue = 13,
            DLL_Error = 14,
            SetGameSize = 19,
            SmartCPU = 21,
            Negotiate = 22,
            SetMapHWnd = 23,
            OnTick = 24,
        }

        public enum UONetMessageCopyData
        {
            Position = 1,
        }

        private enum InitError
        {
            SUCCESS,
            NO_UOWND,
            NO_TID,
            NO_HOOK,
            NO_SHAREMEM,
            LIB_DISABLED,
            NO_PATCH,
            NO_MEMCOPY,
            INVALID_PARAMS,

            UNKNOWN
        }

        public override DateTime ConnectionStart
        {
            get { return DateTime.Now; }
        }

        public override IPAddress LastConnection
        {
            get { return IPAddress.Loopback; }
        }

        public override Process ClientProcess
        {
            get { return Process.GetCurrentProcess(); }
        }

        public override bool ClientRunning
        {
            get
            {
                return true;
            }
        }

        public override void RequestStatbarPatch(bool preAOS)
        {
        }

        public override void SetCustomNotoHue(int hue)
        {
        }

        public override void SetSmartCPU(bool enabled)
        {
        }

        public override void SetGameSize(int x, int y)
        {
        }

        public override Loader_Error LaunchClient(string client)
        {
            Language.Load("ENU");
            return Loader_Error.SUCCESS;
        }

        public override bool ClientEncrypted
        {
            get { return false; }
            set { }
        }

        public override bool ServerEncrypted
        {
            get { return false; }
            set { }
        }

        public override bool InstallHooks(IntPtr mainWindow)
        {
            return true;
        }

        public override void SetConnectionInfo(IPAddress addr, int port)
        {
        }

        public override void SetNegotiate(bool negotiate)
        {
        }

        public override bool Attach(int pid)
        {
            return true;
        }

        public override void Close()
        {
        }

        public override void UpdateTitleBar()
        {
        }

        public override void SetTitleStr(string str)
        {
        }

        public override bool OnMessage(MainForm razor, uint wParam, int lParam)
        {
            return true;
        }

        public override bool OnCopyData(IntPtr wparam, IntPtr lparam)
        {
            return true;
        }

        public override void SendToServer(Packet p)
        {
        }

        public override void SendToServer(PacketReader pr)
        {
        }

        public override void SendToClient(Packet p)
        {
        }

        public override void SendPacketToClient(byte[] packet, int length)
        {
        }

        public override void ForceSendToClient(Packet p)
        {
        }

        public override void ForceSendToServer(Packet p)
        {
        }

        public override void SetPosition(uint x, uint y, uint z, byte dir)
        {
        }

        public void KeyPress(int keyCode)
        {
        }

        public override string GetClientVersion()
        {
            return "0.0.0.0";
        }

        public override string GetUoFilePath()
        {
            return "";
        }

        public override IntPtr GetWindowHandle()
        {
            return IntPtr.Zero;
        }

        public override uint TotalDataIn()
        {
            return 0;
        }

        public override uint TotalDataOut()
        {
            return 0;
        }

        internal override void RequestMove(Direction m_Dir)
        {
        }
    }
};