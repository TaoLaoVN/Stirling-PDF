using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace MaPDF
{
    static class Program
    {
        private static NotifyIcon trayIcon;
        private static Process backendProcess = null;
        private static int port = 8080;
        private static string appUrl = "http://localhost:8080/";
        private static string baseDir = "";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string iconPath = Path.Combine(baseDir, @"frontend\editor\src-tauri\icons\windows\app.ico");
            Icon appIcon = null;
            if (File.Exists(iconPath))
            {
                try { appIcon = new Icon(iconPath); } catch { }
            }
            if (appIcon == null)
            {
                appIcon = SystemIcons.Application;
            }

            // Create System Tray Icon
            trayIcon = new NotifyIcon();
            trayIcon.Icon = appIcon;
            trayIcon.Text = "MaPDF - Tieng Viet";
            trayIcon.Visible = true;

            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add(new MenuItem("Mo MaPDF (Cua so ung dung)", (s, e) => OpenAppWindow()));
            menu.MenuItems.Add(new MenuItem("Mo tren Trinh duyet", (s, e) => Process.Start(appUrl)));
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem("Thoat hoan toan", (s, e) => ExitApp()));
            trayIcon.ContextMenu = menu;
            trayIcon.DoubleClick += (s, e) => OpenAppWindow();

            // Start backend in background thread so UI stays responsive
            Thread bgThread = new Thread(() =>
            {
                EnsureBackendRunning();
                OpenAppWindow();
            });
            bgThread.IsBackground = true;
            bgThread.Start();

            Application.Run();
        }

        static void EnsureBackendRunning()
        {
            if (IsServerOnline(appUrl))
            {
                return;
            }

            string jarPath = Path.Combine(baseDir, @"app\core\build\libs\stirling-pdf-3.1.0.jar");
            if (!File.Exists(jarPath))
            {
                trayIcon.ShowBalloonTip(5000, "MaPDF - Loi", "Khong tim thay file jar: " + jarPath, ToolTipIcon.Error);
                return;
            }

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string javaExe = Path.Combine(userProfile, @".gradle\jdks\eclipse_adoptium-25-amd64-windows.2\bin\java.exe");
            if (!File.Exists(javaExe))
            {
                javaExe = "java.exe";
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = javaExe;
            psi.Arguments = string.Format("-jar \"{0}\" --server.port={1}", jarPath, port);
            psi.WorkingDirectory = baseDir;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            try
            {
                backendProcess = Process.Start(psi);
                trayIcon.ShowBalloonTip(4000, "MaPDF", "Dang khoi dong backend...", ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                trayIcon.ShowBalloonTip(5000, "MaPDF - Loi", "Khong the khoi dong Java: " + ex.Message, ToolTipIcon.Error);
                return;
            }

            // Wait for backend to be ready
            int attempts = 0;
            while (attempts < 60 && !IsServerOnline(appUrl))
            {
                Thread.Sleep(1000);
                attempts++;
            }

            if (IsServerOnline(appUrl))
            {
                trayIcon.ShowBalloonTip(3000, "MaPDF", "Backend da san sang!", ToolTipIcon.Info);
            }
        }

        static void OpenAppWindow()
        {
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            if (!File.Exists(edgePath))
            {
                edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
            }

            string userDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MaPDF_UserData"
            );
            string edgeArgs = string.Format(
                "--app=\"{0}\" --user-data-dir=\"{1}\" --window-size=1300,850",
                appUrl,
                userDataDir
            );

            if (File.Exists(edgePath))
            {
                ProcessStartInfo edgePsi = new ProcessStartInfo(edgePath, edgeArgs);
                edgePsi.UseShellExecute = false;
                Process.Start(edgePsi);
            }
            else
            {
                Process.Start(appUrl);
            }
        }

        static bool IsServerOnline(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 1000;
                req.Method = "GET";
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    return resp.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        static void ExitApp()
        {
            trayIcon.Visible = false;
            if (backendProcess != null && !backendProcess.HasExited)
            {
                try { backendProcess.Kill(); } catch { }
            }
            Application.Exit();
        }
    }
}
