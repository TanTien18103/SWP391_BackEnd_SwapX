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
using BusinessObjects.Models;
using Services.Services.EmailService;

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
        private readonly IEmailService _emailService;
        public ReportService(
            IReportRepo reportRepo,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            AccountHelper accountHelper,
            IAccountRepo accountRepo,
            IStationRepo stationRepo,
            IEmailService emailService
            )
        {
            _reportReport = reportRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _accountRepo = accountRepo;
            _stationRepo = stationRepo;
            _emailService = emailService;
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
                if (reports == null || reports.Count == 0)
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

        public async Task<ResultModel> GetReportsByStationId(string stationId)
        {
            try
            {

                var reports = await _reportReport.GetReportsByStationId(stationId);
                if (reports == null || reports.Count == 0)
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
                    Message = ResponseMessageConstantsReport.GET_REPORTS_BY_STATION_SUCCESS,
                    Data = reports
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsReport.GET_REPORTS_BY_STATION_FAIL,
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

        public async Task<ResultModel> UpdateReportStatus(UpdateReportStatusRequest updateReportStatusRequest)
        {
            try
            {
                var report = await _reportReport.GetReportById(updateReportStatusRequest.ReportId);
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
                var station = await _stationRepo.GetStationById(report.StationId);
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

                if (report.Status == updateReportStatusRequest.Status.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_STATUS_SAME,
                        Data = null
                    };
                }

                if (updateReportStatusRequest.Status != ReportStatusEnums.Completed &&
                    updateReportStatusRequest.Status != ReportStatusEnums.Cancelled)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_INVALID_UPDATE_STATUS,
                        Data = null
                    };
                }

                if (report.Status == ReportStatusEnums.Completed.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.REPORT_ALREADY_COMPLETED,
                        Data = null
                    };
                }

                report.Status = updateReportStatusRequest.Status.ToString();
                report.UpdateDate = TimeHepler.SystemTimeNow;
                await _reportReport.UpdateReport(report);

                var account = await _accountRepo.GetAccountById(report.AccountId);
                if (!string.IsNullOrEmpty(account.Email))
                {
                    // Gửi mail khi cập nhật trạng thái báo cáo
                    var subject = EmailConstants.REPORT_STATUS_UPDATE_SUBJECT;
                    var body = string.Format(
                        EmailConstants.REPORT_STATUS_UPDATE_BODY,
                        account.Name,
                        report.Name,
                        station.StationName,
                        report.Status,
                        report.Description
                    );
                    await _emailService.SendEmail(account.Email, subject, body);
                }else
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsReport.ACCOUNT_EMAIL_NOT_FOUND,
                        Data = null
                    };
                }

                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsReport.UPDATE_REPORT_STATUS_SUCCESS,
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
                    Message = ResponseMessageConstantsReport.UPDATE_REPORT_STATUS_FAILED,
                    Data = null
                };
            }
        }
    }
}
