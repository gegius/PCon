using Microsoft.Extensions.DependencyInjection;
using PCon.Application.VideoSource;
using PCon.Domain.Player;
using PCon.View;

namespace PCon.DI
{
    public static class ServiceCollectionExtensions
        {
            public static IServiceCollection AddVideoSource(this IServiceCollection serviceCollection)
            {
                return serviceCollection
                    .AddSingleton<VideoSourceFactory>()
                    .AddSingleton<YoutubeVideoSource>()
                    .AddSingleton<TwitchVideoSource>()
                    .AddSingleton<WasdVideoSource>();
            }

            public static IServiceCollection AddWindows(this IServiceCollection serviceCollection)
            {
                return serviceCollection
                    .AddSingleton<DesktopSettings>()
                    .AddSingleton<Overlay>()
                    .AddSingleton<OverlaySettings>();
            }

            public static IServiceCollection AddVideoSourceSettings(this IServiceCollection serviceCollection)
            {
                return serviceCollection
                    .AddSingleton<TwitchPlayerSettings>()
                    .AddSingleton<YouTubePlayerSettings>()
                    .AddSingleton<WasdPlayerSettings>();
            }
        }
    
}