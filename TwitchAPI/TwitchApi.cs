using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchAPI.DTO;

namespace TwitchAPI
{
    public class TwitchApi
    {
        public static async Task<Dictionary<string, string>> GetM3U8WithQuality(string userLogin)
        {
            if (!await UserIsOnline(userLogin))
                return new Dictionary<string, string>();
            var (token, signature) = await GetTokenAndSignatureAsync(userLogin);
            var m3U8Url = GetM3U8Url(token, signature, userLogin);
            using var client = new WebClient();
            var m3U8FileContent = client.DownloadString(m3U8Url);
            return ParseM3U8File(m3U8FileContent);
        }

        private static Dictionary<string, string> ParseM3U8File(string m3U8FileContent)
        {
            var m3U8Regex = new Regex(@"https[\w\W]*?m3u8", RegexOptions.Compiled);
            var idRegex = new Regex("GROUP-ID=\"(.*?)\"", RegexOptions.Compiled);
            var m3U8Links = m3U8Regex.Matches(m3U8FileContent).Select(x => x.Value);
            var qualities = idRegex.Matches(m3U8FileContent).Select(x => x.Groups[1].Value);
            return m3U8Links.Zip(qualities, (link, quality) => new {link, quality})
                .ToDictionary(obj => obj.quality, obj => obj.link);
        }

        private static async Task<(string AccessToken, string Signature)> GetTokenAndSignatureAsync(string userLogin)
        {
            var data = Requests.GetPlaybackAccessToken_TemplateRequest(userLogin);

            var response = await SendRequestAsync(data);

            var responseContent = await ReadResponseContentAsync(response);

            return JsonResponseParser.PlaybackAccessToken_TemplateResponseParse(responseContent);
        }

        private static async Task<string> ReadResponseContentAsync(WebResponse response)
        {
            await using var responseStream = response.GetResponseStream();

            if (responseStream is null)
                throw new EndOfStreamException("Response stream was null.");

            var reader = new StreamReader(responseStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        private static string GetM3U8Url(string token, string signature, string userLogin)
        {
            return $"https://usher.ttvnw.net/api/channel/hls/{userLogin}.m3u8?sig={signature}&token={token}";
        }

        public async Task<IEnumerable<UserInfo>> SearchUsersByName(string userLogin)
        {
            var requestData = Requests.GetSearchResultsPage_SearchResultsRequest(userLogin);

            var response = await SendRequestAsync(requestData);

            var responseContent = await ReadResponseContentAsync(response);

            return JsonResponseParser.SearchResultsPage_SearchResultsResponseParse(responseContent);
        }

        private static async Task<bool> UserIsOnline(string userLogin)
        {
            var requestData = Requests.GetVideoPlayerStatusOverlayChannelRequest(userLogin);
            var response = await SendRequestAsync(requestData);
            var responseContent = await ReadResponseContentAsync(response);
            return JsonResponseParser.VideoPlayerStatusOverlayChannelResponseParseUserIsOnline(responseContent);
        }

        public async Task<bool> UserAvailable(string userLogin)
        {
            var requestData = Requests.GetVideoPlayerStatusOverlayChannelRequest(userLogin);
            var response = await SendRequestAsync(requestData);
            var responseContent = await ReadResponseContentAsync(response);
            return JsonResponseParser.VideoPlayerStatusOverlayChannelResponseParseUserAvailable(responseContent);
        }

        public static async Task<IEnumerable<StreamInfo>> GetTopStreams()
        {
            var requestData = Requests.GetStreamsRequest();
            var response = await SendRequestAsync(requestData);
            var responseContent = await ReadResponseContentAsync(response);
            return JsonResponseParser.SteamsResponseParse(responseContent);
        }

        private static async Task<HttpWebResponse> SendRequestAsync(string data)
        {
            var webRequest = WebRequest.Create("https://gql.twitch.tv/gql");
            var (name, value) = ("client-id", await GetClientId("bratishkinoff"));
            webRequest.Method = "POST";
            webRequest.Headers.Add(name, value);
            await using var stream = await webRequest.GetRequestStreamAsync();
            stream.Write(Encoding.UTF8.GetBytes(data));
            return await webRequest.GetResponseAsync() as HttpWebResponse;
        }

        private static async Task<string> GetClientId(string channelName)
        {
            using var client = new WebClient();
            var htmlCode = await client.DownloadStringTaskAsync($"https://www.twitch.tv/{channelName}");
            var regex = new Regex("\"Client-ID\":\"(.*?)\"", RegexOptions.Compiled);
            var match = regex.Match(htmlCode);
            if (!match.Success)
                throw new ArgumentException("Client-ID not found.");
            return match.Groups[1].Value;
        }
    }
}