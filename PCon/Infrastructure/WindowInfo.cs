using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace PCon.Infrastructure
{
    public class WindowInfo
    {
        public static Rect GetWindowBounds(IntPtr handle)
        {
            var bounds = new Rect();
            WinApi.GetWindowRect(handle, ref bounds);
            return bounds;
        }

        public static Size GetMainProcessWindowSize(IntPtr handle)
        {
            var size = new Size();
            var bounds = GetWindowBounds(handle);
            size.Width = bounds.Width;
            size.Height = bounds.Height;
            return size;
        }
        
        public static async Task<IntPtr> GetWindowHandleAsync(string windowTitle)
        {
            await new ProcessChecker(windowTitle).WaitShowAsync(new CancellationToken());
            return WinApi.GetForegroundWindow();
        }
    }
}