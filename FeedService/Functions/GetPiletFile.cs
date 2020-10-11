using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using FeedService.Services;

namespace FeedService.Functions
{
    public  class GetPiletFile
    {
        private readonly IPiletService _piletService;
        public GetPiletFile(IPiletService piletService)
        {
            _piletService = piletService;
        }

        [FunctionName("GetPiletFile")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "files/{name}/{version}/{fileName}")] HttpRequestMessage req,
            ILogger log, string name, string version, string fileName)
        {
            var file = await _piletService.GetPiletFile(fileName, version, name);
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(file, System.Text.Encoding.UTF8, "text/css");
            return response;
        }
    }
}
