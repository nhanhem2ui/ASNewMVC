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
        List<Chat> GetChatsBetweenUsers(short senderId, short receiverId);
        List<short> GetChatUserIds(short currentUserId);
    }
}