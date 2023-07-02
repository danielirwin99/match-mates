using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            // Getting our query to store
            var query = _context.Messages
            // Latest message first
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

            // Which container they wish to view the messages for
            query = messageParams.Container switch
            {
                // If the container equals "Inbox" we want to query from these parameters
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
                // We also want to check to see if the other side (recipient) has not deleted the message as well
                && u.RecipientDeleted == false),

                // If the container equals "Outbox" we want to query from these parameters
                "OutBox" => query.Where(u => u.SenderUsername == messageParams.Username
                // We also want to check to see if the other side (sender) has not deleted the message as well
                && u.SenderDeleted == false),

                // Our Unread Message Container
                _ => query.Where(u => u.RecipientUsername == messageParams.Username
                && u.RecipientDeleted == false
                && u.DateRead == null)

            };

            // After we get the container we are looking for we want to project that to our MessageDTO
            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            // Returning the container as a PagedList
            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            // Getting the messages
            var messages = await _context.Messages
            // Including the Sender and their photo
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            // Looking for both the Sender and Receiver of the message thread
            .Where(
                m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false // The other side has not deleted the message
                && m.SenderUsername == recipientUsername ||
                m.RecipientUsername == recipientUsername && m.SenderDeleted == false // The other side has not deleted the message
                && m.SenderUsername == currentUsername
            )
            // Latest messages first
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

            // Getting a list of Unread Messages and marking them as sent
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            // If there are any unread messages
            if (unreadMessages.Any())
            {
                // Looping over each message
                foreach (var message in unreadMessages)
                {
                    // Changes the read time to now after reading an unopened / unread message
                    message.DateRead = DateTime.UtcNow;
                }
                // Updates the changes
                await _context.SaveChangesAsync();
            }

            // Maps the messages as an Enumerable
            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            // When there is a change (Above 0)
            return await _context.SaveChangesAsync() > 0;
        }
    }
}