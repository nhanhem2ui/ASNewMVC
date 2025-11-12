
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace FUNewsManagement.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ISystemAccountService _accountService;
        private readonly INewsArticleService _newsArticleService;

        private static readonly ConcurrentDictionary<string, ConnectedUser> ConnectedUsers = new();

        public ChatHub(ISystemAccountService accountService, INewsArticleService newsArticleService)
        {
            _accountService = accountService;
            _newsArticleService = newsArticleService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    await Clients.Caller.SendAsync("Error", "Invalid user authentication");
                    return;
                }

                var userInfo = await _accountService.GetAccountByIdAsync(int.Parse(userId));
                if (userInfo == null)
                {
                    await Clients.Caller.SendAsync("Error", "User not found");
                    return;
                }

                var existingConnections = ConnectedUsers.Where(kvp => kvp.Value.UserId == userId).ToList();
                foreach (var existing in existingConnections)
                {
                    ConnectedUsers.TryRemove(existing.Key, out _);
                }

                var connectedUser = new ConnectedUser
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = userInfo.AccountId.ToString(),
                    UserName = userInfo.AccountName ?? "Unknown",
                    UserAvatar = string.Empty, 
                };

                ConnectedUsers.TryAdd(Context.ConnectionId, connectedUser);

                await Clients.All.SendAsync("UpdatedConnectedUsers", ConnectedUsers.Values.ToList());
                await Clients.Caller.SendAsync("Connected", "Successfully connected to chat");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Connection error: {ex.Message}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (ConnectedUsers.TryRemove(Context.ConnectionId, out var user))
                {
                    await Clients.All.SendAsync("UpdatedConnectedUsers", ConnectedUsers.Values.ToList());
                    await Clients.All.SendAsync("UserDisconnected", user.UserId, user.UserName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnection error: {ex.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            try
            {
                if (!ConnectedUsers.TryGetValue(Context.ConnectionId, out var sender))
                {
                    await Clients.Caller.SendAsync("Error", "Sender not found");
                    return;
                }

                if (string.IsNullOrWhiteSpace(message) || message.Length > 500)
                {
                    await Clients.Caller.SendAsync("Error", "Invalid message");
                    return;
                }

                var chatMessage = new NewsArticle
                {
                    NewsTitle = "Chat Message",
                    NewsContent = message.Trim(),
                    CreatedDate = DateTime.Now,
                    AccountId = int.Parse(sender.UserId),
                };
                
                var receiver = ConnectedUsers.Values.FirstOrDefault(u => u.UserId == receiverId);
                if (receiver != null)
                {
                    await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage",
                        sender.UserId, sender.UserName, message, chatMessage.CreatedDate.ToString("o"));
                }
                await Clients.Caller.SendAsync("MessageSent", receiverId, message, chatMessage.CreatedDate.ToString("o"));
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
            }
        }

        public async Task UserTyping(string receiverId, bool isTyping)
        {
            try
            {
                if (!ConnectedUsers.TryGetValue(Context.ConnectionId, out var sender))
                {
                    return;
                }

                var receiver = ConnectedUsers.Values.FirstOrDefault(u => u.UserId == receiverId);
                if (receiver != null)
                {
                    await Clients.Client(receiver.ConnectionId).SendAsync("UserTyping",
                        sender.UserId, sender.UserName, isTyping);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private class ConnectedUser
        {
            public string ConnectionId { get; set; } = string.Empty;
            public string UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string UserAvatar { get; set; } = string.Empty;
            public DateTime ConnectedAt { get; set; } = DateTime.Now;
        }
    }
}
