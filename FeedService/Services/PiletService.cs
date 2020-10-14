using FeedService.Infrastructure;
using FeedService.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeedService.Services
{
  public class PiletService : IPiletService
  {
    private readonly IStorageAccountRepository _storageAccountRepository;
    public PiletService(IStorageAccountRepository storageAccountRepository)
    {
      _storageAccountRepository = storageAccountRepository;

    }

    public async Task<PiletApiResponse> GetLatestPiletsMetaData()
    {
      var latestPiletsInfo = await _storageAccountRepository.GetLatestPiletsPackageFiles();
      var piletResponse = new PiletApiResponse();

      foreach (var pilet in latestPiletsInfo)
      {
        var version = ExtractVersion(pilet);
        var name = ExtractName(pilet);
        var url = GetUrl(name, version);
        piletResponse.Items.Add(new PiletMetaData
        {
          Name = name,
          Link = url,
          Version = version

        });
      }
      return piletResponse;
    }

    public async Task<Stream> GetPiletFile(string fileName, string version, string piletName)
    {
      return await _storageAccountRepository.GetFile(fileName, version, piletName);
    }

    public Task<bool> PublishPilet(byte[] fileData)
    {
      try
      {
        var files = ExtractFiles(fileData);
        _storageAccountRepository.UploadFiles(files);
      }
      catch (Exception)
      {
        return Task.FromResult(false);
      }

      return Task.FromResult(false);
    }

    public List<PackageFile> ExtractFiles(byte[] fileData)
    {
      var packageJsonFile = new PackageJson();
      var files = new List<PackageFile>();
      Stream inStream = new MemoryStream(fileData);
      Stream gzipStream = new GZipInputStream(inStream);
      using (var tarInputStream = new TarInputStream(gzipStream, null))
      {
        TarEntry entry;
        while ((entry = tarInputStream.GetNextEntry()) != null)
        {
          using (var fileContents = new MemoryStream())
          {
            tarInputStream.CopyEntryContents(fileContents);
            var fileName = GetFileName(entry.Name);
            if (fileName == "package.json")
            {
              packageJsonFile = ParsePackageJson(fileContents, entry.Name);
            }

            files.Add(new PackageFile
            {
              FileName = entry.Name,
              ContentArray = fileContents.GetBuffer()
            });
          }
        }
      }

      SetFilePath(files, packageJsonFile);
      return files;
    }

    public PackageJson ParsePackageJson(MemoryStream fileContents, string fileFullName)
    {
      var content = Encoding.UTF8.GetString(fileContents.GetBuffer());
      return JsonSerializer.Deserialize<PackageJson>(content);
    }


    private void SetFilePath(List<PackageFile> files, PackageJson packageJsonFile)
    {
      foreach (var file in files)
      {
        file.Path = packageJsonFile.Name + "/" + packageJsonFile.Version + "/" + file.FileName;
      }
    }

    private string GetFileName(string path)
    {
      var s = path.Split('/');
      return s[s.Length - 1];
    }

    private string ExtractName(string pilet)
    {
      return pilet.Split('/')[0];
    }

    private string ExtractVersion(string pilet)
    {
      return pilet.Split('/')[1];
    }

    private string GetUrl(string name, string version)
    {
      return (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "LocalDevelopment" ? "https://" : "http://")
        + Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") + "/api/pilet/" + name + "/" + version + "/index.js";
    }

  }
}
