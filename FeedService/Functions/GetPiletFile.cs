using FeedService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace FeedService.Functions
{
  public class GetPiletFile
  {
    private readonly IPiletService _piletService;
    public GetPiletFile(IPiletService piletService)
    {
      _piletService = piletService;
    }

    [FunctionName("GetPiletFile")]
    public async Task<FileStreamResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pilet/{name}/{version}/{fileName}")] HttpRequestMessage req,
        ILogger log, string name, string version, string fileName)
    {
      var file = await _piletService.GetPiletFile(fileName, version, name);
      var provider = new FileExtensionContentTypeProvider();
      if (!provider.TryGetContentType(fileName, out string contentType))
      {
        contentType = "application/octet-stream";
      }
      return new FileStreamResult(file, contentType);
    }
  }
}
