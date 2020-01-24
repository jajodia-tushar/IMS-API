using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IMS.Entities.Exceptions;
using IMS.Core.Validators;

namespace IMS.Core.services
{
    public class LogsService : ILogsService
    {
        private ILogDbContext _logDbContext;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ITokenProvider _tokenProvider;
        private IAuditLogsDbContext _auditLogsDbContext;

        public LogsService(IAuditLogsDbContext auditLogsDbContext,ILogDbContext logDbContext,ILogManager logger, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            _logDbContext = logDbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenProvider = tokenProvider;
            _auditLogsDbContext = auditLogsDbContext;
        }
        public async Task<LogsResponse> GetLogsRecord()
        {
            LogsResponse logsResponse = new LogsResponse();
            try
            {
                List<Logs> logsRecords =await _logDbContext.GetLogs();
                if(logsRecords == null ||logsRecords.Count==0)
                {
                    logsResponse.Status = Status.Failure;
                    logsResponse.Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.NotFound,
                        ErrorMessage= Constants.ErrorMessages.EmptyLogList
                    };
                }
                else
                {
                    logsResponse.Status = Status.Success;
                    logsResponse.LogsRecords = logsRecords;
                }
            }
            catch(Exception exception)
            {
               logsResponse.Status = Status.Failure;
               logsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
             new Task(() => { _logger.LogException(exception, "GetLogsRecord", IMS.Entities.Severity.Critical, null, logsResponse); }).Start();
            }
            return logsResponse;
        }
        public async Task<ActivityLogsReponse> GetActivityLogs(int? pageNumber,int? pageSize,string fromDate,string toDate)
        {
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 10;
            var pagingInfo = new PagingInfo();
            ActivityLogsReponse activityLogsReponse = new ActivityLogsReponse()
            {
                Status = Status.Failure,
                PagingInfo = pagingInfo
            };
            int userId = -1;
            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException();
                userId = request.User.Id;
                if(DateTimeValidator.InitializeAndValidateDates(fromDate,toDate,out var startDate,out var endDate) == false)
                {
                    activityLogsReponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidDate);
                    return activityLogsReponse;
                }
                if(currentPageNumber <= 0 || currentPageSize <= 0)
                {
                    throw new InvalidPagingInfo(Constants.ErrorMessages.InvalidPagingDetails);
                }
                Tuple<int, List<ActivityLogs>> activityLogs = await _auditLogsDbContext.GetActivityLogs(currentPageNumber, currentPageSize, startDate, endDate);
                if (activityLogs.Item2.Count < 0)
                {
                    throw new RecordsNotFoundException(Constants.ErrorMessages.ActivityLogsNotPresent);
                }
                pagingInfo.PageNumber = currentPageNumber;
                pagingInfo.PageSize = currentPageSize;
                pagingInfo.TotalResults = activityLogs.Item1;
                activityLogsReponse.Status = Status.Success;
                activityLogsReponse.ActivityLogRecords = activityLogs.Item2;
            }
            catch (CustomException exception)
            {
                activityLogsReponse.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "GetActivityLogs", IMS.Entities.Severity.High,pageNumber+";"+pageSize+";"+fromDate+";"+toDate,activityLogsReponse); }).Start();
            }
            catch (Exception exception)
            {
                activityLogsReponse.Status = Status.Failure;
                activityLogsReponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetActivityLogs", Severity.High, pageNumber + ";" + pageSize + ";" + fromDate + ";" + toDate, activityLogsReponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (activityLogsReponse.Status == Status.Failure)
                    severity = Severity.High;
                new Task(() => { _logger.Log(null, activityLogsReponse, "GetActivityLogs", activityLogsReponse.Status, severity, userId); }).Start();
            }
            return activityLogsReponse;
        }
    }
}
