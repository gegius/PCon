using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCon.Domain;
using PCon.Domain.Player;
using WasdAPI;

namespace PCon.Services.HostingService
{
    // ReSharper disable once IdentifierTypo
    public class WasdHost : IHosting
    {
        // ReSharper disable once IdentifierTypo
        private readonly WasdApi _wasdApi;
        
        // ReSharper disable once IdentifierTypo
        public WasdHost()
        {
            _wasdApi = new WasdApi();
        }
        public IPlayerSettings GetPlayerSettings()
        {
            return new WasdPlayer();
        }

        public async Task<Uri> GetUri(string link)
        {
            var userName = link.Replace("https://wasd.tv/", "");
            var media = await _wasdApi.GetM3U8WithQuality(await _wasdApi.GetIdByName(userName));
            return new Uri(media.First().Value);
        }

        public async IAsyncEnumerable<MediaObject> SearchMedia(string request)
        {
            foreach (var media in await _wasdApi.SearchUsersByName(request))
            {
                if (media.IsLive)
                    yield return new MediaObject(
                        $"https://wasd.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция идёт",
                        $"Описание: {media.UserDescription}.\nТрансляция идёт",
                        media.Name, TimeSpan.Zero, media.ProfileImageUrl, media.ProfileImageUrl);
                else
                    yield return new MediaObject(
                        $"https://wasd.tv/{media.Name}",
                        $"{media.FollowersCount} подписчиков. Трансляция не идёт",
                        author: media.Name, titleThumbnails: media.ProfileImageUrl, duration: TimeSpan.MinValue);
            }
        }

        public async IAsyncEnumerable<MediaObject> SearchTrends()
        {
            foreach (var video in await _wasdApi.GetTopStreams())
            {
                yield return new MediaObject($"https://www.twitch.tv/{video.Broadcaster}",
                    $"Игра: {video.GameName}. Количество зрителей: {video.ViewersCount}. Трансляция идёт",
                    $"Игра: {video.GameName}.\nКоличество зрителей: {video.ViewersCount}\nОписание: {video.Title}",
                    video.Broadcaster, TimeSpan.Zero, video.PreviewImageUrl, video.PreviewImageUrl);
            }
        }
    }
}