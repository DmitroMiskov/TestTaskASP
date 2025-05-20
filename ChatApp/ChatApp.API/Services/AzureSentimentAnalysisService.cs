using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class AzureSentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly TextAnalyticsClient _client;
        private readonly ILogger<AzureSentimentAnalysisService> _logger;

        public AzureSentimentAnalysisService(string endpoint, string apiKey,
        ILogger<AzureSentimentAnalysisService> logger = null)
        {
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentException(nameof(endpoint));
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentException(nameof(apiKey));

            _logger = logger;
            var credentials = new AzureKeyCredential(apiKey);
            _client = new TextAnalyticsClient(new Uri(endpoint), credentials);
        }

        public async Task<float> AnalyzeSentimentAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _logger?.LogWarning("Empty text provided for sentime analysis");
                return 0.5f;
            }

            try
            {
                DocumentSentiment result = await _client.AnalyzeSentimentAsync(text);
                _logger?.LogInformation("Sentime analyzed: {Sentiment}, Scores: P={Positive}, N={Negative}, Nu={Neutral}",
                result.Sentiment,
                result.ConfidenceScores.Positive,
                result.ConfidenceScores.Negative,
                result.ConfidenceScores.Neutral);

                return (float)result.ConfidenceScores.Positive;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error analyzing setiment");
                return 0.5f;
            }
        }
    }
}