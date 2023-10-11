using System.Text.RegularExpressions;
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
        
        _youtubeDl.Options.FilesystemOptions.RestrictFilenames = true;
        _youtubeDl.Options.DownloadOptions.ExternalDownloader = Enums.ExternalDownloader.aria2c;
        _youtubeDl.Options.FilesystemOptions.CacheDir = $"\"./cache";
    }

    public async Task<DlVideoInfo> GetVideoInfo(string videoUrl)
    {
        _logger.Log(LogLevel.Information,"Getting video info for {Url}", videoUrl);
        
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

        _logger.Log(LogLevel.Information,"Sorting formats for {Url}", videoUrl);
        _sorter.Execute(requestedVideoInfo.Formats, videoInfo, requestedVideoInfo.Duration);
        
        return videoInfo;
    }
    
    public async Task<(string, string)> ServeAudioOnly(string videoUrl, Guid uuId)
    {
        _logger.Log(LogLevel.Information,"starting audio only process for {VideoUrl} by {UuId}", videoUrl, uuId);

       var directory = DirectoryCreator("mp3", uuId);
        
        _youtubeDl.VideoUrl = HttpUtility.UrlDecode(videoUrl);
        
        _youtubeDl.Options.VideoFormatOptions.FormatAdvanced = "\"ba*[vcodec=none]\"";
        _youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
        _youtubeDl.Options.PostProcessingOptions.AudioFormat = Enums.AudioFormat.mp3;
        
        var filePath = await DownloadProcess(directory.FullName);

        var filename = filePath.Split("/").Last();
        
        _logger.Log(LogLevel.Information ,"Audio processed into {Filename} {FilePath}", filename,filePath);
        
        return (filePath, filename);
    }

    public async Task<(string, string)> ServeVideo(string videoUrl, string formatCode, Guid uuId)
    {
        _logger.LogInformation(eventId: 2, "starting video serving process for {VideoUrl} by {UuId}", videoUrl, uuId);

        var directory = DirectoryCreator("mp4", uuId);
        
        _youtubeDl.VideoUrl = videoUrl;
        
        _youtubeDl.Options.VideoFormatOptions.FormatAdvanced = formatCode+"+ba";
        _youtubeDl.Options.PostProcessingOptions.RemuxVideo = "mp4";
        
        var filePath = await DownloadProcess(directory.FullName);
        var filename = filePath.Split("/").Last();
        
        _logger.LogInformation("Video processed into {Filename} {Filepath}", filename, filePath);
        
        return (filePath, filename);
    }

    private async Task<string> DownloadProcess(string directory)
    {
        _youtubeDl.Options.FilesystemOptions.Output = $"\"{directory}/%(title)s.%(ext)s\"";
        
        await _youtubeDl.PrepareDownloadAsync();
        
        Task download =  _youtubeDl.DownloadAsync();
        Task getInfo = _youtubeDl.GetDownloadInfoAsync();

        await Task.WhenAll(getInfo, download);

        return Directory.GetFiles(directory).Last();
    }

    private DirectoryInfo DirectoryCreator(string type, Guid uuId)
    {
        int randomDirectory;
        do
        {
            randomDirectory = new Random().Next(0, 10000);
        } while (Directory.Exists(AssemblePath(type, uuId: uuId, randomDirectory)));
        
        var directory = Directory.CreateDirectory(AssemblePath(type, uuId: uuId, randomDirectory));

        return directory;
    }
    
    private string AssemblePath(string type, Guid uuId, int randomDirectory)
    {
        string[] filePaths = 
        { 
            AppDomain.CurrentDomain.BaseDirectory,
            type,
            uuId.ToString(),
            randomDirectory.ToString()
        };

        return Path.Combine(filePaths);
    }
}

