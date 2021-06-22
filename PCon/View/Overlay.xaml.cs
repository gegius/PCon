using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using PCon.Application.HostingService;
using PCon.Infrastructure;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Wpf;

namespace PCon.View
{
    public partial class Overlay
    {
        private readonly string mainProcess;
        private WindowSnapper snapper;
        private readonly VlcMediaPlayer mainPlayer;
        private double duration;
        private Uri mediaUri;
        private DispatcherTimer timerVideoTime;
        private ProcessChecker processChecker;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ServiceProvider _serviceProvider;

        public Overlay(MediaObject video, string mainProcess, VlcControl vlcControl,
            IServiceCollection serviceCollection)
        {
            _serviceProvider = serviceCollection.BuildServiceProvider();
            InitializeComponent();
            this.mainProcess = mainProcess;
            VlcControlPanel.Children.Add(vlcControl);
            mainPlayer = vlcControl.SourceProvider.MediaPlayer;
            InitAll(video);
        }

        private async void CheckMainProcess()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await processChecker.WaitHideAsync("Overlay", cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                Visibility = Visibility.Hidden;
                await processChecker.WaitShowAsync(cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                Visibility = Visibility.Visible;
            }
        }

        private async void InitAll(MediaObject video)
        {
            InitTimer();
            InitSnapper();
            await InitOverlaySettings(video);
            processChecker = new ProcessChecker(mainProcess);
            CheckMainProcess();
            Show();
        }

        public void StopOverlayAttach()
        {
            Button_Pause(null, null);
            cancellationTokenSource.Cancel();
            Visibility = Visibility.Hidden;
        }

        public void StartOverlayAttach()
        {
            Button_Play(null, null);
            cancellationTokenSource = new CancellationTokenSource();
            Visibility = Visibility.Visible;
            CheckMainProcess();
        }

        private async Task InitOverlaySettings(MediaObject video)
        {
            var settings = _serviceProvider.GetService<IHosting>().GetPlayerSettings();
            VideoSlider.Visibility = settings.GetSliderVisibility();
            if (video.Duration != null && video.Duration != TimeSpan.Zero)
            {
                var totalSeconds = video.Duration.Value.TotalSeconds;
                VideoSlider.Maximum = totalSeconds;
                duration = totalSeconds;
            }
            await SetMedia(video);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider) sender).SelectionEnd = e.NewValue;
            if (mainPlayer.State == MediaStates.Ended || mainPlayer.State == MediaStates.NothingSpecial)
            {
                mainPlayer.SetMedia(mediaUri);
                mainPlayer.Play();
            }

            Console.WriteLine(Math.Abs(e.NewValue - e.OldValue));
            if (Math.Abs(e.NewValue - e.OldValue) >= 0.520)
                mainPlayer.Position = (float) e.NewValue / (float) duration;
        }


        private void InitSnapper()
        {
            snapper = new WindowSnapper(this, mainProcess);
            snapper.AttachAsync();
        }

        private void Overlay_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ComplexPanel.Visibility = Visibility.Visible;
        }

        private void Overlay_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ComplexPanel.Visibility = Visibility.Hidden;
        }

        private void Button_Pause(object sender, RoutedEventArgs e)
        {
            if (!mainPlayer.IsPlaying()) return;
            mainPlayer.Pause();
            timerVideoTime.Stop();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (mainPlayer.IsPlaying()) return;
            mainPlayer.Play();
            timerVideoTime.Start();
        }

        private async Task SetMedia(MediaObject video)
        {
            mediaUri = await _serviceProvider.GetService<IHosting>().GetUri(video.Url);
            mainPlayer.SetMedia(mediaUri);
            VolumeSlider.Value = 100;
            Button_Play(null, null);
        }

        private void InitTimer()
        {
            timerVideoTime = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};
            timerVideoTime.Tick += timer_Tick;
            timerVideoTime.Tick += show_Time_Video;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            VideoSlider.Value = mainPlayer.Position * duration;
        }

        private void show_Time_Video(object sender, EventArgs e)
        {
            TimeShow.Content = TimeSpan.FromSeconds(mainPlayer.Position * duration).ToString("hh\\:mm\\:ss");
        }

        private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider) sender).SelectionEnd = e.NewValue;
            mainPlayer.Audio.Volume = (int) e.NewValue;
        }

        private void PlayPlayerCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Button_Play(null, null);
        }

        private void PausePlayerCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Button_Pause(null, null);
        }

        private void HideOverlayCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                StopOverlayAttach();
            }
        }

        private void ShowOverlayCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Visibility == Visibility.Hidden)
            {
                StartOverlayAttach();
            }
        }
    }
}