using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCon.Domain
{
    public static class Monitor
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string GetTopWindowText()
        {
            var hWnd = GetForegroundWindow();
            var length = GetWindowTextLength(hWnd);
            var text = new StringBuilder(length + 1);
            GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }
    }
}