using ImageDownloader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModel
{
    public class SettingsViewModel
    {

        public SettingsModel Model { get; set; }



        public SettingsViewModel()
        {
            SettingsModel model = new SettingsModel();
            model.DownloadDirectory = Properties.Settings.Default.DownloadDirectory;
            model.CopyDirectory = Properties.Settings.Default.CopyDirectory;
            this.Model = model;
        }


        public void Save()
        {
            Properties.Settings.Default.DownloadDirectory = this.Model.DownloadDirectory;
            Properties.Settings.Default.CopyDirectory = this.Model.CopyDirectory;
            Properties.Settings.Default.Save();

        }


    }
}
