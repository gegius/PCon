﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
 using System.Threading.Tasks;

 namespace YoutubeAPI
{
    public class YoutubeApi
    {
        public static string Url { get; } = "https://www.youtube.com/";

        public Task<IEnumerable<string>> GetTrendsVideosAsync()
        {
            var url = $"{Url}/feed/trending";
            return GetVideosFromPageAsync(url);
        }

        public Task<IEnumerable<string>> SearchByQueryAsync(string query)
        {
            return GetVideosFromPageAsync($"{Url}/results?search_query={query}&sp=EgIQAQ%253D%253D");
        }

        private async Task<IEnumerable<string>> GetVideosFromPageAsync(string url)
        {
            using var client = new WebClient();
            var htmlCode = client.DownloadStringTaskAsync(url);
            var regex = new Regex(@"{""url"":""(/watch\?v=\w+)""");
            return regex.Matches(await htmlCode).Select(x => x.Groups[1].ToString());

        }
    }
}