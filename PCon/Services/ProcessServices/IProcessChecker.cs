using System.Threading;
using System.Threading.Tasks;

namespace PCon.Services.ProcessServices
{
    internal interface IProcessChecker
    {
        Task WaitShowAsync(CancellationToken cancellationToken);
        Task WaitHideAsync(string windowHandle, CancellationToken cancellationToken);

        bool IsWindowShowed(string handle);
    }
}
