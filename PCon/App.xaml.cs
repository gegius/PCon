using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PCon.Services.HostingService;
using PCon.View;

namespace PCon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
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
}