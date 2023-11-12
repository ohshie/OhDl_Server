using OhDl_server.DataLayer.Repository;
using OhDl_server.Models;
using Open.Linq.AsyncExtensions;

namespace OhDl_server.DataLayer;

public class DownloadTimeOperator
{
    private readonly IRepository<DownloadTimeStat> _repository;
    private readonly ILogger _logger;

    public DownloadTimeOperator(ILogger<DownloadTimeOperator> logger, IRepository<DownloadTimeStat> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task AddNewDl(TimeSpan timeToDl, double? fileSize, double? videoDuration)
    {
        var stats = new DownloadTimeStat()
        {
            FileSize = (double)fileSize!,
            TimeToDl = timeToDl,
            VideoDuration = (double)videoDuration!
        };

        await _repository.Add(stats);
        _logger.LogInformation("New entry to stats created");
    }

    public async Task<int> GetTimeToDl(double? fileSie)
    {
        var dlTimeStatList = await _repository.GetAll()
            .Where(dts => dts.FileSize * 0.85 < fileSie && dts.FileSize * 1.15 > fileSie).ToList();

        if (!dlTimeStatList.Any()) return 10;
   
        var averageTimeToDl = Convert.ToInt32(dlTimeStatList.Average(dts => dts.TimeToDl.TotalSeconds));

        return averageTimeToDl;
    }
}