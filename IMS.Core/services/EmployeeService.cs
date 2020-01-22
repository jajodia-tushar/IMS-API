using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeDbContext employeeDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        public EmployeeService(IEmployeeDbContext employeeDbContext, ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this.employeeDbContext = employeeDbContext;
            this._logger = logger;
            this._tokenProvider = tokenProvider;
            this._httpContextAccessor = httpContextAccessor;
        }
        
        public Task<EmployeeResponse> Add(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Response> Delete(string id,bool isHardDelete)
        {
            throw new NotImplementedException();
        }
        public Task<EmployeeResponse> Update(Employee employee)
        {
            throw new NotImplementedException();
        }
        public async Task<GetEmployeeResponse> ValidateEmployee(string employeeId)
        {
            GetEmployeeResponse employeeValidationResponse = new GetEmployeeResponse();
            try
            {
                if (!String.IsNullOrEmpty(employeeId))
                {
                    Employee employee = await employeeDbContext.GetEmployeeById(employeeId);
                    if (employee != null)
                    {
                        employeeValidationResponse.Status = Status.Success;
                        employeeValidationResponse.Employee = employee;
                        return employeeValidationResponse;
                    }
                }
                employeeValidationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InValidId);
            }
            catch (Exception exception)
            {
                employeeValidationResponse.Status = Status.Failure;
                employeeValidationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "ValidateEmployee", Severity.Critical, employeeId, employeeValidationResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeValidationResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, employeeValidationResponse, "ValidateEmployee", employeeValidationResponse.Status, severity, -1); }).Start();
            }
            return employeeValidationResponse;
        }

        public async Task<Response> CheckEmailAvailability(string email)
        {
            int userID = 0;
            Response response = new Response();
            if (String.IsNullOrEmpty(email))
            {
                response.Status = Status.Failure;
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidEmailId);
                return response;
            }
            try
            {
                string token= _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                 
                if(await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userID = user.Id;
                    try
                    {
                        bool employeeAlreadyExists = await employeeDbContext.CheckEmpEmailAvailability(email);

                        if (employeeAlreadyExists)
                        {

                            throw new InvalidEmailException("Email Id Already Exists");
                        }
                        else
                        {
                            response.Status = Status.Success;
                            response.Error = null;
                        }
                    }
                    catch (CustomException exception)
                    {
                        response.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                        new Task(() => { _logger.LogException(exception, "CheckEmailAvailability", Severity.Critical, email, response); }).Start();

                    }
                }

                else
                {
                    response.Status = Status.Failure;
                    response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }

            catch (Exception exception)
            {
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "CheckEmailAvailability", Severity.Critical, email, response); }).Start();

            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(email, response, "CheckEmailAvailability", response.Status, severity, userID); }).Start();
            }
            return response;
        }

        public async Task<EmployeeResponse> GetAllEmployees(string employeeId, string employeeName, int pageNumber, int pageSize)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            employeeResponse.Error = new Error();
            int userId = -1;
            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                if (await _tokenProvider.IsValidToken(token))
                {
                    User user = Utility.GetUserFromToken(token);
                    userId = user.Id;
                    if (pageNumber < 0 || pageSize < 0)
                    {
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                    }
                    if (pageNumber==0||pageSize==0)
                    {
                        pageNumber = 1;
                        pageSize = 10;
                    }
                    if(String.IsNullOrEmpty(employeeId))
                    {
                        employeeId = "";
                    }
                    if(string.IsNullOrEmpty(employeeName))
                    {
                        employeeName = "";
                    }
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    employeeResponse= await employeeDbContext.GetAllEmployees(employeeId, employeeName, limit, offset);
                    employeeResponse.PagingInfo.PageNumber = pageNumber;
                    employeeResponse.PagingInfo.PageSize = pageSize;
                    if (employeeResponse.Employees.Count>0)
                    {
                        employeeResponse.Status = Status.Success;                      
                    }
                    else
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.NoEmployeesPresent);
                    }
                }
                else
                {
                    employeeResponse.Status = Status.Failure;
                    employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetEmployees", IMS.Entities.Severity.Critical, "GetEmployees", employeeResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("GetEmployees", employeeResponse, "GetEmployees", employeeResponse.Status, severity, userId); }).Start();
            }
            return employeeResponse;
        }
    }
}
