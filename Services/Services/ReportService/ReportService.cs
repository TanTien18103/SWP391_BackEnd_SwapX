using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.StationRepo;
using Services.ApiModels;
using Services.ApiModels.Report;
using Repositories.Repositories.ReportRepo;
using BusinessObjects.Enums;
using BusinessObjects.Constants;
using BusinessObjects.TimeCoreHelper;

namespace Services.Services.ReportService
{
    public class ReportService : IReportService
    {
        private readonly IReportRepo _reportReport;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        private readonly IAccountRepo _accountRepo;
        private readonly IStationRepo _stationRepo;
        public ReportService(IReportRepo reportRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IAccountRepo accountRepo, IStationRepo stationRepo)
        {
            _reportReport = reportRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _accountRepo = accountRepo;
            _stationRepo = stationRepo;
        }

        public async Task<ResultModel> AddReport(AddReportRequest addReportRequest)
        {
            try
            {

                var account = await _accountRepo.GetAccountById(addReportRequest.AccountId);
                if (account == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        Data = null
                    };
                }
                var station = await _stationRepo.GetStationById(addReportRequest.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null
                    };
                }
                var report = new BusinessObjects.Models.Report
                {
                    ReportId = _accountHelper.GenerateShortGuid(),
                    AccountId = addReportRequest.AccountId,
                    StationId = addReportRequest.StationId,
                    Name = addReportRequest.Name,
                    Image = addReportRequest.Image,
                    Description = addReportRequest.Description,
                    Status = ReportStatusEnums.Pending.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                await _reportReport.AddReport(report);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.ADD_REPORT_SUCCESS,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.ADD_REPORT_FAIL,
                    Data = null
                };
            }
        }


        public async Task<ResultModel> DeleteReport(string reportId)
        {
            try
            {

                var report = await _reportReport.GetReportById(reportId);
                if (report == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                report.Status = ReportStatusEnums.Inactive.ToString();
                report.UpdateDate = TimeHepler.SystemTimeNow;
                await _reportReport.UpdateReport(report);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.DELETE_REPORT_SUCCESS,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.DELETE_REPORT_FAILED,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetAllReports()
        {
            try
            {
                var reports = await _reportReport.GetAllReports();
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.GET_ALL_REPORT_SUCCESS,
                    Data = reports
                };
                if(reports == null || reports.Count == 0)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_NOT_FOUND,
                        Data = null
                    };
                }

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.GET_ALL_REPORT_FAIL,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> GetReportById(string reportId)
        {
            try
            {

                var report = await _reportReport.GetReportById(reportId);
                if (report == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.GET_REPORT_SUCCESS,
                    Data = report
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.GET_REPORT_FAIL,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> UpdateReport(UpdateReportRequest updateReportRequest)
        {
            try
            {
                var report = await _reportReport.GetReportById(updateReportRequest.ReportId);
                if (report == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_NOT_FOUND,
                        Data = null
                    };
                }
                report.Name = updateReportRequest.Name;
                report.Image = updateReportRequest.Image;
                report.Description = updateReportRequest.Description;
                report.UpdateDate = TimeHepler.SystemTimeNow;
                await _reportReport.UpdateReport(report);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.UPDATE_REPORT_SUCCESS,
                    Data = report
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.UPDATE_REPORT_FAILED,
                    Data = null
                };
            }
        }
    }
}
