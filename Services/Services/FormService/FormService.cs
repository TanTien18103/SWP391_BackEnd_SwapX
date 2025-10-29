using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Repositories.Repositories.FormRepo;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.StationScheduleRepo;
using Repositories.Repositories.VehicleRepo;
using Repositories.Repositories.EvDriverRepo;
using Repositories.Repositories.BatteryRepo;
using Services.ApiModels;
using Services.ApiModels.Form;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Repositories.ExchangeBatteryRepo;
using Repositories.Repositories.OrderRepo;

namespace Services.Services.FormService
{
    public class FormService : IFormService
    {
        private readonly IFormRepo _formRepo;
        private readonly AccountHelper _accountHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStationRepo _stationRepo;
        private readonly IStationScheduleRepo _stationScheduleRepo;
        private readonly IVehicleRepo _vehicleRepo;
        private readonly IEvDriverRepo _evDriverRepo;
        private readonly IBatteryRepo _batteryRepo;
        private readonly IExchangeBatteryRepo _exchangeBatteryRepo;
        private readonly IOrderRepository _orderRepository;

        public FormService(
            IFormRepo formRepo,
            AccountHelper accountHelper,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IStationRepo stationRepo,
            IStationScheduleRepo stationScheduleRepo,
            IVehicleRepo vehicleRepo,
            IEvDriverRepo evDriverRepo,
            IBatteryRepo batteryRepo,
            IExchangeBatteryRepo exchangeBatteryRepo,
            IOrderRepository orderRepository
            )
        {
            _formRepo = formRepo;
            _accountHelper = accountHelper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _stationRepo = stationRepo;
            _stationScheduleRepo = stationScheduleRepo;
            _vehicleRepo = vehicleRepo;
            _evDriverRepo = evDriverRepo;
            _batteryRepo = batteryRepo;
            _exchangeBatteryRepo = exchangeBatteryRepo;
            _orderRepository = orderRepository;
        }
        public async Task<ResultModel> AddForm(AddFormRequest addFormRequest)
        {
            try
            {
                if (addFormRequest.Date < TimeHepler.SystemTimeNow)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.INVALID_FORM_DATE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Validate giờ hành chính
                var requestDate = addFormRequest.Date.GetValueOrDefault();
                var requestTime = requestDate.TimeOfDay;

                var morningStart = new TimeSpan(7, 30, 0);
                var morningEnd = new TimeSpan(12, 0, 0);
                var afternoonStart = new TimeSpan(13, 30, 0);
                var afternoonEnd = new TimeSpan(17, 0, 0);

                bool isInWorkingHours =
                    (requestTime >= morningStart && requestTime <= morningEnd) ||
                    (requestTime >= afternoonStart && requestTime <= afternoonEnd);

                if (!isInWorkingHours)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.INVALID_FORM_TIME,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var station = await _stationRepo.GetStationById(addFormRequest.StationId);
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

                //Kiểm tra xe của tài khoản có phải của accountId không
                var evDriver = await _evDriverRepo.GetDriverByAccountId(addFormRequest.AccountId);
                if (evDriver == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
                        Data = null
                    };
                }
                var vehicle = await _vehicleRepo.GetVehicleById(addFormRequest.Vin);
                if (vehicle == null || vehicle.CustomerId != evDriver.CustomerId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                        Data = null
                    };
                }
                var battery = await _batteryRepo.GetBatteryById(addFormRequest.BatteryId);
                var batteryOfVehicle = await _batteryRepo.GetBatteryById(vehicle.BatteryId);
                if (battery == null || battery.StationId != station.StationId)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND,
                        Data = null
                    };
                }
                //Kiểm tra xem pin có tương thích với xe không
                if (battery.Specification != batteryOfVehicle.Specification || battery.BatteryType != batteryOfVehicle.BatteryType)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.INCOMPATIBLE_BATTERY_VEHICLE,
                        Data = null
                    };
                }
                //kiểm tra xem xe đó đã lên tạo form trước đó nhưng chưa được approve
                var existingForms = await _formRepo.GetFormsByVin(addFormRequest.Vin);
                if (existingForms.Any(f => f.Status == FormStatusEnums.Submitted.ToString()))
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.VEHICLE_ALREADY_HAS_PENDING_FORM,
                        Data = null
                    };
                }
                if (battery.Status == BatteryStatusEnums.Charging.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsBattery.BATTERY_CHARGING,
                        Data = null
                    };
                }
                //Kiểm tra xem pin được booked chưa
                if (battery.Status == BatteryStatusEnums.Booked.ToString())
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.BATTERY_ALREADY_BOOKED,
                        Data = null
                    };
                }
                if (evDriver == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsUser.EVDRIVER_NOT_FOUND,
                        Data = null
                    };
                }

                battery.Status = BatteryStatusEnums.Booked.ToString();
                battery.UpdateDate = TimeHepler.SystemTimeNow;
                await _batteryRepo.UpdateBattery(battery);

                var form = new Form
                {
                    FormId = _accountHelper.GenerateShortGuid(),
                    AccountId = addFormRequest.AccountId,
                    StationId = addFormRequest.StationId,
                    Title = addFormRequest.Title,
                    Description = addFormRequest.Description,
                    Date = addFormRequest.Date,
                    Status = FormStatusEnums.Submitted.ToString(),
                    Vin = addFormRequest.Vin,
                    BatteryId = addFormRequest.BatteryId,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };

                var addedForm = await _formRepo.Add(form);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.ADD_FORM_SUCCESS,
                    Data = addedForm,
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.ADD_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> GetAllForms()
        {
            try
            {
                var forms = await _formRepo.GetAll();
                if (forms == null || forms.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsStation.GET_ALL_STATION_SUCCESS,
                    Data = forms,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> GetFormById(string formId)
        {
            try
            {
                var form = await _formRepo.GetById(formId);
                if (form == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.GET_FORM_DETAIL_SUCCESS,
                    Data = form,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> GetFormsByAccountId(string accountId)
        {
            try
            {
                var forms = await _formRepo.GetByAccountId(accountId);
                if (forms == null || forms.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_SUCCESS,
                    Data = forms,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> GetFormsByStationId(string stationId)
        {
            try
            {
                var forms = await _formRepo.GetByStationId(stationId);
                if (forms == null || forms.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_SUCCESS,
                    Data = forms,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> UpdateForm(UpdateFormRequest updateFormRequest)
        {
            try
            {
                var existingForm = await _formRepo.GetById(updateFormRequest.FormId);
                if (existingForm == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (updateFormRequest.Date < TimeHepler.SystemTimeNow)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.INVALID_FORM_DATE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                existingForm.Title = updateFormRequest.Title;
                existingForm.Description = updateFormRequest.Description;
                existingForm.Date = updateFormRequest.Date;
                existingForm.Status = FormStatusEnums.Submitted.ToString();
                existingForm.UpdateDate = TimeHepler.SystemTimeNow;

                var updatedForm = await _formRepo.Update(existingForm);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.UPDATE_FORM_SUCCESS,
                    Data = updatedForm,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.UPDATE_FORM_FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }
        public async Task<ResultModel> DeleteForm(string formId)
        {
            try
            {
                var existingForm = await _formRepo.GetById(formId);
                if (existingForm == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                existingForm.Status = FormStatusEnums.Deleted.ToString();
                existingForm.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedForm = await _formRepo.Update(existingForm);
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.DELETE_FORM_SUCCESS,
                    StatusCode = StatusCodes.Status200OK
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.DELETE_FORM_FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> GetFormByIdDriver(string formId)
        {
            try
            {
                var form = await _formRepo.GetById(formId);
                if (form == null || form.Status == FormStatusEnums.Deleted.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.GET_FORM_DETAIL_SUCCESS,
                    Data = form,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }
        public async Task<ResultModel> GetAllFormsDriver()
        {
            try
            {
                var forms = await _formRepo.GetAll();
                if (forms == null || forms.Count == 0)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_LIST_EMPTY,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var filteredForms = forms.Where(f => f.Status != FormStatusEnums.Deleted.ToString()).ToList();
                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_SUCCESS,
                    Data = filteredForms,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.GET_ALL_FORM_FAIL,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }

        public async Task<ResultModel> UpdateFormStatusStaff(UpdateFormStatusStaffRequest updateFormStatusStaffRequest)
        {
            try
            {
                var existingForm = await _formRepo.GetById(updateFormStatusStaffRequest.FormId);
                if (existingForm == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsForm.FORM_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var station = await _stationRepo.GetStationById(existingForm.StationId);
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
                if (station.Status == StationStatusEnum.Inactive.ToString() || station.Status == StationStatusEnum.Maintenance.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsStation.STATION_INACTIVE_OR_MAINTENANCE,
                        Data = null,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                // Only allow status update from Submitted to Approved or Rejected
                if (existingForm.Status != FormStatusEnums.Submitted.ToString())
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.INVALID_FORM_STATUS_UPDATE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (updateFormStatusStaffRequest.Status != StaffUpdateFormEnums.Approved &&
                    updateFormStatusStaffRequest.Status != StaffUpdateFormEnums.Rejected)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.FAILED,
                        Message = ResponseMessageConstantsForm.INVALID_FORM_STATUS_VALUE,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var vehicle = await _vehicleRepo.GetVehicleById(existingForm.Vin);
                if (vehicle == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsVehicle.VEHICLE_NOT_FOUND,
                        Data = null,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
                var orders = await _orderRepository.GetOrdersByAccountId(existingForm.AccountId);
                var forms = await _formRepo.GetByAccountId(existingForm.AccountId);

                // Lấy danh sách formId tại station hiện tại
                var formIdsAtStation = forms
                    .Where(f => f.StationId == existingForm.StationId)
                    .Select(f => f.FormId)
                    .ToList();

                // Đếm số đơn hàng trả trước và đã thanh toán có form nằm trong danh sách formIdsAtStation
                var paidOrdersAtStationCount = orders.Count(o =>
                    o.ServiceType == PaymentType.PrePaid.ToString() &&
                    o.Status == PaymentStatus.Paid.ToString() &&
                    formIdsAtStation.Contains(o.ServiceId));

                // If approved, set start date and create StationSchedule
                if (updateFormStatusStaffRequest.Status == StaffUpdateFormEnums.Approved)
                {
                    // Create StationSchedule logic here
                    var stationSchedule = new StationSchedule
                    {
                        StationScheduleId = _accountHelper.GenerateShortGuid(),
                        FormId = existingForm.FormId,
                        StationId = existingForm.StationId,
                        Date = existingForm.Date,
                        Description = existingForm.Description,
                        Status = ScheduleStatusEnums.Pending.ToString(),
                        StartDate = TimeHepler.SystemTimeNow,
                        UpdateDate = TimeHepler.SystemTimeNow,
                    };

                    var newstationSchedule = await _stationScheduleRepo.AddStationSchedule(stationSchedule);
                    if (newstationSchedule == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsForm.CREATE_STATION_SCHEDULE_FAILED,
                            StatusCode = StatusCodes.Status500InternalServerError
                        };
                    }
                    var order = await _orderRepository.GetOrderByServiceId(existingForm.FormId);

                    if (paidOrdersAtStationCount >= 3)
                    {
                        if (order != null && (order.Status == PaymentStatus.Paid.ToString() && order.ServiceType == PaymentType.PrePaid.ToString()))
                        {
                            // Create ExchangeBattery record
                            var exchangeBattery = new ExchangeBattery
                            {
                                ExchangeBatteryId = _accountHelper.GenerateShortGuid(),
                                Vin = existingForm.Vin,
                                OldBatteryId = vehicle.BatteryId,
                                NewBatteryId = existingForm.BatteryId,
                                StaffAccountId = null,
                                ScheduleId = newstationSchedule.StationScheduleId,
                                OrderId = order.OrderId,
                                StationId = existingForm.StationId,
                                Status = ExchangeStatusEnums.Pending.ToString(),
                                StartDate = TimeHepler.SystemTimeNow,
                                UpdateDate = TimeHepler.SystemTimeNow,
                            };
                            await _exchangeBatteryRepo.Add(exchangeBattery);
                        }
                        else
                        {
                            return new ResultModel
                            {
                                IsSuccess = false,
                                ResponseCode = ResponseCodeConstants.FAILED,
                                Message = ResponseMessageConstantsForm.ORDER_NOT_PAID,
                                StatusCode = StatusCodes.Status400BadRequest
                            };
                        }
                    }

                    //trường hợp khách thanh toán tại trạm
                    if (order == null)
                    {
                        // Create ExchangeBattery record
                        var exchangeBattery = new ExchangeBattery
                        {
                            ExchangeBatteryId = _accountHelper.GenerateShortGuid(),
                            Vin = existingForm.Vin,
                            OldBatteryId = vehicle.BatteryId,
                            NewBatteryId = existingForm.BatteryId,
                            StaffAccountId = null,
                            ScheduleId = newstationSchedule.StationScheduleId,
                            OrderId = null,
                            StationId = existingForm.StationId,
                            Status = ExchangeStatusEnums.Pending.ToString(),
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow,
                        };
                        await _exchangeBatteryRepo.Add(exchangeBattery);
                    }
                    //trường hợp khách đã thanh toán trước hoặc dùng gói
                    else if (
                        (order.ServiceType == PaymentType.PrePaid.ToString() ||
                         order.ServiceType == PaymentType.UsePackage.ToString())
                        && order.Status == PaymentStatus.Paid.ToString()
                    )
                    {
                        // Create ExchangeBattery record
                        var exchangeBattery = new ExchangeBattery
                        {
                            ExchangeBatteryId = _accountHelper.GenerateShortGuid(),
                            Vin = existingForm.Vin,
                            OldBatteryId = vehicle.BatteryId,
                            NewBatteryId = existingForm.BatteryId,
                            StaffAccountId = null,
                            ScheduleId = newstationSchedule.StationScheduleId,
                            OrderId = order.OrderId,
                            StationId = existingForm.StationId,
                            Status = ExchangeStatusEnums.Pending.ToString(),
                            StartDate = TimeHepler.SystemTimeNow,
                            UpdateDate = TimeHepler.SystemTimeNow,
                        };
                        await _exchangeBatteryRepo.Add(exchangeBattery);
                    }
                    else
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsForm.ORDER_NOT_PAID_OR_INVALID_SERVICE_TYPE,
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                }
                else if (updateFormStatusStaffRequest.Status == StaffUpdateFormEnums.Rejected)
                {
                    var battery = await _batteryRepo.GetBatteryById(existingForm.BatteryId);
                    if (battery != null && battery.Status == BatteryStatusEnums.Booked.ToString())
                    {
                        battery.Status = BatteryStatusEnums.Available.ToString();
                        battery.UpdateDate = TimeHepler.SystemTimeNow;
                        await _batteryRepo.UpdateBattery(battery);
                    }
                    else
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            ResponseCode = ResponseCodeConstants.FAILED,
                            Message = ResponseMessageConstantsBattery.BATTERY_NOT_FOUND_OR_NOT_BOOKED,
                            Data = null,
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                }

                existingForm.Status = updateFormStatusStaffRequest.Status.ToString();
                existingForm.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedForm = await _formRepo.Update(existingForm);

                return new ResultModel
                {
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsForm.UPDATE_FORM_STATUS_SUCCESS,
                    Data = updatedForm,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsForm.UPDATE_FORM_STATUS_FAILED,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = ex.InnerException?.Message,
                };
            }
        }
    }
}
