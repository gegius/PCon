﻿using System.Threading;
using System.Threading.Tasks;
using Monitor = PCon.Domain.Monitor;

namespace PCon.Services.ProcessServices
{
    public class ProcessChecker : IProcessChecker
    {
        private readonly string mainProcess;

        public ProcessChecker(string mainProcess)
        {
            this.mainProcess = mainProcess;
        }
        
        public async Task WaitShowAsync(CancellationToken cancellationToken) //Wait show async
        {
            await Task.Run(() =>
            {
                var isFound = false;
                while (!cancellationToken.IsCancellationRequested && !isFound)
                {
                    if (IsWindowShowed(mainProcess)) isFound = true;
                }
            }, cancellationToken);
        }

        public async Task WaitHideAsync(string windowHandle, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var isFound = false;
                while (!cancellationToken.IsCancellationRequested && !isFound)
                {
                    if (IsWindowHidden(windowHandle)) isFound = true;
                }
            }, cancellationToken);
        }

        public bool IsWindowShowed(string handle)
        {
            var topWindowText = Monitor.GetTopWindowText();
            return topWindowText.Contains(handle);
        }

        private bool IsWindowHidden(string windowHandle)
        {
            var topWindowText = Monitor.GetTopWindowText();
            return !topWindowText.Contains(mainProcess) && !topWindowText.Contains(windowHandle);
        }
    }
}
