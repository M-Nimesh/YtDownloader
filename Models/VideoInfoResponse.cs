// Models/VideoInfoResponse.cs
namespace YouTubeDownloaderApi.Models
{
    public class VideoInfoResponse
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public double? DurationSeconds { get; set; }
        public string? UploadDate { get; set; }
        public List<ThumbnailInfo>? Thumbnails { get; set; } 
        public long? Views { get; set; }
        public List<StreamInfo>? AvailableStreams { get; set; } 
        public string? Error { get; set; }
        public string? StackTrace { get; set; }
    }

    public class ThumbnailInfo
    {
        public string? Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class StreamInfo
    {
        public string? Quality { get; set; }
        public string? Container { get; set; }
        public string? Codec { get; set; }
        public long SizeBytes { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; } // "Muxed"
        public bool IsAudioOnly { get; set; }
        public bool IsVideoOnly { get; set; }
    }
}