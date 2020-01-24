using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Entities;

namespace IMS.DataLayer.Interfaces
{
    public interface IAdminNotificationDbContext
    {
        Task<NotificationResponse> GetAdminNotifications(int currentPageNumber, int currentPageSize);
    }
}
