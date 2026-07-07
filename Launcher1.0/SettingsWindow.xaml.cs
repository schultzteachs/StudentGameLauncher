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
            // Capture the text from the box
            UpdatedSchoolName = SchoolNameTextBox.Text;

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

    }


}
