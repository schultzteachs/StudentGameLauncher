using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (HelloButton.IsChecked == true) 
            { 
                MessageBox.Show("Hello!"); 
            }

            else if (GoodbyeButton.IsChecked == true)
            {
                MessageBox.Show("Goodbye!");
            }
        }
    }



}