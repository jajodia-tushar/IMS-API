using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using IMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class AdminNotificationService : IAdminNotificationService
    {
        private IAdminNotificationDbContext _adminNotificationDbContext;
        private ITokenProvider _tokenProvider;
        private ILogManager _logger;
        private IHttpContextAccessor _httpContextAccessor;
        public AdminNotificationService(IAdminNotificationDbContext adminNotificationDbContext, IHttpContextAccessor httpContextAccessor, ITokenProvider tokenProvider, ILogManager logManager)
        {
            _adminNotificationDbContext = adminNotificationDbContext;
            _tokenProvider = tokenProvider;
            _logger = logManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NotificationResponse> GetAdminNotificationsAsync(int currentPageNumber, int currentPageSize)
        {
            var notificationResponse = new NotificationResponse()
            {
                Status = Status.Failure,
                PagingInfo = new PagingInfo()
                {
                    PageNumber = currentPageNumber,
                    PageSize = currentPageSize,
                }
            };
            int userId = -1;

            try
            {
                RequestData request = await Utility.GetRequestDataFromHeader(_httpContextAccessor, _tokenProvider);
                if (!request.HasValidToken)
                    throw new InvalidTokenException();
                userId = request.User.Id;
                var doNotificationResponse = await _adminNotificationDbContext.GetAdminNotifications(currentPageNumber, currentPageSize);
                notificationResponse.PagingInfo.TotalResults = doNotificationResponse.PagingInfo.TotalResults;
                if (doNotificationResponse.Notifications!=null && doNotificationResponse.Notifications.Count > 0)
                {
                    notificationResponse.Notifications = doNotificationResponse.Notifications;
                    notificationResponse.Status = Status.Success;
                    return notificationResponse;
                }
                notificationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.NotFound, Constants.ErrorMessages.NoNotification);
            }
            catch (CustomException exception)
            {
                notificationResponse.Error = Utility.ErrorGenerator(exception.ErrorCode, exception.ErrorMessage);
                new Task(() => { _logger.LogException(exception, "GetAdminNotifications", IMS.Entities.Severity.Critical, "GetAdminNotification", notificationResponse); }).Start();
            }

            catch (Exception exception)
            {
                notificationResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.ServerError, Constants.ErrorMessages.ServerError);
                new Task(() => { _logger.LogException(exception, "GetAdminNotifications", IMS.Entities.Severity.Critical, "GetAdminNotification", notificationResponse); }).Start();
            }
            finally
            {
                Severity severity = Severity.No;
                if (notificationResponse.Status == Status.Failure)
                    severity = Severity.Critical;
                new Task(() => { _logger.Log("GetAdminNotifications", notificationResponse, "GetAdminNotification", notificationResponse.Status, severity, userId); }).Start();
            }
            return notificationResponse;
        }
    }
}
