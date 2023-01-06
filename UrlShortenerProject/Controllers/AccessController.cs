using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using UrlShortenerProject.Models;
using MongoDB.Driver;

namespace login_register.Controllers
{

    public class AccessController : Controller
    {
        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "https://localhost:7122";

        public AccessController()
        {
            var connectionString = "mongodb+srv://master:yazgel2022@yazgel.zwij6hr.mongodb.net/?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("url-shortener");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "User");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(VMlogin modelLogin)
        {
            var shortenedUrlCollection = _mongoDatabase.GetCollection<VMlogin>("Users");
            VMlogin user = shortenedUrlCollection.Find(x => x.Email == modelLogin.Email && x.PassWord == modelLogin.PassWord).FirstOrDefault();
            if (user != null)
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,modelLogin.Email),
                    new Claim("OtherProperties","Example Role")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelLogin.KeepLoggedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Index", "User");

            }

            ViewData["ValidateMessage"] = "Kullanıcı Bulunamadı";

            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string em,string pass)
        {
            var shortenedUrlCollection = _mongoDatabase.GetCollection<VMlogin>("Users");
            VMlogin user = new VMlogin(){  Email = em, PassWord = pass, KeepLoggedIn=false};

            shortenedUrlCollection.InsertOne(user);

            return RedirectToAction("Login", "Access");
        }

    }
}
