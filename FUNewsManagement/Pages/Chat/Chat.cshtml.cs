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

        private async Task LoadChatUsers(short currentUserId)
        {
            try
            {
                // Get all chats for the current user
                var allChats = _chatService.GetChats()
                    .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                    .ToList();

                // Get unique user IDs that the current user has chatted with
                var chatUserIds = allChats
                    .Select(c => c.SenderId == currentUserId ? c.ReceiverId : c.SenderId)
                    .Distinct()
                    .ToList();

                // Only load accounts for users who have chat history
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