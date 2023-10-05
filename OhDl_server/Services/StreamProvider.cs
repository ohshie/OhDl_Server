using Microsoft.AspNetCore.Mvc;

namespace OhDl_server.YtDlp;

public class StreamProvider
{
    private readonly ILogger<StreamProvider> _logger;

    public StreamProvider(ILogger<StreamProvider> logger)
    {
        _logger = logger;
    }

    public FileStreamResult ServeFileStream(string filename, string filePath, string contentType)
    {
        _logger.Log(LogLevel.Information,"Started serving stream for {filename} of {type}", filename, contentType);
        
        var extension = contentType.Split("/")[1];
        
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = filename + $".{extension}"
        };
    }
}