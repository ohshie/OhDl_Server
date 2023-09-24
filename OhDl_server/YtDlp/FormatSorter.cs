using YoutubeDLSharp.Metadata;

namespace OhDl_server.YtDlp;

public class FormatSorter
{
    public void Execute(FormatData[] formats, VideoInfo videoInfo, float? duration)
    {
        var groupedFormats = formats
            .Where(f => f.Height != null)
            .GroupBy(f => new{f.Height, f.FrameRate});

        foreach (var group in groupedFormats)
        {
            var preferredFormat = group.FirstOrDefault(f => !f.VideoCodec.StartsWith("vp")) // First try to find non-vp codec
                                  ?? group.First();
            
            VideoFormat mappedFormat = videoInfo.Formats
                .FirstOrDefault(f => f.Width == preferredFormat.Width && f.FrameRate == preferredFormat.FrameRate) ?? new();
            
            var fileSize = duration * preferredFormat.Bitrate / 1024;
            if (fileSize > 2048)  mappedFormat.BigFile = true;
            
            if (preferredFormat.VideoCodec.StartsWith("vp"))
            {
                mappedFormat.WebmOnly = true;
            }
            
            mappedFormat.FormatCode = preferredFormat.FormatId;
            mappedFormat.Width = preferredFormat.Width;
            mappedFormat.Height = preferredFormat.Height;
            mappedFormat.FrameRate = preferredFormat.FrameRate;
            if (preferredFormat.AudioCodec != "none")
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

    private string FormatEvaluator(FormatData format)
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
    
    public float? FrameRate { get; set; }

    public bool OneFile { get; set; } = false;

    public bool BigFile { get; set; } = false;

    public bool WebmOnly { get; set; } = false;
}