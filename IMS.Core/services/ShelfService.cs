using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Core.Validators;
using IMS.DataLayer;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;

namespace IMS.Core.services
{
    public class ShelfService : IShelfService
    {
        private IShelfDbContext _shelfDbContext;
        private ILogManager _logger;
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;

        public ShelfService(IShelfDbContext shelfDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            _shelfDbContext = shelfDbContext;
            _logger = logger;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ShelfResponse> GetShelfList()
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            try
            {
                List<Shelf> shelves = await _shelfDbContext.GetAllShelves();

                if (shelves == null || shelves.Count == 0)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.EmptyShelfList
                    };
                   
                }
                else
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = shelves;
                }
            }
            catch(Exception exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetShelfList", Severity.Critical, null, shelfResponse); }).Start();
            }
          
            return shelfResponse;
        }

        public async Task<ShelfResponse> GetShelfByShelfCode(string shelfCode)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            Shelf Shelf =await _shelfDbContext.GetShelfByShelfCode(shelfCode);
            try
            {
                if (Shelf == null || Shelf.Code == null)
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage = Constants.ErrorMessages.InvalidShelfCode
                    };
                }
                else
                {
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = new List<Shelf> { Shelf };
                }
            }
            catch (Exception exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetShelfByShelfCode", Severity.Critical, shelfCode, shelfResponse); }).Start();
            }

            return shelfResponse;
        }

        [Audit("Added Shelf","Shelf")]
        public async Task< ShelfResponse> AddShelf(Shelf shelf)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            bool shelfPresent = await _shelfDbContext.IsShelfPresentByCode(shelf.Code);
            try
            {

                if (shelfPresent == false)
                {
                    List<Shelf> shelfList =await _shelfDbContext.AddShelf(shelf);
                    shelfResponse.Status = Status.Success;
                    shelfResponse.Shelves = shelfList;
                }
                else
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.ShelfIsAlreadyPresent
                    };
                }

            }
            catch (Exception exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "AddShelf", Severity.High, shelf, shelfResponse); }).Start();
            }
            return shelfResponse;
            
        }

        [Audit("Deleted Shelf With ShelfCode","Shelf")]
        public async Task<ShelfResponse> Delete(string shelfCode)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            bool shelfPresent = await _shelfDbContext.IsShelfPresentByCode(shelfCode);
            try
            {

                if (shelfPresent == true && await _shelfDbContext.GetShelfStatusByCode(shelfCode) == true)
                {
                   
                        List<Shelf> shelfList =await _shelfDbContext.DeleteShelfByCode(shelfCode);
                        shelfResponse.Status = Status.Success;
                        shelfResponse.Shelves = shelfList;
                   
                }
                else
                {
                    shelfResponse.Status = Status.Failure;
                    shelfResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.ShelfNotPresent
                    };
                }
            }
            catch (Exception exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Delete", Severity.High, shelfCode, shelfResponse); }).Start();
            }
            return shelfResponse;
        }

        [Audit("Updated Shelf With Shelf Code","Shelf")]
        public async Task<ShelfResponse> Update(Shelf shelf)
        {
            ShelfResponse shelfResponse = new ShelfResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException();
                userId = request.User.Id;
                Response validatorResponse = ShelfValidator.ValidateShelf(shelf);
                if(validatorResponse.Error == null)
                {
                    Shelf updatedShelf = await _shelfDbContext.UpdateShelf(shelf);
                    if (updatedShelf != null)
                    {
                        shelfResponse.Status = Status.Success;
                        shelfResponse.Shelves = new List<Shelf>() { updatedShelf };
                    }
                    else
                    {
                        shelfResponse.Status = Status.Failure;
                        shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.ShelfNotUpdated);
                    }
                }
                else
                {
                    shelfResponse.Error = validatorResponse.Error;
                    shelfResponse.Status = Status.Failure;
                }
            }
            catch(CustomException exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "Update Shelf", IMS.Entities.Severity.Critical,shelf, shelfResponse); }).Start();
            }
            catch(Exception exception)
            {
                shelfResponse.Status = Status.Failure;
                shelfResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Update Shelf", IMS.Entities.Severity.Critical, shelf, shelfResponse); }).Start();
            }
            finally{
                Severity severity = Severity.No;
                if (shelfResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(shelf, shelfResponse, "Update Shelf", shelfResponse.Status, severity, userId); }).Start();
            }
            return shelfResponse;
        }
    }
}
