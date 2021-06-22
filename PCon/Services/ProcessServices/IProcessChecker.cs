using System.Threading;
using System.Threading.Tasks;

namespace PCon.Services.ProcessServices
{
    internal interface IProcessChecker
    {
        Task ShowChecker(CancellationToken cancellationToken);
        Task HideChecker(string windowHandle, CancellationToken cancellationToken);

        bool IsWindowShowed(string handle);
    }
}
