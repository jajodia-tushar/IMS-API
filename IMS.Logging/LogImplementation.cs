using System;
using System.Diagnostics;
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using Newtonsoft.Json;

namespace IMS.Logging
{
    public class LogImplementation : ILogManagement
    {
        private ITokenProvider _tokenProvider;
        private ILogDbContext _logDbContext;
        public LogImplementation(ITokenProvider tokenProvider,ILogDbContext logDbContext)
        {
            _tokenProvider = tokenProvider;
            _logDbContext = logDbContext;
        }
        public void Log(object request, Response response)
        {
            try
            {  

                int userId = _tokenProvider.GetUserIdFromToken(); //_tokenProvider.getUserFromToken();
                string callType = GetCalledMethod();
                string severity = "No";
                if(response.Status==Status.Failure)
                severity=LogConstants.CallTypeSeverityMapping.SeverityOf[callType];
                string requestJson = ConvertToString(request);
                string status = GetResponseStatus(response);
                string responseJson = ConvertToString(response);
                _logDbContext.Log(userId,status, callType, severity, requestJson, responseJson);
               
            }
            catch
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
