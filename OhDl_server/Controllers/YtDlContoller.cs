using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OhDl_server.YtDlp;

namespace OhDl_server.Controllers;

[ApiController]
[Route("[controller]")]
public class YtDlController : ControllerBase
{
    private readonly ILogger<YtDlController> _logger;
    private readonly YtDlOperator _ytDlOperator;

    public YtDlController(ILogger<YtDlController> logger, YtDlOperator ytDlOperator)
    {
        _logger = logger;
        _ytDlOperator = ytDlOperator;
    }

    [EnableCors("AllowSpecificOrigin")]
    [HttpGet(Name = "GetVideoInfo")]
    public async Task<VideoInfo> Get(string videoUrl)
    {
        var videoInfo = await _ytDlOperator.GetVideoInfo(videoUrl);
        return videoInfo;
    }

    [HttpPost(Name = "RequestVideoWithParams")]
    public async Task Post(string videoFormat, string videoUrl)
    {
        await _ytDlOperator.ServeVideo(videoFormat, videoUrl);
    }
}