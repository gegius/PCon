using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCon.Domain;
using PCon.Domain.Player;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PCon.Services.HostingService
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

        public async Task<Uri> GetUri(string link)
        {
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(link);
            return new Uri(streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality().Url);
        }

        public async IAsyncEnumerable<MediaObject> SearchMedia(string request)
        {
            await foreach (var videoInfo in _youtubeClient.Search.GetVideosAsync(request))
            {
                yield return new MediaObject(videoInfo.Url, videoInfo.Title,
                    videoInfo.Title, videoInfo.Author.Title,
                    videoInfo.Duration ,videoInfo.Thumbnails[1].Url,
                    videoInfo.Thumbnails[0].Url);
            }
        }

        public IAsyncEnumerable<MediaObject> SearchTrends()
        {
            return GetVideoFromPage("https://www.youtube.com/feed/trending");
        }

        private async IAsyncEnumerable<MediaObject> GetVideoFromPage(string url)
        {
            using var client = new WebClient();
            var htmlCode = await client.DownloadStringTaskAsync(url);
            var regex = new Regex("{\"url\":\"(/watch.*?)\"");
            var trends = regex.Matches(htmlCode).Select(x => x.Groups[1]).ToArray();
            foreach (var trend in trends)
            {
                var video = await _youtubeClient.Videos.GetAsync("https://www.youtube.com" + trend);
                yield return new MediaObject(video.Url, video.Title,
                    video.Description, video.Author.Title, video.Duration,
                    video.Thumbnails[1].Url, video.Thumbnails[0].Url);
            }
        }
    }
}