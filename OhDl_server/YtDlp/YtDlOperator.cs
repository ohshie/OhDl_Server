using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace OhDl_server.YtDlp;

public class YtDlOperator
{
    private readonly ILogger<YtDlOperator> _logger;
    private readonly YoutubeDL _youtubeDl;
    private readonly FormatSorter _sorter;

    public YtDlOperator(ILogger<YtDlOperator> logger, 
        YoutubeDL youtubeDl, FormatSorter sorter)
    {
        _logger = logger;
        _youtubeDl = youtubeDl;
        this._sorter = sorter;
    }

    public async Task<VideoInfo> GetVideoInfo(string videoUrl)
    {
        var requestedVideoInfo = await _youtubeDl.RunVideoDataFetch(videoUrl);
        
        VideoInfo videoInfo = new()
        {
            VideoName = requestedVideoInfo.Data.Title,
            Hosting = requestedVideoInfo.Data.ExtractorKey,
            Thumbnail = requestedVideoInfo.Data.Thumbnail
        };

        if (!string.IsNullOrEmpty(requestedVideoInfo.Data.Description))
            videoInfo.VideoDesc = requestedVideoInfo.Data.Description;
        
        _sorter.Execute(requestedVideoInfo.Data.Formats, videoInfo, requestedVideoInfo.Data.Duration);
        
        
        return videoInfo;
    }

    public async Task ServeVideo(string videoFormat, string videoUrl)
    {
        var options = new OptionSet()
        {
            Format = $"{videoFormat}+bestaudio",
            FormatSort = "vcodec:h264,res,acodec:m4a",
            //RecodeVideo = VideoRecodeFormat.Mp4
        };

        await _youtubeDl.RunWithOptions(url: videoUrl,
            options: options);

        Console.WriteLine("test");
    }
}

public class VideoInfo
{
    public string VideoName { get; set; } = string.Empty;

    public string VideoDesc { get; set; } = string.Empty;

    public string Thumbnail { get; set; } = string.Empty;
    
    public string Hosting { get; set; } = string.Empty;

    public List<VideoFormat> Formats { get; set; } = new();
}