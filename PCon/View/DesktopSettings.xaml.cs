using System;
using System.Diagnostics;
using System.Threading;
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
        private HwndSource _source;
        private OverlaySettings overlaySettings;
        private ProcessChecker processChecker;
        private string mainProcess;
        private WindowSnapper snapper;
        private bool overlaySettingsStarted;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DesktopSettings(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);
            WinApi.RegisterHotKey(_windowHandle, WindowsHotKeys.HotKeyId, WindowsHotKeys.ModControl,
                WindowsHotKeys.VkTab);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WindowsHotKeys.WmHotKey) return IntPtr.Zero;
            if (wParam.ToInt32() != WindowsHotKeys.HotKeyId) return IntPtr.Zero;
            var vKey = ((int) lParam >> 16) & 0xFFFF;
            if (vKey == WindowsHotKeys.VkTab && overlaySettingsStarted)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (overlaySettings.Visibility)
                {
                    case Visibility.Visible when ProcessChecker.IsWindowShowed("OverlaySettings") ||
                                                 ProcessChecker.IsWindowShowed(mainProcess):
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
            WaitChangedOverlaySettingsVisibility();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            WinApi.UnregisterHotKey(_windowHandle, WindowsHotKeys.HotKeyId);
            base.OnClosed(e);
        }


        private void Button_Click_Start(object sender, RoutedEventArgs e) //добавить вплывающее окно
        {
            if (mainProcess is null)
            {
                PanelInsideBoxPrograms.Children.Add(new Label
                    {Content = "Вы не выбрали программу", Foreground = Brushes.Red});
                return;
            }
            processChecker = new ProcessChecker(mainProcess);
            overlaySettings = new OverlaySettings(mainProcess, _serviceCollection) {DesktopSettings = this};
            InitSnapper();
            overlaySettingsStarted = true;
            overlaySettings.Visibility = Visibility.Hidden;
            Hide();
        }

        private async void WaitChangedOverlaySettingsVisibility() //Поменять название/подумать над работой метода
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

        private void ProcessLabel_OnClick(object sender, RoutedEventArgs e) //переписать заменив перенос процесса выделением
        {
            var label = (Label) sender;
            label.Background = FindResource("AwesomeGreenColor") as Brush;
            if (PanelInsideProcessPrograms.Children.Contains(label))
            {
                PanelInsideBoxPrograms.Children.Clear();
                PanelInsideBoxPrograms.Children.Add(CreateProcessLabel(label.Content));
                mainProcess = label.Content.ToString();
            }
            if (!PanelInsideBoxPrograms.Children.Contains(label)) return;
            mainProcess = null;
            PanelInsideBoxPrograms.Children.Remove(label);
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
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

        private void ProcessLabel_MouseEnter(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void ProcessLabel_MouseLeave(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("Empty") as Brush;
        }

        private void ProcessLabel_MouseUp(object sender, RoutedEventArgs e)
        {
            ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }
    }
}