using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FeedService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedService.Infrastructure
{
    public class StorageAccountRepository : IStorageAccountRepository
    {
        private const string _piletContainer = "pilets";

        public StorageAccountRepository()
        {
        }

        public async Task<List<string>> GetLatestPiletsInfoAsync()
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

        public async Task UploadPilet(List<PackageFile> packageFiles, PackageJson packageJson, byte[] zippedFiles)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(GetConnectionString(), _piletContainer);
                BlobClient blobClient = containerClient.GetBlobClient(packageJson.Name + "/" + packageJson.Version + ".zip");
                using (var st = new MemoryStream(zippedFiles))
                {
                    await blobClient.UploadAsync(st, true);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetPiletFile(string fileName, string version, string piletName)
        {
            var file = "";

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(GetConnectionString());
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_piletContainer);
                BlobClient bc = containerClient.GetBlobClient(piletName + "/" + version + ".zip");
                if (await bc.ExistsAsync())
                {
                    using (var mem = new MemoryStream())
                    {
                        var response = bc.DownloadTo(mem);
                        using (var zipFile = new ZipArchive(mem))
                        {
                            foreach (var zipEntry in zipFile.Entries)
                            {
                                if (zipEntry.Name == fileName)
                                {
                                    using (var stream = zipEntry.Open())
                                    {
                                        using (var copyMem = new MemoryStream())
                                        {
                                            await stream.CopyToAsync(copyMem);
                                            file = Encoding.Default.GetString(copyMem.ToArray()).Replace("\0", string.Empty); ;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

            return await Task.FromResult(file);
        }


        private string GetPiletName(string name)
        {
            var split = name.Split('/');
            return split[0];
        }

        private string GetConnectionString()
        {
            //Add Connection string here
            return "";
        }
    }
}
