using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

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
                Role = RoleTranslator.ToDataContractsObject(user.Role)
            };
        }

        public static Contracts.UsersResponse ToDataContractsObject(Entities.UsersResponse entityUsersResponse)
        {
            Contracts.UsersResponse contractsUsersResponse = new Contracts.UsersResponse();
            if (entityUsersResponse.Status == Entities.Status.Success)
            {
                contractsUsersResponse.Status = Contracts.Status.Success;
                contractsUsersResponse.Users = ToDataContractsObject(entityUsersResponse.Users);
            }
            else
            {
                contractsUsersResponse.Status = Contracts.Status.Failure;
                contractsUsersResponse.Error = Translator.ToDataContractsObject(entityUsersResponse.Error);
            }
            return contractsUsersResponse;
        }

        private static List<Contracts.User> ToDataContractsObject(List<Entities.User> entityUsers)
        {
            List<Contracts.User> contractUsers = new List<Contracts.User>();
            foreach(Entities.User user in entityUsers)
            {
                contractUsers.Add(ToDataContractsObject(user));
            }
            return contractUsers;
        }
    }
}
