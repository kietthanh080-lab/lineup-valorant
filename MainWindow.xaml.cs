using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ValoLineupViewer.Models;
using ValoLineupViewer.Services;

namespace ValoLineupViewer
{
    public partial class MainWindow : Window
    {
        private readonly LineupRepository _repository;
        private HotkeyManager? _hotkeyManager;
        private TrayIconManager? _trayIconManager;
        private List<Lineup> _allLineups = new();
        private Lineup? _selectedLineup;
        private bool _isOverlayVisible = true;

        public MainWindow()
        {
            InitializeComponent();
            _repository = new LineupRepository();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Đăng ký Global Hotkey (F3 hoặc Alt + L)
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            _hotkeyManager = new HotkeyManager();
            _hotkeyManager.Initialize(hwnd);
            _hotkeyManager.OnToggleHotkeyPressed += ToggleOverlay;

            // Khởi tạo icon dưới khay hệ thống (System Tray) + hiện thông báo sẵn sàng
            _trayIconManager = new TrayIconManager(ToggleOverlay);

            // Nạp dữ liệu
            LoadData();
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotkeyManager?.Dispose();
            _trayIconManager?.Dispose();
            base.OnClosed(e);
        }

        private void LoadData()
        {
            _allLineups = _repository.LoadLineups();
            ApplyFilters();

            if (LstLineups.Items.Count > 0)
            {
                LstLineups.SelectedIndex = 0;
            }
        }

        private void ToggleOverlay()
        {
            // Nếu cửa sổ đang mở và người dùng đang gõ văn bản trong TextBox (như ô tìm kiếm), không đóng HUD khi bấm H
            if (_isOverlayVisible && Keyboard.FocusedElement is System.Windows.Controls.TextBox)
            {
                return;
            }

            if (_isOverlayVisible)
            {
                this.Hide();
                _isOverlayVisible = false;
            }
            else
            {
                this.Show();
                this.WindowState = WindowState.Maximized;
                this.Activate();
                this.Focus();
                _isOverlayVisible = true;
            }
        }

