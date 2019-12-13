using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
   public static class UserTranslator
    {
        public static Contracts.User ToDataContractsObject(Entities.User user)
        {
            return new Contracts.User()
            {
                Id = user.Id,
                Username = user.Username,
                Password = null,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role =RoleTranslator.ToDataContractsObject(user.Role)

            };
        }
    }
}
