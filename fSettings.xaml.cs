using ImageDownloader.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageDownloader
{
    /// <summary>
    /// Interaction logic for fSettings.xaml
    /// </summary>
    public partial class fSettings : Window
    {
        
        
        public Window CallerWindow { get; set; }
        
        
        public fSettings()
        {
            InitializeComponent();
        }


        public fSettings(Window callerWindow)
        {
            InitializeComponent();
            this.CallerWindow = callerWindow;
            this.DataContext = new SettingsViewModel();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CallerWindow.Show();
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewModel = this.DataContext as SettingsViewModel;
            viewModel.Save();
            this.Close();

        }
    }
}
