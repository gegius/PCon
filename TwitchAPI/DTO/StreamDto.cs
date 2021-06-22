namespace TwitchAPI.DTO
{
    public class StreamDto
    {
        public string Broadcaster { get; }
        public string Title { get; }
        public string PreviewImageUrl { get; }
        public string GameName { get; }
        public int ViewersCount { get; }
        
        
        public StreamDto(string broadcaster, string title, string previewImageUrl, string gameName, int viewersCount)
        {
            Title = title;
            PreviewImageUrl = previewImageUrl;
            GameName = gameName;
            ViewersCount = viewersCount;
            Broadcaster = broadcaster;
        }
        
        public StreamDto((string broadcaster, string title, string previewImageUrl, string gameName, int viewersCount) args)
        {
            Broadcaster = args.broadcaster;
            Title = args.title;
            PreviewImageUrl = args.previewImageUrl;
            GameName = args.gameName;
            ViewersCount = args.viewersCount;
        }
      
    }
}