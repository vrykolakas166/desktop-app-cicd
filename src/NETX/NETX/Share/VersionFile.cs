using Newtonsoft.Json;

public class VersionFile
{
    [JsonProperty(nameof(Version))]
    public string? Version { get; set; }

    [JsonProperty(nameof(Date))]
    public string? Date { get; set; }

    [JsonProperty(nameof(Author))]
    public string? Author { get; set; }

    [JsonProperty(nameof(Notes))]
    public string? Notes { get; set; }

    public VersionFile()
    {

    }

    [JsonConstructor]
    public VersionFile(string version, string date, string author, string notes)
    {
        Version = version;
        Date = date;
        Author = author;
        Notes = notes;
    }
}