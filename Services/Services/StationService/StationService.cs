using AutoMapper;
using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.AccountRepo;
using Repositories.Repositories.BssStaffRepo;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.StationScheduleRepo;
using Services.ApiModels;
using Services.ApiModels.Station;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.Services.StationService
{
    public class StationService : IStationService
    {
        private readonly IStationRepo _stationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBssStaffRepo _bssStaffRepository;
        private readonly IAccountRepo _accountRepository;
        private readonly IStationScheduleRepo _stationScheduleRepository;
        private readonly AccountHelper _accountHelper;

        public StationService(IStationRepo stationRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBssStaffRepo bssStaffRepository, IAccountRepo accountRepository, AccountHelper accountHelper,IStationScheduleRepo stationScheduleRepo)
        {
            _stationRepository = stationRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _bssStaffRepository = bssStaffRepository;
            _accountRepository = accountRepository;
            _accountHelper = accountHelper;
            _stationScheduleRepository = stationScheduleRepo;
        }

        public async Task<ResultModel> AddStation(AddStationRequest addStationRequest)
        {
            try
            {
                var station = new Station
                {
                    StationId = _accountHelper.GenerateShortGuid(),
                    StationName = addStationRequest.Name,
                    Location = addStationRequest.Location,
                    Status = StationStatusEnum.Active.ToString(),
                    Rating = 0m,
                    BatteryNumber = 0, 
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow,
                };

                await _stationRepository.AddStation(station);
                var createdStation = await _stationRepository.GetStationById(station.StationId);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.ADD_STATION_SUCCESS,
                    Data = createdStation,
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.ADD_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }
        public async Task<ResultModel> GetAllStations()
        {
            try
            {
                var stations = await _stationRepository.GetAllStations();
                if (stations == null || stations.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Map sang object mới, ẩn trường station trong từng battery
                var response = stations.Select(station => new
                {
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Location = station.Location,
                    Status = station.Status,
                    Rating = station.Rating,
                    BatteryNumber = station.BatteryNumber,
                    StartDate = station.StartDate,
                    UpdateDate = station.UpdateDate,
                    BssStaffs = station.BssStaffs.Select(s => new
                    {
                        StaffId= s.StaffId,
                    }).ToList(),
                    Batteries = station.Batteries.Select(b => new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate
                        // KHÔNG có trường station ở đây!
                    }).ToList()
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationById(string stationId)
        {
            try
            {
                var station = await _stationRepository.GetStationById(stationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Map sang object mới, ẩn trường station trong từng battery
                var response = new
                {
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Location = station.Location,
                    Status = station.Status,
                    Rating =station.Rating,
                    BatteryNumber = station.BatteryNumber,
                    StartDate = station.StartDate,
                    UpdateDate = station.UpdateDate,
                    BssStaffs = station.BssStaffs.Select(s => new
                    {
                        StaffId = s.StaffId,
                    }).ToList(),
                    Batteries = station.Batteries.Select(b => new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        StartDate = b.StartDate,
                        UpdateDate = b.UpdateDate
                        // KHÔNG có trường station ở đây!
                    }).ToList()
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateStation(UpdateStationRequest updateStationRequest)
        {
            try
            {
                var existingStation = await _stationRepository.GetStationById(updateStationRequest.StationId);
                if (existingStation == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                if (!string.IsNullOrEmpty(updateStationRequest.Location))
                {
                    existingStation.Location = updateStationRequest.Location;
                }
                if (!string.IsNullOrEmpty(updateStationRequest.Name))
                {
                    existingStation.StationName = updateStationRequest.Name;
                }
                existingStation.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedStation = await _stationRepository.UpdateStation(existingStation);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_SUCCESS,
                    Data = updatedStation,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }
        public async Task<ResultModel> DeleteStation(string stationId)
        {
            try
            {
                var existingStation = await _stationRepository.GetStationById(stationId);
                if (existingStation == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                existingStation.Status = StationStatusEnum.Inactive.ToString();
                existingStation.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedStation = await _stationRepository.UpdateStation(existingStation);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.DELETE_STATION_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.DELETE_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> AddStaffToStation(AddStaffToStationRequest addStaffToStationRequest)
        {
            try
            {
                var station = await _stationRepository.GetStationById(addStaffToStationRequest.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var account = await _accountRepository.GetAccountByStaffId(addStaffToStationRequest.StaffId);
                if (account == null || account.Role != RoleEnums.Bsstaff.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var staff = await _bssStaffRepository.GetBssStaffById(addStaffToStationRequest.StaffId);

                // Kiểm tra nếu nhân viên đã được gán cho một trạm khác
                if (staff.StationId != null && staff.StationId != addStaffToStationRequest.StationId)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.CONFLICT,
                        Message = ResponseMessageConstantsStation.STAFF_ALREADY_ASSIGNED_TO_ANOTHER_STATION,
                        Data = null,
                        StatusCode = StatusCodes.Status409Conflict
                    };
                }

                if (staff.StationId == addStaffToStationRequest.StationId)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.CONFLICT,
                        Message = ResponseMessageConstantsStation.STAFF_ALREADY_ASSIGNED_TO_THIS_STATION,
                        Data = null,
                        StatusCode = StatusCodes.Status409Conflict
                    };
                }

                // Gán nhân viên cho trạm
                staff.StationId = station.StationId;
                staff.UpdateDate = TimeHepler.SystemTimeNow;
                await _bssStaffRepository.UpdateBssStaff(staff);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.ADD_STAFF_TO_STATION_SUCCESS,
                    Data = station,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.ADD_STAFF_TO_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStaffsByStationId(string stationId)
        {
            try
            {
                var station = await _stationRepository.GetStationById(stationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var staffs = await _bssStaffRepository.GetstaffsByStationId(stationId);
                if (staffs == null || staffs.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STAFF_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_STAFFS_BY_STATION_SUCCESS,
                    Data = staffs,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_STAFFS_BY_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> RemoveStaffFromStation(string stationId, string staffId)
        {
            try
            {
                var station = await _stationRepository.GetStationById(stationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var staff = await _bssStaffRepository.GetBssStaffById(staffId);
                if (staff == null || staff.StationId != stationId)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STAFF_NOT_FOUND_IN_STATION,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Gỡ bỏ nhân viên khỏi trạm
                staff.StationId = null;
                staff.UpdateDate = TimeHepler.SystemTimeNow;
                await _bssStaffRepository.UpdateBssStaff(staff);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.REMOVE_STAFF_FROM_STATION_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.REMOVE_STAFF_FROM_STATION_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetStationByStaffId(string staffId)
        {
            try
            {
                var staff = await _bssStaffRepository.GetBssStaffById(staffId);
                if (staff == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.USER_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var station = await _stationRepository.GetStationById(staff.StationId);
                if (station == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STAFF_NOT_ASSIGNED_TO_ANY_STATION,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_STATION_BY_STAFF_SUCCESS,
                    Data = station,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_STATION_BY_STAFF_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateStationStatus(UpdateStationStatusRequest updateStationStatusRequest)
        {
            try
            {
                var existingStation = await _stationRepository.GetStationById(updateStationStatusRequest.StationId);
                if (existingStation == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if(existingStation.Status == updateStationStatusRequest.Status.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.CONFLICT,
                        Message = ResponseMessageConstantsStation.STATION_ALREADY_IN_THIS_STATUS,
                        Data = null,
                        StatusCode = StatusCodes.Status409Conflict
                    };
                }

                if (updateStationStatusRequest.Status == StationStatusEnum.Inactive
                    || updateStationStatusRequest.Status == StationStatusEnum.Maintenance)
                {
                    var today = TimeHepler.SystemTimeNow.Date;

                    var schedules = await _stationScheduleRepository.GetStationSchedulesByStationId(existingStation.StationId);

                    // Chỉ chặn nếu có lịch hôm nay mà chưa bị hủy
                    bool hasTodaySchedule = schedules.Any(s =>
                        s.Date.HasValue &&
                        s.Date.Value.Date == today &&
                        !string.Equals(s.Status, ScheduleStatusEnums.Cancelled.ToString(), StringComparison.OrdinalIgnoreCase)
                    );

                    if (hasTodaySchedule)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.CONFLICT,
                            Message = ResponseMessageConstantsStation.CANNOT_CHANGE_STATUS_DUE_TO_TODAY_SCHEDULE,
                            Data = null,
                            StatusCode = StatusCodes.Status409Conflict
                        };
                    }
                }

                if (!string.IsNullOrEmpty(updateStationStatusRequest.Status.ToString()) &&
                    Enum.TryParse<StationStatusEnum>(updateStationStatusRequest.Status.ToString(), out var statusEnum))
                {
                    existingStation.Status = statusEnum.ToString();
                }
                else
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.BAD_REQUEST,
                        Message = ResponseMessageConstantsStation.INVALID_STATION_STATUS,
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                existingStation.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedStation = await _stationRepository.UpdateStation(existingStation);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_STATUS_SUCCESS,
                    Data = updatedStation,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.UPDATE_STATION_STATUS_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetAllStationOfCustomer()
        {
            try
            {

                var stations = await _stationRepository.GetAllStationsOfCustomer();
                if (stations == null || stations.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsStation.STATION_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }


                // Map sang object mới, ẩn trường station trong từng battery
                var response = stations.Select(station => new
                {
                    StationId = station.StationId,
                    StationName = station.StationName,
                    Location = station.Location,
                    Status = station.Status,
                    Rating = station.Rating,
                    BatteryNumber = station.BatteryNumber,
                    BssStaffs = station.BssStaffs.Select(s => new
                    {
                        StaffId = s.StaffId,
                    }).ToList(),
                    Batteries = station.Batteries.Select(b => new
                    {
                        BatteryId = b.BatteryId,
                        BatteryName = b.BatteryName,
                        Status = b.Status,
                        Capacity = b.Capacity,
                        BatteryType = b.BatteryType,
                        Specification = b.Specification,
                        BatteryQuality = b.BatteryQuality,
                        // KHÔNG có trường station ở đây!
                    }).ToList()
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_SUCCESS,
                    Data = response,
                    StatusCode = StatusCodes.Status200OK
                };


            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_FAIL,
                    Data = ex.Message
                };
            }
        }
    }
}
