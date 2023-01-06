using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using System;
using System.Net;
using UrlShortenerProject.Models;

namespace UrlShortenerProject.Filter
{
    public class CheckBlackList : ActionFilterAttribute
    {
        private readonly IMongoDatabase _mongoDatabase;
        private const string ServiceUrl = "https://localhost:7122";


        public CheckBlackList()
        {
            var connectionString = "mongodb+srv://master:yazgel2022@yazgel.zwij6hr.mongodb.net/?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(connectionString);
            _mongoDatabase = mongoClient.GetDatabase("url-shortener");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestIpAdress = context.HttpContext.Connection.RemoteIpAddress.ToString();

            var blackIpAdressCollection = _mongoDatabase.GetCollection<BlackIpAdress>("blackIp-Adress");
            var isWhiteList = blackIpAdressCollection
                .AsQueryable().Where(x => x.UserIP == requestIpAdress).Any();

            if (isWhiteList)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
