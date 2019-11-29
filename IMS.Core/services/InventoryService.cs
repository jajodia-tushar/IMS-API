using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.services
{
    public class InventoryService : IInventoryService
    {
        private ILogManager _logManager;
       // private IUserService _userService;

        public void TestMethod()
        {
            User loggedInUser;

            string accessToken = string.Empty; // Get Token from header
            if (!string.IsNullOrEmpty(accessToken))
            {
                var claims = Utility.DecodeAccessToken(accessToken);
                loggedInUser = null;//_userService.GetUserByUserId();
                try
                {
                    //Some Business Logic here
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    _logManager.Log(null, null, loggedInUser.Id);

                }

            }
        }
    }
}
