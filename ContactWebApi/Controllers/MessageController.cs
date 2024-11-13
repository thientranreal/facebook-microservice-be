using System.Text;
using Microsoft.AspNetCore.Mvc;
using ContactWebApi.Models;
using ContactWebApi.Repositories;

namespace ContactWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        // GET: api/Message
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            var messages = await _messageRepository.GetAllAsync();

            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }

        // GET: api/Message/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        
        // GET: api/Message/UserMessages/{userId}/latest
        [HttpGet("UserMessages/{userId}/latest")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUserMessages(int userId)
        {
            var latestMessages = await _messageRepository.GetLatestMsg(userId);

            if (!latestMessages.Any())
            {
                return NotFound("No messages found for the given user.");
            }

            return Ok(latestMessages);
        }
        
        // GET: api/Message/UserMessages/{userId}/{contactId}
        [HttpGet("UserMessages/{userId}/{contactId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUserMessagesByContactId(
            int userId, 
            int contactId,
            [FromQuery] string cursor = "",
            [FromQuery] int pageSize = 10
            )
        {
            string? createdAt = null, id = null;
            // Decode the cursor if provided
            if (!string.IsNullOrEmpty(cursor))
            {
                try
                {
                    var decodedBytes = Convert.FromBase64String(cursor);
                    string decodedCursor = Encoding.UTF8.GetString(decodedBytes);
                    string[] splitCursor = decodedCursor.Split('&');
                    createdAt = splitCursor[0];
                    id = splitCursor[1];
                }
                catch (Exception)
                {
                    return BadRequest("Invalid cursor format.");
                }
            }

            List<Message> messages = await _messageRepository
                .GetConversationMessages(userId, contactId, pageSize, id, createdAt);

            if (!messages.Any())
            {
                return NotFound("No messages found for the given user.");
            }
            
            // Encode the cursor for the last message in the current result
            var lastMessage = messages.Last();
            var newCursor = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    lastMessage.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") + "&" + lastMessage.Id));
            
            return Ok(new
            {
                messages,
                cursor = newCursor
            });
        }

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            await _messageRepository.AddAsync(message);

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }
    }
}
