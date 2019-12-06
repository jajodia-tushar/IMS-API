using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IItemDbContext
    {
        List<Item> GetAllItems();
        List<Item> GetItemById(int id);
        List<Item> AddItem(ItemRequest itemRequest);
        List<Item> Delete(int id);
        List<Item> UpdateItem(ItemRequest itemRequest);
    }
}
