using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public string mainProcess;
        private WindowSnapper snapper;
        private bool overlaySettingsStarted;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DesktopSettings(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
                Window.GetWindow(this).DragMove();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);
            WinApi.RegisterHotKey(_windowHandle, WinHotKeys.HotKeyId, WinHotKeys.ModControl,
                WinHotKeys.VkTab);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WinHotKeys.WmHotKey) return IntPtr.Zero;
            if (wParam.ToInt32() != WinHotKeys.HotKeyId) return IntPtr.Zero;
            var vKey = ((int) lParam >> 16) & 0xFFFF;
            if (vKey == WinHotKeys.VkTab && overlaySettingsStarted)
            {
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
            ChangedOverlaySettingsVisibilityAsync();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            WinApi.UnregisterHotKey(_windowHandle, WinHotKeys.HotKeyId);
            base.OnClosed(e);
        }

        public void Start()
        {
            if (mainProcess is null)
            {
                ErrorHandler.ThrowErrorNotSelectedProcess();
                return;
            }

            processChecker = new ProcessChecker(mainProcess);
            overlaySettings = new OverlaySettings(mainProcess, _serviceCollection) {DesktopSettings = this};
            InitSnapper();
            overlaySettingsStarted = true;
            overlaySettings.Visibility = Visibility.Hidden;
            Hide();
        }

        private async void ChangedOverlaySettingsVisibilityAsync()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (await snapper.TryWaitProcessHideAsync(processChecker, "OverlaySettings", cancellationTokenSource)
                ) break;
                overlaySettings.Visibility = Visibility.Hidden;
                if (await snapper.TryWaitProcessShowAsync(mainProcess, cancellationTokenSource)) break;
                overlaySettings.Visibility = Visibility.Visible;
            }
        }

        private void InitSnapper()
        {
            snapper = new WindowSnapper(overlaySettings);
            snapper.AttachAsync(mainProcess);
        }
        

        private void Cross_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Minus_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OpenProcessSelect(object sender, MouseButtonEventArgs e)
        {
            var selectProcess = new SelectProcess(_serviceCollection)
            {
                DesktopSettings = this,
                Owner = this
            };
            if (selectProcess.ShowDialog() == true)
            {
                // Пользователь разрешил действие. Продолжить1
            }
            else
            {
                // Пользователь отменил действие.
            }
        }

        private void OpenSettingsWindow(object sender, MouseButtonEventArgs e)
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        private void OpenQuestionWindow(object sender, MouseButtonEventArgs e)
        {
            var questionWindow = new QuestionWindow
            {
                Owner = this
            };
            questionWindow.ShowDialog();
        }

        private void OpenDota2(object sender, MouseButtonEventArgs e)
        {
            mainProcess = ProcessConst.dota2Process;
            Start();
        }

        private void OpenCSGO(object sender, MouseButtonEventArgs e)
        {
            mainProcess = ProcessConst.csgoProcess;
            Start();
        }

        private void OpenCiva(object sender, MouseButtonEventArgs e)
        {
            mainProcess = ProcessConst.csgoProcess;
            Start();
        }

        private void OpenMB(object sender, MouseButtonEventArgs e)
        {
            mainProcess = ProcessConst.csgoProcess;
            Start();
        }

        private void OpenSeaThieves(object sender, MouseButtonEventArgs e)
        {
            mainProcess = ProcessConst.csgoProcess;
            Start();
        }
    }
}