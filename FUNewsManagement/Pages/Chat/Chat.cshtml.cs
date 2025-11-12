
//using BusinessObjects;
//using BussinessObject;
//using FUNewsManagement.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Service;
//using System.Security.Claims;
//using static FUNewsManagement.Models.ChatModel;

//namespace FUNewsManagement.Pages
//{
//    [Authorize]
//    public class ChatModel : PageModel
//    {
//        private readonly ISystemAccountService _accountService;
//        private readonly INewsArticleService _newsArticleService;

//        public ChatModel(ISystemAccountService accountService, INewsArticleService newsArticleService)
//        {
//            _accountService = accountService;
//            _newsArticleService = newsArticleService;
//        }

//        public SystemAccount CurrentUser { get; set; }
//        public List<ChatUserDto> ChatUsers { get; set; }
//        public SystemAccount CurrentReceiver { get; set; }
//        public List<NewsArticle> ChatHistory { get; set; }

//        public async Task<IActionResult> OnGetAsync(string id)
//        {
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (userId == null) return RedirectToPage("/Auth/Login");

//            CurrentUser = await _accountService.GetAccountByIdAsync(int.Parse(userId));
//            if (CurrentUser == null) return RedirectToPage("/Auth/Login");

//            await LoadChatUsers(userId);

//            if (!string.IsNullOrEmpty(id))
//            {
//                CurrentReceiver = await _accountService.GetAccountByIdAsync(int.Parse(id));
//                await LoadChatHistory(userId, id);
//            }
            
//            return Page();
//        }

//        private async Task LoadChatUsers(string currentUserId)
//        {
//            try
//            {
//                var allAccounts = await _accountService.GetAllAccountsAsync();
//                ChatUsers = allAccounts
//                    .Where(a => a.AccountId.ToString() != currentUserId)
//                    .Select(a => new ChatUserDto
//                    {
//                        UserId = a.AccountId.ToString(),
//                        UserName = a.AccountName,
//                        UserAvatar = "", // Add avatar URL if available
//                        LastMessage = "", // Implement last message logic if needed
//                        LastMessageTime = DateTime.MinValue, // Implement last message time logic if needed
//                        MessageCount = 0 // Implement message count logic if needed
//                    }).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error loading chat users: {ex.Message}");
//                ChatUsers = new List<ChatUserDto>();
//            }
//        }

//        private async Task LoadChatHistory(string currentUserId, string receiverId)
//        { 
//            ChatHistory = new List<NewsArticle>();
//        }
//    }
//}
