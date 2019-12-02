using System;
using System.Diagnostics;
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

                //int userId = _tokenProvider.GetUserIdFromHeadersAuthorization(authorizationValues); //_tokenProvider.getUserFromToken();
                string callType = GetCalledMethod();
                string severity = LogConstants.Severity.No;
                string status =LogConstants.Status.Failure;
                if (response != null && response.Status == Status.Failure)
                {
                    severity = LogConstants.CallTypeSeverityMapping.SeverityOf[callType];
                    status = GetResponseStatus(response);
                }
                string requestJson = ConvertToString(request);
                string responseJson = ConvertToString(response);
                _logDbContext.Log(userId,status, callType, severity, requestJson, responseJson);
               
            }
            catch(Exception e)
            {
                return;
            }
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
