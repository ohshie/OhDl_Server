namespace OhDl_server.Models;

public class VideoInfo
{
        public string VideoName { get; set; } = string.Empty;

        public string VideoDesc { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;
    
        public string Hosting { get; set; } = string.Empty;

        public List<VideoFormat> Formats { get; set; } = new();
}