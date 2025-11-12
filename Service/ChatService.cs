using BusinessObjects;
using Repository;

namespace Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public void DeleteChat(string id)
        {
            var chat = _chatRepository.GetChatById(id);
            if (chat != null)
            {
                _chatRepository.DeleteChat(chat);
            }
        }

        public Chat GetChatById(string id)
        {
            return _chatRepository.GetChatById(id);
        }

        public List<Chat> GetChats()
        {
            return _chatRepository.GetChats();
        }

        public void SaveChat(Chat chat)
        {
            _chatRepository.SaveChat(chat);
        }

        public void UpdateChat(Chat chat)
        {
            _chatRepository.UpdateChat(chat);
        }

        public List<Chat> GetChatsBetweenUsers(short senderId, short receiverId)
        {
            return _chatRepository.GetChatsBetweenUsers(senderId, receiverId);
        }

        public List<short> GetChatUserIds(short currentUserId)
        {
            return _chatRepository.GetChatUserIds(currentUserId);
        }
    }
}