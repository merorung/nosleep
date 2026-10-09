// NoSleep: 실행 중인 동안 시스템 유휴 절전을 막는다. 디스플레이는 꺼질 수 있다.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

static class NoSleep
{
    [DllImport("kernel32.dll")]
    static extern uint SetThreadExecutionState(uint esFlags);

    const uint ES_CONTINUOUS = 0x80000000;
    const uint ES_SYSTEM_REQUIRED = 0x00000001;

    [STAThread]
    static void Main()
    {
        bool isNew;
        using (new Mutex(true, "NoSleep_SingleInstance", out isNew))
        {
            if (!isNew) return;

            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            var menu = new ContextMenuStrip();
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
