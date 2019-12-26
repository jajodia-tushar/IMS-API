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

    }
}
