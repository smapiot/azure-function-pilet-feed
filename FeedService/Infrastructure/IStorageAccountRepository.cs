using FeedService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FeedService.Infrastructure
{
    public interface IStorageAccountRepository
    {
        Task<List<string>> GetLatestPiletsInfoAsync();
        Task UploadPilet(List<PackageFile> packageFiles, PackageJson packageJson, byte[] zippedFiles);
        Task<string> GetPiletFile(string fileName, string version, string piletName);
    }
}
