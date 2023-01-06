using System.Drawing;

namespace UrlShortenerProject.Models
{
    public class QRCodeModel
    {

        public string QRCode { get; set; }
        public string url { get; set; }
        public Image image{ get; set; }
    }
}
