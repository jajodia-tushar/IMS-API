using System;
using System.Diagnostics;
using System.Reflection;
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
        public void Log(Object request, Object response, string callType, Status status, Severity severity, int userId)
        {
            try
            {  
                string requestJson = ConvertToString(request);
                string responseJson = ConvertToString(response);
                _logDbContext.Log(userId,status.ToString(), callType, severity.ToString(), requestJson, responseJson);
            }
            catch(Exception exception)
            {
                throw exception;
            }
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

        public void LogException(Exception exception, string callType, Severity severity, object request, object response)
        {
            string exceptionMessage = exception.Message.ToString();
            string exceptionType = exception.GetType().Name.ToString();
            string stackTrace = exception.StackTrace.ToString();
            string targetSite = exception.TargetSite == null ? null : exception.TargetSite.Name;
            string innerException = exception.InnerException == null ? null : exception.InnerException.Message;
            _logDbContext.LogException(callType,ConvertToString(request),ConvertToString(response),stackTrace,exceptionMessage,innerException,targetSite,exceptionType, severity.ToString());
        }
    }
}
