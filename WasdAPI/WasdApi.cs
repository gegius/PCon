using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WasdAPI.DTO;

namespace WasdAPI
{
    public static class WasdApi
    {
        public static string Url { get; } = "https://wasd.tv";

        public static async Task<IEnumerable<UserInfo>> SearchUsersByName(string username)
        {
            if (username == string.Empty) return Enumerable.Empty<UserInfo>();
            var uri = $"{Url}/api/search/channels?limit=15&offset=0&search_phrase={username}";
            var responseContent = await SendRequestAsync(uri);
            return JsonParser.ParseUsers(responseContent);
        }

        public static async Task<string> GetIdByName(string username)
        {
            var uri = $"{Url}/api/v2/broadcasts/public?channel_name={username.ToLower()}";
            var responseContent = await SendRequestAsync(uri);
            return JsonParser.GetUserIdFromChannelInfo(responseContent);
        }

        public static async Task<bool> UserIsOnline(string username)
        {
            var uri = $"{Url}/api/v2/broadcasts/public?channel_name={username.ToLower()}";
            var responseContent = await SendRequestAsync(uri);
            return JsonParser.GetUserIsOnlineFromChannelInfo(responseContent);
        }

        public static async Task<bool> UserAvailable(string username)
        {
            var uri = $"{Url}/api/v2/broadcasts/public?channel_name={username.ToLower()}";
            try
            {
                await SendRequestAsync(uri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<IEnumerable<StreamInfo>> GetTopStreams()
        {
            var uri =
                $"{Url}/api/v2/media-containers?limit=15&offset=0&media_container_status=RUNNING&media_container_type=SINGLE&order_type=VIEWERS&order_direction=DESC";

            var responseContent = await SendRequestAsync(uri);

            return JsonParser.ParseTopStreams(responseContent);
        }

        public static async Task<Dictionary<string, string>> GetM3U8WithQuality(string userId)
        {
            var m3U8Url = GetM3U8Url(userId);
            using var client = new WebClient();
            var m3U8FileContent = await client.DownloadStringTaskAsync(m3U8Url);
            return ParseM3U8File(m3U8FileContent);
        }

        private static Dictionary<string, string> ParseM3U8File(string m3U8FileContent)
        {
            var m3U8Regex = new Regex(@"https[\w\W]*?m3u8", RegexOptions.Compiled);
            var resolutionRegex = new Regex(@"RESOLUTION=(\d+x\d+)", RegexOptions.Compiled);
            var m3U8Links = m3U8Regex.Matches(m3U8FileContent).Select(x => x.Value);
            var qualities = resolutionRegex.Matches(m3U8FileContent).Select(x => x.Groups[1].Value);
            return m3U8Links.Zip(qualities, (link, quality) => new {link, quality})
                .ToDictionary(obj => obj.quality, obj => obj.link);
        }

        private static string GetM3U8Url(string userId)
        {
            return $"https://cdn.wasd.tv/live/{userId}/index.m3u8";
        }

        private static async Task<string> SendRequestAsync(string uri)
        {
            var request = WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Referer", $"{Url}");
            using var response = await request.GetResponseAsync();
            await using var responseStream = response.GetResponseStream();
            if (responseStream is null)
                throw new ArgumentException("Response stream was null.");
            using var reader = new StreamReader(responseStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}