using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PCon.Services;
using PCon.Services.HostingService;
using PCon.View;

namespace PCon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            base.OnStartup(e);
            var window = new DesktopSettings(serviceCollection);
            window.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHosting>(new YouTubeHost());
        }
    }
    
    public static class Extensions{
        public static IServiceCollection Replace<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime lifetime)
            where TService : class
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
    
            services.Remove(descriptorToRemove);
    
            var descriptorToAdd = new ServiceDescriptor(typeof(TService), implementationFactory, lifetime);
    
            services.Add(descriptorToAdd);
    
            return services;
        }
    }
}