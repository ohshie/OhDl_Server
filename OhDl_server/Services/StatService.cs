using NYoutubeDL.Models;
using OhDl_server.DataLayer;

namespace OhDl_server.Services;

public class StatService
{
    private readonly DownloadTimeOperator _downloadTimeOperator;
    private readonly ILogger _logger;

    public StatService(DownloadTimeOperator downloadTimeOperator, ILogger<StatService> logger)
    {
        _logger = logger;
        _downloadTimeOperator = downloadTimeOperator;
    }

    public async Task TrackVideoStats(string formatCode, TimeSpan timeToProcess, DownloadInfo info)
    {
        var requestedVideoInfo = (VideoDownloadInfo)info;

        var bitRate = requestedVideoInfo.Formats
            .FirstOrDefault(f => f.FormatId == formatCode)!.Tbr is not null
            ? requestedVideoInfo.Formats.FirstOrDefault(f => f.FormatId == formatCode)!.Tbr
            : requestedVideoInfo.Formats.FirstOrDefault(f => f.FormatId == formatCode)!.Abr;

        var fileSize = (bitRate * requestedVideoInfo.Duration) / 8000;

        await _downloadTimeOperator.AddNewDl(timeToProcess, fileSize, requestedVideoInfo.Duration);
    }

    public async Task<int> GetAvgTimeToDl(double? fileSize)
    {
        if (fileSize is null) return 10;

       var timeToDl = await _downloadTimeOperator.GetTimeToDl(fileSize);

       return timeToDl;
    }
}