using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PCon.Application.VideoSource;
using PCon.DI;
using PCon.View;

namespace PCon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /*protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            base.OnStartup(e);
            var window = new DesktopSettings(serviceCollection);
            window.Show();
        }
        */

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddVideoSource()
                .AddWindows();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var window = new DesktopSettings(serviceCollection);
            window.Show();
        }
    }
}