using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using PCon.Application.HostingService;
using PCon.Infrastructure.Extensions;
using Vlc.DotNet.Wpf;

namespace PCon.View
{
    public partial class OverlaySettings
    {
        public DesktopSettings DesktopSettings { get; set; }
        private Overlay overlay;
        private readonly string mainProcess;
        private MediaObject currentMediaObject;
        private VlcControl vlcControlElement;
        private readonly ServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
            SetDefaultSettings();
            CancelSearch();
            ClearEverything();
            ChangeColor(sender);
        }

        private void Twitch_OnClick(object sender, RoutedEventArgs e)
        {
            _serviceCollection.Replace<IHosting>(_ => new TwitchHost(), ServiceLifetime.Singleton);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            SetDefaultSettings();
            CancelSearch();
            ClearEverything();
            ChangeColor(sender);
        }

        private void Wasd_OnClick(object sender, RoutedEventArgs e)
        {
            _serviceCollection.Replace<IHosting>(_ => new WasdHost(), ServiceLifetime.Singleton);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            SetDefaultSettings();
            CancelSearch();
            ClearEverything();
            ChangeColor(sender);
        }

        private void ClearEverything()
        {
            ClearResult();
            ClearSearchField();
            ClearBox();
        }

        private void SetDefaultSettings()
        {
            Box.Visibility = Visibility.Visible;
            ResultBox.Visibility = Visibility.Hidden;
            ResultScrollViewer.ScrollToTop();
        }
        
        private void ClearResult()
        {
            ResultPanel.Children.Clear();
        }

        private void ClearBox()
        {
            BoxPanel.Children.Clear();
        }

        private void ClearSearchField()
        {
            SearchField.Text = "";
        }

        private void CancelSearch()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void ChangeColor(object sender) //Вынести
        {
            foreach (var child in Hostings.Children)
            {
                var button = (Button) child;
                button.Background = button == (Button) sender ? Brushes.Red : Brushes.White;
            }
        }

        private Button CreateMediaButton(MediaObject video)
        {
            var img = new Image {Source = new BitmapImage(new Uri(video.TitleThumbnails))};
            var panel = new StackPanel {Orientation = Orientation.Vertical, Margin = new Thickness(10), Width = 300};
            var textTitle = new TextBlock {Text = video.Title, Width = 300, TextWrapping = TextWrapping.Wrap};
            var boxHidden = new GroupBox {Content = video, Visibility = Visibility.Hidden, Width = 300, Height = 30};
            panel.Children.Add(img);
            panel.Children.Add(textTitle);
            panel.Children.Add(boxHidden);
            var anotherResultButton = new Button {Content = panel, Background = Brushes.White, Margin = new Thickness(2), Style = FindResource("RoundCorner") as Style};
            anotherResultButton.Click += Button_Click_Result;
            return anotherResultButton;
        }

        private async void Find_Media_OnClick(object sender, RoutedEventArgs e)
        {
            SetDefaultSettings(); //Вынести в метод
            CancelSearch();
            ClearResult();
            ClearBox();
            ResultBox.Visibility = Visibility.Visible;
            var text = SearchField.Text;
            var cancelToken = cancellationTokenSource.Token;
            try
            {
                await foreach (var video in _serviceProvider.GetService<IHosting>().SearchMediaAsync(text)
                    .WithCancellation(cancelToken))
                {
                    if (cancelToken.IsCancellationRequested) break;
                    var anotherResultButton = CreateMediaButton(video);
                    ResultPanel.Children.Add(anotherResultButton);
                }
            }
            catch
            {
                ErrorHandler.ThrowErrorConnection();
            }
        }

        private async void Find_Trends_OnClick(object sender, RoutedEventArgs e)
        {
            SetDefaultSettings();
            CancelSearch();
            ClearResult();
            ClearSearchField();
            ClearBox();
            ResultBox.Visibility = Visibility.Visible;
            var cancelToken = cancellationTokenSource.Token;
            try
            {
                await foreach (var video in _serviceProvider.GetService<IHosting>().SearchTrendsAsync()
                    .WithCancellation(cancelToken))
                {
                    if (cancelToken.IsCancellationRequested) break;
                    var anotherResultButton = CreateMediaButton(video);
                    ResultPanel.Children.Add(anotherResultButton);
                }
            }
            catch(Exception es)
            {
                Console.WriteLine(es);
                ErrorHandler.ThrowErrorConnection();
            }
        }

        private void Button_Click_Result(object sender, RoutedEventArgs e) //Спасти
        {
            ClearBox();
            var mainPanel = new StackPanel
                {VerticalAlignment = VerticalAlignment.Top, Orientation = Orientation.Vertical};
            var video = (MediaObject) ((GroupBox) ((StackPanel) ((Button) sender).Content).Children[2]).Content;
            currentMediaObject = video;
            var resultBox = new GroupBox
            {
                Name = "ResultBox", Width = 300, Height = 650, Visibility = Visibility.Visible,
                BorderBrush = Brushes.Black, Margin = new Thickness(10)
            };
            var img = new Image();
            if (video.DescriptionThumbnails != null)
                img = new Image {Source = new BitmapImage(new Uri(video.DescriptionThumbnails))};

            var panel = new StackPanel {Orientation = Orientation.Vertical, Margin = new Thickness(10)};
            var label = new TextBlock
            {
                Text = video.Author + "\n\n" + video.Description, TextWrapping = TextWrapping.Wrap
            };

            panel.Children.Add(img);
            panel.Children.Add(label);
            var scrollBoxSelected = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = panel
            };
            resultBox.Content = scrollBoxSelected;
            mainPanel.Children.Add(resultBox);
            var buttonStart = new Button {Width = 300, Height = 60, Content = "Start", FontSize = 30, FontStyle = FontStyles.Oblique, Margin = new Thickness(10), Style = FindResource("RoundCorner") as Style};
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
            if (ResultPanel.Visibility == Visibility.Visible) Find_Media_OnClick(sender, e);
        }
    }
}