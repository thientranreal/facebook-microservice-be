using ContactWebApi.Models;

namespace ContactWebApi.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message?>?> GetLatestMsg(int userId);
    Task<IEnumerable<Message>?> GetConversationMessages(
        int userId, 
        int contactId, 
        int pageSize,
        string? msgId, 
        string? createdAt
        );
}