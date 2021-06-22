using System;
using System.IO;

namespace TwitchAPI
{
    public static class Requests
    {
        private static readonly string replacedString = "##REPLACETHIS##";

        public static string GetPlaybackAccessToken_TemplateRequest(string input)
        {
            const string fileName = "PlaybackAccessToken_TemplateRequest.json";
            if (!TryGetRequest(fileName, out var result))
                throw new Exception($"File {fileName} not exists.");

            return result.Replace(replacedString, input);
        }

        public static string GetStreamsRequest()
        {
            const string fileName = "StreamsRequest.json";

            if (!TryGetRequest(fileName, out var result))
                throw new Exception($"File {fileName} not exists.");

            return result;
        }

        public static string GetSearchResultsPage_SearchResultsRequest(string input)
        {
            const string fileName = "SearchResultsPage_SearchResultsRequest.json";

            if (!TryGetRequest(fileName, out var result))
                throw new Exception($"File {fileName} not exists.");

            return result.Replace(replacedString, input);
        }

        private static bool TryGetRequest(string fileName, out string result)
        {
            result = default;

            var relativePath = GetRelativePath(fileName);
            if (!File.Exists(relativePath))
                return false;

            result = File.ReadAllText(relativePath);
            return true;
        }

        public static string GetVideoPlayerStatusOverlayChannelRequest(string input)
        {
            const string fileName = "VideoPlayerStatusOverlayChannelRequest.json";
            if (!TryGetRequest(fileName, out var result))
                throw new Exception($"File {fileName} not exists.");

            return result.Replace(replacedString, input);
        }

        private static string GetRelativePath(string fileName)
        {
            return $"./Requests/{fileName}";
        }
    }
}