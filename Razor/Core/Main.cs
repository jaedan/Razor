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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Principal;
using Assistant.Core;
using Assistant.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Assistant
{
    public partial class Engine
    {
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Client.Instance.Close();
                m_Running = false;

                new MessageDialog("Unhandled Exception", !e.IsTerminating, e.ExceptionObject.ToString()).ShowDialog(
                    Engine.ActiveWindow);
            }

            LogCrash(e.ExceptionObject as Exception);
        }

        public static void LogCrash(object exception)
        {
            if (exception == null || (exception is ThreadAbortException))
                return;

            using (StreamWriter txt = new StreamWriter("Crash.log", true))
            {
                txt.AutoFlush = true;
                txt.WriteLine("Exception @ {0}", Engine.MistedDateTime.ToString("MM-dd-yy HH:mm:ss.ffff"));
                txt.WriteLine(exception.ToString());
                txt.WriteLine("");
                txt.WriteLine("");
            }
        }

        private static Version m_ClientVersion = null;

        public static Version ClientVersion
        {
            get
            {
                if (m_ClientVersion == null || m_ClientVersion.Major < 2)
                {
                    string[] split = Client.Instance.GetClientVersion().Split('.');

                    if (split.Length < 3)
                        return new Version(4, 0, 0, 0);

                    int rev = 0;

                    if (split.Length > 3)
                        rev = Utility.ToInt32(split[3], 0);

                    m_ClientVersion = new Version(
                        Utility.ToInt32(split[0], 0),
                        Utility.ToInt32(split[1], 0),
                        Utility.ToInt32(split[2], 0),
                        rev);

                    if (m_ClientVersion.Major == 0) // sanity check if the client returns 0.0.0.0
                        m_ClientVersion = new Version(4, 0, 0, 0);
                }

                return m_ClientVersion;
            }
        }

        public static bool UseNewMobileIncoming
        {
            get
            {
                if (ClientVersion.Major > 7)
                {
                    return true;
                }
                else if (ClientVersion.Major == 7)
                {
                    if (ClientVersion.Minor > 0 || ClientVersion.Build >= 33)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool UsePostHSChanges
        {
            get
            {
                if (ClientVersion.Major > 7)
                {
                    return true;
                }
                else if (ClientVersion.Major == 7)
                {
                    if (ClientVersion.Minor > 0)
                    {
                        return true;
                    }
                    else if (ClientVersion.Build >= 9)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool UsePostSAChanges
        {
            get
            {
                if (ClientVersion.Major >= 7)
                {
                    return true;
                }

                return false;
            }
        }

        public static bool UsePostKRPackets
        {
            get
            {
                if (ClientVersion.Major >= 7)
                {
                    return true;
                }
                else if (ClientVersion.Major >= 6)
                {
                    if (ClientVersion.Minor == 0)
                    {
                        if (ClientVersion.Build == 1)
                        {
                            if (ClientVersion.Revision >= 7)
                                return true;
                        }
                        else if (ClientVersion.Build > 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static string ExePath
        {
            get { return Process.GetCurrentProcess().MainModule.FileName; }
        }

        public static MainForm MainWindow
        {
            get { return m_MainWnd; }
        }

        public static RazorScriptEditor RazorScriptEditorWindow
        {
            get { return m_RazorScriptEditorWnd; }
            set { m_RazorScriptEditorWnd = value; }
        }

        public static bool Running
        {
            get { return m_Running; }
        }

        public static Form ActiveWindow
        {
            get { return m_ActiveWnd; }
            set { m_ActiveWnd = value; }
        }

        public static string Version
        {
            get
            {
                if (m_Version == null)
                {
                    Version v = Assembly.GetCallingAssembly().GetName().Version;
                    m_Version = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}"; //, v.Revision
                }

                return m_Version;
            }
        }

        private static MainForm m_MainWnd;

        private static RazorScriptEditor m_RazorScriptEditorWnd;

        private static Form m_ActiveWnd;

        //private static Thread m_TimerThread;
        private static bool m_Running;
        private static string m_Version;

        private static int _previousHour = -1;
        private static int _Differential;

        public static int Differential //to use in all cases where you rectify normal clocks obtained with utctimer!
        {
            get
            {
                if (_previousHour != DateTime.UtcNow.Hour)
                {
                    _previousHour = DateTime.UtcNow.Hour;
                    _Differential = DateTimeOffset.Now.Offset.Hours;
                }

                return _Differential;
            }
        }

        public static DateTime MistedDateTime
        {
            get { return DateTime.UtcNow.AddHours(Differential); }
        }

        public static CultureInfo Culture;

        private static bool LaunchClient()
        {
            SplashScreen.Message = LocString.LoadingClient;
            string clientPath = Ultima.Files.GetFilePath("client.exe");
            if (clientPath == null || !File.Exists(clientPath))
            {
#if DEBUG
#else
                MessageBox.Show(SplashScreen.Instance,
                    $"Unable to find the client specified.\n\"{(clientPath != null ? clientPath : "-null-")}\"", "Could Not Find Client",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
#endif
            }

            var result = Client.Instance.LaunchClient(clientPath);
            if (result != Client.Loader_Error.SUCCESS)
            {
#if DEBUG
#else
                MessageBox.Show(SplashScreen.Instance,
                    String.Format("Unable to launch the client specified. (Error: {1})\n \"{0}\"",
                        clientPath != null ? clientPath : "-null-", result),
                    "Could Not Start Client", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
#endif
            }

            return true;
        }

        public static void Load()
        {
            Culture = new CultureInfo("en-US", false);
            Culture.NumberFormat.NumberDecimalSeparator = ".";
            Culture.NumberFormat.NumberGroupSeparator = ",";

            /* Load localization files */
            string defLang = Config.GetAppSetting<string>("DefaultLanguage");
            if (defLang == null)
            {
                defLang = "ENU";
            }

            m_Running = true;

            /* Load settings from configuration file */
            Client.Instance.ClientEncrypted = Config.GetAppSetting<int>("ClientEncrypted") == 1;
            Client.Instance.ServerEncrypted = Config.GetAppSetting<int>("ServerEncrypted") == 1;

            /* Initialize engine */
            SplashScreen.Message = LocString.Initializing;
            Initialize(typeof(Engine).Assembly);

            /* Load Profile */
            SplashScreen.Message = LocString.LoadingLastProfile;
            Config.LoadCharList();
            Overrides.Load();
            if (!Config.LoadLastProfile())
                MessageBox.Show(
                    "The selected profile could not be loaded, using default instead.", "Profile Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            /* Start client */
            if (!LaunchClient())
            {
                SplashScreen.End();
                return;
            }

            string addr = Config.GetAppSetting<string>("LastServer");
            int port = Config.GetAppSetting<int>("LastPort");

            IPAddress ip = Resolve(addr);
            if (ip == IPAddress.None || port == 0)
            {
                MessageBox.Show(SplashScreen.Instance, Language.GetString(LocString.BadServerAddr),
                    "Bad Server Address", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                SplashScreen.End();
                return;
            }

            Client.Instance.SetConnectionInfo(ip, port);

            SplashScreen.Message = LocString.WaitingForClient;
        }

        public static void Close()
        {
            m_Running = false;

            Client.Instance.Close();
            Counter.Save();
            Macros.MacroManager.Save();
            Config.Save();
        }

        public static void RunUI()
        {
            Thread.CurrentThread.Name = "Razor UI Thread";

            m_MainWnd = new MainForm();
            m_MainWnd.Show();
            Application.Run(m_MainWnd);
        }

        public static IServiceProvider? Services { get; private set; }

        private static void WireupServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddWindowsFormsBlazorWebView();

    #if DEBUG
            services.AddBlazorWebViewDeveloperTools();
    #endif
        }

        [STAThread]
        public static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException +=
 new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
			Directory.SetCurrentDirectory( Config.GetInstallDirectory() );
#endif

            var host = Host.CreateDefaultBuilder()
                        .ConfigureServices(WireupServices)
                        .Build();
            Services = host.Services;

            /* Load localization files */
            string defLang = Config.GetAppSetting<string>("DefaultLanguage");
            if (defLang == null)
            {
                defLang = "ENU";
            }

            Client.Init(true);

            Load();
            RunUI();
            Close();
        }

        private static string _rootPath = null;

        public static string RootPath =>
            _rootPath ?? (_rootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Engine)).Location));

        /*public static string GetDirectory( string relPath )
        {
            string path = Path.Combine(ExeDirectory, relPath);
            EnsureDirectory( path );
            return path;
        }*/

        public static void EnsureDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private static void Initialize(Assembly a)
        {
            Type[] types = a.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].Namespace == "Ultima")
                    continue;
                MethodInfo init = types[i].GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

                if (init != null)
                    init.Invoke(null, null);
            }
        }

        private static IPAddress Resolve(string addr)
        {
            IPAddress ipAddr = IPAddress.None;

            if (string.IsNullOrEmpty(addr))
                return ipAddr;

            try
            {
                ipAddr = IPAddress.Parse(addr);
            }
            catch
            {
                try
                {
                    IPHostEntry iphe = Dns.GetHostEntry(addr);

                    if (iphe.AddressList.Length > 0)
                        ipAddr = iphe.AddressList[iphe.AddressList.Length - 1];
                }
                catch
                {
                }
            }

            return ipAddr;
        }

        public static bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}