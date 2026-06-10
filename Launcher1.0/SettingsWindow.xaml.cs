using System.Windows;

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
    }


}
