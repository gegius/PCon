using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PCon.Domain;
using PCon.Services.HostingService;
using Vlc.DotNet.Wpf;

namespace PCon.View
{
    public partial class OverlaySettings : Window
    {
        private Overlay overlay;
        private readonly string mainProcess;
        public DesktopSettings DesktopSettings { get; set; }
        private MediaObject currentMediaObject;
        private VlcControl vlcControlElement;
        private ServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;

        public OverlaySettings(string mainProcess, ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceProvider = serviceCollection.BuildServiceProvider();
            InitializeComponent();
            InitVlcPlayer();
            this.mainProcess = mainProcess;
        }

        private void Button_Click_Show(object sender, RoutedEventArgs e)
        {
            overlay?.VlcControlPanel.Children.Clear();
            overlay = new Overlay(currentMediaObject, mainProcess, vlcControlElement, _serviceCollection);
            DesktopSettings.Overlay = overlay;
            DesktopSettings.StopOverlaySettingsAttach();
            Visibility = Visibility.Hidden;
        }

        private void Youtube_OnClick(object sender, RoutedEventArgs e)
        {
            _serviceCollection.Replace<IHosting>(_ => new YouTubeHost(), ServiceLifetime.Singleton);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            SearchPanel.Visibility = Visibility.Visible;    
            ChangeColor(sender);
        }


        
        private void Twitch_OnClick(object sender, RoutedEventArgs e)
        {
            _serviceCollection.Replace<IHosting>(_ => new TwitchHost(), ServiceLifetime.Singleton);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            SearchPanel.Visibility = Visibility.Visible;    
            ChangeColor(sender);
        }
        
        // ReSharper disable once IdentifierTypo
        private void Wasd_OnClick(object sender, RoutedEventArgs e)
        {
            _serviceCollection.Replace<IHosting>(_ => new WasdHost(), ServiceLifetime.Singleton);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            SearchPanel.Visibility = Visibility.Visible;    
            ChangeColor(sender);
        }

        private void ChangeColor(object sender)
        {
            foreach (var child in Hostings.Children)
            {
                var button = (Button) child;
                button.Background = button == (Button) sender ? Brushes.Red : Brushes.White;
            }
        }

        private async void Find_Media_OnClick(object sender, RoutedEventArgs e)
        {
            var text = SearchField.Text;
            ResultPanel.Children.Clear();
            if (BoxPanel.Children.Count > 1)
                BoxPanel.Children.RemoveAt(1);
            await foreach (var video in _serviceProvider.GetService<IHosting>().SearchMedia(text))
            {
                var anotherResultButton = new Button
                {
                    Content = video, ContentStringFormat = null, Visibility = Visibility.Visible, Width = 400,
                    Height = 30
                };
                anotherResultButton.Click += Button_Click_Result;
                ResultPanel.Children.Add(anotherResultButton);
            }
        }
        
        private async void Find_Trends_OnClick(object sender, RoutedEventArgs e)
        {
            ResultPanel.Children.Clear();
            if (BoxPanel.Children.Count > 1)
                BoxPanel.Children.RemoveAt(1);
            await foreach (var video in _serviceProvider.GetService<IHosting>().SearchTrends())
            {
                var anotherResultButton = new Button
                {
                    Content = video, ContentStringFormat = null, Visibility = Visibility.Visible, Width = 400,
                    Height = 30
                };
                anotherResultButton.Click += Button_Click_Result;
                ResultPanel.Children.Add(anotherResultButton);
            }
        }

        private void Button_Click_Result(object sender, RoutedEventArgs e)
        {
            var mainPanel = new StackPanel
                {VerticalAlignment = VerticalAlignment.Top, Orientation = Orientation.Vertical};
            if (BoxPanel.Children.Count > 1)
                BoxPanel.Children.RemoveAt(1);
            var video = (MediaObject) ((Button) sender).Content;
            currentMediaObject = video;
            var resultBox = new GroupBox
            {
                Name = "ResultBox", Width = 175, Height = 300, Visibility = Visibility.Visible,
                BorderBrush = Brushes.Black, Margin = new Thickness(10)
            };
            var label = new Label
            {
                Content = video.Author + "\n" + video.Duration + "\n" + video.Title + "\n" +
                          video.Url
            };

            resultBox.Content = label;
            mainPanel.Children.Add(resultBox);
            var buttonStart = new Button {Width = 175, Content = "Start", Margin = new Thickness(10)};
            if (video.Duration >= TimeSpan.Zero) buttonStart.Click += Button_Click_Show;
            mainPanel.Children.Add(buttonStart);
            BoxPanel.Children.Add(mainPanel);
        }

        private void InitVlcPlayer()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            if (currentAssembly == null) return;
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (!Directory.Exists(Path.Combine(currentDirectory, "libvlc")))
                ZipFile.ExtractToDirectory(Path.Combine(currentDirectory, "libvlc.zip"), currentDirectory);
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc",
                Environment.Is64BitOperatingSystem ? "win-x64" : "win-x86"));
            Console.WriteLine(currentDirectory);
            vlcControlElement = new VlcControl();
            vlcControlElement.SourceProvider.CreatePlayer(libDirectory);
        }

        private void StartSearchCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (ResultPanel.Visibility == Visibility.Visible)
            {
                Find_Media_OnClick(sender, e);
            }
        }
    }
}