using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class DownloadEventArgs: EventArgs
    {
        private string _downloadSource;
        private string _downloadTarget;


        public DownloadEventArgs(string downloadSource, string downloadTarget)
        {
            _downloadSource = downloadSource;
            _downloadTarget = downloadTarget;
        }

        public string DownloadSource
        {
            get { return _downloadSource; }
        }
        public string DownloadTarget
        {
            get { return _downloadTarget; }
        }

    }


    public class DownloadEventErrorArgs: DownloadEventArgs
    {
        private Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
        }

        public DownloadEventErrorArgs(string downloadSource, string downloadTarget, Exception ex): base(downloadSource, downloadTarget)
        {
            _exception = ex;
        }

    }
}
