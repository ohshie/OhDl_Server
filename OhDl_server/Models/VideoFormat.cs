namespace OhDl_server.Models;

public class VideoFormat
{
    public string FormatCode { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int? Width { get; set; }
    
    public int? Height { get; set; }
    
    public double? FrameRate { get; set; }

    public bool OneFile { get; set; } = false;

    public bool BigFile { get; set; } = false;

    public bool WebmOnly { get; set; } = false;
}