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
                dtoLog.LogId = doLog.LogId;
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

        public static ActivityLogsResponse ToDataContractsObject(ActivityLogsReponse doActivityLogsReponse)
        {
            Contracts.ActivityLogsResponse dtoActivityLogSresponse = new ActivityLogsResponse();
            if(doActivityLogsReponse.Status == Entities.Status.Success)
            {
                dtoActivityLogSresponse.Status = Contracts.Status.Success;
                dtoActivityLogSresponse.ActivityLogRecords = ToDataContractsObject(doActivityLogsReponse.ActivityLogRecords);
                dtoActivityLogSresponse.PagingInfo = Translator.ToDataContractsObject(doActivityLogsReponse.PagingInfo);
            }
            else
            {
                dtoActivityLogSresponse.Status = Contracts.Status.Failure;
                dtoActivityLogSresponse.Error = Translator.ToDataContractsObject(doActivityLogsReponse.Error);
            }
            return dtoActivityLogSresponse;
        }

        private static List<Contracts.ActivityLogs> ToDataContractsObject(List<Entities.ActivityLogs> doActivityLogRecords)
        {
            List<Contracts.ActivityLogs> dtoActivityLogRecords = new List<Contracts.ActivityLogs>();
            if(doActivityLogRecords!=null || doActivityLogRecords.Count != 0)
            {
                foreach(var activityLogRecord in doActivityLogRecords)
                {
                    dtoActivityLogRecords.Add(ToDataContractsObject(activityLogRecord));
                }
            }
            return dtoActivityLogRecords;
        }

        private static Contracts.ActivityLogs ToDataContractsObject(Entities.ActivityLogs doActivityLogRecord)
        {
            Contracts.ActivityLogs dtoActivityLogRecord = new Contracts.ActivityLogs();
            if (doActivityLogRecord != null)
            {
                dtoActivityLogRecord.UserName = doActivityLogRecord.UserName;
                dtoActivityLogRecord.Action = doActivityLogRecord.Action;
                dtoActivityLogRecord.Details = doActivityLogRecord.Details;
                dtoActivityLogRecord.PerformedOn = doActivityLogRecord.PerformedOn;
                dtoActivityLogRecord.CreatedOn = doActivityLogRecord.CreatedOn;
                dtoActivityLogRecord.Remarks = doActivityLogRecord.Remarks;
            }
            return dtoActivityLogRecord;
        }
    }
}
