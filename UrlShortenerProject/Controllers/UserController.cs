using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using shortid;
using System.Drawing;
using UrlShortenerProject.Filter;
using UrlShortenerProject.Models;

namespace UrlShortenerProject.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "https://localhost:7122";

        public UserController()
        {

            var connectionString = "mongodb+srv://master:yazgel2022@yazgel.zwij6hr.mongodb.net/?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("url-shortener");
        }
        public IActionResult Index()
        {
            var shortenedUrlCollection = _mongoDatabase.GetCollection<CreateLink>("shortened-urls");
            List<CreateLink> links = new List<CreateLink>();
            if (User.Identity.Name == "admin")
            {
            links = shortenedUrlCollection.Find(x=>x.Status == false ||x.Status == true).ToList().OrderByDescending(x=>x.CreatedAt).ToList();

            }
            else
            {
            links = shortenedUrlCollection.Find(x=>x.kAdi==User.Identity.Name).ToList().OrderByDescending(x=>x.CreatedAt).ToList();

            }

            ViewBag.ShortenedUrlCollection = links.ToList().OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.totalLink =Convert.ToInt32(links.Count());
            ViewBag.totalClick = 0;
            int totalClickCount = 0;
            foreach (var item in links)
            {
            totalClickCount += totalClickCount+item.ClickCount;
            }
            ViewBag.totalClick = totalClickCount;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string tag)
        {
            var shortenedUrlCollection = _mongoDatabase.GetCollection<CreateLink>("shortened-urls");
            CreateLink links = shortenedUrlCollection.Find(x => x.Tag ==tag).FirstOrDefault();
            bool statusFirst = links.Status;
            if (statusFirst) { 
                statusFirst = false;
            }
            else
            {
                statusFirst=true;
            }

            var update = Builders<CreateLink>.Update.Set("Status", statusFirst);
            shortenedUrlCollection.UpdateOne(x => x.Id == links.Id, update);


            return Content("Başarılı");
        }


        [HttpPost]
        public async Task<JsonResult> CreateShortUrl(string longUrl,string desc,string pass,string spe,string tag,DateTime lastDate)
        {
            //var ipAdress = HttpContext.Connection.RemoteIpAddress;


            // get shortened url collection
            var shortenedUrlCollection = _mongoDatabase.GetCollection<CreateLink>("shortened-urls");
            var blackIpAdressCollection = _mongoDatabase.GetCollection<BlackIpAdress>("blackIp-Adress");
            // first check if we have the url stored
            if (spe == null)
                spe = "";


                var shortCode = ShortId.Generate();
                var shortenedUrl = new CreateLink
                {
                    CreatedAt = DateTime.UtcNow,
                    OriginalUrl = longUrl,
                    ShortCode = spe+shortCode,
                    ShortUrl = $"{ServiceUrl}?l={spe}{shortCode}",
                     kAdi = User.Identity.Name,
                      ClickCount = 0,
                       Description = desc,
                        Password = pass,
                         SpecificLink = spe,
                          Status =true,
                           Tag = tag,
                            LastDate=lastDate                       
                };

                // add to database
                await shortenedUrlCollection.InsertOneAsync(shortenedUrl);
            

            //await blackIpAdressCollection.InsertOneAsync(new BlackIpAdress
            //{
            //    UserIP = HttpContext.Connection.RemoteIpAddress.ToString(),
            //});

            return Json(shortenedUrl);
        }

    }
}
