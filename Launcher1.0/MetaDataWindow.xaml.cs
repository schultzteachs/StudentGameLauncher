using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Launcher1._0
{
    /// <summary>
    /// Interaction logic for MetaDataWindow.xaml
    /// </summary>
    public partial class MetaDataWindow : Window
    {
        public string? UpdatedGameName { get; set; }
        public string? UpdatedAuthorName { get; private set; }
        



        public MetaDataWindow(ObservableCollection<Game> games)
        {
            
            InitializeComponent();

            this.DataContext = games;


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
