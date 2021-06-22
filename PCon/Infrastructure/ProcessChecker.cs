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

        public async Task<bool> WaitShowAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var isFound = false;
                while (!cancellationToken.IsCancellationRequested && !isFound)
                {
                    if (IsWindowShowed(process)) isFound = true;
                }
            }, cancellationToken);
            return true;
        }

        public async Task<bool> WaitHideAsync(string windowHandle, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var isFound = false;
                while (!cancellationToken.IsCancellationRequested && !isFound)
                {
                    if (IsWindowHidden(windowHandle)) isFound = true;
                }
            }, cancellationToken);
            return true;
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