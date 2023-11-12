namespace OhDl_server.Models;

public class DownloadTimeStat
{
    public int Id { get; set; }
    public double FileSize { get; set; }
    public TimeSpan TimeToDl { get; set; }
    public double VideoDuration { get; set; }
}