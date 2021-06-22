using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PCon.Infrastructure;

namespace PCon.View
{
    public partial class DesktopSettings
    {

        // ReSharper disable once IdentifierTypo
        private const int HotkeyId = 9000;
        private const uint VkTab = 0x09;
        private const uint ModControl = 0x0002;


        private readonly ServiceCollection _serviceCollection;
        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);

            WinApi.RegisterHotKey(_windowHandle, HotkeyId, ModControl, VkTab);
        }

        // ReSharper disable once IdentifierTypo
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int wmHotkey = 0x0312;
            if (msg != wmHotkey) return IntPtr.Zero;
            if (wParam.ToInt32() != HotkeyId) return IntPtr.Zero;
            var vkey = ((int) lParam >> 16) & 0xFFFF;
            if (vkey == VkTab && overlaySettingsStarted)
            {
                switch (overlaySettings.Visibility)
                {
                    case Visibility.Visible when (ProcessChecker.IsWindowShowed("OverlaySettings") ||
                                                  ProcessChecker.IsWindowShowed(mainProcess)):
                        StopOverlaySettingsAttach();
                        Overlay?.StartOverlayAttach();
                        break;
                    case Visibility.Hidden when ProcessChecker.IsWindowShowed(mainProcess) ||
                                                ProcessChecker.IsWindowShowed("Overlay"):
                        Overlay?.StopOverlayAttach();
                        StartOverlaySettingsAttach();
                        break;
                    case Visibility.Collapsed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            handled = true;

            return IntPtr.Zero;
        }

        public void StopOverlaySettingsAttach()
        {
            cancellationTokenSource.Cancel();
            overlaySettings.Visibility = Visibility.Hidden;
        }

        private void StartOverlaySettingsAttach()
        {
            cancellationTokenSource = new CancellationTokenSource();
            overlaySettings.Visibility = Visibility.Visible;
            var size = WindowInfo.GetMainProcessWindowSize(snapper.WindowHandle);
            overlaySettings.Width = size.Width;
            overlaySettings.Height = size.Height;
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                WaitChangedOverlaySettingsVisibility();
            }
            WaitChangedOverlaySettingsVisibility();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            WinApi.UnregisterHotKey(_windowHandle, HotkeyId);
            base.OnClosed(e);
        }

        private OverlaySettings overlaySettings;
        private ProcessChecker processChecker;
        private string mainProcess;
        private WindowSnapper snapper;
        private bool overlaySettingsStarted;
        public Overlay Overlay { get; set; }
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DesktopSettings(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }


        private void Button_Click_Show(object sender, RoutedEventArgs e)
        {
            if (mainProcess is null)
            {
                PanelInsideBoxPrograms.Children.Add(new Label{Content = "Вы не выбрали программу. CODE=101", Foreground = Brushes.Red});
                return;
            }
            processChecker = new ProcessChecker(mainProcess);
            overlaySettings = new OverlaySettings(mainProcess, _serviceCollection) {DesktopSettings = this};
            InitSnapper();
            overlaySettingsStarted = true;
            overlaySettings.Visibility = Visibility.Hidden;
            Hide();
        }

        private async void WaitChangedOverlaySettingsVisibility()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await processChecker.WaitHideAsync("OverlaySettings", cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                overlaySettings.Visibility = Visibility.Hidden;
                await processChecker.WaitShowAsync(cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                overlaySettings.Visibility = Visibility.Visible;
            }
        }

        private void InitSnapper()
        {
            snapper = new WindowSnapper(overlaySettings, mainProcess);
            snapper.AttachAsync();
        }

        private void Label_OnClick(object sender, RoutedEventArgs e)
        {
            var label = (Label) sender;
            label.Background = FindResource("AwesomeGreenColor") as Brush;
            if (PanelInsideProcessPrograms.Children.Contains(label))
            {
                PanelInsideBoxPrograms.Children.Clear();
                PanelInsideBoxPrograms.Children.Add(CreateLabel(label.Content));
                mainProcess = label.Content.ToString();
            }

            if (!PanelInsideBoxPrograms.Children.Contains(label)) return;
            mainProcess = null;
            PanelInsideBoxPrograms.Children.Remove(label);
        }

        private void Label_MouseEnter(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void Label_MouseLeave(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("Empty") as Brush;
        }

        private void Label_MouseUp(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            PanelInsideProcessPrograms.Children.Clear();
            var processlist = Process.GetProcesses();

            foreach (var process in processlist)
            {
                var name = process.MainWindowTitle;
                if (string.IsNullOrEmpty(name) || name == "DesktopSettings") continue;
                var label = CreateLabel(name);
                PanelInsideProcessPrograms.Children.Add(label);
            }
        }

        private Label CreateLabel(object content)
        {
            var label = new Label {Foreground = Brushes.Black, Content = content};
            label.MouseDown += Label_OnClick;
            label.MouseEnter += Label_MouseEnter;
            label.MouseLeave += Label_MouseLeave;
            label.MouseUp += Label_MouseUp;
            return label;
        }
    }
}