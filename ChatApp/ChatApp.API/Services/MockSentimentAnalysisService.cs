using System;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class MockSentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly Random _random = new Random();
        public Task<float> AnalyzeSentimentAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Task.FromResult(0.5f);
            }

            string lowerText = text.ToLower();
            if (lowerText.Contains("чудов") || lowerText.Contains("гарн") ||
            lowerText.Contains("добр") || lowerText.Contains("супер") ||
            lowerText.Contains("прекрасн"))
            {
                return Task.FromResult((float)_random.NextDouble() * 0.2f);
            }

            return Task.FromResult(0.3f + (float)_random.NextDouble() * 0.4f);
        }
    }
}