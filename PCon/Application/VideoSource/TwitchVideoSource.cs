using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCon.Domain.Player;
using TwitchAPI;

namespace PCon.Application.VideoSource
{
    public class TwitchVideoSource : IVideoSource
    {
        private readonly TwitchApi twitchApi;

        public TwitchVideoSource()
        {
            twitchApi = new TwitchApi();
        }

        public IPlayerSettings GetPlayerSettings()
        {
            return new TwitchPlayerSettings();
        }

        public async Task<Uri> GetUriAsync(string link)
        {
            var userName = link.Replace(TwitchApi.Url, "");
            var media = await TwitchApi.GetM3U8WithQuality(userName);
            return new Uri(media.First().Value);
        }

        public async IAsyncEnumerable<MediaObject> SearchMediaAsync(string query)
        {
            foreach (var media in await twitchApi.SearchUsersByName(query))
            {
                yield return !(media.StreamInfo is null)
                    ? new MediaObject(
                        $"{TwitchApi.Url}{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция идёт",
                        $"Трансляция идёт\n\nИгра: {media.StreamInfo.GameName}.\n\nКоличество зрителей: {media.StreamInfo.ViewersCount}\n\nОписание: {media.StreamInfo.Title}",
                        media.Name, TimeSpan.Zero, media.StreamInfo.PreviewImageUrl, media.ProfileImageUrl)
                    : new MediaObject(
                        $"{TwitchApi.Url}{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция не идёт",
                        author: media.Name, titleThumbnails: media.ProfileImageUrl, duration: TimeSpan.MinValue,
                        description: media.UserDescription);
            }
        }

        public async IAsyncEnumerable<MediaObject> SearchTrendsAsync()
        {
            foreach (var video in await TwitchApi.GetTopStreams())
            {
                yield return new MediaObject($"{TwitchApi.Url}{video.Broadcaster}",
                    $"Игра: {video.GameName}. Количество зрителей: {video.ViewersCount}. Трансляция идёт",
                    $"Трансляция идёт\n\nИгра: {video.GameName}.\n\nКоличество зрителей: {video.ViewersCount}\n\nОписание: {video.Title}",
                    video.Broadcaster, TimeSpan.Zero, video.PreviewImageUrl, video.PreviewImageUrl);
            }
        }
    }
}