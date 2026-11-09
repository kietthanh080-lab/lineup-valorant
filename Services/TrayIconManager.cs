using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace ValoLineupViewer.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private readonly Action _onToggleAction;

        public TrayIconManager(Action onToggleAction)
        {
            _onToggleAction = onToggleAction;
            InitializeTray();
        }

        private void InitializeTray()
        {
            _notifyIcon = new NotifyIcon();

            // Load icon
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string iconPath = Path.Combine(baseDir, "app.ico");
            if (!File.Exists(iconPath))
            {
                string projectIcon = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..", "app.ico"));
                if (File.Exists(projectIcon))
                    iconPath = projectIcon;
            }

            if (File.Exists(iconPath))
            {
                try
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                }
                catch
                {
                    _notifyIcon.Icon = SystemIcons.Application;
                }
            }
            else
            {
                _notifyIcon.Icon = SystemIcons.Application;
            }

            _notifyIcon.Text = "Valorant Lineup HUD (Nhấn phím H để Bật/Tắt)";
            _notifyIcon.Visible = true;

            // Context Menu khi click chuột phải vào icon góc màn hình
            var contextMenu = new ContextMenuStrip();
            
            var itemToggle = new ToolStripMenuItem("Hiện / Ẩn HUD (Phím H)");
            itemToggle.Font = new Font(itemToggle.Font, FontStyle.Bold);
            itemToggle.Click += (s, e) => _onToggleAction();
            contextMenu.Items.Add(itemToggle);

            contextMenu.Items.Add(new ToolStripSeparator());

            var itemExit = new ToolStripMenuItem("Thoát hoàn toàn (Exit)");
            itemExit.Click += (s, e) =>
            {
                Dispose();
                Application.Current.Shutdown();
            };
            contextMenu.Items.Add(itemExit);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Double click để bật/tắt
            _notifyIcon.DoubleClick += (s, e) => _onToggleAction();

            // Hiện thông báo chào mừng góc phải dưới màn hình khi vừa mở app
            ShowNotification(
                "Valorant Lineup HUD đã sẵn sàng!",
                "Ứng dụng đang chạy nền. Nhấn phím 'H' bất kỳ lúc nào trong trận để xem góc ném chiêu!"
            );
        }

        public void ShowNotification(string title, string message)
        {
            if (_notifyIcon != null && _notifyIcon.Visible)
            {
                _notifyIcon.BalloonTipTitle = title;
                _notifyIcon.BalloonTipText = message;
                _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                _notifyIcon.ShowBalloonTip(3000);
            }
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
    }
}
