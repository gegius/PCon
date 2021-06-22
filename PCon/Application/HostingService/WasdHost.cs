using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCon.Domain.Player;
using WasdAPI;

namespace PCon.Application.HostingService
{
    // ReSharper disable once IdentifierTypo
    public class WasdHost : IHosting
    {
        public IPlayerSettings GetPlayerSettings()
        {
            return new WasdPlayer();
        }

        public async Task<Uri> GetUri(string link)
        {
            var userName = link.Replace("https://wasd.tv/", "");
            var media = await WasdApi.GetM3U8WithQuality(await WasdApi.GetIdByName(userName));
            return new Uri(media.First().Value);
        }

        public async IAsyncEnumerable<MediaObject> SearchMedia(string query)
        {
            foreach (var media in await WasdApi.SearchUsersByName(query))
            {
                if (media.IsLive)
                    yield return new MediaObject(
                        $"https://wasd.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция идёт",
                        $"Трансляция идёт\n\nОписание: {media.UserDescription}.",
                        media.Name, TimeSpan.Zero, media.ProfileImageUrl, media.ProfileImageUrl);
                else
                    yield return new MediaObject(
                        $"https://wasd.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция не идёт",
                        author: media.Name, titleThumbnails: media.ProfileImageUrl, duration: TimeSpan.MinValue, description: media.UserDescription);
            }
        }

        public async IAsyncEnumerable<MediaObject> SearchTrends()
        {
            foreach (var video in await WasdApi.GetTopStreams())
            {
                yield return new MediaObject($"https://wasd.tv/{video.Broadcaster}",
                    $"Игра: {video.GameName}. Количество зрителей: {video.ViewersCount}. Трансляция идёт",
                    $"Трансляция идёт\n\nИгра: {video.GameName}.\n\nКоличество зрителей: {video.ViewersCount}\n\nОписание: {video.Title}",
                    video.Broadcaster, TimeSpan.Zero, video.PreviewImageUrl, video.PreviewImageUrl);
            }
        }
    }
}