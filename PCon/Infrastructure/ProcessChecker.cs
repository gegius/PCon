using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCon.Infrastructure
{
    public class ProcessChecker
    {
        private readonly string process;

        public ProcessChecker(string process)
        {
            this.process = process;
        }

        private static async Task WaitAsync(string windowHandle, CancellationToken cancellationToken,
            Func<string, bool> func)
        {
            await Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (func(windowHandle)) break;
                }
            }, cancellationToken);
        }

        public static async Task WaitProcessShowAsync(string windowHandle, CancellationToken cancellationToken)
        {
            await WaitAsync(windowHandle, cancellationToken, IsWindowShowed);
        }

        public async Task WaitProcessHideAsync(string windowHandle, CancellationToken cancellationToken)
        {
            await WaitAsync(windowHandle, cancellationToken, IsWindowHidden);
        }

        public static bool IsWindowShowed(string handle)
        {
            var topWindowText = GetTopWindowText();
            return topWindowText.Contains(handle);
        }

        private bool IsWindowHidden(string windowHandle)
        {
            var topWindowText = GetTopWindowText();
            return !topWindowText.Contains(process) && !topWindowText.Contains(windowHandle);
        }

        private static string GetTopWindowText()
        {
            var hWnd = WinApi.GetForegroundWindow();
            var length = WinApi.GetWindowTextLength(hWnd);
            var text = new StringBuilder(length + 1);
            WinApi.GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }
    }
}