using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Contracts;
using IMS.Core;
using IMS.Core.Translators;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private ILogsService _logsService;
        private ILogManager _logger;
        public LogsController(ILogsService logsService, ILogManager logManager)
        {
            _logsService = logsService;
            _logger = logManager;
        }
        // GET: api/Logs
        [HttpGet]
        public async Task<LogsResponse> GetLogs()
        {
            LogsResponse dtoLogsResponse = null;
            try
            {
                IMS.Entities.LogsResponse doLogsResponse = await _logsService.GetLogsRecord();
                dtoLogsResponse = LogsTranslator.ToDataContractsObject(doLogsResponse);

            }
            catch (Exception exception)
            {
                dtoLogsResponse = new IMS.Contracts.LogsResponse()
                {
                    Status = Status.Failure,
                    Error = new Error()
                    {
                        ErrorCode = Constants.ErrorCodes.ServerError,
                        ErrorMessage = Constants.ErrorMessages.ServerError
                    }
                };
                new Task(() => { _logger.LogException(exception, "GetLogs", IMS.Entities.Severity.Critical,null,dtoLogsResponse); }).Start();
            }
            return dtoLogsResponse;
        }     
    }
}
