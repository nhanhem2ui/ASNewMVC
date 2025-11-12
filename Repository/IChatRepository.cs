using BusinessObjects;

namespace Repository
{
    public interface IChatRepository
    {
        void SaveChat(Chat chat);
        void UpdateChat(Chat chat);
        void DeleteChat(Chat chat);
        List<Chat> GetChats();
        Chat GetChatById(string id);
        List<Chat> GetChatsBetweenUsers(string senderId, string receiverId);
        List<string> GetChatUserIds(string currentUserId);
    }
}