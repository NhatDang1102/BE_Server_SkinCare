using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.DTOs;

public interface IChatService
{
    Task<string> ChatAsync(Guid userId, string prompt);
    Task<List<ChatHistoryItem>> GetHistoryAsync(Guid userId, int take = 20);

}
