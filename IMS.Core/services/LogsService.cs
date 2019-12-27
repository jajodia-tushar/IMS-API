using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class LogsService : ILogsService
    {
        private ILogDbContext _logDbContext;
        public LogsService(ILogDbContext logDbContext)
        {
            _logDbContext = logDbContext;
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
            catch(Exception ex)
            {
               logsResponse.Status = Status.Failure;
               logsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
            }
            return logsResponse;
        }
    }
}
