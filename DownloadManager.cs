using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));


        //Events
        public event EventHandler<DownloadEventArgs> DownloadStarted;
        public event EventHandler<DownloadEventArgs> DownloadFinished;
        public event EventHandler<DownloadEventErrorArgs> DownloadError;

        public DownloadManager()
        {

        }


        public void StartDownload(string urlToDownload)
        {
            OnDownloadStarted(new DownloadEventArgs(string.Empty, string.Empty));



            OnDownloadFinished(new DownloadEventArgs(string.Empty, string.Empty));

        }



        protected virtual void OnDownloadStarted(DownloadEventArgs e)
        {
            DownloadStarted?.Invoke(this, e);
        }
        protected virtual void OnDownloadFinished(DownloadEventArgs e)
        {
            DownloadFinished?.Invoke(this, e);
        }
        protected virtual void OnDownloadError(DownloadEventErrorArgs e)
        {
            DownloadError?.Invoke(this, e);
        }
    }

}
