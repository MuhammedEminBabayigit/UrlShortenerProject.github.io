using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

public class QRController : Controller
{
    [Authorize]
    public IActionResult Index(string shortUrl)
    {
        ViewBag.ShortUrl = shortUrl;

       return View();
    }

    

  
}