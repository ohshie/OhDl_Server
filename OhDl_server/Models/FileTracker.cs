namespace OhDl_server.Models;

public class FileTracker
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string UuId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
}