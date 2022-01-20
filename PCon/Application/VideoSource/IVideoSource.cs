using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCon.Domain.Player;

namespace PCon.Application.VideoSource
{
    public interface IVideoSource
    {
        IPlayerSettings GetPlayerSettings();
        Task<Uri> GetUriAsync(string link);
        IAsyncEnumerable<MediaObject> SearchMediaAsync(string query);
        IAsyncEnumerable<MediaObject> SearchTrendsAsync();
    }
}