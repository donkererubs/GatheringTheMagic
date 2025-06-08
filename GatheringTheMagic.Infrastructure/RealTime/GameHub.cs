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
        Task ReceivePrivateMessage(string fromUser, string message);
        Task ReceivePrivateMessageHistory(string withUser, ChatMessage[] history);
    }

    public class GameHub(IUserConnectionManager connectionManager, IChatHistoryService chatHistory, IPrivateChatHistoryService privateChatHistoryService) : Hub<IGameClient>
    {
        private readonly IUserConnectionManager _connectionManager = connectionManager;
        private readonly IChatHistoryService _chatHistory = chatHistory;
        private readonly IPrivateChatHistoryService _privateChatHistoryService = privateChatHistoryService;

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
            // 1) Track the new user
            _connectionManager.AddUser(userName, Context.ConnectionId);

            // 2) Broadcast updated lobby
            await Clients.All.ReceiveUserList(_connectionManager.GetAllUsers().ToArray());

            // 3) Send down *private* chat histories for any existing threads
            var partners = _privateChatHistoryService.GetChatPartners(userName);
            foreach (var partner in partners)
            {
                // fetch the ordered history between these two users
                var history = _privateChatHistoryService
                    .GetHistory(userName, partner)
                    .OrderBy(m => m.Timestamp)
                    .ToArray();

                // send it to the newly‐connected client
                await Clients.Caller.ReceivePrivateMessageHistory(partner, history);
            }
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
         
        public async Task SendPrivateMessage(string targetUser, string message)
        {
            // determine sender & recipient
            var fromUser = _connectionManager.GetUserByConnectionId(Context.ConnectionId);
            var targetConn = _connectionManager.GetConnectionId(targetUser);
            if (fromUser == null || targetConn == null) return;

            // 1) Persist the message
            var chatMsg = new ChatMessage(fromUser, message, DateTime.UtcNow);
            _privateChatHistoryService.AddMessage(fromUser, targetUser, chatMsg);

            // 2) Forward it in real time
            await Clients.Client(targetConn)
                .ReceivePrivateMessage(fromUser, message);
        }
    }
}
