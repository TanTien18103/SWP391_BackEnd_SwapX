using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.Form;
using Services.ApiModels;
using Services.ApiModels.Form;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Form
{
    public class FormService : IFormService
    {
        private readonly IFormRepo _formRepo;
        private readonly AccountHelper _accountHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FormService(IFormRepo formRepo, AccountHelper accountHelper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _formRepo = formRepo;
            _accountHelper = accountHelper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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

                var form = new BusinessObjects.Models.Form
                {
                    FormId = _accountHelper.GenerateShortGuid(),
                    AccountId = addFormRequest.AccountId,
                    StationId = addFormRequest.StationId,
                    Title = addFormRequest.Title,
                    Description = addFormRequest.Description,
                    Date = addFormRequest.Date,
                    Status = FormStatusEnums.Submitted.ToString(),
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
    }
}
