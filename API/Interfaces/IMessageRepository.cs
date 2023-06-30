using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        // Method of adding message
        void AddMessage(Message message);

        // Method of deleting message
        void DeleteMessage(Message message);

        // Getting our message via the id
        Task<Message> GetMessage(int id);

        // Returns a list from the DTO of the messages
        // Treating this like an unread inbox
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();
    }
}