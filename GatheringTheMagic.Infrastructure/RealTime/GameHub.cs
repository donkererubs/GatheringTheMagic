using GatheringTheMagic.Application.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GatheringTheMagic.Infrastructure.RealTime
{
    public interface IGameClient
    {
        Task ReceiveUserList(string[] users);
        Task ReceiveGameRequest(string fromUser);
        Task ReceiveGameResponse(string fromUser, bool accepted);
        Task ReceiveMessage(string fromUser, string message);
        Task ReceiveMessageHistory(Application.Services.ChatMessage[] history);
    }

    public class GameHub(IUserConnectionManager connectionManager, IChatHistoryService chatHistory) : Hub<IGameClient>
    {
        private readonly IUserConnectionManager _connectionManager = connectionManager;
        private readonly IChatHistoryService _chatHistory = chatHistory;

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.RemoveConnection(Context.ConnectionId);
            await Clients.All.ReceiveUserList(_connectionManager.GetAllUsers().ToArray());
            await base.OnDisconnectedAsync(exception);
        }

        public override async Task OnConnectedAsync()
        {
            // 1) Immediately send current users
            await Clients.Caller.ReceiveUserList(_connectionManager.GetAllUsers().ToArray());
            // 2) Then send full chat history
            var history = _chatHistory.GetHistory()
                .OrderBy(m => m.Timestamp)
                .ToArray();
            await Clients.Caller.ReceiveMessageHistory(history);
            await base.OnConnectedAsync();
        }

        public async Task Register(string userName)
        {
            _connectionManager.AddUser(userName, Context.ConnectionId);
            await Clients.All.ReceiveUserList(_connectionManager.GetAllUsers().ToArray());
        }

        public async Task RequestGame(string targetUser)
        {
            var targetConn = _connectionManager.GetConnectionId(targetUser);
            var fromUser = _connectionManager.GetUserByConnectionId(Context.ConnectionId);
            if (targetConn != null)
                await Clients.Client(targetConn).ReceiveGameRequest(fromUser);
        }

        public async Task RespondGameRequest(string requesterUser, bool accepted)
        {
            var reqConn = _connectionManager.GetConnectionId(requesterUser);
            var fromUser = _connectionManager.GetUserByConnectionId(Context.ConnectionId);
            if (reqConn != null)
                await Clients.Client(reqConn).ReceiveGameResponse(fromUser, accepted);
        }

        public async Task SendMessage(string message)
        {
            var from = _connectionManager.GetUserByConnectionId(Context.ConnectionId);
            if (from == null) return;

            // store
            var msg = new ChatMessage(from, message, DateTime.UtcNow);
            _chatHistory.AddMessage(msg);

            // broadcast
            await Clients.All.ReceiveMessage(from, message);
        }
    }
}
