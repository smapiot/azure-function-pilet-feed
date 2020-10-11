using FeedService.Models;
using FeedService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeedService
{
    public class Pilet
    {
        private readonly IPiletService _piletService;
        public Pilet(IPiletService piletService)
        {
            _piletService = piletService;
        }


        [FunctionName("Pilet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            if (req.Method == HttpMethod.Post)
            {
                try
                {
                    var packageJsonData = new PackageJson();
                    var formData = await req.Content.ReadAsMultipartAsync();
                    var packageFiles = new List<PackageFile>();
                    var content = formData.Contents.First();
                    var fileData = await content.ReadAsByteArrayAsync();
                    await _piletService.PublishPilet(fileData);
                    return new OkObjectResult("Success");
                }
                catch (Exception)
                {

                    throw;
                }
            }
            var lastestPilets = await _piletService.GetLatestPilets();
            return new OkObjectResult(lastestPilets);
        }
    }
}
