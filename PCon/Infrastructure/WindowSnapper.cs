using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PCon.Infrastructure
{
    public class WindowSnapper
    {
        private readonly DispatcherTimer _timer;
        public IntPtr WindowHandle { get; private set; }
        private Rect _lastBounds;
        private readonly Window _window;
        private readonly string _windowTitle;

        public WindowSnapper(Window window, string windowTitle)
        {
            _window = window;
            _window.Topmost = true;
            _windowTitle = windowTitle;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};
            _timer.Tick += (x, y) => SnapToWindow();
            _timer.IsEnabled = false;
        }

        public async void AttachAsync()
        {
            WindowHandle = await WindowInfo.GetWindowHandleAsync(_windowTitle);
            _timer.Start();
        }

        private void SnapToWindow()
        {
            var bounds = WindowInfo.GetWindowBounds(WindowHandle);

            if (bounds == _lastBounds) return;
            switch (_window.ToString())
            {
                case "PCon.View.OverlaySettings":
                    _window.Top = bounds.Top;
                    _window.Left = bounds.Left;
                    break;
                case "PCon.View.Overlay":
                    _window.Top = bounds.Top;
                    _window.Left = bounds.Right - _window.Width;
                    break;
            }

            _lastBounds = bounds;
        }
    }
}