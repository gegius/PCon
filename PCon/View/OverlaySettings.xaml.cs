using System;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using PCon.Application.VideoSource;
using PCon.Infrastructure;
using PCon.Infrastructure.Extensions;
using PCon.Infrastructure.PCon.Infrastructure;
using Vlc.DotNet.Wpf;

namespace PCon.View
{
    public partial class OverlaySettings
    {
        public DesktopSettings DesktopSettings { get; set; }
        private Overlay _overlay;
        private readonly string _mainProcess;
        private MediaObject _currentMediaObject;
        private VlcControl _vlcControlElement;
        private readonly ServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private VideoSourceFactory.VideoSourceName videoSourceName = VideoSourceFactory.VideoSourceName.Twitch;

        public OverlaySettings(string mainProcess, ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _serviceProvider = serviceCollection.BuildServiceProvider();
            InitializeComponent();
            InitVlcPlayer();
            this._mainProcess = mainProcess;
        }

        private void Button_Click_Show(object sender, RoutedEventArgs e)
        {
            _overlay?.VlcControlPanel.Children.Clear();
            _overlay = new Overlay(_currentMediaObject, videoSourceName, _mainProcess, _vlcControlElement, _serviceCollection);
            DesktopSettings.Overlay = _overlay;
            DesktopSettings.StopOverlaySettingsAttach();
            Visibility = Visibility.Hidden;
        }

        private void Youtube_OnClick(object sender, RoutedEventArgs e)
        {
            videoSourceName = VideoSourceFactory.VideoSourceName.Youtube;
            HostingInit(sender);
        }

        private void Twitch_OnClick(object sender, RoutedEventArgs e)
        {
            videoSourceName = VideoSourceFactory.VideoSourceName.Twitch;
            HostingInit(sender);
        }

        private void Wasd_OnClick(object sender, RoutedEventArgs e)
        {
            videoSourceName = VideoSourceFactory.VideoSourceName.Wasd;
            HostingInit(sender);
        }

        private void Local_OnClick(object sender, RoutedEventArgs e)
        {
            videoSourceName = VideoSourceFactory.VideoSourceName.Local;
            HostingInit(sender);
        }

        private void HostingInit(object sender)
        {
            ChangeColor(sender);
            ClearSearchField();
            SetInitialState();
        }

        private void SetInitialState()
        {
            SetDefaultSettings();
            ClearResult();
            ClearBox();
            CancelSearch();
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
            SearchField.Text = string.Empty;
        }

        private void CancelSearch()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void ChangeColor(object sender)
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
            var anotherResultButton = new Button
            {
                Content = panel, Background = Brushes.White, Margin = new Thickness(2),
                Style = FindResource("RoundCorner") as Style
            };
            anotherResultButton.Click += Button_Click_Result;
            return anotherResultButton;
        }

        private async void FindMedia_OnClick(object sender, RoutedEventArgs e)
        {
            SetInitialState();
            ResultBox.Visibility = Visibility.Visible;
            var text = SearchField.Text;
            var cancelToken = _cancellationTokenSource.Token;
            try
            {
                var factory = _serviceProvider.GetService<VideoSourceFactory>();
                var videoSource = factory.GetVideoSource(videoSourceName);
                await foreach (var video in videoSource.SearchMediaAsync(text)
                    .WithCancellation(cancelToken))
                {
                    if (cancelToken.IsCancellationRequested)
                        break;
                    
                    var anotherResultButton = CreateMediaButton(video);
                    ResultPanel.Children.Add(anotherResultButton);
                }
            }
            catch (Exception exe)
            {
                Console.WriteLine(exe);
                ErrorHandler.ThrowErrorConnection();
            }
        }


        private async void FindTrends_OnClick(object sender, RoutedEventArgs e)
        {
            SetInitialState();
            ClearSearchField();
            ResultBox.Visibility = Visibility.Visible;
            var cancelToken = _cancellationTokenSource.Token;
            try
            {
                var factory = _serviceProvider.GetService<VideoSourceFactory>();
                var videoSource = factory.GetVideoSource(videoSourceName);
                await foreach (var video in videoSource.SearchTrendsAsync()
                    .WithCancellation(cancelToken))
                {
                    if (cancelToken.IsCancellationRequested) break;
                    var anotherResultButton = CreateMediaButton(video);
                    ResultPanel.Children.Add(anotherResultButton);
                }
            }
            catch (Exception es)
            {
                Console.WriteLine(es);
                ErrorHandler.ThrowErrorConnection();
            }
        }

        private StackPanel CreatePanel()
        {
            var panel = new StackPanel {Orientation = Orientation.Vertical, Margin = new Thickness(10)};

            var label = new TextBlock
            {
                Text = _currentMediaObject.Author + "\n\n" + _currentMediaObject.Description,
                TextWrapping = TextWrapping.Wrap
            };

            var img = new Image();
            if (_currentMediaObject.DescriptionThumbnails != null)
                img = new Image {Source = new BitmapImage(new Uri(_currentMediaObject.DescriptionThumbnails))};

            panel.Children.Add(img);
            panel.Children.Add(label);

            return panel;
        }

        private Button CreateButtonStart()
        {
            var buttonStart = new Button
            {
                Width = 300, Height = 60, Content = "Start", FontSize = 30, FontStyle = FontStyles.Oblique,
                Margin = new Thickness(10), Style = FindResource("RoundCorner") as Style
            };

            if (_currentMediaObject.Duration >= TimeSpan.Zero)
                buttonStart.Click += Button_Click_Show;

            return buttonStart;
        }

        private void Button_Click_Result(object sender, RoutedEventArgs e)
        {
            ClearBox();
            _currentMediaObject =
                (MediaObject) ((GroupBox) ((StackPanel) ((Button) sender).Content).Children[2]).Content;
            var panel = CreatePanel();

            var scrollBoxSelected = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = panel
            };

            var resultBox = new GroupBox
            {
                Name = "ResultBox",
                Width = 300,
                Height = 650,
                Visibility = Visibility.Visible,
                BorderBrush = Brushes.Black,
                Margin = new Thickness(10),
                Content = scrollBoxSelected
            };

            var mainPanel = new StackPanel
                {VerticalAlignment = VerticalAlignment.Top, Orientation = Orientation.Vertical};

            var buttonStart = CreateButtonStart();

            mainPanel.Children.Add(resultBox);
            mainPanel.Children.Add(buttonStart);
            BoxPanel.Children.Add(mainPanel);
        }

        private void InitVlcPlayer()
        {
            try
            {
                var libDirectory = LibVlcHelper.GetVlcPlayerPath();
                _vlcControlElement = new VlcControl();
                _vlcControlElement.SourceProvider.CreatePlayer(libDirectory);
            }
            catch
            {
                ErrorHandler.ThrowErrorNoVlcPlayer();
            }
        }

        private void StartSearchCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (ResultPanel.Visibility == Visibility.Visible) FindMedia_OnClick(sender, e);
        }
    }
}