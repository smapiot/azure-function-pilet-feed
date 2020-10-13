using FeedService.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FeedService.Infrastructure
{
  public interface IStorageAccountRepository
  {
    Task<List<string>> GetLatestPiletsPackageFiles();
    Task UploadFiles(List<PackageFile> files);
    Task<Stream> GetFile(string fileName, string version, string piletName);
  }
}
