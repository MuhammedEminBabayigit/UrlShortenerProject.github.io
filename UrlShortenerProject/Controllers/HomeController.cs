using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UrlShortenerProject.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using shortid;
using UrlShortenerProject.Filter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace UrlShortenerProject.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "https://localhost:7122";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            var connectionString = "mongodb+srv://master:yazgel2022@yazgel.zwij6hr.mongodb.net/?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("url-shortener");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string u, string l)
        {
            
            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("once-shortened-urls");
            var shortenedUrlCollectionT = _mongoDatabase.GetCollection<CreateLink>("shortened-urls");
            // first check if we have the short code

            if (u != null)
            {
                var shortenedUrl = await shortenedUrlCollection
               .AsQueryable()
               .FirstOrDefaultAsync(x => x.ShortCode == u);

                if (shortenedUrl == null)
                {
                    var model = new ShortenedUrl();
                    return View(model);
                }

                if ( shortenedUrl.LastDate <= DateTime.Now )
                {
                    return RedirectToAction("Error");
                }

                return Redirect(shortenedUrl.OriginalUrl);

            }
            else
            {
                var shortenedUrl = await shortenedUrlCollectionT
               .AsQueryable()
               .FirstOrDefaultAsync(x => x.ShortCode == l);

                // if the short code does not exist, send back to home page
                if (shortenedUrl == null)
                {
                    var model = new ShortenedUrl();
                    return View(model);
                }
                if (shortenedUrl.Password != null)
                {
                    if (shortenedUrl.Status != false)
                    {
                        if (DateTime.Now < shortenedUrl.LastDate)
                        {
                            ViewBag.passwordSt = "hp";
                            var update = Builders<CreateLink>.Update.Set("ClickCount", shortenedUrl.ClickCount + 1);
                            shortenedUrlCollectionT.UpdateOne(x => x.Id == shortenedUrl.Id, update);
                            CreateLink linkSon = shortenedUrl;
                            return RedirectToAction("PasswordCheck","Home",linkSon);
                            //return Redirect(shortenedUrl.OriginalUrl);
                            //click arttırılacak

                        }
                        else
                        {
                            return RedirectToAction("Error");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
                else
                {
                    if (shortenedUrl.Status != false)
                    {
                        if (DateTime.Now < shortenedUrl.LastDate)
                        {
                            ViewBag.passwordSt = "nhp";
                            var update = Builders<CreateLink>.Update.Set("ClickCount", shortenedUrl.ClickCount+1);
                            shortenedUrlCollectionT.UpdateOne(x => x.Id == shortenedUrl.Id,update);
                            return Redirect(shortenedUrl.OriginalUrl);
                            //click arttırılacak


                        }
                        else
                        {
                            return RedirectToAction("Error");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
            }



        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }
        public IActionResult PasswordCheck(CreateLink link)
        {
            ViewBag.urlSon = link.OriginalUrl;
            ViewBag.PassSon = link.Password;
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(CheckBlackList))]
        public async Task<JsonResult> CreateShortUrl(string longUrl)
        {
            //var ipAdress = HttpContext.Connection.RemoteIpAddress;


            // get shortened url collection
            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("once-shortened-urls");
            var blackIpAdressCollection = _mongoDatabase.GetCollection<BlackIpAdress>("blackIp-Adress");
            // first check if we have the url stored
            var shortenedUrl = await shortenedUrlCollection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.OriginalUrl == longUrl);

            // if the long url has not been shortened
            if (shortenedUrl == null)
            {
                var shortCode = ShortId.Generate();
                shortenedUrl = new ShortenedUrl
                {
                    CreatedAt = DateTime.UtcNow,
                    LastDate = DateTime.UtcNow.AddDays(7).AddHours(3),
                    Status = true,
                    OriginalUrl = longUrl,
                    ShortCode = shortCode,
                    ShortUrl = $"{ServiceUrl}?u={shortCode}"
                };

                // add to database
                await shortenedUrlCollection.InsertOneAsync(shortenedUrl);
            }

            await blackIpAdressCollection.InsertOneAsync(new BlackIpAdress
            {
                UserIP = HttpContext.Connection.RemoteIpAddress.ToString(),
            });

            return Json(shortenedUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}