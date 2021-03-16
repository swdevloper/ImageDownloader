using log4net;
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
                this.Cursor = Cursors.Wait;
                this.dgImages.ItemsSource = null;
                this.dgImages.Items.Clear();
                DownloadManager downloadManager = new DownloadManager();
                downloadManager.DownloadStarted += DownloadManager_DownloadStarted;
                downloadManager.DownloadFinished += DownloadManager_DownloadFinished;
                downloadManager.DownloadError += DownloadManager_DownloadError;
                

                await downloadManager.StartDownload(url, downloadDirectoryFullname);

                List<ImageElement> imageList = downloadManager.GetImagesFromDirectory(downloadDirectoryFullname);
                this.dgImages.ItemsSource = imageList;


            }
            this.Cursor = Cursors.Arrow;


        }

        private void DownloadManager_DownloadError(object sender, DownloadEventErrorArgs e)
        {
            
        }

        private void DownloadManager_DownloadFinished(object sender, DownloadEventArgs e)
        {
            MessageBox.Show("Download finished");
        }

        private void DownloadManager_DownloadStarted(object sender, DownloadEventArgs e)
        {
            
        }

        //private void Btn_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Hallo Button", "Nachrit", MessageBoxButton.OK, MessageBoxImage.Warning);
        //}
    }
}
