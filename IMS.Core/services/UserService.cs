using IMS.Core.Validators;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class UserService : IUserService
    {
        private IUserDbContext _userDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        private IAccessControlDbContext _accessControlDbContext;

        public UserService(IUserDbContext userDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor, IAccessControlDbContext accessControlDbContext)
        {
            this._userDbContext = userDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
            this._accessControlDbContext = accessControlDbContext;
        }

        public async Task<UsersResponse> GetAllPendingAdminApprovals()
        {
            UsersResponse usersResponse = new UsersResponse()
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser = request.User;
                userId = requestedUser.Id;

                List<User> users = await _userDbContext.GetAllPendingAdminApprovals();
                if (users.Count == 0)
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                else
                {
                    usersResponse.Users = users;
                    usersResponse.Status = Status.Success;
                }



            }
            catch (CustomException e)
            {
                usersResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, " GetAllPendingApprovals", Severity.Medium, "Get Request", usersResponse); }).Start();
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetAllPendingApprovals", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("Get Request", usersResponse, "GetAllPendingApprovals", usersResponse.Status, severity, userId); }).Start();
            }

            return usersResponse;
        }
        [Audit("Deleted User with Id","User")]
        public async Task<Response> DeleteUser(int userIdToBeDeleted, bool isHardDelete,string remark)
        {
            User requestedUser = new User() ;
            requestedUser.Id = -1;
            Response deleteUserResponse = new Response();
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    requestedUser =  request.User;
                    if (requestedUser.Id==userIdToBeDeleted)
                    {
                        deleteUserResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UnAuthorized);
                        return deleteUserResponse;
                    }
                    if (userIdToBeDeleted > 0)
                    {
                        bool isDeleted = await _userDbContext.DeleteUser(userIdToBeDeleted, isHardDelete);
                        if (isDeleted)
                        {
                            deleteUserResponse.Status = Status.Success;
                            return deleteUserResponse;
                        }
                    }
                    deleteUserResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InValidId);
                }
                else
                {
                    deleteUserResponse.Status = Status.Failure;
                    deleteUserResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                deleteUserResponse.Status = Status.Failure;
                deleteUserResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "DeleteUser", IMS.Entities.Severity.Medium, userIdToBeDeleted, deleteUserResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (deleteUserResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(userIdToBeDeleted, deleteUserResponse, "DeleteUser",deleteUserResponse.Status, severity,requestedUser.Id ); }).Start();
            }
            return deleteUserResponse;
        }

        public async Task<UsersResponse> GetUsersByRole(string roleName)
        {
            UsersResponse usersResponse = new UsersResponse();
            if (String.IsNullOrEmpty(roleName))
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.MissingValues);
            }
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    try
                    {
                        usersResponse.Status = Status.Failure;
                        usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                        List<User> userList = await _userDbContext.GetUsersByRole(roleName);

                        if (userList.Count != 0)
                        {
                            usersResponse.Status = Status.Success;
                            usersResponse.Users = userList;
                        }
                        return usersResponse;
                    }
                    catch (Exception exception)
                    {
                        new Task(() => { _logger.LogException(exception, "GetUserRoleByName", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();
                    }
                }
                else
                {
                    usersResponse.Status = Status.Failure;
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetUserRoleByName", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(roleName, usersResponse, "Get Users By Role Name", usersResponse.Status, severity, userId); }).Start();
            }
            return usersResponse;
        }
        public async Task<UsersResponse> GetAllUsers()
        {
            UsersResponse usersResponse = new UsersResponse()
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser = request.User;
                userId = requestedUser.Id;

                List<User> users = await _userDbContext.GetAllUsers(requestedUser.Role);
                if (users.Count == 0)
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                else
                {
                    usersResponse.Users = users;
                    usersResponse.Status = Status.Success;
                }



            }
            catch (CustomException e)
            {
                usersResponse.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, " GetAllUsers", Severity.Medium, "Get Request", usersResponse); }).Start();
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetAllUsers", IMS.Entities.Severity.Medium, "Get Request", usersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log("Get Request", usersResponse, "GetAllUsers", usersResponse.Status, severity, userId); }).Start();
            }

            return usersResponse;
        }

        [Audit("Updated User","User")]
        public async Task<UsersResponse> UpdateUser(User user,string remark)
        {
            UsersResponse userResponse = new UsersResponse();
            userResponse.Status = Status.Failure;
            try
            {
                if (!UserValidator.UpdateUserValidation(user))
                {
                    userResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.MissingValues);
                }
                else
                {
                    RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                    if (request.HasValidToken)
                    {
                        User requestesUser = request.User;
                        Response validityResponse = await CheckValidityOfUpdateUserRequest(requestesUser, user);
                        if (validityResponse.Status.Equals(Status.Success))
                        {
                            try
                            {
                                User updatedUser = await _userDbContext.UpdateUser(user);
                                if (updatedUser != null)
                                {
                                    userResponse.Status = Status.Success;
                                    userResponse.Users = new List<User>() { updatedUser };
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception exception)
                            {
                                throw exception;
                            }
                        }
                        else
                        {
                            userResponse.Error = Utility.ErrorGenerator(validityResponse.Error.ErrorCode, validityResponse.Error.ErrorMessage);
                        }
                    }
                    else
                    {
                        userResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                    }
                }
            }
            catch (InvalidEmailException exception)
            {
                userResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "UserUpdation", IMS.Entities.Severity.Medium, user, userResponse); }).Start();
            }
            catch (Exception exception)
            {
                userResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "UserUpdation", IMS.Entities.Severity.Medium, user, userResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (userResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(user, userResponse, "Update User", userResponse.Status, severity, -1); }).Start();
            }
            return userResponse;
        }

        [Audit("Added User","User")]
        public async Task<UsersResponse> AddUser(User newUser)
        {
            UsersResponse response = new UsersResponse
            {
                Status = Status.Failure
            };
            int userId = -1;
            try
            {

                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException(Constants.ErrorMessages.InvalidToken);
                User requestedUser = request.User;
                userId = requestedUser.Id;

                if (Validators.UserValidator.ValidateNewUser(newUser))
                {
                    bool hasAccess = await _accessControlDbContext.HasAccessControl(requestedUser.Role, newUser.Role);
                    if (!hasAccess)
                        throw new AccessDeniedException();
                    bool isEmailOrUsernameRepeated = await _userDbContext.CheckEmailOrUserNameAvailability(newUser.Email, newUser.Username);
                    if (isEmailOrUsernameRepeated)
                        throw new InvalidEmailException("Given UserName or Email is already registered");
                    int isApproved = 1;
                    int isActive = 1;
                    newUser.Password = Utility.Hash(newUser.Password);
                    bool isSaved = await _userDbContext.Save(newUser, isApproved, isActive);
                    if (isSaved)
                    {
                        response.Users = new List<User>();
                        response.Users.Add(newUser);
                        response.Status = Status.Success;
                    }
                    else
                        throw new Exception();

                }
                else
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.UserDetailsMissing);

            }
            catch (CustomException e)
            {
                response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                new Task(() => { _logger.LogException(e, "Adding New User", Severity.Medium, newUser, response); }).Start();
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "Adding New User", Severity.Medium, newUser, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Medium;
                new Task(() => { _logger.Log(newUser, response, "Adding New User", response.Status, severity, userId); }).Start();
            }
            return response;
        }
        public async Task<Response> CheckValidityOfUpdateUserRequest(User requestedUser, User userToBeUpdated)
        {
            Response response = new Response();
            response.Status = Status.Failure;
            User existingUserToBeUpdated = await _userDbContext.GetUserById(userToBeUpdated.Id);
            if (existingUserToBeUpdated != null)
            {
                if (await _accessControlDbContext.HasAccessControl(requestedUser.Role, userToBeUpdated.Role))
                {
                    if (requestedUser.Role.Name.ToLower() == Constants.Roles.Admin)
                    {
                        if (userToBeUpdated.Role.Id != existingUserToBeUpdated.Role.Id)
                        {
                            response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.UnAuthorized);
                        }
                        else
                        {
                            response.Status = Status.Success;
                        }
                    }
                    else
                    {
                        response.Status = Status.Success;
                    }
                }
                else
                {
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.UnAuthorized);
                }
            }
            else
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
            }
            return response;
        }
        public async Task<UsersResponse> ApproveAdmin(int userId)
        {
            UsersResponse usersResponse = new UsersResponse();
            if (userId <= 0)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InValidId);
            }
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    try
                    {
                        usersResponse.Status = Status.Failure;
                        usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoUsers);
                        User approvedUser = await _userDbContext.ApproveAdmin(userId);

                        if (user != null)
                        {
                            usersResponse.Status = Status.Success;
                            usersResponse.Users = new List<User>() { approvedUser };
                        }
                        return usersResponse;
                    }
                    catch (Exception exception)
                    {
                        new Task(() => { _logger.LogException(exception, "ApproveAdmin", IMS.Entities.Severity.Medium, userId, usersResponse); }).Start();
                    }
                }
                else
                {
                    usersResponse.Status = Status.Failure;
                    usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                usersResponse.Status = Status.Failure;
                usersResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "ApproveAdmin", IMS.Entities.Severity.Medium, userId, usersResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (usersResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(userId, usersResponse, "ApproveAdmin", usersResponse.Status, severity, -1); }).Start();
            }
            return usersResponse;
        }

        public async Task<Response> CheckUsernameAvailability(string username)
        {
            Response response = new Response();
            if (String.IsNullOrEmpty(username))
            {
                response.Status = Status.Failure;
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidUsername);
                return response;
            }
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    try
                    {                       
                        bool isUserNamePresent = await _userDbContext.CheckUserNameAvailability(username);
                        if (isUserNamePresent)
                        {
                            throw new InvalidUserNameException("User Name Already Exists");
                        }
                        else
                        {
                            response.Status = Status.Success;
                            response.Error = null;
                        }
                    }
                    catch (CustomException e)
                    {
                        response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                        new Task(() => { _logger.LogException(e, "CheckUsernameAvailability", Severity.Critical, username, response); }).Start();
                    }
                }
                else
                {
                    response.Status = Status.Failure;
                    response.Error =
                        Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized,
                        Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "CheckUsernameAvailability", Severity.Critical, username, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(username, response, "CheckUsernameAvailability", response.Status, severity, userId); }).Start();
            }
            return response;
        }

        public async Task<Response> CheckEmailAvailability(string emailId)
        {
            Response response = new Response();
            if (String.IsNullOrEmpty(emailId))
            {
                response.Status = Status.Failure;
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidEmailId);
                return response;
            }
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    try
                    {
                        bool isUserNamePresent = await _userDbContext.CheckEmailAvailability(emailId);
                        if (isUserNamePresent)
                        {
                            throw new InvalidEmailException("Email Id Already Exists");
                        }
                        else
                        {
                            response.Status = Status.Success;
                            response.Error = null;
                        }
                    }
                    catch (CustomException e)
                    {
                        response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                        new Task(() => { _logger.LogException(e, "CheckEmailAvailability", Severity.Critical, emailId, response); }).Start();
                    }
                }
                else
                {
                    response.Status = Status.Failure;
                    response.Error =
                        Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized,
                        Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception e)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(e, "CheckEmailAvailability", Severity.Critical, emailId, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(emailId, response, "CheckEmailAvailability", response.Status, severity, userId); }).Start();
            }
            return response;
        }
    }
}

