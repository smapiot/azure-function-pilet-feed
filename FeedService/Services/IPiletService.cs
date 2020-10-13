using FeedService.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FeedService.Services
{
  public interface IPiletService
  {
    PackageJson ParsePackageJson(MemoryStream fileContents, string fileFullName);
    Task<PiletApiResponse> GetLatestPiletsMetaData();
    Task<bool> PublishPilet(byte[] fileData);
    Task<Stream> GetPiletFile(string fileName, string version, string name);
    List<PackageFile> ExtractFiles(byte[] fileData);
  }
}
