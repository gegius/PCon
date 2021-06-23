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
        private readonly string _mainProcess;
        private WindowSnapper _snapper;
        private readonly VlcMediaPlayer _mainPlayer;
        private double _duration;
        private Uri _mediaUri;
        private DispatcherTimer _timerVideoTime;
        private ProcessChecker _processChecker;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ServiceProvider _serviceProvider;

        public Overlay(MediaObject video, string mainProcess, VlcControl vlcControl,
            IServiceCollection serviceCollection)
        {
            _serviceProvider = serviceCollection.BuildServiceProvider();
            InitializeComponent();
            _mainProcess = mainProcess;
            VlcControlPanel.Children.Add(vlcControl);
            _mainPlayer = vlcControl.SourceProvider.MediaPlayer;
            InitAll(video);
        }

        private async void ChangedOverlayVisibilityAsync() //Поменять название/подумать над работой метода
        {

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if(await _snapper.TryWaitProcessHideAsync(_processChecker, "Overlay", _cancellationTokenSource)) break;
                Visibility = Visibility.Hidden;
                if(await _snapper.TryWaitProcessShowAsync(_mainProcess, _cancellationTokenSource)) break;
                Visibility = Visibility.Visible;
            }
        }
        
        private async void InitAll(MediaObject video)
        {
            InitTimer();
            InitSnapper();
            await InitOverlaySettings(video);
            _processChecker = new ProcessChecker(_mainProcess);
            ChangedOverlayVisibilityAsync();
            Show();
        }

        public void StopOverlayAttach()
        {
            Button_Pause(null, null);
            _cancellationTokenSource.Cancel();
            Visibility = Visibility.Hidden;
            _mainPlayer.Audio.Volume = 0;
        }

        public void StartOverlayAttach()
        {
            Button_Play(null, null);
            _cancellationTokenSource = new CancellationTokenSource();
            Visibility = Visibility.Visible;
            ChangedOverlayVisibilityAsync();
            _mainPlayer.Audio.Volume = (int) VolumeSlider.Value;
        }

        private async Task InitOverlaySettings(MediaObject video)
        {
            var settings = _serviceProvider.GetService<IHosting>().GetPlayerSettings();
            VideoSlider.Visibility = settings.SliderVisibility;
            Play.Visibility = settings.PlayButtonVisibility;
            Pause.Visibility = settings.PauseButtonVisibility;
            if (video.Duration != null && video.Duration != TimeSpan.Zero)
            {
                var totalSeconds = video.Duration.Value.TotalSeconds;
                VideoSlider.Maximum = totalSeconds;
                _duration = totalSeconds;
            }

            await SetMedia(video);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider) sender).SelectionEnd = e.NewValue;
            if (_mainPlayer.State == MediaStates.Ended || _mainPlayer.State == MediaStates.NothingSpecial)
            {
                _mainPlayer.SetMedia(_mediaUri);
                _mainPlayer.Play();
            }

            if (Math.Abs(e.NewValue - e.OldValue) >= 0.520)
                _mainPlayer.Position = (float) e.NewValue / (float) _duration;
        }

        private void InitSnapper()
        {
            _snapper = new WindowSnapper(this);
            _snapper.AttachAsync(_mainProcess);
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
            if (!_mainPlayer.IsPlaying()) return;
            _mainPlayer.Pause();
            _timerVideoTime.Stop();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (_mainPlayer.IsPlaying()) return;
            _mainPlayer.Play();
            _timerVideoTime.Start();
        }

        private async Task SetMedia(MediaObject video)
        {
            try
            {
                _mediaUri = await _serviceProvider.GetService<IHosting>().GetUriAsync(video.Url);
                _mainPlayer.SetMedia(_mediaUri);
                VolumeSlider.Value = 70;
                Button_Play(null, null);
            }
            catch
            {
                ErrorHandler.ThrowErrorConnection();
            }
        }

        private void InitTimer()
        {
            _timerVideoTime = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};
            _timerVideoTime.Tick += timer_Tick;
            _timerVideoTime.Tick += show_Time_Video;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            VideoSlider.Value = _mainPlayer.Position * _duration;
        }

        private void show_Time_Video(object sender, EventArgs e)
        {
            TimeShow.Content = TimeSpan.FromSeconds(_mainPlayer.Position * _duration).ToString("hh\\:mm\\:ss");
        }

        private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider) sender).SelectionEnd = e.NewValue;
            _mainPlayer.Audio.Volume = (int) e.NewValue;
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