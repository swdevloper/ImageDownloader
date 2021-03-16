using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

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


        public async Task StartDownload(string urlToDownload, string directoryToCeckOrCreate)
        {
            OnDownloadStarted(new DownloadEventArgs(string.Empty, string.Empty));
            

            //Check Directory or create Directory
            CheckOrCreateDirectory(directoryToCeckOrCreate);


            //Parse Url to html string
            string parsedHtml = await ParseUrlToHtml(urlToDownload);
            if(parsedHtml!=string.Empty)
            {
                //Create list of image tags/elements from html
                List<ImageElement> imageList = GetAllImageElementsFromHtml(parsedHtml, urlToDownload, directoryToCeckOrCreate);
                if(imageList.Count!=0)
                {
                    //Download images
                    DownloadImages(imageList);
                }
            }


            OnDownloadFinished(new DownloadEventArgs(string.Empty, string.Empty));
        }




        public void CheckOrCreateDirectory(string directoryToCeckOrCreate)
        {
            try
            {
                if (!Directory.Exists(directoryToCeckOrCreate))
                {
                    Directory.CreateDirectory(directoryToCeckOrCreate);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("{0}: Error occured", MethodBase.GetCurrentMethod().Name), ex);
            }
        }



        public async Task<string> ParseUrlToHtml(string urlToDownload)
        {
            string html = string.Empty;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(urlToDownload);
            response.EnsureSuccessStatusCode();
            html = await response.Content.ReadAsStringAsync();
            return html;
        }


        public List<ImageElement> GetAllImageElementsFromHtml(string parsedHtml, string urlToDownload, string targetDirectory)
        {
            List<ImageElement> imageElements = new List<ImageElement>();
            try
            {
                string imageSource = string.Empty;
                //Regular Expression
                //. --> Genau ein Zeichen
                //* --> Beliebige Anzahl Zeichen
                //? --> Optional
                //string pattern = "<img*>
                //https://regex101.com/r/EE08dw/2
                string pattern = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
                Regex rx = new Regex(pattern);
                foreach (Match m in rx.Matches(parsedHtml))
                {
                    ImageElement imageElement = new ImageElement();
                    imageElement.HtmlTag = m.Value;

                    imageSource = m.Groups[1].Value;
                    //Check relative or absolut source path
                    if (imageSource.StartsWith(@"/"))
                    {
                        Uri uri = new Uri(urlToDownload);
                        imageSource = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, imageSource);
                    }

                    if(!imageSource.StartsWith("data:image"))
                    {
                        imageElement.Url = imageSource;
                        imageElement.Name = Path.GetFileName(imageElement.Url);
                        imageElement.ImageType = Path.GetExtension(imageElement.Url);
                        imageElement.Path = Path.Combine(targetDirectory, string.Format("{0}{1}", Guid.NewGuid(), imageElement.ImageType));
                        if (imageElement.ImageType == ".png" || imageElement.ImageType == ".jpg" || imageElement.ImageType == ".jpeg" || imageElement.ImageType == ".gif")
                        {
                            imageElements.Add(imageElement);
                        }
                    }

                }
               
            }
            catch (Exception ex)
            {
                log.Error(string.Format("{0}: Error occured", MethodBase.GetCurrentMethod().Name), ex);
            }
            return imageElements;







        }



        public void DownloadImages(List<ImageElement> imageElements)
        {
            foreach (ImageElement imageElement in imageElements)
            {


                DownloadImage(imageElement);

            }
        }


        private void DownloadImage(ImageElement imageElement)
        {

            WebClient webClient = new WebClient();
            Uri imageUri = new Uri(imageElement.Url);
            webClient.DownloadFileAsync(imageUri, imageElement.Path);

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
