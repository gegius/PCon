using System;
using Microsoft.Extensions.DependencyInjection;

namespace PCon.Application.VideoSource
{
    public class VideoSourceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public VideoSourceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IVideoSource GetVideoSource(VideoSourceName name)
        {
            return name switch
            {
                VideoSourceName.Youtube => _serviceProvider.GetService<YoutubeVideoSource>(),
                VideoSourceName.Twitch => _serviceProvider.GetService<TwitchVideoSource>(),
                VideoSourceName.Wasd => _serviceProvider.GetService<WasdVideoSource>(),
                _ => throw new NotSupportedException("Unsupported video source")
            };
        }
        
        public enum VideoSourceName
        {
            Youtube,
            Twitch,
            Wasd
        }
    }
}