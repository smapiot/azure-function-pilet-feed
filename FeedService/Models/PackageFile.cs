namespace FeedService.Models
{
  public class PackageFile
  {
    public string Path { get; set; }
    public string FileName { get; set; }
    public byte[] ContentArray { get; set; }
  }
}
