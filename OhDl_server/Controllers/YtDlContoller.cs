using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OhDl_server.DataLayer;
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
    private readonly FileOperator _fileOperator;

    public YtDlController(ILogger<YtDlController> logger, 
        YtDlOperator ytDlOperator, StreamProvider streamProvider,
        FileOperator fileOperator)
    {
        _logger = logger;
        _ytDlOperator = ytDlOperator;
        _streamProvider = streamProvider;
        _fileOperator = fileOperator;
    }

    [EnableCors("Base")]
    [HttpPost("RequestVideoInfo",Name = "RequestVideoInfo")]
    public async Task<IActionResult> RequestVideoInfo([FromBody]DlVideoInfo video)
    {
        if (!Validator.Validator.ValidateUrl(video.VideoUrl, true)) return BadRequest(new {message = "Bad Url"});
        
        var videoInfo = await _ytDlOperator.GetVideoInfo(video.VideoUrl);

        return Ok(videoInfo);
    }
    
    [EnableCors("Base")]
    [HttpPost("RequestVideo",Name = "RequestVideo")]
    public async Task<IActionResult> PostVideoByFormat([FromBody] RequestedVideo video)
    {
       if(!Request.Headers.TryGetValue("X-User-ID", out var userID))
           return BadRequest("No cookie provided");

       video.UserId = userID;
       
       var (filePath, filename) = await _ytDlOperator.ServeVideo(video.VideoUrl, video.FormatCode, video.UserId);
        
       if (!System.IO.File.Exists(filePath)) return NotFound(new {message = "No file was downloaded for some reason."});
        
       await _fileOperator.RegisterNewFile(filename, "audio", userID);
       
       var stream = _streamProvider.ServeFileStream(filename, filePath, "video/mp4");

       return stream;
    }

    [EnableCors("Base")]
    [HttpPost("RequestAudioOnly",Name = "RequestAudioOnly")]
    public async Task<IActionResult> PostAudioOnly([FromBody] RequestedVideo video)
    {
        if(!Request.Headers.TryGetValue("X-User-ID", out var userID))
            return BadRequest("No cookie provided");

        video.UserId = userID;
        
        var (filePath, filename) = await _ytDlOperator.ServeAudioOnly(video.VideoUrl, video.UserId);

        if (!System.IO.File.Exists(filePath)) return NotFound(new {message = "No file was downloaded for some reason."});

        await _fileOperator.RegisterNewFile(filename, "audio", userID);
        
        var stream = _streamProvider.ServeFileStream(filename, filePath, "audio/mp3");

        return stream;
    }
}