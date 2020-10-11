using FeedService.Infrastructure;
using FeedService.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

    /// <summary>
    /// Gets the latest pilets from storage account.
    /// </summary>
    /// <returns></returns>
    public async Task<PiletApiResponse> GetLatestPilets()
    {
      var latestPiletsInfo = await _storageAccountRepository.GetLatestPiletsInfoAsync();
      var piletResponse = new PiletApiResponse();

      foreach (var pilet in latestPiletsInfo)
      {
        var version = ExtractVersion(pilet);
        var name = ExtractName(pilet);
        /// This service runs locally. If you want to publish it
        //you need to handle change this URL accordingly.
        var url = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") + "/api/files/" + name + "/" + version + "/index.js";

        piletResponse.Items.Add(new PiletMetaData
        {
          Name = name,
          Link = url,
          Version = version

        });
      }
      return piletResponse;
    }

    /// <summary>
    /// Returns individual files requested by Piral instance.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="version"></param>
    /// <param name="piletName"></param>
    /// <returns>File content.</returns>
    public async Task<string> GetPiletFile(string fileName, string version, string piletName)
    {
      return await _storageAccountRepository.GetPiletFile(fileName, version, piletName);
    }

    /// <summary>
    /// Publishes the Pilet to a storage account.
    /// </summary>
    /// <returns></returns>

    public Task<bool> PublishPilet(byte[] fileData)
    {
      try
      {
        var packageJson = new PackageJson();
        var packageFiles = new List<PackageFile>();
        Stream inStream = new MemoryStream(fileData);
        Stream gzipStream = new GZipInputStream(inStream);
        using (var tarInputStream = new TarInputStream(gzipStream, Encoding.UTF8))
        {
          TarEntry entry;
          while ((entry = tarInputStream.GetNextEntry()) != null)
          {
            using (var fileContents = new MemoryStream())
            {
              tarInputStream.CopyEntryContents(fileContents);
              var content = Encoding.UTF8.GetString(fileContents.GetBuffer());
              var fileName = GetFileName(entry.Name);
              var package = new PackageFile
              {
                Content = content
              };
              switch (fileName)
              {
                case "index.js.map":
                  package.Path = entry.Name;
                  package.FileName = fileName;
                  break;
                case "main.css":
                  package.Path = entry.Name;
                  package.FileName = fileName;
                  break;
                case "index.js":
                  package.Path = entry.Name;
                  package.FileName = fileName;
                  break;
                case "package.json":
                  package.Path = entry.Name;
                  package.FileName = fileName;
                  packageJson = JsonSerializer.Deserialize<PackageJson>(content);
                  break;
                default:
                  package.Path = entry.Name;
                  package.FileName = fileName;
                  break;
              }
              packageFiles.Add(package);
            }
          }
        }

        var zippedFiles = CreateZipFile(packageJson, packageFiles);
        _storageAccountRepository.UploadPilet(packageFiles, packageJson, zippedFiles.ToArray());
      }
      catch (Exception)
      {
        return Task.FromResult(false);
      }
      return Task.FromResult(false);
    }

    private MemoryStream CreateZipFile(PackageJson packageJson, IEnumerable<PackageFile> files)
    {
      try
      {
        using (MemoryStream ms = new MemoryStream())
        {
          using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
          {
            foreach (var file in files)
            {
              ZipArchiveEntry entry = archive.CreateEntry(file.Path);
              using (StreamWriter writer = new StreamWriter(entry.Open()))
              {
                writer.Write(file.Content);
              }
            }
          }

          return ms;
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    private static string GetFileName(string path)
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
      return pilet.Split('/')[1].Substring(0, 5);
    }
  }
}
