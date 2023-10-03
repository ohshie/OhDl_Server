namespace OhDl_server.Models;

public class RequestedVideo
{
    public string VideoUrl { get; set; } = string.Empty;
    public string FormatCode { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}