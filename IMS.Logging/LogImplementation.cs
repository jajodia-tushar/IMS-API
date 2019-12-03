using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace IMS.Logging
{
    public class LogImplementation : ILogManager
    {
       
        private ILogDbContext _logDbContext;
        public LogImplementation(ILogDbContext logDbContext)
        {
           
            _logDbContext = logDbContext;
        }
        public void Log(object request, Response response, int userId)
        {
            try
            {  

                string callType = GetCalledMethod();
                string severity = GetSeverity(response,callType);
                string status = GetStatus(response);
                string requestJson = ConvertToString(request);
                string responseJson = ConvertToString(response);
                _logDbContext.Log(userId,status, callType, severity, requestJson, responseJson);
            }
            catch(Exception e)
            {
            }
        }

        private string GetSeverity(Response response,string callType)
        {
            string severity = LogConstants.Severity.Critical;
            if (response!=null)
            {
                if (response.Status == Status.Failure)
                    severity = LogConstants.CallTypes.SeverityMapping[callType];
                else
                    severity = LogConstants.Severity.No;
            }

                return severity;
        }

        private string GetStatus(Response response)
        { 
           string status = LogConstants.Status.Failure;
            if (response !=null && response.Status == Status.Success)
                status = LogConstants.Status.Success;
            return status;
        }

        private string GetCalledMethod()
        {
            StackTrace stackTrace = new StackTrace();

           
           return stackTrace.GetFrame(2).GetMethod().Name;
        }

        public string GetResponseStatus(Response responseType)
        {
            if (responseType.Status == Status.Success)
                return LogConstants.Status.Success;
            return LogConstants.Status.Failure;
        }

        public string ConvertToString(object requestType)
        {
            if (requestType != null)
                return JsonConvert.SerializeObject(requestType);
            return null;
        }

       
    }
}
