using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCon.Domain.Player;

namespace PCon.Application.HostingService
{
    public interface IHosting
    {
        IPlayerSettings GetPlayerSettings();
        Task<Uri> GetUri(string link);
        IAsyncEnumerable<MediaObject> SearchMedia(string request);
        IAsyncEnumerable<MediaObject> SearchTrends();
    }
}