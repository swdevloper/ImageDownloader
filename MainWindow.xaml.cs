using ImageDownloader.ViewModel;
using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        
        
        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.txbDestinationDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //this.txbDestinationDirectory.Text = Properties.Settings.Default.CopyDirectory;
            this.DataContext = new SettingsViewModel();



            //for (int i = 0; i < 5; i++)
            //{
            //    Button btn = new Button();
            //    btn.Content = "Button per Code";
            //    btn.Click += Btn_Click;
            //    this.panCopyMove.Children.Add(btn);
            //}


        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            //URL: https://www.vlbg.wifi.at:443/Images/bild.jpg
            //URL: https://www.vlbg.wifi.at:443/
            //URL: www.vlbg.wifi.at
            //Regular Expressions

            string url = this.txbUrl.Text;
            string rootDirectoryDownload = Properties.Settings.Default.RootDirectoryDownload;
            string downloadDirectory = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string downloadDirectoryFullname = System.IO.Path.Combine(rootDirectoryDownload, downloadDirectory);

            // TODO Eingabe auf gültige URL prüfen
            if (url!=string.Empty)
            {
                this.Cursor = System.Windows.Input.Cursors.Wait;
                this.dgImages.ItemsSource = null;
                this.dgImages.Items.Clear();
                DownloadManager downloadManager = new DownloadManager();
                downloadManager.DownloadStarted += DownloadManager_DownloadStarted;
                downloadManager.DownloadFinished += DownloadManager_DownloadFinished;
                downloadManager.DownloadError += DownloadManager_DownloadError;
                

                await downloadManager.StartDownload(url, downloadDirectoryFullname);

                //List<ImageElement> imageList = downloadManager.GetImagesFromDirectory(downloadDirectoryFullname);
                //this.dgImages.ItemsSource = imageList;


            }
            this.Cursor = System.Windows.Input.Cursors.Arrow;


        }

        private void DownloadManager_DownloadError(object sender, DownloadEventErrorArgs e)
        {
            
        }

        private void DownloadManager_DownloadFinished(object sender, DownloadFinishedEventArgs e)
        {
            this.dgImages.ItemsSource = e.ImageList;
        }

        private void DownloadManager_DownloadStarted(object sender, DownloadEventArgs e)
        {
            
        }

        private void chkSelectDeselct_Checked(object sender, RoutedEventArgs e)
        {
            var itemList = (List<ImageElement>)this.dgImages.ItemsSource;
            if(itemList!=null)
            {
                foreach (var item in itemList)
                {
                    item.IsSelected = true;
                }
                this.dgImages.Items.Refresh();
            }
        }

        private void chkSelectDeselct_Unchecked(object sender, RoutedEventArgs e)
        {
            var itemList = (List<ImageElement>)this.dgImages.ItemsSource;
            if (itemList != null)
            {
                foreach (var item in itemList)
                {
                    item.IsSelected = false;
                }
                this.dgImages.Items.Refresh();
            }
        }

        private void btnSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            //Alter Dialog
            //var dialog = new FolderBrowserDialog();
            //DialogResult result = dialog.ShowDialog();
            //if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
            //{
            //    this.txbDestinationDirectory.Text = dialog.SelectedPath;
            //}

            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.EnsurePathExists = true;
                dialog.IsFolderPicker = true;
                if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.txbDestinationDirectory.Text = dialog.FileName;
                }


            }
            

        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            List<ImageElement> selectedItems = ((List<ImageElement>)this.dgImages.ItemsSource).Where(img => img.IsSelected == true).ToList();
            if (selectedItems.Count > 0)
            {
                DownloadManager manager = new DownloadManager();
                manager.CopyImages(selectedItems, this.txbDestinationDirectory.Text);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            //Variante 2: Mit Constructor
            //fSettings f = new fSettings();
            fSettings f = new fSettings(this);

            f.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //f.Owner = this;
            //Variante 1: Mit Property
            //f.CallerWindow = this;

            f.Show();
            this.Hide();
        }

        //private void Btn_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Hallo Button", "Nachrit", MessageBoxButton.OK, MessageBoxImage.Warning);
        //}
    }
}
