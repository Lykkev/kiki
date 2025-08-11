using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kick_ChatBOT.Services
{
    public interface IApiClient : IDisposable
    {
        Task<Tuple<ChannelInfo, string>> GetChannelInfoAsync(string channelName, string bearerToken, CancellationToken ct);
        Task<Tuple<bool, int, string>> SendMessageAsync(long chatroomId, string message, string bearerToken, CancellationToken ct);
    }
}


