using System.Windows;
using System.Windows.Controls;
using ValoLineupViewer.Models;

namespace ValoLineupViewer
{
    public partial class AddLineupWindow : Window
    {
        public Lineup? CreatedLineup { get; private set; }

        public AddLineupWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtAddTitle.Text))
            {
                System.Windows.MessageBox.Show("Vui lòng nhập tiêu đề cho Lineup!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CreatedLineup = new Lineup
            {
                Map = (CmbAddMap.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Ascent",
                Agent = (CmbAddAgent.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Sova",
                Side = (CmbAddSide.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Attacker",
                Site = (CmbAddSite.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "A",
                Ability = TxtAddAbility.Text.Trim(),
                Title = TxtAddTitle.Text.Trim(),
                ThrowType = TxtAddThrowType.Text.Trim(),
                StandDescription = TxtAddStandDesc.Text.Trim(),
                StandImageUrl = TxtAddStandImg.Text.Trim(),
                AimDescription = TxtAddAimDesc.Text.Trim(),
                AimImageUrl = TxtAddAimImg.Text.Trim(),
                VideoUrl = string.IsNullOrWhiteSpace(TxtAddVideoUrl.Text) ? "https://www.tiktok.com/@lineupmaster" : TxtAddVideoUrl.Text.Trim(),
                Author = "LineupMaster",
                Notes = TxtAddNotes.Text.Trim(),
                Difficulty = "Tùy chỉnh"
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
