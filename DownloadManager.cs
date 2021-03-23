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
using System.Xml.Linq;

namespace ImageDownloader
{
    public class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));


        //Events
        public event EventHandler<DownloadEventArgs> DownloadStarted;
        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;
        public event EventHandler<DownloadEventErrorArgs> DownloadError;

        public DownloadManager()
        {

        }


        public async Task StartDownload(string urlToDownload, string directoryToCeckOrCreate)
        {
            OnDownloadStarted(new DownloadEventArgs(string.Empty, string.Empty));

            List<ImageElement> imageList = new List<ImageElement>();

            //Check Directory or create Directory
            Task checkDirectoryTask = CheckOrCreateDirectory(directoryToCeckOrCreate);


            //Parse Url to html string
            string parsedHtml = await ParseUrlToHtml(urlToDownload);

            await checkDirectoryTask;

            if (parsedHtml!=string.Empty)
            {
                //Create list of image tags/elements from html
                imageList = await GetAllImageElementsFromHtml(parsedHtml, urlToDownload, directoryToCeckOrCreate);
                if(imageList.Count!=0)
                {
                    //Download images
                    DownloadImages(imageList);
                }
            }


            OnDownloadFinished(new DownloadFinishedEventArgs(string.Empty, string.Empty, imageList));
        }




        public async Task CheckOrCreateDirectory(string directoryToCeckOrCreate)
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


        public async Task<List<ImageElement>> GetAllImageElementsFromHtml(string parsedHtml, string urlToDownload, string targetDirectory)
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



        public async Task DownloadImages(List<ImageElement> imageElements)
        {
            foreach (ImageElement imageElement in imageElements)
            {
               await  DownloadImage(imageElement);
            }
        }


        private async Task DownloadImage(ImageElement imageElement)
        {

            WebClient webClient = new WebClient();
            Uri imageUri = new Uri(imageElement.Url);
            webClient.DownloadFileAsync(imageUri, imageElement.Path);

        }

        
        
        
        internal void CopyImages(List<ImageElement> selectedItems, string destinationDirectory)
        {
            foreach (ImageElement item in selectedItems)
            {
                string destinationFilename = Path.Combine(destinationDirectory, Path.GetFileName(item.Path));
                CopyImages(item.Path, destinationFilename);
            }
        }

        private void CopyImages(string sourceFilename, string destinationFilename)
        {
            if(File.Exists(destinationFilename))
            {
                File.Delete(destinationFilename); 
            }
            File.Copy(sourceFilename, destinationFilename);
        }


        public List<ImageElement> GetImagesFromDirectory(string targetDirectory)
        {
            List<ImageElement> imageElements = new List<ImageElement>();
            var files = Directory.GetFiles(targetDirectory).ToList();
            foreach (var image in files)
            {
                ImageElement element = new ImageElement();
                element.Path = image;
                imageElements.Add(element);
            }


            return imageElements;
        }




        protected virtual void OnDownloadStarted(DownloadEventArgs e)
        {
            DownloadStarted?.Invoke(this, e);
        }
        protected virtual void OnDownloadFinished(DownloadFinishedEventArgs e)
        {
          DownloadFinished?.Invoke(this, e);
        }
        protected virtual void OnDownloadError(DownloadEventErrorArgs e)
        {
            DownloadError?.Invoke(this, e);
        }
    }

}
