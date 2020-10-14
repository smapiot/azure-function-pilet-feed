using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FeedService.Models
{
  public class PackageJson
  {
    /// <summary>
    /// Represents the pilet name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonPropertyName("version")]
    public string Version { get; set; }
    [JsonPropertyName("preview")]
    public bool Preview { get; set; }
    [JsonPropertyName("custom")]
    public string Custom { get; set; }
    [JsonPropertyName("author")]
    public Author AuthorInfo { get; set; }
    [JsonPropertyName("main")]
    public string Main { get; set; }
    [JsonPropertyName("license")]
    public string License { get; set; }
    [JsonPropertyName("devDependencies")]
    public DevDependencies DevDependencies { get; set; }
    [JsonPropertyName("dependencies")]
    public IDictionary<string, string> Dependencies { get; set; }
    [JsonPropertyName("peerDependencies")]
    public PeerDependencies PeerDependencies { get; set; }
  }

  public class Author
  {
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
  }

  public class DevDependencies
  {
    [JsonPropertyName("names")]
    public List<string> Names { get; set; }
  }

  public class PeerDependencies
  {
  }
}
