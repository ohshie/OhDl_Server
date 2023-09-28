using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OhDl_server.YtDlp;

namespace OhDl_server.Controllers;

[ApiController]
[Route("[controller]")]
public class YtDlController : ControllerBase
{
    private readonly ILogger<YtDlController> _logger;
    private readonly YtDlOperator _ytDlOperator;
    private readonly HttpClient _httpClient;

    public YtDlController(ILogger<YtDlController> logger, 
        YtDlOperator ytDlOperator,
        HttpClient httpClient)
    {
        _logger = logger;
        _ytDlOperator = ytDlOperator;
        _httpClient = httpClient;
    }

    [EnableCors("Base")]
    [HttpGet(Name = "GetVideoInfo")]
    public async Task<VideoInfo> Get(string videoUrl)
    {
        var videoInfo = await _ytDlOperator.GetVideoInfo(videoUrl);
        return videoInfo;
    }

    [EnableCors("Base")]
    [HttpPost(Name = "RequestAudioOnly")]
    public async Task<IActionResult> PostAudioOnly(string videoUrl)
    {
        var (filePath, filename) = await _ytDlOperator.ServeAudioOnly(videoUrl);

        if (!System.IO.File.Exists(filePath)) return NotFound();

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        
        return new FileStreamResult(stream, "audio/mp3")
        {
            FileDownloadName = filename+".mp3"
        };
    }
}