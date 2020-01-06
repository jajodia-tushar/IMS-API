using NotificationApi.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApi.Core
{
    public interface IEmailService
    {
        Task<Response> Send(Email email);
    }
}
