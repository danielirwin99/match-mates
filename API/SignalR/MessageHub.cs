using System.Runtime.CompilerServices;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    // Deriving from SignalR Hub
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageHub(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IHubContext<PresenceHub> presenceHub)
        {
            _presenceHub = presenceHub;
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            // Getting access to our HttpContext --> We need to this because:
            // To initialise the connect it sends up a http request
            var httpContext = Context.GetHttpContext();

            // Query string for the users username
            var otherUser = httpContext.Request.Query["user"];

            // Gets the two groups that we are working with
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            // Adds them to the connection
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Adds the groupName of the two
            await AddToGroup(groupName);

            // Getting the chain of messages between the two groups
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            // Receiving the messages from SignalR instead of an API Call --> Pass through the messages
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        // Our Live Send Message Controller
        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            // Getting the User
            var username = Context.User.GetUsername();

            // Checking if the user tries to send a message to themself
            if (username == createMessageDTO.RecipientUsername.ToLower())
                // Exceptions cost more resources than HTTP Requests
                throw new HubException("You cannot send messages to yourself");

            // Getting the sender
            var sender = await _userRepository.GetUserByUsernameAsync(username);

            // Getting the recipient
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null) throw new HubException("User Not Found");

            // What will be in the message request
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            // Getting our two users group name
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            // If we do have the recipients username then we want to change the date read to UTC
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            // If we do not have that message group
            else
            {
                // THESE LINES ALLOW THE USER TO RECEIVE A MESSAGE NOTIFICATION
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                // If the connections is NOT NULL --> Then we know the user is connected to our application
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            // This is for Entity Framework to track the message
            _messageRepository.AddMessage(message);

            // Returns the response to use and saves it
            if (await _messageRepository.SaveAllAsync())
            {
                // Getting our two users usernames
                // Sends the Message
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
            }

        }

        // Caller is you and other is the other user
        private string GetGroupName(string caller, string other)
        {
            // Sorts it alphabetically between the two --> Returns an INT
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            // Orders it depending on which is true or not
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            // Accessing the group
            var group = await _messageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            // Assures us that we have the group
            if (group == null)
            {
                group = new Group(groupName);
                // Adds the group to the repo group
                _messageRepository.AddGroup(group);
            }

            // Adds the connection that we got from the group
            group.Connections.Add(connection);

            // Saves it to the repo
            return await _messageRepository.SaveAllAsync();
        }

        // Removing them from the group and NOT SIGNALR
        private async Task RemoveFromMessageGroup()
        {
            // Getting our connection
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);

            // Removes it
            _messageRepository.RemoveConnection(connection);

            // Saves the new information after removing it
            await _messageRepository.SaveAllAsync();
        }
    }
}