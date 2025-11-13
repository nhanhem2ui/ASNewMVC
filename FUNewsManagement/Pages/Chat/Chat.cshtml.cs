using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using System.Security.Claims;
using static FUNewsManagement.Models.ChatModel;

namespace FUNewsManagement.Pages.Chat
{
    [Authorize]
    public class ChatModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly IChatService _chatService;

        public ChatModel(ISystemAccountService accountService, IChatService chatService)
        {
            _accountService = accountService;
            _chatService = chatService;
        }

        public SystemAccount CurrentUser { get; set; }
        public List<ChatUserDto> ChatUsers { get; set; } = new List<ChatUserDto>();
        public SystemAccount CurrentReceiver { get; set; }
        public List<BusinessObjects.Chat> ChatHistory { get; set; } = new List<BusinessObjects.Chat>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToPage("/Auth/Login");

            CurrentUser = _accountService.GetSystemAccountById(short.Parse(userId));
            if (CurrentUser == null) return RedirectToPage("/Auth/Login");

            await LoadChatUsers(short.Parse(userId));

            if (!string.IsNullOrEmpty(id))
            {
                CurrentReceiver = _accountService.GetSystemAccountById(short.Parse(id));
                await LoadChatHistory(short.Parse(userId), short.Parse(id));
            }

            return Page();
        }

        public IActionResult OnGetChatHistory(string receiverId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new JsonResult(new { error = "User not authenticated" }) { StatusCode = 401 };
            }

            try
            {
                if (!short.TryParse(receiverId, out short receiverIdShort))
                {
                    return new JsonResult(new { error = "Invalid receiver ID" }) { StatusCode = 400 };
                }

                var currentUserId = short.Parse(userId);
                var messages = _chatService.GetChats()
                    .Where(c => (c.SenderId == currentUserId && c.ReceiverId == receiverIdShort) ||
                               (c.SenderId == receiverIdShort && c.ReceiverId == currentUserId))
                    .OrderBy(c => c.Timestamp)
                    .ToList();

                var chatHistory = messages.Select(m => new
                {
                    chatId = m.ChatId,
                    senderId = m.SenderId.ToString(),
                    senderName = _accountService.GetSystemAccountById((short)(m.SenderId ?? 0))?.AccountName ?? "Unknown",
                    receiverName = _accountService.GetSystemAccountById((short)(m.ReceiverId ?? 0))?.AccountName ?? "Unknown",
                    receiverId = m.ReceiverId.ToString(),
                    message = m.Message,
                    timestamp = m.Timestamp.ToString("o")
                }).ToList();
                return new JsonResult(chatHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chat history: {ex.Message}");
                return new JsonResult(new { error = "Error loading chat history" }) { StatusCode = 500 };
            }
        }

        private async Task LoadChatUsers(short currentUserId)
        {
            try
            {
                var allChats = _chatService.GetChats()
                    .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                    .ToList();

                var chatUserIds = allChats
                    .Select(c => c.SenderId == currentUserId ? c.ReceiverId : c.SenderId)
                    .Distinct()
                    .ToList();

                if (!chatUserIds.Any())
                {
                    ChatUsers = new List<ChatUserDto>();
                    return;
                }

                var chatAccounts = _accountService.GetSystemAccounts()
                    .Where(a => chatUserIds.Contains(a.AccountId))
                    .ToList();

                ChatUsers = chatAccounts
                    .Select(a =>
                    {
                        var userId = a.AccountId;
                        var userChats = allChats
                            .Where(c => c.SenderId == userId || c.ReceiverId == userId)
                            .OrderByDescending(c => c.Timestamp)
                            .ToList();

                        var lastChat = userChats.FirstOrDefault();

                        return new ChatUserDto
                        {
                            UserId = userId,
                            UserName = a.AccountName ?? "Unknown",
                            LastMessage = lastChat?.Message ?? "",
                            LastMessageTime = lastChat?.Timestamp ?? DateTime.MinValue,
                            MessageCount = userChats.Count
                        };
                    })
                    .OrderByDescending(u => u.LastMessageTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chat users: {ex.Message}");
                ChatUsers = new List<ChatUserDto>();
            }
        }

        private async Task LoadChatHistory(short currentUserId, short receiverId)
        {
            try
            {
                ChatHistory = _chatService.GetChatsBetweenUsers(currentUserId, receiverId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chat history: {ex.Message}");
                ChatHistory = new List<BusinessObjects.Chat>();
            }
        }
    }
}