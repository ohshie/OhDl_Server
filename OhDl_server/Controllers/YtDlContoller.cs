using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OhDl_server.YtDlp;
using OhDl_server.Models;

namespace OhDl_server.Controllers;

[ApiController]
[Route("[controller]")]
public class YtDlController : ControllerBase
{
    private readonly ILogger<YtDlController> _logger;
    private readonly YtDlOperator _ytDlOperator;
    private readonly StreamProvider _streamProvider;

    public YtDlController(ILogger<YtDlController> logger, 
        YtDlOperator ytDlOperator, StreamProvider streamProvider)
    {
        _logger = logger;
        _ytDlOperator = ytDlOperator;
        _streamProvider = streamProvider;
    }

    [EnableCors("Base")]
    [HttpGet("GetVideoInfo",Name = "GetVideoInfo")]
    public async Task<VideoInfo> GetInfo(string videoUrl)
    {
        if (!Validator.Validator.ValidateUrl(videoUrl, true)) return null;
        
        var videoInfo = await _ytDlOperator.GetVideoInfo(videoUrl);

        return videoInfo;
    }
    
    [EnableCors("Base")]
    [HttpPost("RequestVideo",Name = "RequestVideo")]
    public async Task<IActionResult> PostVideoByFormat(string videoUrl, string formatCode)
    {
        var (filePath, fileName) = await _ytDlOperator.ServeVideo(videoUrl, formatCode);
        
        if (!System.IO.File.Exists(filePath)) return NotFound();
        
        return _streamProvider.ServeFileStream(fileName, filePath, "video/mp4");
    }

    [EnableCors("Base")]
    [HttpPost("RequestAudioOnly",Name = "RequestAudioOnly")]
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