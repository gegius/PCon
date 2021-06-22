using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCon.Domain
{
    public class Monitor
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        //  int GetWindowText(
        //      __in   HWND hWnd,
        //      __out  LPTSTR lpString,
        //      __in   int nMaxCount
        //  );
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        //  DWORD GetWindowThreadProcessId(
        //      __in   HWND hWnd,
        //      __out  LPDWORD lpdwProcessId
        //  );
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        //HANDLE WINAPI OpenProcess(
        //  __in  DWORD dwDesiredAccess,
        //  __in  BOOL bInheritHandle,
        //  __in  DWORD dwProcessId
        //);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        //  DWORD WINAPI GetModuleBaseName(
        //      __in      HANDLE hProcess,
        //      __in_opt  HMODULE hModule,
        //      __out     LPTSTR lpBaseName,
        //      __in      DWORD nSize
        //  );
        [DllImport("psapi.dll")]
        private static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        //  DWORD WINAPI GetModuleFileNameEx(
        //      __in      HANDLE hProcess,
        //      __in_opt  HMODULE hModule,
        //      __out     LPTSTR lpFilename,
        //      __in      DWORD nSize
        //  );
        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        public static string GetTopWindowText()
        {
            IntPtr hWnd = GetForegroundWindow();
            int length = GetWindowTextLength(hWnd);
            StringBuilder text = new StringBuilder(length + 1);
            GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }

        public static string GetTopWindowName()
        {
            var hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out var lpdwProcessId);

            var hProcess = OpenProcess(0x0410, false, lpdwProcessId);

            var text = new StringBuilder(1000);
            //GetModuleBaseName(hProcess, IntPtr.Zero, text, text.Capacity);
            GetModuleFileNameEx(hProcess, IntPtr.Zero, text, text.Capacity);

            CloseHandle(hProcess); 

            return text.ToString();
        }
    }
}