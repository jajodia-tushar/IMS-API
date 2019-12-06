using IMS.DataLayer.Interfaces;
using IMS.Entities;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IInventoryDbContext
    {
        Task<List<Entities.ItemQuantityMapping>> GetShelfItemsByShelfCode(int shelfId);
    }
}
