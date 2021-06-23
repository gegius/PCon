using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCon.Domain.Player;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PCon.Application.HostingService
{
    public class YouTubeHost : IHosting
    {
        private readonly YoutubeClient _youtubeClient;

        public YouTubeHost()
        {
            _youtubeClient = new YoutubeClient();
        }

        public IPlayerSettings GetPlayerSettings()
        {
            return new YouTubePlayer();
        }

        public async Task<Uri> GetUriAsync(string link)
        {
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(link);
            return new Uri(streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality().Url);
        }

        public IAsyncEnumerable<MediaObject> SearchTrendsAsync()
        {
            return GetVideoFromPageAsync("https://www.youtube.com/feed/trending");
        }

        public IAsyncEnumerable<MediaObject> SearchMediaAsync(string query)
        {
            return GetVideoFromPageAsync($"https://www.youtube.com/results?search_query={query}&sp=EgIQAQ%253D%253D");
        }

        private async IAsyncEnumerable<MediaObject> GetVideoFromPageAsync(string url)
        {
            using var client = new WebClient();
            var htmlCode = await client.DownloadStringTaskAsync(url);
            var regex = new Regex(@"{""url"":""(/watch\?v=\w+)""");
            foreach (Match trend in regex.Matches(htmlCode))
            {
                var video = await _youtubeClient.Videos.GetAsync("https://www.youtube.com" + trend.Groups[1]);
                yield return new MediaObject(video.Url, video.Title,
                    $"Длительность: {video.Duration}\n\n{video.Description}", video.Author.Title, video.Duration,
                    video.Thumbnails[2].Url, video.Thumbnails[2].Url);
            }
        }
    }
}