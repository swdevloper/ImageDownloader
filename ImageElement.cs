using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class ImageElement
    {

        public string Url { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ImageType { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public bool IsSelected { get; set; }
        public string HtmlTag { get; set; }

        public ImageElement()
        {
            IsSelected = true;
        }


    }
}
