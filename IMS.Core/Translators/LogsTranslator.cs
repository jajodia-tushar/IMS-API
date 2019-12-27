using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class LogsTranslator
    {
        public static Contracts.LogsResponse ToDataContractsObject(Entities.LogsResponse doLogsResponse)
        {
            if(doLogsResponse!=null)
            {
                return new Contracts.LogsResponse()
                {
                    Status = doLogsResponse.Status == Entities.Status.Success ? Contracts.Status.Success : Contracts.Status.Failure,
                    Error  = doLogsResponse.Error == null?null:Translator.ToDataContractsObject(doLogsResponse.Error),
                    LogsRecords = doLogsResponse.LogsRecords==null?null :ToDataContractsObject(doLogsResponse.LogsRecords)
                };
            }
            return null;
        }

        private static List<Contracts.Logs> ToDataContractsObject(List<Entities.Logs> doLogsRecords)
        {
            List<Contracts.Logs> dtoLogRecords = new List<Contracts.Logs>();
            if(doLogsRecords!=null)
            {
                foreach(var log in doLogsRecords)
                {
                    dtoLogRecords.Add(ToDataContractsObject(log));
                }
                return dtoLogRecords;
            }
            return dtoLogRecords;
        }

        private static Contracts.Logs ToDataContractsObject(Entities.Logs doLog)
        {
            Contracts.Logs dtoLog = new Contracts.Logs();
            if(doLog!=null)
            {
                dtoLog.Id = doLog.Id;
                dtoLog.UserId = doLog.UserId;
                dtoLog.CallType = doLog.CallType;
                dtoLog.Request = doLog.Request;
                dtoLog.Response = doLog.Response;
                dtoLog.Severity = doLog.Severity;
                dtoLog.Status = doLog.Status;
                dtoLog.DateTime = doLog.DateTime;

                return dtoLog;
            }
            return dtoLog;
        }
    }
}
