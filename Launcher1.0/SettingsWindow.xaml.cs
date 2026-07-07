using System.Windows;
using System.Windows.Media;

namespace Launcher1._0
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public string? UpdatedSchoolName { get; set; }
        public string? UpdatedAppHex { get; private set; }
        public string? UpdatedCardHex { get; private set; }
        public SettingsWindow()
        {
            InitializeComponent();

            SchoolNameTextBox.Text = "Default School";
        }
        public SettingsWindow(string currentSchoolName)
        {
            InitializeComponent();

            // Put the current name into the textbox right away
            SchoolNameTextBox.Text = currentSchoolName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Capture the text from the school name box
            UpdatedSchoolName = SchoolNameTextBox.Text;

            // 2. Convert the color picker selections into Hex strings for MainWindow/JSON
            if (BgColorPicker.SelectedColor.HasValue)
            {
                UpdatedAppHex = ColorToHex(BgColorPicker.SelectedColor.Value);
            }

            if (CardColorPicker.SelectedColor.HasValue)
            {
                UpdatedCardHex = ColorToHex(CardColorPicker.SelectedColor.Value);
            }

            // Setting DialogResult to true automatically closes the popup window
            this.DialogResult = true;
        }

        private void BgColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Verify a color was actually selected by the user
            if (e.NewValue.HasValue)
            {
                // Create a new solid color brush using the picked value
                SolidColorBrush newBgBrush = new SolidColorBrush(e.NewValue.Value);

                // Update the global App.xaml resource mapping instantly
                Application.Current.Resources["AppBackgroundBrush"] = newBgBrush;
            }
        }

        private void CardColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Verify a color was actually selected by the user
            if (e.NewValue.HasValue)
            {
                // Create a new solid color brush using the picked value
                SolidColorBrush newBgBrush = new SolidColorBrush(e.NewValue.Value);

                // Update the global App.xaml resource mapping instantly
                Application.Current.Resources["CardBackgroundBrush"] = newBgBrush;
            }
        }

        /// <summary>
        /// Helper method to turn a WPF Color struct into a standard "#RRGGBB" hex string
        /// </summary>
        private string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }


}
