using ContactWebApi.Models;

namespace ContactWebApi.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message?>?> GetLatestMsg(int userId);
    Task<List<Message>> GetConversationMessages(
        int userId, 
        int contactId, 
        int pageSize,
        string? msgId, 
        string? createdAt
        );
}