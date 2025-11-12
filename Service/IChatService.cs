using BusinessObjects;

namespace Service
{
    public interface IChatService
    {
        void SaveChat(Chat chat);
        void UpdateChat(Chat chat);
        void DeleteChat(string id);
        List<Chat> GetChats();
        Chat GetChatById(string id);
        List<Chat> GetChatsBetweenUsers(string senderId, string receiverId);
        List<string> GetChatUserIds(string currentUserId);
    }
}