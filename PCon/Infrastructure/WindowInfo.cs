using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PCon.Infrastructure
{
    public static class WindowInfo
    {
        public static Rect GetWindowBounds(IntPtr handle)
        {
            var bounds = new Rect();
            WinApi.GetWindowRect(handle, ref bounds);
            return bounds;
        }

        public static Size GetStartOverlayPosition(IntPtr handle)
        {
            var size = new Size();
            var bounds = GetWindowBounds(handle);
            size.Width = bounds.Width;
            size.Height = bounds.Height;
            return size;
        }

        public static Size GetWindowHandleSize(IntPtr handle)
        {
            var size = new Size();
            var bounds = GetWindowBounds(handle);
            size.Height = 1080;
            size.Width = 1920;
            return size;
        }

        public static async Task<IntPtr> GetWindowHandleAsync(string process)
        {
            await ProcessChecker.WaitProcessShowAsync(process, new CancellationToken());
            return WinApi.GetForegroundWindow();
        }
    }
}