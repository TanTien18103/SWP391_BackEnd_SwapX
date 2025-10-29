using BusinessObjects.Constants;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.StationScheduleRepo;
using Services.ApiModels;
using Services.ApiModels.StationSchedule;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Repositories.FormRepo;
using BusinessObjects.TimeCoreHelper;

namespace Services.Services.StationScheduleService
{
    public class StationScheduleService : IStationScheduleService
    {
        private readonly IStationScheduleRepo _stationScheduleRepo;
        private readonly IStationRepo _stationRepo;
        private readonly IFormRepo _formRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        public StationScheduleService(IStationScheduleRepo stationScheduleRepo, IStationRepo stationRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper, IFormRepo formRepo)
        {
            _stationScheduleRepo = stationScheduleRepo;
            _stationRepo = stationRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
            _formRepo = formRepo;
        }

        public async Task<ResultModel> AddStationSchedule(AddStationScheduleRequest addStationScheduleRequest)
        {
            try
            {

                var station = await _stationRepo.GetStationById(addStationScheduleRequest.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }
                var form = await _formRepo.GetById(addStationScheduleRequest.FormId);
                if (form == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null
                    };
                }
                var stationSchedule = new BusinessObjects.Models.StationSchedule
                {
                    StationScheduleId = _accountHelper.GenerateShortGuid(),
                    StationId = addStationScheduleRequest.StationId,
                    Date = addStationScheduleRequest.Date,
                    FormId = addStationScheduleRequest.FormId,
                    Description = addStationScheduleRequest.Description,
                    Status = StationScheduleStatusEnums.Pending.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                await _stationScheduleRepo.AddStationSchedule(stationSchedule);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.ADD_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedule
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseMessageConstantsStationSchedule.ADD_STATION_SCHEDULE_FAIL,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResultModel> DeleteStationSchedule(string stationScheduleId)
        {
            try
            {

                var stationSchedule = await _stationScheduleRepo.GetStationScheduleById(stationScheduleId);
                if (stationSchedule == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }
                stationSchedule.Status = StationScheduleStatusEnums.Deleted.ToString();
                stationSchedule.UpdateDate = TimeHepler.SystemTimeNow;
                await _stationScheduleRepo.UpdateStationSchedule(stationSchedule);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.DELETE_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedule
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.DELETE_STATION_SCHEDULE_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetAllStationSchedules()
        {
            try
            {
                var stationSchedules = await _stationScheduleRepo.GetAllStationSchedules();
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.GET_ALL_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedules
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.GET_ALL_STATION_SCHEDULE_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationScheduleById(string stationScheduleId)
        {
            try
            {
                var stationSchedule = await _stationScheduleRepo.GetStationScheduleById(stationScheduleId);
                if (stationSchedule == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedule
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_FAIL,
                    Data = ex.Message
                };
            }
        }


        public async Task<ResultModel> UpdateStationSchedule(UpdateStationScheduleRequest updateStationScheduleRequest)
        {
            try
            {
                var stationSchedule = await _stationScheduleRepo.GetStationScheduleById(updateStationScheduleRequest.StationScheduleId);
                if (stationSchedule == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }
                if (stationSchedule.Status == StationScheduleStatusEnums.Deleted.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_INACTIVE,
                        Data = null
                    };
                }
                if (updateStationScheduleRequest.Date != null)
                {
                    stationSchedule.Date = updateStationScheduleRequest.Date;
                }
                if (updateStationScheduleRequest.Description != null)
                {
                    stationSchedule.Description = updateStationScheduleRequest.Description;
                }
                if (updateStationScheduleRequest.FormId != null)
                {
                    var form = await _formRepo.GetById(updateStationScheduleRequest.FormId);
                    if (form == null)
                    {
                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                            Data = null
                        };
                    }
                    else
                    {
                        stationSchedule.FormId = updateStationScheduleRequest.FormId;
                    }
                }
                if (updateStationScheduleRequest.StationId != null)
                {
                    var station = await _stationRepo.GetStationById(updateStationScheduleRequest.StationId);
                    if (station == null)
                    {
                        return new ResultModel
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.NOT_FOUND,
                            Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                            Data = null
                        };
                    }
                    else
                    {
                        stationSchedule.StationId = updateStationScheduleRequest.StationId;
                    }
                }
                stationSchedule.UpdateDate = TimeHepler.SystemTimeNow;
                await _stationScheduleRepo.UpdateStationSchedule(stationSchedule);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.UPDATE_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedule
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.UPDATE_STATION_SCHEDULE_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationScheduleByStationId(string stationId)
        {
            try
            {
                var stationSchedules = await _stationScheduleRepo.GetStationSchedulesByStationId(stationId);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_BY_STATION_ID_SUCCESS,
                    Data = stationSchedules
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_BY_STATION_ID_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateStatusStationSchedule(UpdateStatusStationScheduleRequest updateStatusStationScheduleRequest)
        {
            try
            {
                var stationSchedule = await _stationScheduleRepo.GetStationScheduleById(updateStatusStationScheduleRequest.StationScheduleId);
                if (stationSchedule == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }
                var form = await _formRepo.GetById(stationSchedule.FormId);
                if (form.Status != FormStatusEnums.Approved.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStationSchedule.FORM_NOT_APPROVED,
                        Data = null
                    };
                }
                if (updateStatusStationScheduleRequest.Status == StationScheduleStatusEnums.Completed &&
                    stationSchedule.Date.HasValue &&
                    stationSchedule.Date!.Value.Date > TimeHepler.SystemTimeNow.Date)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStationSchedule.CANNOT_COMPLETE_BEFORE_DATE,
                        Data = null
                    };
                }

                if (stationSchedule.Status == StationScheduleStatusEnums.Completed.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_ALREADY_COMPLETED,
                        Data = null
                    };
                }
                if (stationSchedule.Status == StationScheduleStatusEnums.Deleted.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStationSchedule.CANNOT_UPDATE_STATUS_FROM_INACTIVE,
                        Data = null
                    };
                }
                stationSchedule.Status = updateStatusStationScheduleRequest.Status.ToString();
                stationSchedule.UpdateDate = TimeHepler.SystemTimeNow;
                await _stationScheduleRepo.UpdateStationSchedule(stationSchedule);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.UPDATE_STATUS_STATION_SCHEDULE_SUCCESS,
                    Data = stationSchedule
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.UPDATE_STATUS_STATION_SCHEDULE_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationSchedulesByAccountId(string accountId)
        {
            try
            {
                var stationSchedules = await _stationScheduleRepo.GetStationSchedulesByAccountId(accountId);
                if (stationSchedules == null || !stationSchedules.Any())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStationSchedule.STATION_SCHEDULE_NOT_FOUND,
                        Data = null
                    };
                }

                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_BY_ACCOUNT_ID_SUCCESS,
                    Data = stationSchedules
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStationSchedule.GET_STATION_SCHEDULE_BY_ACCOUNT_ID_FAILED,
                    Data = ex.Message
                };
            }
        }
    }
}
