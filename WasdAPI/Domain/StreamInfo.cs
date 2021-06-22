namespace WasdAPI.Domain
{
    public class StreamInfo
    {
        public string GameName { get; }
        public string Broadcaster { get; }
        public string M3U8Url { get; }
        public string PreviewImageUrl { get; }
        public int ViewersCount { get; }
        public string Title { get; }
        
        public StreamInfo(string broadcaster, string title, string previewImageUrl, string gameName, int viewersCount, string m3U8Url)
        {
            Title = title;
            PreviewImageUrl = previewImageUrl;
            GameName = gameName;
            ViewersCount = viewersCount;
            M3U8Url = m3U8Url;
            Broadcaster = broadcaster;
        }
    }
}