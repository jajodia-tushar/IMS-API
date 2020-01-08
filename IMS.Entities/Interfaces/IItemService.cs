using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IItemService
    {
        Task<ItemResponse> GetAllItems();
        Task<ItemResponse> GetItemById(int id);
        Task<ItemResponse> AddItem(Item item);
        Task<ItemResponse> Delete(int id,bool isHardDelete);
        Task<ItemResponse> UpdateItem(Item item);
    }
}