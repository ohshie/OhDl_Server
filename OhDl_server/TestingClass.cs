using OhDl_server.YtDlp;

namespace OhDl_server;

public class TestingClass
{
    private readonly YtDlOperator _ytDlOperator;

    public TestingClass(YtDlOperator ytDlOperator)
    {
        _ytDlOperator = ytDlOperator;
    }

    public async Task Test()
    {
        var videoUrl = "https://www.youtube.com/watch?v=uea6c9_FCyE";
        await _ytDlOperator.ServeAudioOnly(videoUrl);
        //await _ytDlOperator.GetVideoInfo(videoUrl);
    }
}