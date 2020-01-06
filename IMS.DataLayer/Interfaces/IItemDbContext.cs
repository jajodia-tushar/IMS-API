using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IItemDbContext
    {
        Task<List<Item>> GetAllItems();
        Task<Item> GetItemById(int id);
        Task<int> AddItem(Item item);
        Task<bool> Delete(int id,bool isHardDelete);
        Task<Item> UpdateItem(Item item);
        Task<bool> IsItemAlreadyDeleted(int id,bool isHardDelete);
    }
}
