using Microsoft.AspNetCore.Http.HttpResults;
using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Models;


namespace OhDl_server.YtDlp;

public class YtDlOperator
{
    private readonly ILogger<YtDlOperator> _logger;
    private readonly YoutubeDLP _youtubeDl;
    private readonly FormatSorter _sorter;

    public YtDlOperator(ILogger<YtDlOperator> logger, 
        YoutubeDLP youtubeDl,
        FormatSorter sorter)
    {
        _logger = logger;
        _youtubeDl = youtubeDl;
        _sorter = sorter;

        _youtubeDl.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
        _youtubeDl.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);
    }

    public async Task<VideoInfo> GetVideoInfo(string videoUrl)
    {
        _youtubeDl.VideoUrl = videoUrl;
        
        await _youtubeDl.PrepareDownloadAsync();
        await _youtubeDl.GetDownloadInfoAsync();
        
        VideoDownloadInfo requestedVideoInfo = (VideoDownloadInfo)_youtubeDl.Info;

        VideoInfo videoInfo = new()
        {
            VideoName = requestedVideoInfo.Title,
            Hosting = requestedVideoInfo.ExtractorKey,
            Thumbnail = requestedVideoInfo.Thumbnail
        };

        if (!string.IsNullOrEmpty(requestedVideoInfo.Description))
            videoInfo.VideoDesc = requestedVideoInfo.Description;

        _sorter.Execute(requestedVideoInfo.Formats, videoInfo, requestedVideoInfo.Duration);
        
        return videoInfo;
    }
    
    public async Task<(string, string)> ServeAudioOnly(string videoUrl)
    {
        _logger.LogInformation(eventId: 1,"starting audio only process");
        
        _youtubeDl.VideoUrl = videoUrl;

        _youtubeDl.Options.VideoFormatOptions.FormatAdvanced = "\"ba*[vcodec=none]\"";
        _youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
        _youtubeDl.Options.PostProcessingOptions.AudioFormat = Enums.AudioFormat.mp3;
        
        _youtubeDl.Options.FilesystemOptions.Output = "\"./audio/%(title)s.%(ext)s\"";
       
        await _youtubeDl.PrepareDownloadAsync();
        
        Task download =  _youtubeDl.DownloadAsync();
        Task getInfo = _youtubeDl.GetDownloadInfoAsync();

        await Task.WhenAll(getInfo, download);

        var fileName = _youtubeDl.Info.Title;
        var fileLocation = $"./audio/{fileName}.mp3";
        
        _logger.LogInformation("Audio processed");
        
        return (fileLocation, fileName);
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