using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class LogsService : ILogsService
    {
        private ILogDbContext _logDbContext;
        private ILogManager _logger;
        public LogsService(ILogDbContext logDbContext,ILogManager logger)
        {
            _logDbContext = logDbContext;
            _logger = logger;
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
    }
}
