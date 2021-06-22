using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PCon.Domain
{
    public class WindowSnapper
    {
        private struct Rect
        {
            private bool Equals(Rect other)
            {
                return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
            }

            public override bool Equals(object obj)
            {
                return obj is Rect other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Left;
                    hashCode = (hashCode * 397) ^ Top;
                    hashCode = (hashCode * 397) ^ Right;
                    hashCode = (hashCode * 397) ^ Bottom;
                    return hashCode;
                }
            }

            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            private int Bottom { get; set; }

            public int Height => Bottom - Top;
            public int Width => Right - Left;

            public static bool operator !=(Rect r1, Rect r2)
            {
                return !(r1 == r2);
            }

            public static bool operator ==(Rect r1, Rect r2)
            {
                return r1.Left == r2.Left && r1.Right == r2.Right && r1.Top == r2.Top && r1.Bottom == r2.Bottom;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        private readonly DispatcherTimer _timer;
        private IntPtr _windowHandle;
        private Rect _lastBounds;
        private bool isFoundProcess;
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

        public async void Attach()
        {
            _windowHandle = await GetWindowHandle(_windowTitle);
            _timer.Start();
        }

        private void SnapToWindow()
        {
            var bounds = GetWindowBounds(_windowHandle);

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

        private static Rect GetWindowBounds(IntPtr handle)
        {
            var bounds = new Rect();
            GetWindowRect(handle, ref bounds);
            return bounds;
        }

        public Size GetMainProcessWindowSize()
        {
            var size = new Size();
            var bounds = GetWindowBounds(_windowHandle);
            size.Width = bounds.Width;
            size.Height = bounds.Height;
            return size;
        }

        private async Task<IntPtr> GetWindowHandle(string windowTitle)
        {
            var result = await Task.Run(() =>
            {
                while (!isFoundProcess)
                {
                    var listProcesses = Process.GetProcesses();
                    foreach (var pList in listProcesses)
                    {
                        if (!pList.MainWindowTitle.Contains(windowTitle)) continue;
                        isFoundProcess = true;
                        return pList.MainWindowHandle;
                    }
                }

                return default;
            });
            return result;
        }
    }
}