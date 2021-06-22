using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCon.Domain.Player;
using TwitchAPI;

namespace PCon.Application.HostingService
{
    public class TwitchHost : IHosting
    {
        private readonly TwitchApi twitchTwitchApi;

        public TwitchHost()
        {
            twitchTwitchApi = new TwitchApi();
        }

        public IPlayerSettings GetPlayerSettings()
        {
            return new TwitchPlayer();
        }

        public async Task<Uri> GetUri(string link)
        {
            var userName = link.Replace("https://www.twitch.tv/", "");
            var media = await TwitchApi.GetM3U8WithQuality(userName);
            return new Uri(media.First().Value);
        }

        public async IAsyncEnumerable<MediaObject> SearchMedia(string request)
        {
            foreach (var media in await twitchTwitchApi.SearchUsersByName(request))
            {
                yield return !(media.StreamInfo is null)
                    ? new MediaObject(
                        $"https://www.twitch.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция идёт",
                        $"Игра: {media.StreamInfo.GameName}.\nКоличество зрителей: {media.StreamInfo.ViewersCount}\nОписание: {media.StreamInfo.Title}",
                        media.Name, TimeSpan.Zero, media.StreamInfo.PreviewImageUrl, media.ProfileImageUrl)
                    : new MediaObject(
                        $"https://www.twitch.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция не идёт",
                        author: media.Name, titleThumbnails: media.ProfileImageUrl, duration: TimeSpan.MinValue);
            }
        }

        public async IAsyncEnumerable<MediaObject> SearchTrends()
        {
            foreach (var video in await TwitchApi.GetTopStreams())
            {
                yield return new MediaObject($"https://www.twitch.tv/{video.Broadcaster}",
                    $"Игра: {video.GameName}. Количество зрителей: {video.ViewersCount}. Трансляция идёт",
                    $"Игра: {video.GameName}.\nКоличество зрителей: {video.ViewersCount}\nОписание: {video.Title}",
                    video.Broadcaster, TimeSpan.Zero, video.PreviewImageUrl, video.PreviewImageUrl);
            }
        }
    }
}