using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _dbContext;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        private readonly bool _sentimentAnalysisEnabled;

        public ChatHub(
            ChatDbContext dbContext, 
            ISentimentAnalysisService sentimentAnalysisService,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _sentimentAnalysisService = sentimentAnalysisService;
            _sentimentAnalysisEnabled = configuration.GetValue<bool>("EnableSentimentAnalysis");
        }

        public async Task GetMessageHistory()
        {
            var messages = await _dbContext.Messages
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);
        }

        public async Task SendMessage(string username, string message)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(message))
                return;

            var newMessage = new Message
            {
                Username = username,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            // Аналіз тональності, якщо увімкнено
            if (_sentimentAnalysisEnabled)
            {
                try
                {
                    newMessage.SentimentScore = await _sentimentAnalysisService.AnalyzeSentimentAsync(message);
                    Console.WriteLine($"Sentiment analysis: {newMessage.SentimentScore}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing sentiment: {ex.Message}");
                }
            }

            // Зберігаємо повідомлення в базі даних
            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Виведіть повідомлення для діагностики
            Console.WriteLine($"Sending message from {username}: {message}");

            // Надсилаємо повідомлення всім підключеним клієнтам
            await Clients.All.SendAsync("ReceiveMessage", newMessage);
        }
    }
}