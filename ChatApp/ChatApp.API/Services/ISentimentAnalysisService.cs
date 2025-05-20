using System.Threading.Tasks;

namespace ChatApp.Services
{
    public interface ISentimentAnalysisService
    {
        Task<float> AnalyzeSentimentAsync(string text);
    }
}