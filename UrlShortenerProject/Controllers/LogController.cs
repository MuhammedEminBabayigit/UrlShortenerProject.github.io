using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UrlShortenerProject.Models;

namespace UrlShortenerProject.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "https://localhost:7122";

        public LogController(ILogger<HomeController> logger)
        {
            _logger = logger;

            var connectionString = "mongodb+srv://master:yazgel2022@yazgel.zwij6hr.mongodb.net/?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("url-shortener");
        }

        public async Task<IActionResult> Index(string u)
        {
            // get shortened url collection
            var shortenedUrlCollection = _mongoDatabase.GetCollection<ShortenedUrl>("shortened-urls");
            // first check if we have the short code
            var model = await shortenedUrlCollection
                .AsQueryable().ToListAsync();

            return View(model);
        }
    }
}
