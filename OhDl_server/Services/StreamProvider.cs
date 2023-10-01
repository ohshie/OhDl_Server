using Microsoft.AspNetCore.Mvc;

namespace OhDl_server.YtDlp;

public class StreamProvider
{
    public FileStreamResult ServeFileStream(string filename, string filePath, string contentType)
    {
        var extension = contentType.Split("/")[1];
        
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = filename + $".{extension}"
        };
    }
}