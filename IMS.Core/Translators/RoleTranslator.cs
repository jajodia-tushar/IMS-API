using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class RoleTranslator
    {
        public static Contracts.Role ToDataContractsObject(Entities.Role role)
        {
            return new Contracts.Role()
            {
                Id = role.Id,
                Name = role.Name
            };
        }


        internal static Entities.Role ToDataContractsObject(Contracts.Role role)
        {
            if(role!=null)
            return new Entities.Role()
            {
                Id = role.Id,
                Name = role.Name
            };
            return null;
        }


        public static Contracts.ListOfRolesResponse ToDataContractsObject(Entities.ListOfRolesResponse doRolesResponse)
        {
            if (doRolesResponse!= null)
                return new Contracts.ListOfRolesResponse
                {
                    Status = doRolesResponse.Status == Entities.Status.Success ? Contracts.Status.Success : Contracts.Status.Failure,
                    Error = doRolesResponse.Error == null ? null : Translator.ToDataContractsObject(doRolesResponse.Error),
                    Roles= doRolesResponse.Roles==null?null:ToDataContractsObject(doRolesResponse.Roles)
                };
            return null;
        }

        public static List<Contracts.Role> ToDataContractsObject(List<Entities.Role> doRoles)
        {
            List<Contracts.Role> dtoRoles = null;
            if (doRoles != null)
            {
                dtoRoles = new List<Contracts.Role>();
                foreach (var role in doRoles)
                    dtoRoles.Add(ToDataContractsObject(role));
                return dtoRoles;

            }
            return null;
        }
    }
}
