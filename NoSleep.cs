// NoSleep: 실행 중인 동안 시스템 유휴 절전을 막는다. 디스플레이는 꺼질 수 있다.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

static class NoSleep
{
    [DllImport("kernel32.dll")]
    static extern uint SetThreadExecutionState(uint esFlags);

    const uint ES_CONTINUOUS = 0x80000000;
    const uint ES_SYSTEM_REQUIRED = 0x00000001;

    const string AppName = "NoSleep";
    const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    // 작업 관리자 "시작 앱"에서 사용 안 함으로 바꾸면 여기에 기록된다.
    const string ApprovedKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    [STAThread]
    static void Main()
    {
        bool isNew;
        using (new Mutex(true, "NoSleep_SingleInstance", out isNew))
        {
            if (!isNew) return;

            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            // exe를 옮겼어도 자동 실행 경로가 현재 위치를 가리키도록 갱신한다.
            if (IsAutoStart()) SetAutoStart(true);

            var autoStart = new ToolStripMenuItem("Windows 시작 시 실행") { Checked = IsAutoStart(), CheckOnClick = true };
            autoStart.CheckedChanged += (s, e) => SetAutoStart(autoStart.Checked);

            var menu = new ContextMenuStrip();
            menu.Items.Add(autoStart);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("종료", null, (s, e) => Application.Exit());

            var tray = new NotifyIcon
            {
                Icon = MakeIcon(),
                Text = "NoSleep - 절전 방지 중",
                ContextMenuStrip = menu,
                Visible = true
            };

            Application.Run();

            tray.Visible = false;
            SetThreadExecutionState(ES_CONTINUOUS);
        }
    }

    static bool IsAutoStart()
    {
        using (var k = Registry.CurrentUser.OpenSubKey(RunKey))
            return k != null && k.GetValue(AppName) != null;
    }

    static void SetAutoStart(bool on)
    {
        using (var k = Registry.CurrentUser.CreateSubKey(RunKey))
        {
            if (on) k.SetValue(AppName, "\"" + Application.ExecutablePath + "\"");
            else k.DeleteValue(AppName, false);
        }
        // 작업 관리자에서 꺼 둔 기록이 남아 있으면 켜도 실행되지 않으므로 지운다.
        using (var k = Registry.CurrentUser.OpenSubKey(ApprovedKey, true))
            if (k != null) k.DeleteValue(AppName, false);
    }

    static Icon MakeIcon()
    {
        var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(Brushes.LimeGreen, 1, 1, 14, 14);
        }
        return Icon.FromHandle(bmp.GetHicon());
    }
}
