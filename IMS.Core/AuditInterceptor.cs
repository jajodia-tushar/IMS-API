using IMS.Entities;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SimpleProxy;
using SimpleProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using IMS.DataLayer.Interfaces;

namespace IMS.Core
{
    public class AuditInterceptor : IMethodInterceptor
    {
        private ITokenProvider _tokenProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private IAuditLogsDbContext _auditLogsDbContext;
        public AuditInterceptor(IAuditLogsDbContext auditLogsDbContext, ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            _auditLogsDbContext = auditLogsDbContext;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        public void BeforeInvoke(InvocationContext invocationContext)
        {
        }
        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            var attribute = invocationContext.GetAttributeFromMethod<AuditAttribute>();
            var processName = attribute.ProcessName;
            var className = attribute.ClassName;
            string status = GetStatus(methodResult);
            if (status.Equals("Success"))
            {
                string performedOn = "";
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                User user = Utility.GetUserFromToken(token);
                string userName = user.Firstname + " " + user.Lastname;
                string action = invocationContext.GetExecutingMethodName();
                var parameterValue = invocationContext.GetParameterValue(0);
                //string remarks = FindRemarks(invocationContext, processName);
                performedOn = GetPerformedOn(action, parameterValue);
                string details = userName + " " + processName +" "+ performedOn;
                _auditLogsDbContext.AddAuditLogs(userName, action, details, performedOn, null,className);
            }
        }

        private string GetPerformedOn(string action, object parameterValue)
        {
            string performedOn = "";
            if (action.ToLower().Contains("delete") || action.Contains("ApproveEmployeeBulkOrder") || action.ToLower().Contains("reject") || action.ToLower().Contains("cancel"))
                performedOn = parameterValue.ToString();
            else if (action.ToLower().Equals("placeemployeeorder") || action.ToLower().Equals("savevendororder") || action.ToLower().Equals("approvevendororder"))
                performedOn = GetOrderId(parameterValue);
            else
                performedOn = GetNameFromRequest(parameterValue);
            return performedOn;
        }

        private string FindRemarks(InvocationContext invocationContext, string processName)
        {
            string remarks = "";
            if(processName.ToLower().Contains("reject") || processName.ToLower().Contains("Deleted Vendor Order"))
            {
                remarks = invocationContext.GetParameterValue(1).ToString();
            }
            else if (processName.ToLower().Contains("delete"))
            {
                remarks = invocationContext.GetParameterValue(2).ToString();
            }
            else if (processName.ToLower().Contains("update"))
            {
                remarks = invocationContext.GetParameterValue(1).ToString();
            }
            return remarks;
        }
        private string GetOrderId(object obj)
        {
            foreach(var property in obj.GetType().GetProperties())
            {
                if (property.Name.ToLower().Contains("order"))
                {
                    foreach (var innerProperty in property.GetValue(obj, null).GetType().GetProperties())
                    {
                        if (innerProperty.Name.ToLower().Contains("id"))
                        {
                            var id = innerProperty.GetValue(property.GetValue(obj, null), null);
                            return id.ToString();
                        }
                    }
                }
            }
            return "";
        }
        private string GetNameFromRequest(Object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.Name.ToLower().Contains("firstname") || property.Name.ToLower().Contains("name"))
                {
                    return property.GetValue(obj, null).ToString();
                }
            }
            return "";
        }
        private string GetStatus(object methodResult)
        {
            foreach (var property in methodResult.GetType().GetProperties())
            {
                if (property.Name.Equals("Result"))
                {
                    Console.WriteLine(property.Name + "------>" + property.GetValue(methodResult, null));
                    Console.WriteLine("Type--------->"+property.GetType());
                    foreach(var innerProprerty in property.GetValue(methodResult,null).GetType().GetProperties())
                    {
                        if (innerProprerty.Name.Equals("Status"))
                        {
                            var status = innerProprerty.GetValue(property.GetValue(methodResult, null), null);
                            Console.WriteLine(status);
                            return status.ToString();
                        }
                    }
                }
            }
            return "";
        }
    }
}
