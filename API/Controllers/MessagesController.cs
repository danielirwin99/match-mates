
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        // SENDING THE MESSAGES AS THE USER
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            // Getting the User
            var username = User.GetUsername();

            // Checking if the user tries to send a message to themself
            if (username == createMessageDTO.RecipientUsername.ToLower())
            {
                return BadRequest("Cannot send messages to yourself");
            }

            // Getting the sender
            var sender = await _userRepository.GetUserByUsernameAsync(username);

            // Getting the recipient
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null) NotFound();

            // What will be in the message request
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            // This is for Entity Framework to track the message
            _messageRepository.AddMessage(message);

            // Returns the response to use and saves it
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to send message");
        }

        // GETTING OUR MESSAGES AS THE USER
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            // Getting our username
            messageParams.Username = User.GetUsername();

            // Getting our messages
            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            // Adding our Pagination Headers to the response
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            // So the request fires
            return messages;
        }

        // THREAD GET REQUEST
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            // username coming from the route parameter above
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

        // DELETING A MESSAGE
        [HttpDelete("{id}")]
        // Passing through the id of the route parameter
        public async Task<ActionResult> DeleteMessage(int id)
        {
            // Getting our username
            var username = User.GetUsername();

            // Getting our message
            var message = await _messageRepository.GetMessage(id);

            // Making sure its either the sender or the recipient that is deleting ONLY
            if (message.SenderUsername != username
            && message.RecipientUsername != username) return Unauthorized("You are not authorised to do this");


            // Seeing if either deleted the message
            if (message.SenderUsername == username) message.SenderDeleted = true;

            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            // Seeing if both sides have deleted the message --> Delete it from the DB
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            // If the repository returns true (Nothing broken in the code) --> return 200 Ok Response
            if (await _messageRepository.SaveAllAsync()) return Ok();

            // If something did break --> Send this
            return BadRequest("Problem deleting the message");
        }
    }
}