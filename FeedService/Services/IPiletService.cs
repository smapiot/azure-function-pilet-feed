using FeedService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FeedService.Services
{
    public interface IPiletService
    {
        Task<PiletApiResponse> GetLatestPilets();
        Task<bool> PublishPilet(byte[] fileData);
        Task<string> GetPiletFile(string fileName, string version, string name);
    }
}
