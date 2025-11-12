using BusinessObjects;
using DataAccess;

namespace Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDAO _dao;

        public ChatRepository(ChatDAO dao)
        {
            _dao = dao;
        }

        public void DeleteChat(Chat chat) => _dao.DeleteChat(chat);

        public Chat GetChatById(string id) => _dao.GetChatById(int.Parse(id));

        public List<Chat> GetChats() => _dao.GetChats();

        public void SaveChat(Chat chat) => _dao.SaveChat(chat);

        public void UpdateChat(Chat chat) => _dao.UpdateChat(chat);

        public List<Chat> GetChatsBetweenUsers(short senderId, short receiverId)
        {
            var allChats = _dao.GetChats();
            return allChats.Where(c =>
                (c.SenderId == senderId && c.ReceiverId == receiverId) ||
                (c.SenderId == receiverId && c.ReceiverId == senderId)
            ).OrderBy(c => c.Timestamp).ToList();
        }

        public List<short> GetChatUserIds(short currentUserId)
        {
            var allChats = _dao.GetChats();
            var userIds = allChats
                .Where(c => c.SenderId == currentUserId || c.ReceiverId == currentUserId)
                .SelectMany(c => new[] { c.SenderId, c.ReceiverId })
                .Where(id => id.HasValue && id.Value != currentUserId)
                .Select(id => id.Value)
                .Distinct()
                .ToList();
            return userIds;
        }
    }
}