using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class NotificationTranslator
    {
        public static Contracts.NotificationResponse ToDataContractObject(Entities.NotificationResponse doNotificationResponse)
        {
            try
            {
                var dtoNotificationResponse = new Contracts.NotificationResponse();
                if (doNotificationResponse.Status.Equals(Entities.Status.Success))
                {
                    dtoNotificationResponse.Status = Contracts.Status.Success;
                    dtoNotificationResponse.Notifications = ToDataContractObject(doNotificationResponse.Notifications);
                }
                else
                {
                    dtoNotificationResponse.Status = Contracts.Status.Failure;
                    dtoNotificationResponse.Error = doNotificationResponse.Error == null ? null : Translator.ToDataContractsObject(doNotificationResponse.Error);
                }
                dtoNotificationResponse.PagingInfo = Translator.ToDataContractsObject(doNotificationResponse.PagingInfo);
                return dtoNotificationResponse;
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        private static List<Contracts.Notification> ToDataContractObject(List<Entities.Notification> notifications)
        {
            try
            {
                var dtoNotifications = new List<Contracts.Notification>();
                foreach (var notification in notifications)
                {
                    dtoNotifications.Add(ToDataContractObject(notification));
                }
                return dtoNotifications;
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        private static Contracts.Notification ToDataContractObject(Entities.Notification notification)
        {
            try
            {
                return new Contracts.Notification()
                {
                    RequestId = notification.RequestId,
                    RequestedBy = notification.RequestedBy,
                    LastModified = notification.LastModified,
                    RequestStatus = (Contracts.RequestStatus)notification.RequestStatus,
                    RequestType = (Contracts.RequestType)notification.RequestType
                };
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }
    }
}