        private void ApplyFilters()
        {
            if (CmbMap == null || CmbAgent == null || CmbSide == null || CmbSite == null || TxtSearch == null)
                return;

            string rawMap = (CmbMap.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
            string selectedMap = rawMap.Contains("(") ? rawMap.Substring(0, rawMap.IndexOf('(')).Trim() : rawMap;
            string selectedAgent = (CmbAgent.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
            string selectedSide = (CmbSide.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
            string selectedSite = (CmbSite.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";
            string search = TxtSearch.Text.Trim().ToLowerInvariant();
            bool rankOnly = ChkRankOnly?.IsChecked == true;

            var filtered = _allLineups.Where(item =>
            {
                // Lọc theo chế độ Map đang trong bể Rank (Active Competitive Pool)
                // Nếu người dùng chọn đích danh 1 map cụ thể, ưu tiên hiển thị map đó
                if (rankOnly && selectedMap == "Tất cả" && !item.IsRankedMap)
                    return false;

                if (selectedMap != "Tất cả" && !item.Map.Equals(selectedMap, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedAgent != "Tất cả" && !item.Agent.Equals(selectedAgent, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedSide.StartsWith("Attacker") && !item.Side.Equals("Attacker", StringComparison.OrdinalIgnoreCase))
                    return false;
                if (selectedSide.StartsWith("Defender") && !item.Side.Equals("Defender", StringComparison.OrdinalIgnoreCase))
                    return false;

                if (selectedSite != "Tất cả" && !item.Site.Equals(selectedSite, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (!string.IsNullOrEmpty(search))
                {
                    bool matchTitle = item.Title.ToLowerInvariant().Contains(search);
                    bool matchAbility = item.Ability.ToLowerInvariant().Contains(search);
                    bool matchNotes = item.Notes.ToLowerInvariant().Contains(search);
                    if (!matchTitle && !matchAbility && !matchNotes)
                        return false;
                }

                return true;
            }).ToList();

            LstLineups.ItemsSource = filtered;
            TxtListCount.Text = $"DANH SÁCH LINEUP ({filtered.Count})";

            if (filtered.Count > 0)
            {
                LstLineups.SelectedIndex = 0;
            }
            else
            {
                ClearDetail();
            }
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void LstLineups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstLineups.SelectedItem is Lineup lineup)
            {
                _selectedLineup = lineup;
                DisplayLineup(lineup);
            }
        }

        private void DisplayLineup(Lineup lineup)
        {
            _selectedLineup = lineup;
            BadgeAgent.Text = lineup.Agent.ToUpperInvariant();
            BadgeMap.Text = lineup.Map.ToUpperInvariant();
            BadgeAbility.Text = lineup.Ability;
            BadgeAuthor.Text = string.IsNullOrWhiteSpace(lineup.Author) ? "@LineupMaster" : "@" + lineup.Author.TrimStart('@');
            BadgeRanked.Visibility = lineup.IsRankedMap ? Visibility.Visible : Visibility.Collapsed;
            TxtDetailTitle.Text = lineup.Title;
            TxtThrowType.Text = lineup.ThrowType;

            TxtStandDesc.Text = lineup.StandDescription;
            TxtAimDesc.Text = lineup.AimDescription;
            TxtNotes.Text = string.IsNullOrWhiteSpace(lineup.Notes) ? "Không có ghi chú thêm." : lineup.Notes;

            LoadImageSafely(ImgStand, TxtStandPlaceholder, lineup.StandImageUrl, "[Hình ảnh vị trí đứng]");
            LoadImageSafely(ImgAim, TxtAimPlaceholder, lineup.AimImageUrl, "[Hình ảnh điểm căn tâm]");

            BtnOpenVideo.Visibility = !string.IsNullOrWhiteSpace(lineup.VideoUrl) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnOpenVideo_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLineup != null && !string.IsNullOrWhiteSpace(_selectedLineup.VideoUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _selectedLineup.VideoUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Không thể mở video: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearDetail()
        {
            _selectedLineup = null;
            BadgeAgent.Text = "-";
            BadgeMap.Text = "-";
            BadgeAbility.Text = "-";
            BadgeAuthor.Text = "-";
            TxtDetailTitle.Text = "Không tìm thấy Lineup phù hợp với bộ lọc hiện tại";
            TxtThrowType.Text = "-";
            TxtStandDesc.Text = "Hãy thử thay đổi hoặc xóa bộ lọc để hiển thị danh sách.";
            TxtAimDesc.Text = "-";
            TxtNotes.Text = "-";

            ImgStand.Source = null;
            ImgAim.Source = null;
            TxtStandPlaceholder.Visibility = Visibility.Visible;
            TxtAimPlaceholder.Visibility = Visibility.Visible;
            BtnOpenVideo.Visibility = Visibility.Collapsed;
            BadgeRanked.Visibility = Visibility.Collapsed;
        }

        private void LoadImageSafely(System.Windows.Controls.Image imgControl, TextBlock placeholder, string? url, string defaultText)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                imgControl.Source = null;
                placeholder.Text = defaultText;
                placeholder.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                Uri uri;
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    uri = new Uri(url);
                }
                else
                {
                    string localPath = System.IO.Path.IsPathRooted(url) ? url : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, url);
                    if (!System.IO.File.Exists(localPath))
                    {
                        string projPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..", url));
                        if (System.IO.File.Exists(projPath))
                            localPath = projPath;
                    }

                    if (!System.IO.File.Exists(localPath))
                    {
                        imgControl.Source = null;
                        placeholder.Text = defaultText;
                        placeholder.Visibility = Visibility.Visible;
                        return;
                    }

                    uri = new Uri(localPath, UriKind.Absolute);
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();

                imgControl.Source = bitmap;
                placeholder.Visibility = Visibility.Collapsed;
            }
            catch
            {
                imgControl.Source = null;
                placeholder.Text = defaultText + " (Không tải được ảnh)";
                placeholder.Visibility = Visibility.Visible;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ToggleOverlay();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Bấm ra vùng đen mờ bên ngoài để đóng nhanh HUD
            ToggleOverlay();
        }

        private void HudContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Chặn sự kiện để click bên trong khung HUD không bị đóng cửa sổ
            e.Handled = true;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ToggleOverlay();
        }

        private void BtnAddLineup_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddLineupWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true && addWindow.CreatedLineup != null)
            {
                _allLineups.Insert(0, addWindow.CreatedLineup);
                _repository.SaveLineups(_allLineups);
                ApplyFilters();
                LstLineups.SelectedItem = addWindow.CreatedLineup;
            }
        }
    }
}
