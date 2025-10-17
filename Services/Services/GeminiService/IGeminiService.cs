using Services.ApiModels;
using Services.ApiModels.GeminiAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.GeminiService
{
    public interface IGeminiService
    {
        Task<ResultModel> GetGeminiData(GeminiAIRequest geminiAIRequest);
        Task<ResultModel> AnalyzeBatteryDataAsync();
    }
}
