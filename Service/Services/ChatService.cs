using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contract.DTOs;
using Service.Interfaces;

namespace Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IRedisService _redis;
        private readonly IOpenAiService _openAi;
        private const int DefaultHistoryDays = 3;

        public ChatService(IRedisService redis, IOpenAiService openAi)
        {
            _redis = redis;
            _openAi = openAi;
        }

        public async Task<string> ChatAsync(Guid userId, string prompt)
        {
            var response = await _openAi.GetChatResponseAsync(prompt);

            var chat = new ChatHistoryItem
            {
                ChatId = Guid.NewGuid().ToString(),
                Prompt = prompt,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            var key = $"chatlog:{userId}:{chat.ChatId}";
            var json = JsonSerializer.Serialize(chat);

            await _redis.SetStringAsync(key, json, TimeSpan.FromDays(DefaultHistoryDays));
            return response;
        }

        public async Task<List<ChatHistoryItem>> GetHistoryAsync(Guid userId, int take = 20)
        {
            var pattern = $"chatlog:{userId}:*";
            var vals = await _redis.GetValuesByPatternAsync(pattern);
            var list = vals
                .Select(v => JsonSerializer.Deserialize<ChatHistoryItem>(v))
                .Where(x => x != null)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToList();
            return list;
        }
    }

}
