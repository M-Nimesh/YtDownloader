using YouTubeDownloaderApi.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace YouTubeDownloaderApi.Services
{

    internal class YtDlpVideoInfo
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("uploader")] // Channel Title
        public string? Uploader { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("duration")] // Seconds
        public double? Duration { get; set; }

        [JsonPropertyName("upload_date")] // YYYYMMDD format
        public string? UploadDate { get; set; }

        [JsonPropertyName("view_count")]
        public long? ViewCount { get; set; }

        [JsonPropertyName("thumbnails")]
        public List<YtDlpThumbnail>? Thumbnails { get; set; }

        [JsonPropertyName("formats")]
        public List<YtDlpFormat>? Formats { get; set; }
    }

    internal class YtDlpThumbnail
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }
    }

    internal class YtDlpFormat
    {
        [JsonPropertyName("format_id")]
        public string? FormatId { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("ext")] // container extension, e.g., "mp4", "webm"
        public string? Ext { get; set; }

        [JsonPropertyName("vcodec")] // video codec, e.g., "avc1.640028"
        public string? VCodec { get; set; }

        [JsonPropertyName("acodec")] // audio codec, e.g., "mp4a.40.2"
        public string? ACodec { get; set; }

        [JsonPropertyName("filesize")]
        public long? FileSize { get; set; }

        [JsonPropertyName("height")] // resolution height for video
        public int? Height { get; set; }

        [JsonPropertyName("tbr")] // total bitrate (video+audio) in Kbits/s
        public double? Tbr { get; set; }

        [JsonPropertyName("abr")] // audio bitrate in Kbits/s
        public double? Abr { get; set; }

        [JsonPropertyName("vbr")] // video bitrate in Kbits/s
        public double? Vbr { get; set; }
    }


    public class YouTubeDLService
    {
        private const string YtDlpExecutable = "yt-dlp"; // yt-dlp.exe is expected to be in PATH or current directory

        public async Task<VideoInfoResponse> GetYouTubeVideoInfo(string youtubeVideoUrl, bool isDevelopment)
        {
            var response = new VideoInfoResponse();

            try
            {

                var startInfo = new ProcessStartInfo
                {
                    FileName = YtDlpExecutable,
                    Arguments = $"--dump-json \"{youtubeVideoUrl}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start yt-dlp process. Make sure yt-dlp is installed and in your system's PATH.");
                }

                string jsonOutput = await process.StandardOutput.ReadToEndAsync();
                string errorOutput = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"yt-dlp exited with code {process.ExitCode}. Error: {errorOutput}. Check if the URL is valid or if yt-dlp needs updating.");
                }

                var ytDlpInfo = JsonSerializer.Deserialize<YtDlpVideoInfo>(jsonOutput);

                if (ytDlpInfo == null)
                {
                    throw new InvalidOperationException("Failed to parse yt-dlp JSON output. The output might be invalid or unexpected.");
                }


                response.Id = ytDlpInfo.Id;
                response.Title = ytDlpInfo.Title;
                response.Author = ytDlpInfo.Uploader;
                response.Description = ytDlpInfo.Description;
                response.DurationSeconds = ytDlpInfo.Duration;

                // YYYYMMDD format එක YYYY-MM-DD බවට convert කරන්න
                if (!string.IsNullOrWhiteSpace(ytDlpInfo.UploadDate) &&
                    DateTime.TryParseExact(ytDlpInfo.UploadDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var uploadDate))
                {
                    response.UploadDate = uploadDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    response.UploadDate = ytDlpInfo.UploadDate;
                }

                response.Views = ytDlpInfo.ViewCount;

                // Thumbnails සියල්ලම එකතු කරන්න
                response.Thumbnails = ytDlpInfo.Thumbnails?
                    .OrderByDescending(t => t.Width) 
                    .Select(t => new ThumbnailInfo
                    {
                        Url = t.Url,
                        Width = t.Width ?? 0,
                        Height = t.Height ?? 0
                    }).ToList() ?? new List<ThumbnailInfo>(); 

                var availableStreams = new List<StreamInfo>();

                // Video + Audio (Muxed) formats පමණක් filter කරන්න
                // yt-dlp format එකක video codec (vcodec) සහ audio codec (acodec) දෙකම තිබේ නම්,
                // එය "muxed" හෝ "combined" stream එකකි.
                var muxedFormats = ytDlpInfo.Formats?
                    .Where(f => !string.IsNullOrEmpty(f.VCodec) && f.VCodec != "none" &&
                                !string.IsNullOrEmpty(f.ACodec) && f.ACodec != "none" &&
                                f.Url != null) // URL එකක් තිබිය යුතුය
                    .OrderByDescending(f => f.Height ?? 0) // Resolution අනුව වර්ග කරන්න
                    .ThenByDescending(f => f.Tbr ?? 0) // Total bitrate අනුව වර්ග කරන්න
                    .ToList();

                if (muxedFormats != null)
                {
                    foreach (var format in muxedFormats)
                    {
                        availableStreams.Add(new StreamInfo
                        {
                            Quality = format.Height.HasValue ? $"{format.Height}p" : "Unknown Quality",
                            Container = format.Ext,
                            Codec = $"{format.VCodec} ({format.ACodec})",
                            SizeBytes = format.FileSize ?? 0,
                            Url = format.Url,
                            Type = "Muxed",
                            IsAudioOnly = false,
                            IsVideoOnly = false
                        });
                    }
                }

                response.AvailableStreams = availableStreams;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                if (isDevelopment)
                {
                    response.StackTrace = ex.StackTrace;
                }
            }

            return response;
        }
    }
}