using Microsoft.Extensions.Configuration;
using BusinessObjects.Constants;
using Google.Apis.Core;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Mscc.GenerativeAI;
using Repositories.Repositories.BatteryHistoryRepo;
using Services.ApiModels;
using Services.ApiModels.GeminiAI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Services.Services.GeminiService
{
    public class GeminiService : IGeminiService
    {
        private readonly GenerativeModel _model;
        private readonly IBatteryHistoryRepo _batteryHistoryRepo;
        public GeminiService(IBatteryHistoryRepo batteryHistoryRepo, IConfiguration configuration)
        {
            _batteryHistoryRepo = batteryHistoryRepo;
             var apiKey = configuration["Gemini:ApiKey"];
            var ai = new GoogleAI(apiKey);
            _model = ai.GenerativeModel(Model.Gemini20Flash);
        }
        public async Task<ResultModel> GetGeminiData(GeminiAIRequest geminiAIRequest)
        {
            try
            {
                
                var response = await _model.GenerateContent(geminiAIRequest.request);
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = ResponseCodeConstants.AI_SERVICE_SUCCESS,
                    Data = response.Text,
                    StatusCode = StatusCodes.Status200OK
                };  
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = ResponseCodeConstants.AI_SERVICE_ERROR,
                    Data = null,
                    StatusCode = 500
                };
            }
        }
        public async Task<ResultModel> AnalyzeBatteryDataAsync()
        {
            try
            {
                var batteryHistory= await _batteryHistoryRepo.GetAllBatteryHistories();
                var jsonData = JsonSerializer.Serialize(batteryHistory, new JsonSerializerOptions
                {
                    WriteIndented = true // format đẹp cho AI dễ đọc
                });
                var prompt = $"""
            Đây là dữ liệu pin trong hệ thống SwapX:
            {jsonData}

            Hãy giúp tôi phân tích:
            1. Trạm nào có lượt đổi pin cao nhất.
            2. Mức năng lượng trung bình khi đổi pin.
            3. Gợi ý cách tối ưu hoạt động trạm.
            """;
                var response = await _model.GenerateContent(prompt);
                return new ResultModel
                {
                    IsSuccess = true,
                    Message = ResponseCodeConstants.AI_SERVICE_SUCCESS,
                    Data = response.Text,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex) { 
             
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = ResponseCodeConstants.AI_SERVICE_ERROR,
                    Data = null,
                    StatusCode = 500
                };
            }
        }

    }
}
