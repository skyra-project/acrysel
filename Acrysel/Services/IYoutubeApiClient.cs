using System.Collections.Generic;
using System.Threading.Tasks;
using Acrysel.Services.Entities;
using Remora.Results;

namespace Acrysel.Services;

public interface IYoutubeApiClient
{
    public Task<Result<IEnumerable<YoutubeChannelDescriptor>>> SearchForChannelAsync(string nameQuery);
}