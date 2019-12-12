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
        Task<ItemResponse> AddItem(ItemRequest itemRequest);
        Task<ItemResponse> Delete(int id);
        Task<ItemResponse> UpdateItem(ItemRequest itemRequest);
    }
}