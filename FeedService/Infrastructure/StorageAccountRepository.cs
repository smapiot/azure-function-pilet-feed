using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FeedService.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.Infrastructure
{
  public class StorageAccountRepository : IStorageAccountRepository
    {
    private const string _piletContainer = "pilets";

    public StorageAccountRepository()
    {
    }

    public async Task<List<string>> GetLatestPiletsPackageFiles()
    {
      var piletsGroupedWithAllVersions = new Dictionary<string, List<string>>();
      var latestPiletsInfo = new List<string>();
      BlobServiceClient blobServiceClient = new BlobServiceClient(GetConnectionString());
      BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_piletContainer);
      var blobs = containerClient.GetBlobs();
      await foreach (BlobItem item in containerClient.GetBlobsAsync())
      {
        var piletName = GetPiletName(item.Name);
        if (piletsGroupedWithAllVersions.ContainsKey(piletName))
        {
          piletsGroupedWithAllVersions[piletName].Add(item.Name);
        }
        else
        {
          piletsGroupedWithAllVersions.Add(piletName, new List<string> { item.Name });
        }
      }

      foreach (var piletGroupedWithAllVersions in piletsGroupedWithAllVersions)
      {
        latestPiletsInfo.Add(piletGroupedWithAllVersions.Value.OrderBy(i => i).LastOrDefault());
      }


      return latestPiletsInfo.Distinct().ToList();
    }

    public async Task UploadFiles(List<PackageFile> files)
    {
      foreach (var file in files)
      {
        BlobContainerClient containerClient = new BlobContainerClient(GetConnectionString(), _piletContainer);
        BlobClient blobClient = containerClient.GetBlobClient(file.Path);
        using (var st = new MemoryStream(file.ContentArray))
        {
          await blobClient.UploadAsync(st, true);
        }
      }
    }

    public async Task<Stream> GetFile(string fileName, string version, string piletName)
    {
      BlobServiceClient blobServiceClient = new BlobServiceClient(GetConnectionString());
      BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_piletContainer);
      string name = piletName + "/" + version + "/package/dist/" + fileName;
      BlobClient bc = containerClient.GetBlobClient(name);
      BlobDownloadInfo download = await bc.DownloadAsync();
      return download.Content;
    }


    private string GetPiletName(string name)
    {
      var split = name.Split('/');
      return split[0];
    }

    private string GetConnectionString()
    {
      //Add your connection string here.
      return "";
    }
  }
}
