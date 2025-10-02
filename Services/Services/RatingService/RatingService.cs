using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.TimeCoreHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Repositories.RatingRepo;
using Services.ApiModels;
using Services.ApiModels.Rating;
using Services.ServicesHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Services.Services.RatingService
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepo _ratingRepo;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AccountHelper _accountHelper;
        public RatingService(IRatingRepo ratingRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, AccountHelper accountHelper)
        {
            _ratingRepo = ratingRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _accountHelper = accountHelper;
        }

        public async Task<ResultModel> AddRating(AddRatingRequest addRatingRequest)
        {
            try
            {

                var userId = _accountHelper.GenerateShortGuid();
                var newRating = new BusinessObjects.Models.Rating
                {
                    RatingId = _accountHelper.GenerateShortGuid(),
                    AccountId = addRatingRequest.AccountId,
                    Rating1 = addRatingRequest.Rating1,
                    Description = addRatingRequest.Description,
                    StationId = addRatingRequest.StationId,
                    Status = AccountStatusEnums.Active.ToString(),
                    StartDate = TimeHepler.SystemTimeNow,    
                    UpdateDate = TimeHepler.SystemTimeNow
                };
                
                var createdRating = await _ratingRepo.AddRating(newRating);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status201Created,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsRating.ADD_RATING_SUCCESS,
                    Data = createdRating
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {

                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsRating.ADD_RATING_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> DeleteRating(string ratingId)
        {
            try
            {
                var existingRating = await _ratingRepo.GetRatingById(ratingId);
                if (existingRating == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsRating.RATING_NOT_FOUND,
                        Data = null
                    };
                }
                existingRating.Status = AccountStatusEnums.Inactive.ToString();
                existingRating.UpdateDate = TimeHepler.SystemTimeNow;
                var deletedRating = await _ratingRepo.UpdateRating(existingRating);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsRating.DELETE_RATING_SUCCESS,
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsRating.DELETE_RATING_FAILED,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetAllRatings()
        {
            try
            {

                var ratings = await _ratingRepo.GetAllRatings();
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsRating.GET_ALL_RATING_SUCCESS,
                    Data = ratings
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsRating.GET_ALL_RATING_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> GetRatingById(string ratingId)
        {
            try
            {

                var rating = await _ratingRepo.GetRatingById(ratingId);
                if (rating == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsRating.RATING_NOT_FOUND,
                        Data = null
                    };
                }
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsRating.GET_ALL_RATING_SUCCESS,
                    Data = rating
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsRating.GET_ALL_RATING_FAIL,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResultModel> UpdateRating(UpdateRatingRequest updateRatingRequest)
        {
            try
            {

                var existingRating = await _ratingRepo.GetRatingById(updateRatingRequest.RatingId);
                if (existingRating == null)
                {
                    return new ResultModel
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        ResponseCode = ResponseCodeConstants.NOT_FOUND,
                        Message = ResponseMessageConstantsRating.RATING_NOT_FOUND,
                        Data = null
                    };
                }
                if(updateRatingRequest.Rating1 != null)
                {
                    existingRating.Rating1 = updateRatingRequest.Rating1;
                }
                if (!string.IsNullOrEmpty(updateRatingRequest.Description))
                {
                    existingRating.Description = updateRatingRequest.Description;
                }
                existingRating.UpdateDate = TimeHepler.SystemTimeNow;
                var updatedRating = await _ratingRepo.UpdateRating(existingRating);
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status200OK,
                    IsSuccess = true,
                    ResponseCode = ResponseCodeConstants.SUCCESS,
                    Message = ResponseMessageConstantsRating.UPDATE_RATING_SUCCESS,
                    Data = updatedRating
                };

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    ResponseCode = ResponseCodeConstants.FAILED,
                    Message = ResponseMessageConstantsRating.UPDATE_RATING_FAILED,
                    Data = ex.Message
                };
            }
        }
    }
}
