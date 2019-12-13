using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
