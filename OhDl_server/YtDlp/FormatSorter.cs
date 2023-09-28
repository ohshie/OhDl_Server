using NYoutubeDL.Models;
using YoutubeDLSharp.Metadata;

namespace OhDl_server.YtDlp;

public class FormatSorter
{
    public void Execute(List<FormatDownloadInfo> formats, VideoInfo videoInfo, double? duration)
    {
        var groupedFormats = formats
            .Where(f => f.Height != null & f.Height > 144)
            .GroupBy(f => new{f.Height, f.Fps});

        foreach (var group in groupedFormats)
        {
            
            var preferredFormat = group.FirstOrDefault(f => f.Vcodec != null && !f.Vcodec.StartsWith("vp"))
                                  ?? group.First();
            
            VideoFormat mappedFormat = videoInfo.Formats
                .FirstOrDefault(f => f.Width == preferredFormat.Width && f.FrameRate == preferredFormat.Fps) ?? new();
            
            var fileSize = duration * preferredFormat.Abr / 1024;
            if (fileSize > 2048)  mappedFormat.BigFile = true;
            
            if (preferredFormat.Vcodec != null && preferredFormat.Vcodec.StartsWith("vp"))
            {
                mappedFormat.WebmOnly = true;
            }
            
            mappedFormat.FormatCode = preferredFormat.FormatId;
            mappedFormat.Width = preferredFormat.Width;
            mappedFormat.Height = preferredFormat.Height;

            if (preferredFormat.Fps != null)
            {
                mappedFormat.FrameRate = preferredFormat.Fps;
            }
           
            if (preferredFormat.Acodec != null && preferredFormat.Acodec != "none")
            {
                mappedFormat.OneFile = true;
            }

            mappedFormat.Size = FormatEvaluator(preferredFormat);
            
            if (!videoInfo.Formats.Contains(mappedFormat))
            {
                videoInfo.Formats.Add(mappedFormat);
            };
        }
    }

    private string FormatEvaluator(FormatDownloadInfo format)
    {
        return format.Height switch
        {
            < 144 => "Puny",
            >= 144 and < 360 => "Tiny",
            >= 360 and < 480 => "ExtraSmall",
            >= 480 and < 720 => "Small",
            >= 720 and < 1080 => "Medium",
            >= 1080 and < 1440 => "Large",
            >= 1440 and < 2160 => "ExtraLarge",
            _ => "Huge"
        };
    }
}

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