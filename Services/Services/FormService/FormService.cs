using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.FormRepo;
using Repositories.Repositories.StationRepo;
using Repositories.Repositories.StationScheduleRepo;
using Services.ApiModels;
using Services.ApiModels.Form;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public FormService(IFormRepo formRepo, AccountHelper accountHelper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IStationRepo stationRepo, IStationScheduleRepo stationScheduleRepo)
        {
            _formRepo = formRepo;
            _accountHelper = accountHelper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _stationRepo = stationRepo;
            _stationScheduleRepo = stationScheduleRepo;
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

                // Kiểm tra số form đã trả trước ở station này
                var paidForms = await _formRepo.GetFormsByAccountAndStation(addFormRequest.AccountId, addFormRequest.StationId);
                int prepaidCount = paidForms.Count(f => f.Status == FormStatusEnums.SubmittedPaidFirst.ToString());

                // Nếu >= 3 lần thì approve luôn
                string formStatus = prepaidCount >= 3
                    ? FormStatusEnums.Approved.ToString()
                    : FormStatusEnums.Submitted.ToString();

                var form = new Form
                {
                    FormId = _accountHelper.GenerateShortGuid(),
                    AccountId = addFormRequest.AccountId,
                    StationId = addFormRequest.StationId,
                    Title = addFormRequest.Title,
                    Description = addFormRequest.Description,
                    Date = addFormRequest.Date,
                    Status = formStatus,
                    StartDate = TimeHepler.SystemTimeNow,
                    UpdateDate = TimeHepler.SystemTimeNow
                };

                var addedForm = await _formRepo.Add(form);

                // Create StationSchedule if approved
                if (formStatus == FormStatusEnums.Approved.ToString())
                {
                    var stationSchedule = new StationSchedule
                    {
                        StationScheduleId = _accountHelper.GenerateShortGuid(),
                        FormId = addedForm.FormId,
                        StationId = addedForm.StationId,
                        Date = addedForm.Date,
                        StartDate = addedForm.StartDate,
                        Status = ScheduleStatusEnums.Pending.ToString(),
                        UpdateDate = TimeHepler.SystemTimeNow
                    };

                    await _stationScheduleRepo.AddStationSchedule(stationSchedule);
                }

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

                existingForm.Status = updateFormStatusStaffRequest.Status.ToString();
                existingForm.UpdateDate = TimeHepler.SystemTimeNow;

                // If approved, set start date and create StationSchedule
                if (updateFormStatusStaffRequest.Status == StaffUpdateFormEnums.Approved)
                {
                    existingForm.StartDate = TimeHepler.SystemTimeNow;

                    // Create StationSchedule logic here
                    var stationSchedule = new StationSchedule
                    {
                        StationScheduleId = _accountHelper.GenerateShortGuid(),
                        FormId = existingForm.FormId,
                        StationId = existingForm.StationId,
                        Date = existingForm.Date,
                        StartDate = existingForm.StartDate,
                        Status = ScheduleStatusEnums.Pending.ToString(),
                        UpdateDate = TimeHepler.SystemTimeNow
                    };

                    await _stationScheduleRepo.AddStationSchedule(stationSchedule);
                }

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
