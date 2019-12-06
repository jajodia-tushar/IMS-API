using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IInventoryDbContext
    {
        List<Entities.ItemQuantityMapping> GetShelfItemsByShelfId(int shelfId);
    }
}
