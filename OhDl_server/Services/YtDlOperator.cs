using System.Web;
using NYoutubeDL;
using NYoutubeDL.Helpers;
using NYoutubeDL.Models;
using OhDl_server.Models;

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

    public async Task<DlVideoInfo> GetVideoInfo(string videoUrl)
    {
        _logger.Log(LogLevel.Information,"Getting video info for {url}", videoUrl);
        
        _youtubeDl.VideoUrl = HttpUtility.UrlDecode(videoUrl);
        
        await _youtubeDl.PrepareDownloadAsync();
        await _youtubeDl.GetDownloadInfoAsync();
        
        VideoDownloadInfo requestedVideoInfo = (VideoDownloadInfo)_youtubeDl.Info;

        DlVideoInfo videoInfo = new()
        {
            VideoName = requestedVideoInfo.Title,
            Hosting = requestedVideoInfo.ExtractorKey,
            Thumbnail = requestedVideoInfo.Thumbnail
        };

        if (!string.IsNullOrEmpty(requestedVideoInfo.Description))
            videoInfo.VideoDesc = requestedVideoInfo.Description;

        _logger.LogInformation(eventId: 1,"Sorting formats for {url}", videoUrl);
        _sorter.Execute(requestedVideoInfo.Formats, videoInfo, requestedVideoInfo.Duration);
        
        return videoInfo;
    }
    
    public async Task<(string, string)> ServeAudioOnly(string videoUrl, Guid uuId)
    {
        _logger.LogInformation(eventId: 1,"starting audio only process for {videoUrl} by {uuId}", videoUrl, uuId);
        
        _youtubeDl.VideoUrl = HttpUtility.UrlDecode(videoUrl);

        _youtubeDl.Options.VideoFormatOptions.FormatAdvanced = "\"ba*[vcodec=none]\"";
        _youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
        _youtubeDl.Options.PostProcessingOptions.AudioFormat = Enums.AudioFormat.mp3;
        
        _youtubeDl.Options.FilesystemOptions.Output = $"\"./audio/{uuId}/%(title)s.%(ext)s\"";
       
        await DownloadProcess();

        var fileName = _youtubeDl.Info.Title;
        var fileLocation = $"./audio/{uuId}/{fileName}.mp3";
        
        _logger.LogInformation("Audio processed");
        
        return (fileLocation, fileName);
    }

    public async Task<(string, string)> ServeVideo(string videoUrl, string formatCode, Guid uuId)
    {
        _logger.LogInformation(eventId: 2, "starting video serving process");

        _youtubeDl.VideoUrl = videoUrl;

        _youtubeDl.Options.VideoFormatOptions.FormatAdvanced = formatCode+"+ba";
        _youtubeDl.Options.PostProcessingOptions.RemuxVideo = "mp4";
        
        _youtubeDl.Options.FilesystemOptions.Output = $"\"./video/{uuId}/%(title)s.%(ext)s\"";

        await DownloadProcess();
        
        string fileName = _youtubeDl.Info.Title;
        string fileLocation = $"./video/{uuId}/{fileName}.mp4";
        
        _logger.LogInformation("Video processed");
        
        return (fileLocation, fileName);
    }

    private async Task DownloadProcess()
    {
        await _youtubeDl.PrepareDownloadAsync();
        
        Task download =  _youtubeDl.DownloadAsync();
        Task getInfo = _youtubeDl.GetDownloadInfoAsync();

        await Task.WhenAll(getInfo, download);
    }
}

