using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCon.Domain.Player;
using YoutubeAPI;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace PCon.Application.VideoSource
{
    public class YoutubeVideoSource : IVideoSource
    {
        private readonly YoutubeClient _youtubeClient;
        private readonly YoutubeApi _youtubeApi;

        public YoutubeVideoSource()
        {
            _youtubeClient = new YoutubeClient();
            _youtubeApi = new YoutubeApi();
        }

        public IPlayerSettings GetPlayerSettings()
        {
            return new YouTubePlayerSettings();
        }

        public async Task<Uri> GetUriAsync(string link)
        {
            var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(link);
            return new Uri(streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality().Url);
        }

        public async IAsyncEnumerable<MediaObject> SearchTrendsAsync()
        {
            foreach (var trend in await _youtubeApi.GetTrendsVideosAsync())
            {
                var video = await _youtubeClient.Videos.GetAsync(YoutubeApi.Url + trend);
                yield return new MediaObject(video.Url, video.Title,
                    $"Длительность: {video.Duration}\n\n{video.Description}", video.Author.Title, video.Duration,
                    video.Thumbnails[2].Url, video.Thumbnails[2].Url);
            }
        }

        public async IAsyncEnumerable<MediaObject> SearchMediaAsync(string query)
        {
            foreach (var link in await _youtubeApi.SearchByQueryAsync(query))
            {
                var video = await _youtubeClient.Videos.GetAsync(YoutubeApi.Url + link);
                yield return new MediaObject(video.Url, video.Title,
                    $"Длительность: {video.Duration}\n\n{video.Description}", video.Author.Title, video.Duration,
                    video.Thumbnails[2].Url, video.Thumbnails[2].Url);
            }
        }
    }
}