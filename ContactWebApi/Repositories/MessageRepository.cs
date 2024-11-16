using ContactWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactWebApi.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    private readonly ContactDbContext _context;

    public MessageRepository(ContactDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message?>?> GetLatestMsg(int userId)
    {
        var latestMessages = await _context.Messages
            .Where(m => m.Sender == userId || m.Receiver == userId)
            .GroupBy(m => new { 
                Sender = m.Sender == userId ? m.Receiver : m.Sender, 
                Receiver = m.Sender == userId ? m.Sender : m.Receiver 
            })
            .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
            .ToListAsync();

        return latestMessages;
    }
    
    public async Task<IEnumerable<Message>?> GetConversationMessages(
        int userId, 
        int contactId, 
        int pageSize,
        string? msgId, 
        string? createdAt
        )
    {
        IEnumerable<Message> messages;
        if (msgId != null && createdAt != null)
        {
            string sqlQuery = @"
                    SELECT * FROM Messages
                    WHERE (Sender = {0} AND Receiver = {1}
                               OR Receiver = {0} AND Sender = {1})
                    AND (CreatedAt = {2} AND Id < {3} OR CreatedAt < {2})
                    ORDER BY CreatedAt DESC, Id DESC 
                    LIMIT {4}";
                
            messages = await _context.Messages
                .FromSqlRaw(sqlQuery, userId, contactId, createdAt, msgId, pageSize)
                .ToListAsync();
        }
        else
        {
            string sqlQuery = @"
                SELECT * FROM Messages
                WHERE (Sender = {0} AND Receiver = {1}
                           OR Receiver = {0} AND Sender = {1})
                ORDER BY CreatedAt DESC, Id DESC 
                LIMIT {2}";
                
            messages = await _context.Messages
                .FromSqlRaw(sqlQuery, userId, contactId, pageSize)
                .ToListAsync();
        }

        return messages;
    }
}