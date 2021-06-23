using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PCon.Domain.HotKeys;
using PCon.Infrastructure;

namespace PCon.View
{
    public partial class DesktopSettings
    {
        public Overlay Overlay { get; set; }
        private readonly ServiceCollection _serviceCollection;
        private IntPtr _windowHandle;
        private HwndSource _hwndSource;
        private OverlaySettings _overlaySettings;
        private ProcessChecker _processChecker;
        private string _mainProcess;
        private WindowSnapper _snapper;
        private bool _overlaySettingsStarted;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public DesktopSettings(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
            _hwndSource = HwndSource.FromHwnd(_windowHandle);
            _hwndSource?.AddHook(HwndHook);
            WinApi.RegisterHotKey(_windowHandle, WindowsHotKeys.HotKeyId, WindowsHotKeys.ModControl,
                WindowsHotKeys.VkTab);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WindowsHotKeys.WmHotKey) return IntPtr.Zero;
            if (wParam.ToInt32() != WindowsHotKeys.HotKeyId) return IntPtr.Zero;
            var vKey = ((int) lParam >> 16) & 0xFFFF;
            if (vKey == WindowsHotKeys.VkTab && _overlaySettingsStarted)
            {
                switch (_overlaySettings.Visibility)
                {
                    case Visibility.Visible when _processChecker.IsWindowShowed("OverlaySettings") ||
                                                 _processChecker.IsWindowShowed(_mainProcess):
                        StopOverlaySettingsAttach();
                        Overlay?.StartOverlayAttach();
                        break;
                    case Visibility.Hidden when _processChecker.IsWindowShowed(_mainProcess) ||
                                                _processChecker.IsWindowShowed("Overlay"):
                        Overlay?.StopOverlayAttach();
                        StartOverlaySettingsAttach();
                        break;
                    case Visibility.Collapsed:
                        break;
                }
            }
            handled = true;
            return IntPtr.Zero;
        }

        public void StopOverlaySettingsAttach()
        {
            _cancellationTokenSource.Cancel();
            _overlaySettings.Visibility = Visibility.Hidden;
        }

        private void StartOverlaySettingsAttach()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _overlaySettings.Visibility = Visibility.Visible;
            var size = WindowInfo.GetMainProcessWindowSize(_snapper.WindowHandle);
            _overlaySettings.Width = size.Width;
            _overlaySettings.Height = size.Height;
            ChangeOverlayVisibility().Wait();
        }

        protected override void OnClosed(EventArgs e)
        {
            _hwndSource.RemoveHook(HwndHook);
            WinApi.UnregisterHotKey(_windowHandle, WindowsHotKeys.HotKeyId);
            base.OnClosed(e);
        }
        
        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            if (_mainProcess is null)
            {
                ErrorHandler.ThrowErrorNotSelectedProcess();
                return;
            }
            
            _processChecker = new ProcessChecker(_mainProcess);
            _overlaySettings = new OverlaySettings(_mainProcess, _serviceCollection) {DesktopSettings = this};
            InitSnapper();
            _overlaySettingsStarted = true;
            _overlaySettings.Visibility = Visibility.Hidden;
            Hide();
        }

        private async void WaitChangedOverlaySettingsVisibility() //Поменять название/подумать над работой метода
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                await _processChecker.WaitHideAsync("OverlaySettings", _cancellationTokenSource.Token);
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                _overlaySettings.Visibility = Visibility.Hidden;
                await _processChecker.WaitShowAsync(_cancellationTokenSource.Token);
                if (_cancellationTokenSource.Token.IsCancellationRequested) break;
                _overlaySettings.Visibility = Visibility.Visible;
            }
        }

        private async Task ChangeOverlayVisibility()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_processChecker.IsWindowHidden("OverlaySettings"))
                        _overlaySettings.Visibility = Visibility.Hidden;
                    if (_processChecker.IsWindowShowed(_mainProcess))
                        _overlaySettings.Visibility = Visibility.Visible;
                }
            });
        }
        private void InitSnapper()
        {
            _snapper = new WindowSnapper(_overlaySettings, _mainProcess);
            _snapper.AttachAsync();
        }

        private void ProcessLabel_OnClick(object sender, RoutedEventArgs e)
        {
            var label = (Label) sender;
            label.Background = FindResource("AwesomeGreenColor") as Brush;
            if (!PanelInsideProcessPrograms.Children.Contains(label)) return;
            ChangeLabelColor(sender);
            _mainProcess = label.Content.ToString();
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            _mainProcess = default;
            PanelInsideProcessPrograms.Children.Clear();
            var processlist = Process.GetProcesses();
            foreach (var process in processlist)
            {
                var name = process.MainWindowTitle;
                if (IsCorrectProcess(name))
                    PanelInsideProcessPrograms.Children.Add(CreateProcessLabel(name));
            }
        }

        private static bool IsCorrectProcess(string name)
        {
            return !string.IsNullOrEmpty(name) && name != "DesktopSettings";
        }

        private Label CreateProcessLabel(object content)
        {
            var label = new Label {Foreground = Brushes.Black, Content = content};
            label.MouseDown += ProcessLabel_OnClick;
            label.MouseEnter += ProcessLabel_MouseEnter;
            label.MouseLeave += ProcessLabel_MouseLeave;
            label.MouseUp += ProcessLabel_MouseUp;
            return label;
        }
        
        private void ChangeLabelColor(object sender)
        {
            foreach (var child in PanelInsideProcessPrograms.Children)
            {
                var label = (Label) child;
                label.Background = label == (Label) sender ? FindResource("AwesomeGreenColor") as Brush : FindResource("Empty") as Brush;
            }
        }

        private void ProcessLabel_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void ProcessLabel_MouseLeave(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("Empty") as Brush;
        }

        private void ProcessLabel_MouseUp(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }
    }
}