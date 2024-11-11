using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactWebApi.Models;

namespace ContactWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ContactDbContext _context;

        public MessageController(ContactDbContext context)
        {
            _context = context;
        }

        // GET: api/Message/UserMessages/{userId}/latest
        [HttpGet("UserMessages/{userId}/latest")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUserMessages(int userId)
        {
            var latestMessages = await _context.Messages
                .Where(m => m.Sender == userId || m.Receiver == userId)
                .GroupBy(m => new { 
                    Sender = m.Sender == userId ? m.Receiver : m.Sender, 
                    Receiver = m.Sender == userId ? m.Sender : m.Receiver 
                }) // Nhóm theo Sender và Receiver, đảo ngược nếu cần
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault()) // Lấy tin nhắn mới nhất trong mỗi nhóm
                .ToListAsync();

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
            [FromQuery] String cursor = "",
            [FromQuery] int pageSize = 10
            )
        {
            String createdAt = "", id = "";
            // Decode the cursor if provided
            if (!string.IsNullOrEmpty(cursor))
            {
                try
                {
                    var decodedBytes = Convert.FromBase64String(cursor);
                    String decodedCursor = Encoding.UTF8.GetString(decodedBytes);
                    String[] splitCursor = decodedCursor.Split('&');
                    createdAt = splitCursor[0];
                    id = splitCursor[1];
                }
                catch (Exception)
                {
                    return BadRequest("Invalid cursor format.");
                }
            }

            string sqlQuery;
            List<Message> messages;

            if (!string.IsNullOrEmpty(createdAt))
            {
                sqlQuery = @"
                    SELECT * FROM Messages
                    WHERE (Sender = {0} AND Receiver = {1}
                               OR Receiver = {0} AND Sender = {1})
                    AND (CreatedAt = {2} AND Id < {3} OR CreatedAt < {2})
                    ORDER BY CreatedAt DESC, Id DESC 
                    LIMIT {4}";
                
                messages = await _context.Messages
                    .FromSqlRaw(sqlQuery, userId, contactId, createdAt, id, pageSize)
                    .ToListAsync();
            }
            else
            {
                sqlQuery = @"
                SELECT * FROM Messages
                WHERE (Sender = {0} AND Receiver = {1}
                           OR Receiver = {0} AND Sender = {1})
                ORDER BY CreatedAt DESC, Id DESC 
                LIMIT {2}";
                
                messages = await _context.Messages
                    .FromSqlRaw(sqlQuery, userId, contactId, pageSize)
                    .ToListAsync();
            }

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
        
        // GET: api/Message
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Message/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Message/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Message/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
