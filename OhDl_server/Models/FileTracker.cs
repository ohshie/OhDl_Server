namespace OhDl_server.Models;

public class FileTracker
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string UuId { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
}