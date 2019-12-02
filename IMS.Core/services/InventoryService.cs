using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class InventoryService : IInventoryService
    {
        private ILogManager _logManager;
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
       
        public InventoryService(ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            _logManager = logger;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;

        }
        // private IUserService _userService;

        public Response TestMethod()
        {
            Response customizedResponse = new Response();
            int userId = -1;
            try
            {

                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValid = false;//_tokenProvider.IsValidToken(token, out userId);
                                     /* the above method checks whether token is valid or not with database
                                      * if it is valid then returns true and sets userId
                                      * else returns false without setting userId
                                      */
                User user = Utility.GetUserFromToken(token);
                if (isValid)
                {


                    //Business Logic
                }
                else
                {
                    customizedResponse.Status = Status.Failure;
                    customizedResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.UnAuthorized,
                        ErrorMessage = Constants.ErrorMessages.TokenExpiried
                    };
                }
            }
            catch
            {

                customizedResponse.Status = Status.Failure;
                customizedResponse.Error = new Error()
                {
                    ErrorCode = Constants.ErrorCodes.ServerError,
                    ErrorMessage = Constants.ErrorMessages.ServerError
                };

            }
            /*finally
            {
                _logManager.Log(null,customizedResponse,userId);
            }*/
            return customizedResponse;
        }
    }
}
