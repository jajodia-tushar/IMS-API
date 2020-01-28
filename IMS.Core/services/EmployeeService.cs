using IMS.Core.Validators;
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
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
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

        public async Task<Response> CheckEmployeeIdAvailability(string employeeId)
        {
            Response response = new Response();
            int userId = 0;
            if (String.IsNullOrEmpty(employeeId))
            {
                response.Status = Status.Failure;
                response.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InValidId);
                return response;
            }

            
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    try
                    {
                        bool isEmployeeIdPresent = await employeeDbContext.CheckEmployeeIdAvailability(employeeId);
                        if (!isEmployeeIdPresent)
                        {
                            response.Status = Status.Success;
                            response.Error = null;
                        }
                        else
                        {
                            throw new EmployeeIdAlreadyExists("Employee Id Already Exists");
                        }
                    }
                    catch (CustomException e)
                    {
                        response.Error = Utility.ErrorGenerator(e.ErrorCode, e.ErrorMessage);
                        new Task(() => { _logger.LogException(e, "CheckEmployeeIdAvailability", Severity.Critical, employeeId, response); }).Start();
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
                new Task(() => { _logger.LogException(e, "CheckEmployeeIdAvailability", Severity.Critical, employeeId, response); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (response.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log(employeeId, response, "CheckEmployeeIdAvailability", response.Status, severity, userId); }).Start();
            }
            return response;
        }


        public async Task<EmployeeResponse> GetAllEmployees(string filter, int pageNumber, int pageSize)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            employeeResponse.Error = new Error();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;
                    if (pageNumber < 0 || pageSize < 0)
                    {
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidPagingDetails);
                        return employeeResponse;
                    }
                    if (pageNumber == 0 || pageSize == 0)
                    {
                        pageNumber = 1;
                        pageSize = 10;
                    }
                    if (String.IsNullOrEmpty(filter))
                    {
                        filter = "";
                    }
                    int limit = pageSize;
                    int offset = (pageNumber - 1) * pageSize;
                    employeeResponse = await employeeDbContext.GetAllEmployees(filter, limit, offset);
                    employeeResponse.PagingInfo.PageNumber = pageNumber;
                    employeeResponse.PagingInfo.PageSize = pageSize;
                    if (employeeResponse.Employees.Count > 0)
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
        [Audit("Added Employee","Employee")]
        public async Task<EmployeeResponse> Add(Employee employee)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;

                    employeeResponse = EmployeeValidator.EmployeeValidate(employee);
                    if (employeeResponse.Error == null)
                    {
                        bool isRepeatedEmployeeDetails = await employeeDbContext.EmployeeDetailsRepetitionCheck(employee);
                        if (!isRepeatedEmployeeDetails)
                        {
                            string latestCreatedEmployeeId = await employeeDbContext.CreateEmployee(employee);
                            Employee createdEmployee = await employeeDbContext.GetEmployeeById(latestCreatedEmployeeId);
                            if (createdEmployee.Id.Equals(employee.Id))
                            {
                                employeeResponse.Status = Status.Success;
                                List<Employee> createdEmployeeList = new List<Employee>();
                                createdEmployeeList.Add(createdEmployee);
                                employeeResponse.Employees = createdEmployeeList;
                            }
                            else
                            {
                                employeeResponse.Status = Status.Failure;
                                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.EmployeeNotAdded);
                            }
                            return employeeResponse;
                        }
                        else
                        {
                            employeeResponse.Status = Status.Failure;
                            employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.AlreadyPresentEmployee);
                        }
                    }
                    return employeeResponse;
                }
                else
                {
                    employeeResponse.Status = Status.Failure;
                    employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                employeeResponse.Status = Status.Failure;
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Add", Severity.High, employee, employeeResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(employee, employeeResponse, "Add", employeeResponse.Status, severity, userId); }).Start();
            }
            return employeeResponse;
        }

        [Audit("Deleted Employee With Id","Employee")]
        public async Task<Response> Delete(string id, bool isHardDelete)
        {
            Response employeeResponse = new Response();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;

                    if (String.IsNullOrEmpty(id))
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidEmployeeId);
                        return employeeResponse;
                    }
                    Employee isEmployeePresent = await employeeDbContext.GetEmployeeById(id);
                    if (isEmployeePresent != null)
                    {
                        bool isDeleted = await employeeDbContext.Delete(id, isHardDelete);
                        if (isDeleted)
                        {
                            employeeResponse.Status = Status.Success;
                        }
                        else
                        {
                            employeeResponse.Status = Status.Failure;
                            employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.EmployeeNotDeleted);
                        }
                    }
                    else
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.InvalidEmployeeId);
                    }
                    return employeeResponse;
                }
                else
                {
                    employeeResponse.Status = Status.Failure;
                    employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                employeeResponse.Status = Status.Failure;
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Delete", Severity.Critical, id + ";" + isHardDelete, employeeResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(id + ";" + isHardDelete, employeeResponse, "Delete", employeeResponse.Status, severity, userId); }).Start();
            }
            return employeeResponse;
        }
        [Audit("Updated Employee","Employee")]
        public async Task<EmployeeResponse> Update(Employee employee)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (request.HasValidToken)
                {
                    User user = request.User;
                    userId = user.Id;

                    if (string.IsNullOrEmpty(employee.Id) || string.IsNullOrEmpty(employee.Firstname) || string.IsNullOrEmpty(employee.Email))
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidEmployeeDetails);
                        return employeeResponse;
                    }
                    Employee updatedEmployee = await employeeDbContext.Update(employee);
                    if (updatedEmployee.Id.Equals(employee.Id) && updatedEmployee.Email.Equals(employee.Email) && updatedEmployee.ContactNumber.Equals(employee.ContactNumber))
                    {
                        employeeResponse.Status = Status.Success;
                        List<Employee> updatedEmployeeList = new List<Employee>();
                        updatedEmployeeList.Add(updatedEmployee);
                        employeeResponse.Employees = updatedEmployeeList;
                    }
                    else
                    {
                        employeeResponse.Status = Status.Failure;
                        employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.Conflict, Constants.ErrorMessages.NotUpdated);
                    }

                    return employeeResponse;
                }
                else
                {
                    employeeResponse.Status = Status.Failure;
                    employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.UnAuthorized, Constants.ErrorMessages.InvalidToken);
                }
            }
            catch (Exception exception)
            {
                employeeResponse.Status = Status.Failure;
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "Update", Severity.Critical, employee, employeeResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (employeeResponse.Status == Status.Failure)
                    severity = Severity.High;

                new Task(() => { _logger.Log(employee, employeeResponse, "Update", employeeResponse.Status, severity, userId); }).Start();
            }
            return employeeResponse;
        }
    }
}
