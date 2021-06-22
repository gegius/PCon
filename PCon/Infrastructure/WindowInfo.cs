using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
namespace PCon.Infrastructure
{
    public class WindowInfo
    {
        private bool isFoundProcess;

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
        
        public static async Task<IntPtr> GetWindowHandle(string windowTitle)
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