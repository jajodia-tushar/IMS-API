using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface INotificationProvider
    {
        Task<bool> SendEmail(Email email);
    }
}
